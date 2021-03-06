﻿using EntVenta;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatVentas
{
    public class DatTikect
    {
        public int Insertar_Ticket(Ticket t)
        {
            using (SqlConnection conn = new SqlConnection(MasterConnection.connection))
            {
                try
                {
                    int resultado = 0;
                    conn.Open();
                    SqlCommand sc = new SqlCommand("sp_insertarTicket", conn);
                    sc.CommandType = CommandType.StoredProcedure;
                    sc.Parameters.AddWithValue("@Identificadorfiscal", t.Identificador_Fiscal);
                    sc.Parameters.AddWithValue("@direccion", t.Direccion);
                    sc.Parameters.AddWithValue("@provincia", t.Provincia);
                    sc.Parameters.AddWithValue("@moneda", t.Moneda);
                    sc.Parameters.AddWithValue("@agradecimiento", t.Agradecimiento);
                    sc.Parameters.AddWithValue("@paginaweb", t.Pagina_Web);
                    sc.Parameters.AddWithValue("@anuncio", t.Anuncio);
                    sc.Parameters.AddWithValue("@datosfiscales", t.Datos_Fiscales);
                    sc.Parameters.AddWithValue("@pordefecto", t.Default);
                    resultado = sc.ExecuteNonQuery();
                    conn.Close();
                    return resultado;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
            }
        }

        public DataTable Mostrar_FormatoTicket()
        {
            using (SqlConnection conn = new SqlConnection(MasterConnection.connection))
            {
                try
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter("ps_Mostrar_FormatoTicket", conn);
                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public int Editar_FormatoTicket(Ticket t, Empresa E)
        {
            using (SqlConnection conn = new SqlConnection(MasterConnection.connection))
            {
                try
                {
                    int filasAfectadas;

                    SqlCommand sc = new SqlCommand("sp_Editar_FormatoTicket", conn);
                    sc.CommandType = CommandType.StoredProcedure;

                    sc.Parameters.AddWithValue("@identificadorfiscal", t.Identificador_Fiscal);
                    sc.Parameters.AddWithValue("@direccion", t.Direccion);
                    sc.Parameters.AddWithValue("@provincia", t.Provincia);
                    sc.Parameters.AddWithValue("@nombremoneda", t.Moneda);
                    sc.Parameters.AddWithValue("@agradecimiento", t.Agradecimiento);
                    sc.Parameters.AddWithValue("@pagina", t.Pagina_Web);
                    sc.Parameters.AddWithValue("@anuncio", t.Anuncio);
                    sc.Parameters.AddWithValue("@datosfiscales", t.Datos_Fiscales);
                    sc.Parameters.AddWithValue("@pordefecto", t.Default);
                    sc.Parameters.AddWithValue("@nombreempresa", E.Nombre);
                    sc.Parameters.AddWithValue("@logo", E.Logo);

                    conn.Open();
                    filasAfectadas = sc.ExecuteNonQuery();
                    conn.Close();

                    return filasAfectadas;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
            }
        }
    }
}
