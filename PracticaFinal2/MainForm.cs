using PracticaFinal2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace PracticaFinal2
{
    public partial class MainForm : Form
    {
        const string RUTA_CLIENTES = @"Data\Clientes.csv";
        const string RUTA_PRODUCTOS = @"Data\Productos.csv";
        const string RUTA_FACTURAS = @"Data\Factura.csv";
        const string RUTA_DETALLE_FACTURAS = @"Data\DFactura.csv";
        
        readonly Panel[] panels;
        readonly LinkLabel[] links;

        private BindingList<Producto> productos = new BindingList<Producto>();
        private BindingList<Cliente> clientes = new BindingList<Cliente>();
        private BindingList<Factura> facturas = new BindingList<Factura>();
        
        public MainForm()
        {
            InitializeComponent();
            panels = new Panel[] { pnlCliente, pnlProducto, pnlFacturas, pnlNuevaFactura, pnlRepCli, pnlRepProd, pnlRepFac };
            links = new LinkLabel[] { lnkCliente, lnkProducto, lnkFacturas, lnkRepPorCli, lnkRepPorFac, lnkRepPorProd };
            pnlCliente.Dock = DockStyle.Fill;
            pnlProducto.Dock = DockStyle.Fill;
            pnlFacturas.Dock = DockStyle.Fill;
            pnlRepCli.Dock = DockStyle.Fill;
            pnlRepProd.Dock = DockStyle.Fill;
            pnlRepFac.Dock = DockStyle.Fill;
            pnlNuevaFactura.Dock = DockStyle.Fill;
            grdProductos.AutoGenerateColumns = false;
            grdProductos.DataSource = productos;
            grdClientes.AutoGenerateColumns = false;
            grdClientes.DataSource = clientes;
            grdFacturas.AutoGenerateColumns = false;
            grdFacturas.DataSource = facturas;
            lnkCliente.LinkVisited = true;
        }

        #region Carga de datos
        private void MainForm_Load(object sender, EventArgs e)
        {
            CargarClientes();
            CargarProductos();
            CargarFacturas();
        }

        private void CargarFacturas()
        {
            if (!File.Exists(RUTA_FACTURAS)) return;

            using (var sr = new StreamReader(RUTA_FACTURAS))
            {
                // Salto la cabecera
                sr.ReadLine();

                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string[] campos = linea.Split(';');
                    var f = new Factura
                    {
                        FacturaId = campos[0],
                        ClienteId = campos[1],
                        Fecha = Convert.ToDateTime(campos[3])
                    };

                    CargarDetalleDeFactura(f);

                    facturas.Add(f);
                }
            }
        }

        private void CargarDetalleDeFactura(Factura fact)
        {
            if (!File.Exists(RUTA_DETALLE_FACTURAS)) return;

            using (var sr = new StreamReader(RUTA_DETALLE_FACTURAS))
            {
                // Salto la cabecera
                sr.ReadLine();

                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string[] campos = linea.Split(';');
                    if (fact.FacturaId == campos[0])
                    {
                        var df = new DetalleFactura
                        {
                            FacturaId = campos[0],
                            ProductoId = campos[1],
                            Descripcion = campos[2],
                            Cantidad = Convert.ToDecimal(campos[3]),
                            PrecioUnitario = Convert.ToDecimal(campos[4])
                        };
                        fact.Items.Add(df);
                    }
                }
            }
        }

        private void CargarClientes()
        {
            if (!File.Exists(RUTA_CLIENTES)) return;

            using (var sr = new StreamReader(RUTA_CLIENTES))
            {
                // Salto la cabecera
                sr.ReadLine();

                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string[] campos = linea.Split(';');
                    var c = new Cliente
                    {
                        ClienteId = campos[0],
                        NombreCompleto = campos[1]
                    };
                    clientes.Add(c);
                }
            }
        }

        private void CargarProductos()
        {
            if (!File.Exists(RUTA_PRODUCTOS)) return;

            using (var sr = new StreamReader(RUTA_PRODUCTOS))
            {
                // Salto la cabecera
                sr.ReadLine();

                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string[] campos = linea.Split(';');
                    var p = new Producto
                    {
                        ProductoId = campos[0],
                        Descripcion = campos[1],
                        Precio = Convert.ToDecimal(campos[2]),
                        Existencia = Convert.ToDecimal(campos[3])
                    };
                    productos.Add(p);
                }
            }
        }

        #endregion

        #region Guardado de archivos

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            EscribirArchivoClientes();
            EscribirArchivoProductos();
        }

        private void EscribirArchivoClientes()
        {
            string cabecera = "Cliente;NombreCompleto";
            EscribirArchivo(RUTA_CLIENTES, cabecera, grdClientes);
        }

        private void EscribirArchivoProductos()
        {
            string cabecera = "Producto;Descripcion;Precio;Existencia";
            EscribirArchivo(RUTA_PRODUCTOS, cabecera, grdProductos);
        }

        private static void EscribirArchivo(string ruta, string cabecera, DataGridView gridView)
        {
            using (var sw = new StreamWriter(ruta))
            {
                sw.WriteLine(cabecera);

                foreach (DataGridViewRow r in gridView.Rows)
                {
                    string linea = "";

                    for (int i = 0; i < r.Cells.Count; i++)
                    {
                        if (i > 0)
                        {
                            linea += ";";
                        }

                        linea += r.Cells[i].Value;
                    }

                    sw.WriteLine(linea);
                }
            }
        }

        #endregion

        
        #region Agregar Cliente
        private void btnAgregarCliente_Click(object sender, EventArgs e)
        {
            if (!EsClienteValido())
            {
                MessageBox.Show("Debe completar los datos del cliente");
                return;
            }

            clientes.Add(new Cliente
            {
                ClienteId = txtNroCliente.Text,
                NombreCompleto = txtNombreCliente.Text
            });
        }

        private bool EsClienteValido()
        {
            return txtNroCliente.Text != "" && txtNombreCliente.Text != "";
        }

        #endregion

        #region Agregar Producto
        
        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (!EsProductoValido())
            {
                MessageBox.Show("Debe completar los datos del producto");
            }

            var p = new Producto
            {
                ProductoId = txtIdProd.Text,
                Descripcion = txtDescProd.Text,
                Precio = Convert.ToDecimal(txtPrecioProd.Text),
                Existencia = Convert.ToDecimal(txttStockProd.Text)
            };

            productos.Add(p);
        }

        private bool EsProductoValido()
        {
            return txtIdProd.Text != "" 
                && txtDescProd.Text != "" 
                && txtPrecioProd.Text != ""
                && txttStockProd.Text != "";
        }

        #endregion

        
        #region Facturas
        
        private void grdFacturas_SelectionChanged(object sender, EventArgs e)
        {
            if (grdFacturas.SelectedRows.Count > 0)
            {
                DataGridViewRow row = grdFacturas.SelectedRows[0];
                grdDFactura.DataSource = row.DataBoundItem;
            }
        }
        
        #endregion

        #region Nueva Factura

        private void btnNuevaFactura_Click(object sender, EventArgs e)
        {
            txtNuevaFacNro.Clear();
            grdDFacNueva.Rows.Clear();
            
            pnlFacturas.Visible = false;
            pnlNuevaFactura.Visible = true;

            cboNuevaFactCliente.DataSource = clientes;
            cboItemProd.DataSource = productos;
        }

        private void btnAgregarItem_Click(object sender, EventArgs e)
        {
            Producto p = (Producto)cboItemProd.SelectedItem;

            grdDFacNueva.Rows.Add(
                p.ProductoId, 
                p.Descripcion, 
                nupItemCant.Value, 
                p.Precio
            );

        }

        private void btnGuardarFactura_Click(object sender, EventArgs e)
        {
            if (!EsFacturaValida())
            {
                MessageBox.Show("Debe Ingresar Numero de Factura");
                return;
            }

            var nueva = new Factura()
            {
                FacturaId = txtNuevaFacNro.Text,
                ClienteId = cboNuevaFactCliente.SelectedValue.ToString(),
                Fecha = dtpNuevaFacFecha.Value
            };

            foreach(DataGridViewRow r in grdDFacNueva.Rows)
            {
                nueva.Items.Add(new DetalleFactura
                {
                    FacturaId = txtNuevaFacNro.Text,
                    ProductoId = (string)r.Cells[0].Value,
                    Descripcion  = (string)r.Cells[1].Value,
                    Cantidad = Convert.ToDecimal(r.Cells[2].Value),
                    PrecioUnitario = Convert.ToDecimal(r.Cells[3].Value)
                }); 
            }

            facturas.Add(nueva);

            EscribirEncabezadoNuevaFactura(nueva);
            EscribirDetalleNuevaFactura(nueva);

            pnlNuevaFactura.Visible = false;
            pnlFacturas.Visible = true;
        }

        private void EscribirDetalleNuevaFactura(Factura fac)
        {
            using (var sw = new StreamWriter(RUTA_DETALLE_FACTURAS, append: true))
            {
                foreach (DetalleFactura item in fac.Items)
                {
                    sw.Write(item.FacturaId);
                    sw.Write(";");
                    sw.Write(item.ProductoId);
                    sw.Write(";");
                    sw.Write(item.Descripcion);
                    sw.Write(";");
                    sw.Write(item.Cantidad);
                    sw.Write(";");
                    sw.Write(item.PrecioUnitario);
                    sw.WriteLine();
                }
            }
        }

        private void EscribirEncabezadoNuevaFactura(Factura fac)
        {
            using (var sw = new StreamWriter(RUTA_FACTURAS, append: true))
            {
                sw.Write(fac.FacturaId);
                sw.Write(";");
                sw.Write(fac.ClienteId);
                sw.Write(";");
                sw.Write(fac.Monto);
                sw.Write(";");
                sw.Write(fac.Fecha.ToString("dd/MM/yyyy"));
                sw.WriteLine();
            }
        }

        private bool EsFacturaValida()
        {
            return txtNuevaFacNro.Text != "";
        }

        private void btnNuevaFacCancelar_Click(object sender, EventArgs e)
        {
            pnlNuevaFactura.Visible = false;
            pnlFacturas.Visible = true;
        }

        #endregion


        #region Reportes

        private void btnBuscarRepCli_Click(object sender, EventArgs e)
        {
            grdRepCli.Rows.Clear();
            
            foreach(var f in facturas)
            {
                if (f.ClienteId == txtRepCli.Text)
                {
                    foreach(var it in f.Items)
                    {
                        grdRepCli.Rows.Add(
                            it.ProductoId,
                            it.Descripcion,
                            it.Cantidad,
                            it.PrecioUnitario,
                            it.FacturaId
                        );
                    }
                }
            }
        }

        private void btnRepProd_Click(object sender, EventArgs e)
        {
            grdRepProd.Rows.Clear();
            
            var list = new List<string>();
            
            foreach (var f in facturas)
            {
                foreach(var it in f.Items)
                {
                    if (it.ProductoId == txtRepProd.Text)
                    {
                        Cliente c = ObtenerClientePorId(f.ClienteId);
                        if (c != null)
                        {
                            // Esto es para no repetir el cliente si 
                            // compro el producto en otras facturas
                            if (!list.Contains(c.ClienteId))
                            {
                                list.Add(c.ClienteId);
                                grdRepProd.Rows.Add(c.ClienteId, c.NombreCompleto);
                            }

                        }
                    }
                }
            }
        }

        private Cliente ObtenerClientePorId(string id)
        {
            foreach (var c in clientes)
            {
                if (c.ClienteId == id)
                {
                    return c;
                }
            }
            return null;
        }

        private void btnRepFac_Click(object sender, EventArgs e)
        {
            grdRepFacCli.Rows.Clear();
            grdRepFacProd.Rows.Clear();

            foreach(var f in facturas)
            {
                if (f.FacturaId == txtRepFac.Text)
                {
                    var c = ObtenerClientePorId(f.ClienteId);

                    grdRepFacCli.Rows.Add(c.ClienteId, c.NombreCompleto);

                    foreach (var it in f.Items)
                    {
                        grdRepFacProd.Rows.Add(
                            it.ProductoId,
                            it.Descripcion,
                            it.Cantidad,
                            it.PrecioUnitario,
                            it.FacturaId
                        );
                    }
                }
            }
        }

        #endregion

        #region Paneles
        private void link_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel link = (LinkLabel)sender;

            string pnlName = (string)link.Tag;

            ActivarPanel(pnlName);
            ActivarLink(link);
        }

        private void ActivarLink(LinkLabel link)
        {
            foreach (var lnk in links)
            {
                lnk.LinkVisited = lnk.Name == link.Name;
            }
        }

        private void ActivarPanel(string pnlName)
        {
            foreach (var pnl in panels)
            {
                pnl.Visible = pnl.Name == pnlName;
            }
        }

        #endregion
    }
}
