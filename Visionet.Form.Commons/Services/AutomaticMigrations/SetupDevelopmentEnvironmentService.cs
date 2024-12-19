using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Visionet.Form.Entities;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Visionet.Form.Commons.Services.AutomaticMigrations
{
    public class SetupDevelopmentEnvironmentService
    {
        private readonly FormDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserEmailStore<User> _userStore;
        private readonly IOpenIddictApplicationManager _appManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        public SetupDevelopmentEnvironmentService(
            FormDbContext applicationDbContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<User> userStore,
            IOpenIddictApplicationManager openIddictApplicationManager,
            IOpenIddictScopeManager openIddictScopeManager
            )
        {
            _db = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = (IUserEmailStore<User>)userStore;
            _appManager = openIddictApplicationManager;
            _scopeManager = openIddictScopeManager;
        }

        public async Task MigrateAsync(CancellationToken cancellationToken)
        {
            await _db.Database.MigrateAsync(cancellationToken);
            var admin = await AddUserAdministrator(cancellationToken);
            var user = await AddUserRoleUser(cancellationToken);
            await AddAdministratorRole();
            await AddUserRole();
            await AddRoleToUser(admin, "Administrator");
            await AddRoleToUser(user, "User");

            await CreateApiServerApp(cancellationToken);
            await CreateCmsApiScope(cancellationToken);
            await CreateCmsApp(cancellationToken);
            await AddSkill(cancellationToken);
        }

        private async Task<User> AddUserAdministrator(CancellationToken cancellationToken)
        {
            var exist = await _userManager.FindByNameAsync("administrator@accelist.com");
            if (exist != null)
            {
                return exist;
            }

            var user = new User
            {
                FullName = "Administrator",
                EmailConfirmed = true,
            };
            await _userStore.SetUserNameAsync(user, "administrator@accelist.com", cancellationToken);
            await _userStore.SetEmailAsync(user, "administrator@accelist.com", cancellationToken);
            await _userManager.CreateAsync(user, "HelloWorld1!");

            return user;
        }

        private async Task<User> AddUserRoleUser(CancellationToken cancellationToken)
        {
            var exist = await _userManager.FindByNameAsync("user@accelist.com");
            if (exist != null)
            {
                return exist;
            }

            var user = new User
            {
                FullName = "User",
            };
            await _userStore.SetUserNameAsync(user, "user@accelist.com", cancellationToken);
            await _userStore.SetEmailAsync(user, "user@accelist.com", cancellationToken);
            await _userManager.CreateAsync(user, "HelloWorld1!");

            return user;
        }

        private async Task CreateApiServerApp(CancellationToken cancellationToken)
        {
            var exist = await _appManager.FindByClientIdAsync("accelist-eform-api-server", cancellationToken);
            if (exist != null)
            {
                return;
            }

            await _appManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "accelist-eform-api-server",
                DisplayName = "Accelist EForm API Server",
                Type = ClientTypes.Confidential,
                ClientSecret = "Accelist2023!",
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Introspection,
                    Permissions.Endpoints.Revocation,
                    Permissions.GrantTypes.ClientCredentials
                }
            }, cancellationToken);
        }

        private async Task CreateCmsApiScope(CancellationToken cancellationToken)
        {
            var exist = await _scopeManager.FindByNameAsync("accelist-eform-api:cms", cancellationToken);
            if (exist != null)
            {
                return;
            }

            await _scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "accelist-eform-api:cms",
                DisplayName = "Accelist EForm CMS (Front-End) API Scope",
                Resources =
                {
                    "accelist-eform-api-server"
                }
            }, cancellationToken);
        }

        private async Task<string?> CreateCmsApp(CancellationToken cancellationToken)
        {
            var exist = await _appManager.FindByClientIdAsync("accelist-eform-cms", cancellationToken);
            if (exist != null)
            {
                return await _appManager.GetIdAsync(exist, cancellationToken);
            }

            var o = await _appManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "accelist-eform-cms",
                DisplayName = "Accelist EForm CMS (Front-End)",
                RedirectUris = {
                    new Uri("http://localhost:3000/api/auth/callback/oidc"),
                    new Uri("https://oauth.pstmn.io/v1/callback")
                },
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Revocation,
                    Permissions.ResponseTypes.Code,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Roles,
                    Permissions.Scopes.Phone,
                    Permissions.Scopes.Address,
                    Permissions.Prefixes.Scope + "accelist-eform-api:cms"
                },
                Type = ClientTypes.Public
            }, cancellationToken);

            return await _appManager.GetIdAsync(o, cancellationToken);
        }

        private async Task AddAdministratorRole()
        {
            var exist = await _roleManager.RoleExistsAsync("Administrator");
            if (exist)
            {
                return;
            }

            await _roleManager.CreateAsync(new IdentityRole
            {
                Name = "Administrator"
            });
        }
        private async Task AddUserRole()
        {
            var exist = await _roleManager.RoleExistsAsync("User");
            if (exist)
            {
                return;
            }

            await _roleManager.CreateAsync(new IdentityRole
            {
                Name = "User"
            });
        }

        private async Task AddRoleToUser(User user, string roleName)
        {
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                return;
            }
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task AddSkill(CancellationToken cancellationToken)
        {
            var newSkill = new List<Skill>()
            {
                new Skill
                {
                    Id = 1,
                    Name = "ASP.NET",
                },
                new Skill
                {
                    Id = 2,
                    Name = "C#",
                },
                new Skill
                {
                    Id = 3,
                    Name = "JavaScript",
                },
                new Skill
                {
                    Id = 4,
                    Name = "TypeScript",
                },
                new Skill
                {
                    Id = 5,
                    Name = "CSS",
                },
                new Skill
                {
                    Id = 6,
                    Name = "Vue.js",
                },
                new Skill
                {
                    Id = 7,
                    Name = "Angular.js"
                },
                new Skill
                {
                    Id = 8,
                    Name = "React Native",
                },
                new Skill
                {
                    Id = 9,
                    Name = "PostgreeSql",
                },
                new Skill
                {
                    Id = 10,
                    Name = "Phyton",
                },
                new Skill
                {
                    Id = 11,
                    Name = "C",
                },
            };

            var skillDb = await _db.Skills.AsNoTracking().ToListAsync();
            var insertedSkill = new List<Skill>();
            foreach (var cs in newSkill)
            {
                var isAny = skillDb.Any(q => q.Name.ToLower() == cs.Name.ToLower());
                if (!isAny)
                {
                    insertedSkill.Add(cs);
                }
            }
            if (insertedSkill.Count > 0)
            {
                await _db.Skills.AddRangeAsync(insertedSkill);
                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
