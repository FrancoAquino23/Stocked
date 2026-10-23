using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stocked.Api.Data;
using Stocked.Api.Repositories;
using Stocked.Api.Services.Auth;
using Stocked.Api.Services.Calendar;
using Stocked.Api.Services.Inventory;
using Stocked.Api.Services.Pantry;
using Stocked.Api.Services.Preparations;
using Stocked.Api.Services.Recipes;
using Stocked.Api.Services.Spoonacular;

var builder = WebApplication.CreateBuilder(args);

const string AngularDevClientPolicy = "AngularDevClient";

// Registrar los servicios de la aplicación

builder.Services.AddControllers()
    // Enums legibles en JSON en vez del valor numérico
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Más información sobre configurar Swagger/OpenAPI: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Botón "Authorize" en Swagger UI para probar endpoints protegidos con el JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegar el token obtenido en /api/auth/login (sin la palabra 'Bearer')"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

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

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IngredientRepository>();
builder.Services.AddScoped<RecipeRepository>();
builder.Services.AddScoped<FavoriteRecipeRepository>();
builder.Services.AddScoped<PantryRepository>();
builder.Services.AddScoped<PantryAdjustmentRepository>();
builder.Services.AddScoped<RecipePreparationRepository>();
builder.Services.AddScoped<MealPlanEntryRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<PantryService>();
builder.Services.AddScoped<RecipePreparationService>();
builder.Services.AddScoped<MealPlanService>();
builder.Services.AddScoped<ShoppingListService>();

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

// Traducir cualquier excepción sin capturar a una respuesta JSON consistente
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var isExternalApiFailure = exceptionFeature?.Error is HttpRequestException;
        var statusCode = isExternalApiFailure ? StatusCodes.Status502BadGateway : StatusCodes.Status500InternalServerError;
        var message = isExternalApiFailure
            ? "No se pudo completar la solicitud a un servicio externo."
            : "Ocurrió un error inesperado.";

        if (exceptionFeature is not null)
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ExceptionHandler");
            logger.LogError(exceptionFeature.Error, "Excepción sin capturar en {Path}", exceptionFeature.Path);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new { status = statusCode, message });
    });
});

// Configurar el pipeline de peticiones HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(AngularDevClientPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
