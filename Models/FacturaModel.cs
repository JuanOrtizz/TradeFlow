using SQLite;

namespace TradeFlow.Models
{
    public class FacturaModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Indexed]
        public int ClienteId { get; set; }

        [Ignore]
        public ClienteModel? Cliente { get; set; }

        public decimal Total { get; set; }

        [Ignore]
        public List<DetalleFacturaModel> Items { get; set; } = new List<DetalleFacturaModel>();
    }
}
