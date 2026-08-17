using SQLite;

namespace TradeFlow.Models
{
    public class LocalidadModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [SQLite.MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}
