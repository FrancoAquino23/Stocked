# Stocked

¿Tienes todo lo que necesitas para cocinar esta semana?

Stocked es tu banco de recetas y tu despensa en un solo lugar: te dice qué puedes cocinar con lo
que ya tienes, qué te falta, y te recuerda qué comprar antes de que se acabe.

## Qué hace

- **Banco de recetas** — busca por nombre, ingrediente, cocina, dieta, intolerancias o tiempo de
  preparación, con miles de recetas reales vía Spoonacular.
- **Recomendadas para ti** — una sección que te sugiere qué cocinar según lo que ya tienes en la
  despensa, sin que tengas que buscar nada.
- **Despensa** — lleva el control de lo que tienes y cuánto sueles comprar de cada cosa.
- **Favoritos y recientes** — guarda lo que te gusta, con un vistazo rápido de qué tan cerca estás
  de poder prepararlo.
- **Calendario y lista de compras** — configura tu día de despensa y la app calcula sola qué te
  falta comprar esa semana.
- **Cuentas reales** — cada quien con su propia despensa, favoritos y calendario.

## Stack

.NET 8 + Entity Framework Core en el backend, PostgreSQL como base de datos, Angular 18
(standalone components, signals) en el frontend, JWT para autenticación, y la API de
[Spoonacular](https://spoonacular.com/food-api) como fuente de recetas.

## Correrlo en local

Necesitas .NET 8 SDK, Node 18+, Docker (para Postgres) y una API key gratuita de Spoonacular.

```bash
# Base de datos
docker compose up -d

# Backend
cd backend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=stocked;Username=stocked_dev;Password=stocked_dev_password"
dotnet user-secrets set "Spoonacular:ApiKey" "<tu-api-key>"
dotnet user-secrets set "Jwt:SecretKey" "<una-llave-larga-y-aleatoria>"
dotnet ef database update --project Stocked.Api.csproj --startup-project Stocked.Api.csproj
dotnet run --project Stocked.Api.csproj
```

```bash
# Frontend
cd frontend
npm install
ng serve
```

Backend en `http://localhost:5215` (Swagger en `/swagger`), frontend en `http://localhost:4200`.

## Pruebas

```bash
cd backend/Stocked.Api.Tests && dotnet test
cd frontend && ng test --watch=false --browsers=ChromeHeadless
```

## En vivo

- Frontend: `<pendiente>`
- Backend: `<pendiente>`

Desplegado en Azure (Static Web Apps + App Service + Database for PostgreSQL). Las credenciales
viven como variables de entorno del servicio, nunca en el repositorio.
