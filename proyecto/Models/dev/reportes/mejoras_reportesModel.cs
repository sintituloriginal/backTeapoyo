using System.Data.SqlClient;

using System;
using System.Collections.Generic;
using proyecto.objects.dev.reportes;

namespace proyecto.Models.dev.reportes
{
    public class mejoras_reportesModel : Conexion
    {
       public object cargarDatos()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT " +
                                " u.USER_ID," +
                                " u.FULLNAME," +
                                " u.USERNAME," +
                                " g.NAME as PERFIL," +
                                " u.EMAIL," +
                                " u.ESTADO" +
                                " FROM WEB_USERS u" +
                                " INNER JOIN WEB_GROUPS g ON g.GROUP_ID = u.GROUP_ID";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<data_mejoras_reporte> lista = new List<data_mejoras_reporte>();

                while (reader.Read())
                {
                    data_mejoras_reporte obj = new data_mejoras_reporte();
                    obj.ID = reader["USER_ID"].ToString();
                    obj.FULLNAME = reader["FULLNAME"].ToString();
                    obj.USERNAME = reader["USERNAME"].ToString();
                    obj.PERFIL = reader["PERFIL"].ToString();
                    obj.EMAIL = reader["EMAIL"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
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

        public object cargarGraficos()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {                
                
                                
                graficos obj = new graficos();
                obj.grafico_dona = get_grafico_dona();
                obj.grafico_columna = get_grafico_columna();
                obj.grafico_torta = get_grafico_torta();
                obj.grafico_linea = get_grafico_linea();
                
               
                
                return obj;
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

        public object get_grafico_dona()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT " +
                                " 36 as y," +
                                " 'item' as name," +
                                " 'texto' as legendText ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<data_graficos> lista = new List<data_graficos>();

                while (reader.Read())
                {
                    data_graficos obj = new data_graficos();
                    obj.y = Convert.ToInt32(reader["y"]);
                    obj.name = reader["name"].ToString();
                    obj.legendText = reader["legendText"].ToString();
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
        public object get_grafico_columna()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT " +
                                " 1 as x," +
                                " 1000 as y," +
                                " 'label' as label ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<data_graficos> lista = new List<data_graficos>();

                while (reader.Read())
                {
                    data_graficos obj = new data_graficos();
                    obj.x = Convert.ToInt32(reader["x"]);
                    obj.y = Convert.ToInt32(reader["y"]);
                    obj.label = reader["label"].ToString();
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
        public object get_grafico_torta()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT " +
                                " 10 as y," +
                                " 'indexLabel' as indexLabel ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<data_graficos> lista = new List<data_graficos>();

                while (reader.Read())
                {
                    data_graficos obj = new data_graficos();
                    obj.y = Convert.ToInt32(reader["y"]);
                    obj.indexLabel = reader["indexLabel"].ToString();
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
        public object get_grafico_linea()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT " +
                                " 10 as x," +
                                " 450 as y ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<data_graficos> lista = new List<data_graficos>();

                while (reader.Read())
                {
                    data_graficos obj = new data_graficos();
                    obj.x = Convert.ToInt32(reader["x"]);
                    obj.y = Convert.ToInt32(reader["y"]);
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
        public object filtrar(data_mejoras_reporte datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {

                string query = "SELECT " +
                                " u.USER_ID," +
                                " u.FULLNAME," +
                                " u.USERNAME," +
                                " g.NAME as PERFIL," +
                                " u.EMAIL," +
                                " u.ESTADO" +
                                " FROM WEB_USERS u" +
                                " INNER JOIN WEB_GROUPS g ON g.GROUP_ID = u.GROUP_ID WHERE 1=1";
                if (datos.USERNAME != null)
                {
                    query += " and u.USERNAME = :username ";

                }
                if (datos.PERFIL != null)
                {
                    query += " and g.NAME = :perfil ";

                }
                if (datos.ESTADO != null)
                {
                    query += " and ESTADO = :estado ";

                }

                cmd.Parameters.AddWithValue("username", datos.USERNAME);
                cmd.Parameters.AddWithValue("perfil", datos.PERFIL);
                cmd.Parameters.AddWithValue("estado", datos.ESTADO);


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();

                List<data_mejoras_reporte> lista = new List<data_mejoras_reporte>();

                while (reader.Read())
                {
                    data_mejoras_reporte obj = new data_mejoras_reporte();
                    obj.ID = reader["USER_ID"].ToString();
                    obj.FULLNAME = reader["FULLNAME"].ToString();
                    obj.USERNAME = reader["USERNAME"].ToString();
                    obj.PERFIL = reader["PERFIL"].ToString();
                    obj.EMAIL = reader["EMAIL"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
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