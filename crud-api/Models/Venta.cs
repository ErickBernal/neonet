namespace CrudApi.Models;

public class Venta
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public List<DetalleVenta> Detalles { get; set; } = new();
}

public class DetalleVenta
{
    public int Id { get; set; }
    public int VentaId { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
