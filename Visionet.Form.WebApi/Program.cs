using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices(options =>
{
    options.PostgreSqlConnectionString = configuration.GetConnectionString("PostgreSql");
    options.AddWebAppOnlyServices = true;
});

builder.Services.AddEntityFrameworkCoreAutomaticMigrations();

builder.Services.AddOpenIdConnectServer(options => {
    // Use api/generate-rsa-keys to get new random values 
    options.SigningKey = configuration["oidcSigningKey"];
    options.EncryptionKey = configuration["oidcEncryptionKey"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
