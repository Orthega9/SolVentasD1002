﻿using BusVenta;
using DatVentas;
using EntVenta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VentasD1002
{
    public partial class frmABProducto : Form
    {
        public frmABProducto()
        {
            InitializeComponent();
        }

        public static string TipoPresentacion;
        public static string Categoria;
        public static string PresentacionMenudeo;
        public static bool EsNuevo;

        private int idUsuario;
        private int idCaja;
        private string serialPC;
        
        private void frmABProducto_Load(object sender, EventArgs e)
        {
            panelCategoria.Visible = false;
            pnlActualizarStock.Visible = false;

            if (CheckInventarios.Checked == true)
            {
                PANELINVENTARIO.Visible = true;
            }
            else
            {
                PANELINVENTARIO.Visible = false;
            }

            List<string> lst = new DatProducto().listadoActualizacion();
            ManagementObject mos = new ManagementObject(@"Win32_PhysicalMedia='\\.\PHYSICALDRIVE0'");

            serialPC = mos.Properties["SerialNumber"].Value.ToString().Trim();
            //idUsuario = new DatCatGenerico().Obtener_InicioSesion( EncriptarTexto.Encriptar(serialPC) );
            idUsuario = new BusUser().ObtenerUsuario(EncriptarTexto.Encriptar(serialPC)).Id;
            idCaja = new DatBox().Obtener_CajaSerial(serialPC);

            LlenarCategorias();
        }

        private void CheckInventarios_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckInventarios.Checked)
            {
                PANELINVENTARIO.Visible = true;
            }
            else
            {
                PANELINVENTARIO.Visible = false;
            }
        }

        private void LlenarCategorias()
        {
            try
            {
                DataTable dtPresentacion = DatVentas.DatCatGenerico.ListarCat_TipoPresentacion();
                cboPresentacion.DataSource = dtPresentacion;
                cboPresentacion.DisplayMember = "Nombre";
                cboPresentacion.ValueMember = "Id_TipoPresentacion";
                if (!EsNuevo)
                {
                    cboPresentacion.SelectedValue = TipoPresentacion;
                }
                

                DataTable dtPresentacionMenuedo = DatVentas.DatCatGenerico.ListarCat_TipoPresentacion();

                cboPresentacionMenudeo.DataSource = dtPresentacionMenuedo;
                cboPresentacionMenudeo.DisplayMember = "Nombre";
                cboPresentacionMenudeo.ValueMember = "Id_TipoPresentacion";
                if (!EsNuevo)
                {
                    cboPresentacionMenudeo.Text = PresentacionMenudeo;
                }
                

                DataTable dtProducto = DatVentas.DatCatGenerico.ListarCat_Producto();
                cboCategoria.DataSource = dtProducto;
                cboCategoria.DisplayMember = "Nombre";
                cboCategoria.ValueMember = "Id_CatProducto";
                if (!EsNuevo)
                {
                    cboCategoria.SelectedValue = Categoria;
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerarCodigo_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int numero = random.Next(100000, 200000);
            txtcodigodebarras.Text = "AJ" + numero.ToString();
        }

        private void pbCatPresentacion_Click(object sender, EventArgs e)
        {
            txtNombreCategoria.Clear();
            txtDescCategoria.Clear();
            lblDescCategoria.Text = "Nombre Corto:";
            lblTituloCategorias.Text = "Tipo de Presentación";
            panelCategoria.Visible = true;
        }

        private void btnAgregarCategorias_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNombreCategoria.Text == "" || txtDescCategoria.Text == "")
                {
                    MessageBox.Show("Favor de llenar los dos campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    int valor = lblTituloCategorias.Text.Equals("Tipo de Presentación") ? 2 : 1;
                    CatalogoGenerico c = new CatalogoGenerico();
                    c.Nombre = txtNombreCategoria.Text;
                    c.Descripcion = txtDescCategoria.Text;
                    new BusCatGenerico().AgregarCategoriasGenericas(c, valor);
                    LlenarCategorias();
                    panelCategoria.Visible = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panelCategoria.Visible = false;
            txtDescCategoria.Clear();
            txtNombreCategoria.Clear();
        }

        private void catProducto_Click(object sender, EventArgs e)
        {
            txtNombreCategoria.Clear();
            txtDescCategoria.Clear();
            panelCategoria.Visible = true;
            lblTituloCategorias.Text = "Nueva Categoría";
            lblDescCategoria.Text = "Descripción:";
        }

        private void checkActualizarStock_CheckedChanged(object sender, EventArgs e)
        {
            Check_ActualizarStock();
        }

        private void Check_ActualizarStock()
        {
            if (checkActualizarStock.Checked == true)
            {
                pnlActualizarStock.Visible = true;
                txtStockActualizado.Text = txtstock2.Text;
                txtPiezasActualizadas.Text = lblPiezasStock.Text;
            }
            else
            {
                pnlActualizarStock.Visible = false;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            decimal stock = String.IsNullOrEmpty(txtStockActualizar.Text) ? 0 : Convert.ToDecimal(txtStockActualizar.Text);
            decimal StockActual = Convert.ToDecimal(txtStockActualizado.Text);

            txtStockActualizado.Text = (StockActual + stock).ToString();

            decimal unidades = String.IsNullOrEmpty(txtPiezasActualizar.Text) ? 0 : Convert.ToDecimal(txtPiezasActualizar.Text);
            decimal unidadActual = Convert.ToDecimal(txtPiezasActualizadas.Text);

            txtPiezasActualizadas.Text = (unidadActual + unidades).ToString();

            txtStockActualizar.Clear();
            txtPiezasActualizar.Clear();
        }

        private void btnCancelarActualizacionStock_Click(object sender, EventArgs e)
        {
            pnlActualizarStock.Visible = false;
            txtPiezasActualizar.Clear();
            txtStockActualizar.Clear();
            txtStockActualizado.Clear();
            txtPiezasActualizar.Clear();
            checkActualizarStock.Checked = false;
        }

        private void btnActualizarStock_Click(object sender, EventArgs e)
        {
            txtstock2.Text = txtStockActualizado.Text;
            lblPiezasStock.Text = txtPiezasActualizadas.Text;

            pnlActualizarStock.Visible = false;
            txtPiezasActualizar.Clear();
            txtStockActualizar.Clear();
            txtStockActualizado.Clear();
            txtPiezasActualizar.Clear();
            checkActualizarStock.Checked = false;
        }

        private void cboPresentacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTipoPresentacion.Text = cboPresentacion.Text;
            lblAuxHay.Text = cboPresentacion.Text.ToUpper() + 'S';
            lblAuxHay2.Text = cboPresentacion.Text.ToUpper() + 'S';
        }

        public void LimpiarCotroles()
        {
            txtcodigodebarras.Clear();
            txtdescripcion.Clear();
            txtpreciomayoreo.Clear();
            txtPMMayoreo.Clear();
            txtPMenudeo.Clear();
            porunidad.Checked = false;
            agranel.Checked = false;
            CheckInventarios.Checked = false;
            txtstockminimo.Clear();
            txtstock2.Clear();
            No_aplica_fecha.Checked = false;
            cboCategoria.SelectedValue = 0;
            cboPresentacion.SelectedValue = 0;
            cboPresentacionMenudeo.SelectedValue = 0;
            txtPresentacion.Clear();
            txtApartirDe.Clear();
            lblIdProducto.Text = "";
            txtTotalUnidades.Clear();
            lblPiezasStock.Text = "0";
            TipoPresentacion = String.Empty;
            Categoria = String.Empty;
            PresentacionMenudeo = String.Empty;

        }

        private void AgregarProducto_Click(object sender, EventArgs e)
        {
            if (lblIdProducto.Text == "")
            {
                if (txtcodigodebarras.Text == string.Empty | txtdescripcion.Text == "" | txtPMenudeo.Text == "" | txtPMMayoreo.Text == "" || cboPresentacion.SelectedValue.Equals("") ||
                     txtApartirDe.Text.Equals(""))
                {
                    MessageBox.Show("Favor de llenar todos los campos ", "Importante", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    InsertarProducto();
                }
            }
            else
            {
                ActualizarProducto();
            }
        }

        private void InsertarProducto()
        {
            try
            {
                Producto p = new Producto();
                p.codigo = txtcodigodebarras.Text;
                p.Descripcion = txtdescripcion.Text;
                p.Presentacion = txtPresentacion.Text;
                p.precioMenudeo = Convert.ToDecimal(txtPMenudeo.Text);
                p.precioMMayoreo = Convert.ToDecimal(txtPMMayoreo.Text);
                p.APartirDe = Convert.ToDecimal(txtApartirDe.Text);
                p.precioMayoreo = Convert.ToDecimal(txtpreciomayoreo.Text);
                p.IdTipoPresentacion = Convert.ToInt32(cboPresentacion.SelectedValue);
                p.IdCategoria = Convert.ToInt32(cboCategoria.SelectedValue);
                p.TotalUnidades = Convert.ToDecimal(txtTotalUnidades.Text);
                p.Estado = true;
                if (porunidad.Checked == true) p.seVendeA = "UNIDAD";
                if (agranel.Checked == true) p.seVendeA = "GRANEL";

                string _pMenuedo = cboPresentacionMenudeo.Text;
                p.PresentacionMenudeo = _pMenuedo.Equals("--Seleccione--") ? "PIEZA" : _pMenuedo;

                if (PANELINVENTARIO.Visible == true)
                {
                    p.usaInventario = "SI";
                    p.stockMinimo = Convert.ToDecimal(txtstockminimo.Text) * Convert.ToDecimal(txtTotalUnidades.Text);
                    decimal _totalStock = (Convert.ToDecimal(txtstock2.Text) * Convert.ToDecimal(txtTotalUnidades.Text)) + Convert.ToDecimal(lblPiezasStock.Text); ;
                    p.stock = _totalStock.ToString(); ;

                    if (No_aplica_fecha.Checked == true)
                    {
                        p.Caducidad = "NO APLICA";
                    }

                    if (No_aplica_fecha.Checked == false)
                    {
                        p.Caducidad = txtfechaoka.Text;
                    }
                }
                if (PANELINVENTARIO.Visible == false)
                {
                    p.usaInventario = "NO";
                    p.stockMinimo = 0;
                    p.Caducidad = "NO APLICA";
                    p.stock = "ILIMITADO";
                }
                Kardex kardex = new Kardex();
                kardex.Fecha = DateTime.Today;
                kardex.Motivo = "Registro inicial de producto";
                kardex.Cantidad = txtstock2.Text == string.Empty ? Convert.ToDecimal(0) : Convert.ToDecimal(txtstock2.Text);
                kardex.Id_Usuario = idUsuario;
                kardex.Tipo = "ENTRADA";
                kardex.Estado = "CONFIRMADO";
                kardex.Id_Caja = idCaja;
                // p.kardex = kardex;

                new BusProducto().AgregarProducto(p, kardex);
                MessageBox.Show("Producto agregado correctamente", "Operacion Realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult result = MessageBox.Show("Desea agregar un nuevo producto", "Nuevo Producto", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {

                    LimpiarCotroles();
                }
                else
                {
                    LimpiarCotroles();
                    this.Dispose();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarProducto()
        {
            try
            {
                Producto p = new Producto();
                p.codigo = txtcodigodebarras.Text;
                p.Descripcion = txtdescripcion.Text;
                p.Presentacion = txtPresentacion.Text;
                p.precioMenudeo = Convert.ToDecimal(txtPMenudeo.Text);
                p.precioMMayoreo = Convert.ToDecimal(txtPMMayoreo.Text);
                p.APartirDe = Convert.ToDecimal(txtApartirDe.Text);
                p.precioMayoreo = Convert.ToDecimal(txtpreciomayoreo.Text);
                p.IdTipoPresentacion = Convert.ToInt32(cboPresentacion.SelectedValue);
                p.IdCategoria = Convert.ToInt32(cboCategoria.SelectedValue);
                p.TotalUnidades = Convert.ToDecimal(txtTotalUnidades.Text);
                p.Estado = true;
                //p.Caducidad = (No_aplica_fecha.Checked == true) ?  "NO APLICA" : txtfechaoka.Text;
                if (porunidad.Checked == true) p.seVendeA = "UNIDAD";
                if (agranel.Checked == true) p.seVendeA = "GRANEL";
                // p.usaInventario = (CheckInventarios.Checked == true) ? "SI" : "NO";
                // p.stockMinimo = Convert.ToDecimal(txtstockminimo.Text);
                //p.stock = txtstock2.Text;
                p.Id = Convert.ToInt32(lblIdProducto.Text);

                if (PANELINVENTARIO.Visible == true)
                {
                    p.usaInventario = "SI";
                    p.stockMinimo = Convert.ToDecimal(txtstockminimo.Text) * Convert.ToDecimal(txtTotalUnidades.Text);
                    decimal _totalStock = (Convert.ToDecimal(txtstock2.Text) * Convert.ToDecimal(txtTotalUnidades.Text)) + Convert.ToDecimal(lblPiezasStock.Text);
                    p.stock = _totalStock.ToString(); ;

                    if (No_aplica_fecha.Checked == true)
                    {
                        p.Caducidad = "NO APLICA";
                    }

                    if (No_aplica_fecha.Checked == false)
                    {
                        p.Caducidad = txtfechaoka.Text;
                    }
                }
                if (PANELINVENTARIO.Visible == false)
                {
                    p.usaInventario = "NO";
                    p.stockMinimo = 0;
                    p.Caducidad = "NO APLICA";
                    p.stock = "ILIMITADO";
                }

                string _pMenudeo = cboPresentacionMenudeo.Text;
                p.PresentacionMenudeo = _pMenudeo.Equals("--Seleccione--") ? "PIEZA" : _pMenudeo;

                new BusProducto().ActualizarProducto(p);
                MessageBox.Show("Producto actualizado correctamente", "Operacion Realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DatProducto.Agregar_ActualizacionProducto(txtcodigodebarras.Text);
                frmProductos.producto = p.Descripcion;
                LimpiarCotroles();
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCotroles();
            this.Dispose();
        }

       
    }
}
