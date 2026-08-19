using CrudApi.Data;
using CrudApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Default'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(
                "http://localhost:5173", 
                "http://localhost:8081"  
              )
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("Frontend");

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var intentos = 0;
    const int maxIntentos = 10;
    while (true)
    {
        try
        {
            db.Database.EnsureCreated(); 
            break;
        }
        catch (Exception) when (++intentos < maxIntentos)
        {
            Console.WriteLine($"SQL Server no está listo aún, reintentando ({intentos}/{maxIntentos})...");
            Thread.Sleep(3000);
        }
    }
/*
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Teclado mecánico", Precio = 45.99m, Stock = 20 },
            new Producto { Nombre = "Mouse inalámbrico", Precio = 19.99m, Stock = 50 }
        );
        db.SaveChanges();
    }

    if (!db.Clientes.Any())
    {
        db.Clientes.AddRange(
            new Cliente { Nombre = "Ana Gómez", Email = "ana.gomez@correo.com" },
            new Cliente { Nombre = "Carlos Pérez", Email = "carlos.perez@correo.com" },
            new Cliente { Nombre = "Lucía Fernández", Email = "lucia.fernandez@correo.com" }
        );
        db.SaveChanges();
    }
    */
}

app.MapGet("/api/productos", async (AppDbContext db) =>
    await db.Productos.ToListAsync());

app.MapGet("/api/productos/{id:int}", async (int id, AppDbContext db) =>
    await db.Productos.FindAsync(id)
        is Producto producto
            ? Results.Ok(producto)
            : Results.NotFound(new { mensaje = $"Producto {id} no encontrado" }));

app.MapGet("/api/productos/buscar", async (string nombre, AppDbContext db) =>
    await db.Productos
        .Where(p => p.Nombre.Contains(nombre))
        .OrderBy(p => p.Nombre)
        .ToListAsync());

app.MapGet("/api/productos/buscar-sql", async (string nombre, AppDbContext db) =>
    await db.Productos
        .FromSqlInterpolated($"SELECT * FROM Productos WHERE Nombre LIKE '%' + {nombre} + '%'")
        .ToListAsync());

app.MapPost("/api/productos", async (Producto producto, AppDbContext db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{producto.Id}", producto);
});

app.MapPut("/api/productos/{id:int}", async (int id, Producto input, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound(new { mensaje = $"Producto {id} no encontrado" });

    producto.Nombre = input.Nombre;
    producto.Precio = input.Precio;
    producto.Stock = input.Stock;

    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapDelete("/api/productos/{id:int}", async (int id, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound(new { mensaje = $"Producto {id} no encontrado" });

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.MapGet("/api/clientes", async (AppDbContext db) =>
    await db.Clientes.ToListAsync());

app.MapGet("/api/clientes/{id:int}", async (int id, AppDbContext db) =>
    await db.Clientes.FindAsync(id)
        is Cliente cliente
            ? Results.Ok(cliente)
            : Results.NotFound(new { mensaje = $"Cliente {id} no encontrado" }));

app.MapPost("/api/clientes", async (Cliente cliente, AppDbContext db) =>
{
    db.Clientes.Add(cliente);
    await db.SaveChangesAsync();
    return Results.Created($"/api/clientes/{cliente.Id}", cliente);
});

app.MapPost("/api/ventas", async (VentaRequest request, AppDbContext db) =>
{
    if (request.Items.Count == 0)
        return Results.BadRequest(new { mensaje = "El carrito está vacío" });

    var cliente = await db.Clientes.FindAsync(request.ClienteId);
    if (cliente is null)
        return Results.BadRequest(new { mensaje = "Cliente no válido" });

    var venta = new Venta { ClienteId = request.ClienteId, Fecha = DateTime.UtcNow };

    foreach (var item in request.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto is null)
            return Results.BadRequest(new { mensaje = $"Producto {item.ProductoId} no existe" });

        if (item.Cantidad <= 0)
            return Results.BadRequest(new { mensaje = "La cantidad debe ser mayor a 0" });

        venta.Detalles.Add(new DetalleVenta
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio 
        });
    }

    db.Ventas.Add(venta);
    await db.SaveChangesAsync();

    var total = venta.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

    return Results.Created($"/api/ventas/{venta.Id}", new
    {
        venta.Id,
        venta.Fecha,
        venta.ClienteId,
        Total = total,
        venta.Detalles
    });
});

app.MapGet("/api/ventas/cliente/{clienteId:int}", async (int clienteId, AppDbContext db) =>
{
    var ventas = await db.Ventas
        .Where(v => v.ClienteId == clienteId)
        .Include(v => v.Detalles)
            .ThenInclude(d => d.Producto)
        .OrderByDescending(v => v.Fecha)
        .ToListAsync();

    var resultado = ventas.Select(v => new
    {
        v.Id,
        v.Fecha,
        v.ClienteId,
        Total = v.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario),
        Detalles = v.Detalles.Select(d => new
        {
            d.Id,
            d.ProductoId,
            ProductoNombre = d.Producto != null ? d.Producto.Nombre : "(producto eliminado)",
            d.Cantidad,
            d.PrecioUnitario
        })
    });

    return Results.Ok(resultado);
});

app.Run();
