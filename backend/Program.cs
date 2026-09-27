using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stocked.Api.Data;
using Stocked.Api.Repositories;
using Stocked.Api.Services.Pantry;
using Stocked.Api.Services.Recipes;
using Stocked.Api.Services.Spoonacular;

var builder = WebApplication.CreateBuilder(args);

const string AngularDevClientPolicy = "AngularDevClient";

// Registrar los servicios de la aplicación

builder.Services.AddControllers();
// Más información sobre configurar Swagger/OpenAPI: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<StockedDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<SpoonacularOptions>(
    builder.Configuration.GetSection(SpoonacularOptions.SectionName));

builder.Services.AddHttpClient<SpoonacularClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<SpoonacularOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
});

builder.Services.AddScoped<IngredientRepository>();
builder.Services.AddScoped<RecipeRepository>();
builder.Services.AddScoped<FavoriteRecipeRepository>();
builder.Services.AddScoped<PantryRepository>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<PantryService>();

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
