using BusinessLogic.Data;
using BusinessLogic.Logic;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
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

// Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddAutoMapper(typeof(MappingProfiles));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IProductRepository, ProductRepository>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
