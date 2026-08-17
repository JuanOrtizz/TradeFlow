using SQLite;

namespace TradeFlow.Models
{
    public class ClienteModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [Indexed]
        public int LocalidadId { get; set; }

        [Ignore]
        public LocalidadModel? Localidad { get; set; }
    }
}
