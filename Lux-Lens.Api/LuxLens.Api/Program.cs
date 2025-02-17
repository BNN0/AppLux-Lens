using Lux_Lens.ApplicationServices.Lens;
using Lux_Lens.Core.Entities;
using Lux_Lens.DataAccess;
using Lux_Lens.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
