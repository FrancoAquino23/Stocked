using Microsoft.EntityFrameworkCore;
using Stocked.Api.Data;

var builder = WebApplication.CreateBuilder(args);

const string AngularDevClientPolicy = "AngularDevClient";

// Registrar los servicios de la aplicación

builder.Services.AddControllers();
// Más información sobre configurar Swagger/OpenAPI: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<StockedDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    // Solo el servidor de desarrollo de Angular puede llamar a la API mientras no exista un origen de producción
    options.AddPolicy(AngularDevClientPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configurar el pipeline de peticiones HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(AngularDevClientPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
