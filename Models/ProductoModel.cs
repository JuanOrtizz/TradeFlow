using SQLite;

namespace TradeFlow.Models
{
    public class ProductoModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public decimal Precio { get; set; }

        public bool Activo { get; set; } = true;
    }
}
