namespace PracticaFinal2.Models
{
    public class DetalleFactura
    {
        public string FacturaId { get; set; }
        public string ProductoId { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
