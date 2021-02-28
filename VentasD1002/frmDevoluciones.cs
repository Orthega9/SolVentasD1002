﻿using BusVenta;
using DatVentas;
using EntVenta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reportes;
using Telerik.Reporting.Processing;
using System.Management;

namespace VentasD1002
{
    public partial class frmDevoluciones : Form
    {
        private PrintDocument TICKET;
        string serialPC;

        public frmDevoluciones()
        {
            InitializeComponent();
            pnlProcesar.Visible = false;
        }

        private void frmDevoluciones_Load(object sender, EventArgs e)
        {
            txtBuscar.SelectAll();
            txtBuscar.Focus();
            gdvResultado.Visible = false;
            ManagementObject mos = new ManagementObject(@"Win32_PhysicalMedia='\\.\PHYSICALDRIVE0'");
            serialPC = mos.Properties["SerialNumber"].Value.ToString().Trim();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtBuscar.Text) && txtBuscar.Text != "Ingrese el nombre del cliente o el num de comprobante ")
                {
                    gdvResultado.Visible = true;
                    gdvResultado.DataSource = new DatProducto().BuscarVenta(txtBuscar.Text);
                    gdvResultado.Columns[0].Visible = false;
                    DataTablePersonalizado.Multilinea(ref gdvResultado);
                }
                else
                {
                    if (String.IsNullOrEmpty(txtBuscar.Text))
                    {
                        gdvResultado.Visible = false;
                        txtBuscar.Text = "Ingrese el nombre del cliente o el num de comprobante ";
                        txtBuscar.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error en la búsqueda : "+ex.Message, "Error de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gdvResultado_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int oneKey = Convert.ToInt32(gdvResultado.SelectedCells[0].Value);
                lblCliente.Text = gdvResultado.SelectedCells[2].Value.ToString();
                lblFolio.Text = gdvResultado.SelectedCells[3].Value.ToString();
                lblComprobante.Text = gdvResultado.SelectedCells[4].Value.ToString();
                lblFechaVenta.Text = Convert.ToDateTime(gdvResultado.SelectedCells[1].Value).ToString("dd 'de' MMMM 'de' yyyy");

                gdvResultado.Visible = false;
                txtBuscar.ResetText();

                DataTable dt = new DatDetalleVenta().ObtenerDatos_DetalleVenta(oneKey);
                gdvDatos.DataSource = dt;

                DataTablePersonalizado.Multilinea(ref gdvDatos);
                gdvDatos.Columns[1].Visible = false;
                gdvDatos.Columns[2].Visible = false;
                gdvDatos.Columns[3].Visible = false;
                gdvDatos.Columns[4].Visible = false;
                gdvDatos.Columns[5].Visible = false;
                gdvDatos.Columns[11].Visible = false;
                gdvDatos.Columns[12].Visible = false;
                gdvDatos.Columns[13].Visible = false;
                gdvDatos.Columns[14].Visible = false;

                var dtDatos = from dr in dt.AsEnumerable()
                              select new
                              {
                                  MontoTotal = dr.Field<decimal>("Monto_Total"),
                                  Cajero = dr.Field<string>("Nombre"),
                              };

                lblCajero.Text = dtDatos.Select(x => x.Cajero).FirstOrDefault();
                lblMontoTotal.Text = dtDatos.Select(x => x.MontoTotal).FirstOrDefault().ToString(); 

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscar_Click(object sender, EventArgs e)
        {
            txtBuscar.SelectAll();
        }

        private void btnControl_Click(object sender, EventArgs e)
        {
            decimal devuelto = 0;
            foreach (DataGridViewRow dr in gdvDatos.Rows)
            {
                if (Convert.ToBoolean(dr.Cells[0].Value))
                {
                    devuelto += Convert.ToDecimal(dr.Cells[10].Value);
                    pnlProcesar.Visible = true;
                    lblTotalDevuelto.Text = devuelto.ToString();
                }
            }

            if (devuelto == 0)
            {
                MessageBox.Show("Seleccione el producto para continuar con la devolución", "No hay datos seleccionados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnlProcesar.Visible = false;
            lblTotalDevuelto.Text = "0.00";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Venta venta = new Venta();
                foreach (DataGridViewRow dr in gdvDatos.Rows)
                {
                    if (Convert.ToBoolean(dr.Cells[0].Value))
                    {
                        int onekey = Convert.ToInt32(dr.Cells[1].Value);
                       
                        var s4 = dr.Cells[4].Value;
                      
                        var Total = dr.Cells[10].Value;
                        var MontoTotal = dr.Cells[12].Value;
                     
                        string codigo = Convert.ToString(dr.Cells[3].Value);
                        int idVenta = Convert.ToInt32(dr.Cells[4].Value);
                    
                        Venta data = DatVenta.ObtenerVenta(idVenta);
                        

                        venta.Id = data.Id;
                        venta.MontoTotal = data.MontoTotal - Convert.ToDecimal(Total);
                        if (data.Vuelto < 0 )
                        {
                            venta.Vuelto = data.Vuelto + Convert.ToDecimal(Total);
                        }
                        else if(data.Vuelto >= 0 && data.Saldo == 0)
                        {
                            venta.Vuelto = data.Vuelto + Convert.ToDecimal(Total);
                        }

                        if (data.Saldo > 0)
                        {
                            venta.Saldo = data.Saldo - Convert.ToDecimal(Total);
                            venta.Vuelto = venta.Saldo < 0 ? venta.Saldo *(-1) : venta.Saldo;
                            venta.Saldo = venta.Saldo < 0 ? 0 : venta.Saldo;
                            
                        }
                     
                        venta.EstadoPago = venta.Saldo <= 0 ? "PAGADO" : "PENDIENTE";

                        new DatDetalleVenta().EditarDevolucion_DetalleVenta(onekey);
                        DatVenta.EditarDatos_Devoluciones(venta);
                        Producto p = new BusProducto().ObtenerProducto(codigo);
                        if (p.usaInventario == "SI")
                        {
                            string presentancion = Convert.ToString(dr.Cells[7].Value);
                            if (p.precioMayoreo == Convert.ToDecimal(dr.Cells[6].Value) || presentancion.Equals("PQTE") || presentancion.Equals("SIX")
                                || presentancion.Equals("CJA") || presentancion.Equals("BTO") || presentancion.Equals("RJA"))
                            {
                                decimal cantidadVendida = Convert.ToDecimal(p.TotalUnidades * Convert.ToDecimal(dr.Cells[6].Value));
                                new BusProducto().Actualizar_Stock(p.Id, (Convert.ToDecimal(p.stock) + cantidadVendida));
                            }
                            else
                            {
                                new BusProducto().Actualizar_Stock(p.Id, (Convert.ToDecimal(p.stock) + Convert.ToDecimal(dr.Cells[6].Value)));
                            }
                        }

                      

                    }
                }

                DialogResult result = MessageBox.Show("Desea Reimprimir el ticket", "Operación realizada", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ImprimirTicket(venta);
                }

                pnlProcesar.Visible = false;
                lblTotalDevuelto.Text = "0.00";
                gdvDatos.DataSource = null;
                Limpiar_campos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al actualizar los datos : "+ex.Message, "Error de actualización", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImprimirTicket(Venta venta)
        {
            #region TICKET
            string textoNumero = Convertir_NumeroLetra.NumeroATexto(venta.MontoTotal.ToString());
            // DataTable dt = DatDetalleVenta.ObtenerDatos_Ticket(venta.Id, textoNumero);
            ParametrosReporte reporte = DatVenta.Consultar_Ticket_Parametro(venta.Id);
            reporte.LetraNumero = textoNumero;

            //rptTicket rptTicket = new rptTicket();
            ReportTicket rptTicket = new ReportTicket();
            rptTicket.tbTicket.DataSource = reporte.lstDetalleVenta;
            rptTicket.DataSource = reporte;
            reportViewer1.Report = rptTicket;
            reportViewer1.RefreshReport();
            #endregion

            try
            {
                string impresora = DatBox.Obtener_ImpresoraTicket(serialPC, "TICKET");
                TICKET = new PrintDocument();
                TICKET.PrinterSettings.PrinterName = impresora;

                if (TICKET.PrinterSettings.IsValid)
                {
                    PrinterSettings printerSettings = new PrinterSettings();
                    printerSettings.PrinterName = impresora;

                    ReportProcessor reportProcessor = new ReportProcessor();
                    reportProcessor.PrintReport(reportViewer1.ReportSource, printerSettings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir el ticket : " + ex.Message, "Error de impresión", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Limpiar_campos()
        {
            lblCajero.Text = "";
            lblCliente.Text = "";
            lblComprobante.Text = "";
            lblFechaVenta.Text = "";
            lblFolio.Text = "";
            lblMontoTotal.Text = "";
            lblTotalDevuelto.Text = "";
            
        }
    }
}
