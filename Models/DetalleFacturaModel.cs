using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace TradeFlow.Models
{
    public class DetalleFacturaModel : INotifyPropertyChanged
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

        [MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        private int _cantidad;
        public int Cantidad
        {
            get => _cantidad;
            set { if (_cantidad != value) { _cantidad = value; OnPropertyChanged(); OnPropertyChanged(nameof(CantidadPrecioTexto)); } }
        }

        public decimal PrecioUnitario { get; set; }

        public int DescuentoPorcentaje { get; set; }

        private decimal _precioFinal;
        public decimal PrecioFinal
        {
            get => _precioFinal;
            set { if (_precioFinal != value) { _precioFinal = value; OnPropertyChanged(); } }
        }

        private decimal _subtotal;
        public decimal Subtotal
        {
            get => _subtotal;
            set { if (_subtotal != value) { _subtotal = value; OnPropertyChanged(); } }
        }

        [Ignore]
        public bool TieneDescuento => DescuentoPorcentaje > 0;

        [Ignore]
        public string DescuentoTexto => DescuentoPorcentaje > 0 ? $"Descuento {DescuentoPorcentaje}%" : "Sin descuento";

        [Ignore]
        public string CantidadPrecioTexto => $"Cant: {Cantidad} x ${PrecioUnitario:N2}";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
