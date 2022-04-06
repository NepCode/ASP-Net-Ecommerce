using BusinessLogic.Data;
using BusinessLogic.Logic;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors(options => {
    options.AddPolicy("CORSRule", rule =>
        rule.WithOrigins("*")
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
    });

// Generic Class using pattern repository
builder.Services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));


// IdentityCore
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services
    .AddIdentityCore<User>()
    .AddSignInManager<SignInManager<User>>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SecurityDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });





// DbContext
var server = builder.Configuration.GetValue<string>("DBServer") ?? "localhost";
var port = builder.Configuration["DBPort"] ?? "1433";
var user = builder.Configuration["DBUser"] ?? "sa";
var password = builder.Configuration["DBPassword"] ?? "A!a4765f8d7";
var database = builder.Configuration["DBName"] ?? "asp_netcore_ecommerce";

builder.Services.AddDbContext<DataContext>(opts =>
{
    //opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    opts.UseSqlServer( $"Server={server},{port};Initial Catalog={database};User ID ={user};Password={password}");
    //opts.UseSqlServer( $"Server=db,{port};Initial Catalog={database};User ID ={user};Password={password}");
});
builder.Services.AddDbContext<SecurityDbContext>(opts =>
{
    opts.UseSqlServer($"Server={server},{port};Initial Catalog={database};User ID ={user};Password={password}");
});

// Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddAutoMapper(typeof(MappingProfiles));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.TryAddSingleton<ISystemClock, SystemClock>();

builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();

var app = builder.Build();

// Run migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate();
        await DataContextSeedData.LoadDataAsync(context, loggerFactory);

        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var identityContext = services.GetRequiredService<SecurityDbContext>();
        await identityContext.Database.MigrateAsync();
        await SecurityDbContextSeedData.SeedUserAsync(userManager, roleManager);
    }
    catch (Exception e)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(e.Message);
    }

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/errors", "?code={0}");

app.UseCors("CORSRule");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
