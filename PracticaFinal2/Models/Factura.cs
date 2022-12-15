using System;
using System.Collections.Generic;

namespace PracticaFinal2.Models
{
    public class Factura
    {
        public string FacturaId { get; set; }
        public string ClienteId { get; set; }
        public decimal Monto { 
            get
            {
                decimal monto = 0;
                foreach (var item in Items)
                { 
                    monto += item.Cantidad * item.PrecioUnitario;
                }
                return monto;
            }
        }
        public DateTime Fecha { get; set; }
        public List<DetalleFactura> Items { get; set; } = new List<DetalleFactura>();
    }
}
