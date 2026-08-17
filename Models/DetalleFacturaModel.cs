using SQLite;

namespace TradeFlow.Models
{
    public class DetalleFacturaModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int FacturaId { get; set; }

        [Ignore]
        public FacturaModel? Factura { get; set; }

        [Indexed]
        public int ProductoId { get; set; }

        [Ignore]
        public ProductoModel? Producto { get; set; }

        [MaxLength(100)]
        public string ProductoNombre { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public int DescuentoPorcentaje { get; set; }

        public decimal PrecioFinal { get; set; }

        public decimal Subtotal { get; set; }
    }
}
