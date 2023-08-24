using Lux_Lens.ApplicationServices.Lens;
using Lux_Lens.Core.Entities;
using Lux_Lens.DataAccess;
using Lux_Lens.DataAccess.Repositories;
using LuxLens.Api;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ILensService, LensService>();
builder.Services.AddTransient<IRepository<int, Lens>, Repository<int, Lens>>();
builder.Services.AddAutoMapper(typeof(Lux_Lens.ApplicationServices.MapperProfile));
//Conexion de Bd

string connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<LensDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<LensDbContext>()
    .AddSignInManager<SignInManager<IdentityUser>>();


//CORS
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowWebApp", builder =>
       {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lux-Lens API", Version = "v1" });

// Agrega el esquema de seguridad para Swagger
});


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<LensDbContext>();
    context.Database.Migrate();
}


app.UseCors("AllowWebApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.ConfigObject.DefaultModelExpandDepth = -1; // Opcional, para colapsar todos los modelos por defecto en Swagger UI
    });
    app.UseRouting();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
