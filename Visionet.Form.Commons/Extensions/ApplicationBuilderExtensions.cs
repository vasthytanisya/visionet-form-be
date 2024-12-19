using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Quartz;
using SendGrid.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Security.Claims;
using System.Security.Cryptography;
using Visionet.Form.Commons.Extensions;
using Visionet.Form.Commons.RequestHandlers.Employees;
using Visionet.Form.Commons.Services;
using Visionet.Form.Commons.Services.AutomaticMigrations;
using Visionet.Form.Commons.Validations.Employees;
using Visionet.Form.Entities;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Microsoft.Extensions.Hosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IHostBuilder ConfigureSerilogWithSentry(this IHostBuilder host, Action<HostBuilderContext, ConfigureSerilogOptions>? optionsBuilder = default)
        {
            return host.ConfigureServices((ctx, services) =>
            {
                var opts = new ConfigureSerilogOptions();
                optionsBuilder?.Invoke(ctx, opts);

                var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext();

                if (opts.WriteErrorLogsToFile.HasValue())
                {
                    loggerConfiguration.WriteTo.File(opts.WriteErrorLogsToFile, LogEventLevel.Warning, rollingInterval: RollingInterval.Day);
                }

                if (opts.WriteJsonToConsoleLog)
                {
                    // required for log aggregation in Kubernetes cluster with container log collector
                    // agent such as Fluent Bit to ElasticSearch + Kibana
                    // https://docs.fluentbit.io/manual/installation/kubernetes
                    loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
                }
                else
                {
                    loggerConfiguration.WriteTo.Console();
                }

                if (opts.WriteToSentry)
                {
                    loggerConfiguration.WriteTo.Sentry();
                }

                Log.Logger = loggerConfiguration.CreateLogger();
            }).UseSerilog();
        }

        public static IHostBuilder ConfigureSerilogWithSentry(this IHostBuilder host, Action<ConfigureSerilogOptions>? optionsBuilder = default)
        {
            return host.ConfigureSerilogWithSentry((ctx, opts) =>
            {
                optionsBuilder?.Invoke(opts);
            });
        }

        public static void AddMinIoService(this IServiceCollection services, Action<MinIoOptions>? optionsBuilder = default)
        {
            var opts = new MinIoOptions();
            optionsBuilder?.Invoke(opts);

            services.AddSingleton(di =>
            {
                return new MinioClient()
                .WithEndpoint(opts.EndPoint).WithCredentials(opts.AccessKey, opts.ServerKey)
                .WithSSL(opts.IsUseSsl)
                .Build();
            });

            services.AddTransient<IStorageService, StorageService>();
        }
        public static void AddApplicationServices(this IServiceCollection services, Action<ApplicationServicesOptions>? optionsBuilder = default)
        {
            var opts = new ApplicationServicesOptions();
            optionsBuilder?.Invoke(opts);

            services.AddApplicationDbContext(opts.PostgreSqlConnectionString);

            services.AddDataProtection().PersistKeysToDbContext<FormDbContext>();

            services.AddIdentity<User, IdentityRole>(options =>
            {
                // Configure Identity to use the same JWT claims as OpenIddict instead
                // of the legacy WS-Federation claims it uses by default (ClaimTypes),
                // which saves you from doing the mapping in your authorization controller.
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<FormDbContext>().AddDefaultTokenProviders();

            services.AddScoped<CustomSignInManager>();

            services.AddHttpContextAccessor();
            services.AddScoped(di =>
            {
                var contextAccessor = di.GetRequiredService<IHttpContextAccessor>();
                return contextAccessor.HttpContext?.User ?? new ClaimsPrincipal();
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                if (opts.AlwaysSecureCookiePolicy)
                {
                    options.Secure = CookieSecurePolicy.Always;
                }
                else
                {
                    options.Secure = CookieSecurePolicy.SameAsRequest;
                }
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddHealthChecks().AddDbContextCheck<FormDbContext>();

            //services.AddTransient<IMailService, SmtpMailService>();

            services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();
            //services.AddValidatorsFromAssemblyContaining<CandidateListValidator>();
            //services.AddValidatorsFromAssemblyContaining<AddAppliedPositionValidator>();
            services.AddMediatR(typeof(CreateEmployeeHandler));
            services.AddTransient<CreateEmployeeHandler>();
            //services.AddTransient<CandidateFormValidation>();
            //services.AddTransient<CandidateService>();
            //services.AddTransient<BlobInformationHandler>();
        }
        public static void AddSendGridService(this IServiceCollection services, Action<SendGridOptions>? optionsBuilder = default)
        {
            var opts = new SendGridOptions();
            optionsBuilder?.Invoke(opts);

            services.AddSendGrid(options =>
            {
                options.ApiKey = opts.ApiKey;
            });

            services.AddTransient<IMailService, SendGridService>();
        }

        public static void AddOpenIdConnectServer(this IServiceCollection services, Action<OpenIdConnectServerOptions>? optionsBuilder = default)
        {
            var opts = new OpenIdConnectServerOptions();
            optionsBuilder?.Invoke(opts);

            // Quartz.NET is added to support OpenIddict token cleanup
            // However, it is also usable for apps with Cron Job from a Worker Service
            // When doing so, just move Quartz service registration outside `AddOpenIdConnectServer`
            // Then consider adding SQL database persistent storage for Quartz.NET:
            // https://www.quartz-scheduler.net/documentation/quartz-3.x/quick-start.html#creating-and-initializing-database
            // Do not use clustering mode of Quartz.NET unless you know what you're doing... (Just run in a single pod)
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            services.AddOpenIddict()
                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the Entity Framework Core stores and models.
                    // Note: call ReplaceDefaultEntities() to replace the default entities.
                    options.UseEntityFrameworkCore()
                            .UseDbContext<FormDbContext>();

                    // Configure OpenIddict to use Quartz to prune revoked/expired token.
                    options.UseQuartz(builder =>
                    {
                        // Configure the minimum lifespan tokens must have to be pruned.
                        builder.Configure(o =>
                        {
                            o.MinimumTokenLifespan = TimeSpan.FromDays(7);
                        });
                    });
                })
                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    // Enable the authorization, token, introspection and userinfo endpoints.
                    options.SetAuthorizationEndpointUris(OpenIdSettings.Endpoints.Authorization)
                            .SetTokenEndpointUris(OpenIdSettings.Endpoints.Token)
                            .SetIntrospectionEndpointUris(OpenIdSettings.Endpoints.Introspection)
                            .SetUserinfoEndpointUris(OpenIdSettings.Endpoints.Userinfo)
                            .SetRevocationEndpointUris(OpenIdSettings.Endpoints.Revoke)
                            .SetIntrospectionEndpointUris(OpenIdSettings.Endpoints.Introspect)
                            .SetLogoutEndpointUris(OpenIdSettings.Endpoints.Logout);

                    // Enable the client credentials flow for machine to machine auth.
                    options.AllowClientCredentialsFlow();

                    // Enable the authorization code flow and refresh token flow for native and web apps.
                    options.AllowAuthorizationCodeFlow();
                    options.AllowRefreshTokenFlow();
                    
                    // Enforce Proof Key for Code Exchange when requesting an authorization code.
                    options.RequireProofKeyForCodeExchange();

                    // Expose all the supported claims in the discovery document.
                    options.RegisterClaims(OpenIdSettings.Claims);

                    // Expose all the supported scopes in the discovery document.
                    options.RegisterScopes(OpenIdSettings.Scopes);

                    // Register the signing and encryption credentials.

                    //var signingRsa = RSA.Create();
                    //signingRsa.ImportRSAPrivateKey(Convert.FromBase64String(opts.SigningKey), out _);
                    //var encryptionRsa = RSA.Create();
                    //encryptionRsa.ImportRSAPrivateKey(Convert.FromBase64String(opts.EncryptionKey), out _);

                    //options.AddSigningKey(new RsaSecurityKey(signingRsa))
                    //    .AddEncryptionKey(new RsaSecurityKey(encryptionRsa));

                    // Register the ASP.NET Core host and configure the ASP.NET Core options.
                    options.UseAspNetCore()
                            .DisableTransportSecurityRequirement()
                            .EnableAuthorizationEndpointPassthrough()
                            .EnableTokenEndpointPassthrough()
                            .EnableUserinfoEndpointPassthrough()
                            .EnableLogoutEndpointPassthrough();

                    // Create Data Protection tokens instead of JWT tokens.
                    // ASP.NET Core Data Protection uses its own key ring to encrypt and protect tokens against tampering
                    // and is supported for all types of tokens, except identity tokens, that are always JWT tokens.
                    options.UseDataProtection();

                    // Configures OpenIddict to use reference tokens, so that the access token payloads
                    // are stored in the database (only an identifier is returned to the client application).
                    options.UseReferenceAccessTokens()
                        .UseReferenceRefreshTokens();

                    options.SetAccessTokenLifetime(TimeSpan.FromHours(24));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));
                    options.SetRefreshTokenReuseLeeway(TimeSpan.FromSeconds(60));
                })
                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Enable authorization entry validation, which is required to be able
                    // to reject access tokens retrieved from a revoked authorization code.
                    options.EnableAuthorizationEntryValidation();

                    // Enables token validation so that a database call is made for each API request,
                    // required when the OpenIddict server is configured to use reference tokens.
                    options.EnableTokenEntryValidation();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                    options.UseDataProtection();
                });
        }

        public static void AddEntityFrameworkCoreAutomaticMigrations(this IServiceCollection services)
        {
            services.AddScoped<SetupDevelopmentEnvironmentService>();
            services.AddHostedService<SetupDevelopmentEnvironmentHostedService>();
        }
    }
}
