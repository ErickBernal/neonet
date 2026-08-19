> **Nota:** este proyecto ahora forma parte de `proyecto-completo/`, junto
> con el frontend en Vue 3. Para levantarlo todo junto (API + SQL Server +
> Frontend), usa el `docker-compose.yml` que está en la carpeta raíz
> `proyecto-completo/`, no el de aquí abajo.

# CRUD ASP.NET Core Web API con Docker (sin instalar .NET)

API de ejemplo para gestionar "Productos" con operaciones CRUD, usando
Minimal APIs + Entity Framework Core (base de datos en memoria).

## Requisitos

- Docker Desktop instalado en Windows (con WSL2 activado, que es lo normal).
  Eso es lo único que necesitas, **no hace falta instalar el SDK de .NET**.

## Cómo correrlo

Abre PowerShell o CMD en la carpeta del proyecto y ejecuta:

```bash
docker compose up --build
```

Esto construye la imagen (dentro del contenedor se descarga el SDK de .NET
solo para compilar) y levanta el contenedor final, que ya trae únicamente
el runtime, mucho más liviano.

También puedes hacerlo sin docker-compose:

```bash
docker build -t crud-api .
docker run -p 8080:8080 crud-api
```

## Probar la API

- Swagger (interfaz visual para probar los endpoints):
  http://localhost:8080/swagger

- Endpoints:
  - `GET    /api/productos`        -> listar todos
  - `GET    /api/productos/{id}`   -> obtener uno
  - `POST   /api/productos`        -> crear
  - `PUT    /api/productos/{id}`   -> actualizar
  - `DELETE /api/productos/{id}`   -> eliminar

Ejemplo con curl:

```bash
curl http://localhost:8080/api/productos

curl -X POST http://localhost:8080/api/productos ^
  -H "Content-Type: application/json" ^
  -d "{\"id\":3,\"nombre\":\"Monitor 24''\",\"precio\":150.00,\"stock\":10}"
```

## Base de datos: SQL Server

El proyecto ya usa SQL Server real (no en memoria), corriendo en su propio
contenedor. `docker compose up --build` levanta dos servicios:

- `crud-api`: tu Web API
- `sqlserver`: SQL Server 2022, con los datos guardados en un volumen
  (`sqlserver_data`), así que **persisten** aunque reinicies los contenedores

La API espera a que SQL Server esté listo (reintenta la conexión varias
veces) y crea la base de datos y la tabla automáticamente al arrancar.

### Si ya intentaste levantarlo antes y falló

Si en un intento anterior la base de datos se llegó a crear pero sin tablas
(por ejemplo si viste un error `Invalid object name 'Productos'`), necesitas
borrar el volumen para que se cree todo desde cero:

```bash
docker compose down -v
docker compose up --build
```

El flag `-v` borra los volúmenes (incluyendo `sqlserver_data`), así que
también perderás los datos que tuvieras guardados.

### Consultas de ejemplo

Además del CRUD normal, hay dos endpoints que muestran cómo consultar:

```bash
# Filtrar por nombre usando LINQ (EF Core lo traduce a SQL)
curl "http://localhost:8080/api/productos/buscar?nombre=mouse"

# Lo mismo, pero con SQL crudo (parametrizado, sin riesgo de inyección)
curl "http://localhost:8080/api/productos/buscar-sql?nombre=mouse"
```

### Conectarte directamente a la base con un cliente SQL

Si quieres ver los datos con Azure Data Studio, DBeaver o SSMS:

- Servidor: `localhost,1433`
- Usuario: `sa`
- Contraseña: `TuPassword123!`
- Base de datos: `ProductosDb`

> En producción nunca dejes la contraseña en el `docker-compose.yml` en
> texto plano; usa un archivo `.env` o Docker secrets.

## Bonus: generar un proyecto nuevo sin instalar .NET

Si en el futuro quieres crear otro proyecto desde cero (sin este ejemplo),
puedes usar el SDK de .NET dentro de un contenedor temporal, montando tu
carpeta actual:

```bash
docker run --rm -v ${PWD}:/app -w /app mcr.microsoft.com/dotnet/sdk:8.0 dotnet new webapi -n MiApi
```

Eso genera los archivos del proyecto en tu máquina, usando el SDK que vive
únicamente dentro del contenedor (se descarta al terminar).
