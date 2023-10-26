using BusinessLogic.Data;
using BusinessLogic.Logic;
using Core.Configuration;
using Core.Interfaces;
using Core.Models;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using WebAPI.Extensions;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers( options =>
{
    //options.Filters.Add<ValidateModelStateAttribute>();
}).AddFluentValidation(fv =>
{
    // Validate child properties and root collection elements
    fv.ImplicitlyValidateChildProperties = true;
    fv.ImplicitlyValidateRootCollectionElements = true;
    // Automatic registration of validators in assembly
    fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});

/*builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});*/

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
builder.Services.AddScoped<IEmailService, EmailService>();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));




// DbContext
var server = Environment.GetEnvironmentVariable("DB_HOST") ?? builder.Configuration.GetValue<string>("ConnectionDB:DBServer");
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? builder.Configuration["ConnectionDB:DBPort"];
var user = Environment.GetEnvironmentVariable("DB_USER") ?? builder.Configuration["ConnectionDB:DBUser"];
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? builder.Configuration["ConnectionDB:DBPassword"];
var database = Environment.GetEnvironmentVariable("DB_NAME") ?? builder.Configuration["ConnectionDB:DBName"];

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
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();

//builder.Services.Configure<IPWhitelistOptions>(builder.Configuration.GetSection("IPWhitelistOptions"));


var app = builder.Build();

//app.UseIPWhitelist();


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
