using System.Data.SqlClient;
using proyecto.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proyecto.Models.web
{
    public class filtrosModel : Conexion
    {
        public object cargarFiltros(rutaRequest request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT " +
                                " ID," +
                                " V_MODEL," +
                                " LINK," +
                                " ID_TIPO_FILTRO," +
                                " LABEL," +
                                " BUSQUEDA_AVANZADA," +
                                " ESTADO, " +
                                " QUERY, " +
                                " DISABLED, "+
                                " DEPENDIENTE " +
                                " FROM WEB_FILTROS" +
                                " WHERE LINK = '"+request.ruta+"' "+
                                " AND ESTADO = 'Habilitado' " +
                                " AND BUSQUEDA_AVANZADA = '"+request.busq_avanzada+"' "+
                                " ORDER BY ORDEN ASC ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<data_filtros> lista = new List<data_filtros>();

                while (reader.Read())
                {
                    data_filtros obj = new data_filtros();
                    obj.id = reader["ID"].ToString();
                    obj.v_model = reader["V_MODEL"].ToString();
                    obj.link = reader["LINK"].ToString();
                    obj.id_tipo_filtro = reader["ID_TIPO_FILTRO"].ToString();
                    obj.label = reader["LABEL"].ToString();
                    obj.busqueda_avanzada = reader["BUSQUEDA_AVANZADA"].ToString();
                    obj.estado = reader["ESTADO"].ToString();
                    obj.disabled = reader["DISABLED"].ToString();
                    obj.dependiente = reader["DEPENDIENTE"].ToString();
                    if (obj.id_tipo_filtro == "2" || obj.id_tipo_filtro == "3" || obj.id_tipo_filtro == "4" )
                    {
                        if (obj.dependiente == "no")
                        {
                            obj.items = items_filtros(reader["QUERY"].ToString());
                        }else
                        {
                            List<item_filtros> lista_dos = new List<item_filtros>();
                            item_filtros obj_dos = new item_filtros();
                            obj_dos.id = "";
                            obj_dos.text = "";
                            lista_dos.Add(obj_dos);
                            obj.items = lista_dos;
                        }
                        
                    }
                    lista.Add(obj);
                }
                reader.Close();
                return lista;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public object items_filtros(string query_filtro)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = query_filtro;

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<item_filtros> lista = new List<item_filtros>();

                while (reader.Read())
                {
                    item_filtros obj = new item_filtros();
                    obj.id = reader["ID"].ToString();
                    obj.text = reader["TEXTO"].ToString();
                    lista.Add(obj);
                }
                reader.Close();
                return lista;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}