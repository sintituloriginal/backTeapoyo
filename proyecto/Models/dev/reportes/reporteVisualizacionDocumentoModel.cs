using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using proyecto.objects.dev.reportes;
using System.Globalization;

namespace proyecto.Models.dev.reportes
{
    public class reporteVisualizacionDocumentoModel : Conexion
    {
        public object cargarDatos(ReporteVisualizacionDocumento request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                SqlCommand cmd = new SqlCommand();

                string query = @"select 
                                    wlvd.fecha_revision as fecha_revision,
	                                dd.codigo_documento as cod_documento,
	                                dd.nombre as nombre,
	                                dd.version as version,
	                                wlvd.tipo_actividad as tipo_actividad,
	                                wus.FULLNAME as FULLNAME
                                    from web_log_vizualizacion_documento wlvd
                                    join web_users wus
                                    ON wlvd.id_usuario = wus.USER_ID
                                    join dev_documento dd
                                    ON dd.indice = wlvd.id_indice
                                    where 1=1";

                if (!String.IsNullOrEmpty(request.codigoDocumento))
                {
                    query += " and dd.codigo_documento = @codDocumento";
                    cmd.Parameters.AddWithValue("@codDocumento", request.codigoDocumento);
                }

                if (!String.IsNullOrEmpty(request.fullNameUsuario))
                {
                    query += " and id_usuario = @id_usuario";
                    cmd.Parameters.AddWithValue("@id_usuario", request.fullNameUsuario);
                }

                if (!String.IsNullOrEmpty(request.tipoActividad))
                {
                    string visualizar = ""; 
                    switch (request.tipoActividad)
                    {
                        case "1":
                            visualizar = "Visualizar";
                            break;
                        case "2":
                            visualizar = "Impresion";
                            break;
                        case "3":
                            visualizar = "Descargado";
                            break;
                    }
                    query += " and tipo_actividad = @tipoActividad";
                    cmd.Parameters.AddWithValue("@tipoActividad", visualizar);
                }

                if (!String.IsNullOrEmpty(request.fechaRevision))
                {
                                
                    query += " and CONVERT(date,wlvd.fecha_revision) BETWEEN @fechaInicio AND @fechaFin";
                    cmd.Parameters.AddWithValue("@fechaInicio", request.fechaRevision);
                    cmd.Parameters.AddWithValue("@fechaFin", request.fechaRevisionFin);
                }
                query += " order by wlvd.fecha_revision DESC, dd.codigo_documento, wus.FULLNAME ASC ";
                cmd.CommandText = query;
                cmd.Connection = conn;
                Console.WriteLine(query);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<ReporteVisualizacionDocumento> lista = new List<ReporteVisualizacionDocumento>();

                while (reader.Read())
                {
                    ReporteVisualizacionDocumento obj = new ReporteVisualizacionDocumento();
                    //obj.ID = reader["USER_ID"].ToString();
                    obj.fechaRevision = reader["fecha_revision"].ToString();
                    obj.codigoDocumento = reader["cod_documento"].ToString();
                    obj.nombreDocumento = reader["nombre"].ToString();
                    obj.versionDocumento = reader["version"].ToString();
                    obj.tipoActividad = reader["tipo_actividad"].ToString();
                    obj.fullNameUsuario = reader["FULLNAME"].ToString();
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

        public string totalVisualizados(ReporteVisualizacionDocumento request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string totalVisualizados = "0";
                SqlCommand cmd = new SqlCommand();
                string query = @"select 
                                    count(1) as totalVisualizados
                                   from web_log_vizualizacion_documento wlvd
                                   join dev_documento dd ON dd.indice = wlvd.id_indice
	                               where wlvd.tipo_actividad = 'Visualizar'";


                if (!String.IsNullOrEmpty(request.codigoDocumento))
                {
                    query += " and dd.codigo_documento = @codDocumento";
                    cmd.Parameters.AddWithValue("@codDocumento", request.codigoDocumento);
                }

                if (!String.IsNullOrEmpty(request.fullNameUsuario))
                {
                    query += " and id_usuario = @id_usuario";
                    cmd.Parameters.AddWithValue("@id_usuario", request.fullNameUsuario);
                }

                if (!String.IsNullOrEmpty(request.fechaRevision))
                {
                    query += " and CONVERT(date,wlvd.fecha_revision) BETWEEN @fechaInicio AND @fechaFin";
                    cmd.Parameters.AddWithValue("@fechaInicio", request.fechaRevision);
                    cmd.Parameters.AddWithValue("@fechaFin", request.fechaRevisionFin);
                }



                cmd.CommandText = query;
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    totalVisualizados = reader["totalVisualizados"].ToString();
                }
                reader.Close();
                return totalVisualizados;
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
        public string totalDescargados(ReporteVisualizacionDocumento request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string totalDescargados = "0";
                SqlCommand cmd = new SqlCommand();
                string query = @"select 
                                    count(1) as totalDescargados
                                   from web_log_vizualizacion_documento wlvd
                                   join dev_documento dd ON dd.indice = wlvd.id_indice
	                               where wlvd.tipo_actividad = 'Descargado'";

                if (!String.IsNullOrEmpty(request.codigoDocumento))
                {
                    query += " and dd.codigo_documento = @codDocumento";
                    cmd.Parameters.AddWithValue("@codDocumento", request.codigoDocumento);
                }

                if (!String.IsNullOrEmpty(request.fullNameUsuario))
                {
                    query += " and id_usuario = @id_usuario";
                    cmd.Parameters.AddWithValue("@id_usuario", request.fullNameUsuario);
                }

                if (!String.IsNullOrEmpty(request.fechaRevision))
                {
                    query += " and CONVERT(date,wlvd.fecha_revision) BETWEEN @fechaInicio AND @fechaFin";
                    cmd.Parameters.AddWithValue("@fechaInicio", request.fechaRevision);
                    cmd.Parameters.AddWithValue("@fechaFin", request.fechaRevisionFin);
                }



                cmd.CommandText = query;
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    totalDescargados = reader["totalDescargados"].ToString();
                }
                reader.Close();
                return totalDescargados;
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

        public string totalImpresos(ReporteVisualizacionDocumento request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string totalImpresos = "0";
                SqlCommand cmd = new SqlCommand();
                string query = @"select 
                                    count(1) as totalDescargados
                                   from web_log_vizualizacion_documento wlvd
                                   join dev_documento dd ON dd.indice = wlvd.id_indice
	                               where wlvd.tipo_actividad = 'Impresion'";

                if (!String.IsNullOrEmpty(request.codigoDocumento))
                {
                    query += " and dd.codigo_documento = @codDocumento";
                    cmd.Parameters.AddWithValue("@codDocumento", request.codigoDocumento);
                }

                if (!String.IsNullOrEmpty(request.fullNameUsuario))
                {
                    query += " and id_usuario = @id_usuario";
                    cmd.Parameters.AddWithValue("@id_usuario", request.fullNameUsuario);
                }

                if (!String.IsNullOrEmpty(request.fechaRevision))
                {
                    query += " and CONVERT(date,wlvd.fecha_revision) BETWEEN @fechaInicio AND @fechaFin";
                    cmd.Parameters.AddWithValue("@fechaInicio", request.fechaRevision);
                    cmd.Parameters.AddWithValue("@fechaFin", request.fechaRevisionFin);
                }



                cmd.CommandText = query;

                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    totalImpresos = reader["totalDescargados"].ToString();
                }
                reader.Close();
                return totalImpresos;
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

        public object listaUsuarios()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = @"SELECT USER_ID as id, FULLNAME as text
                                    FROM web_users";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<filtros> lista = new List<filtros>();

                while (reader.Read())
                {
                    filtros obj = new filtros();

                    obj.value = reader["id"].ToString();
                    obj.label = reader["text"].ToString();

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

        public object listaDocumentos()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = @"SELECT codigo_documento as id, nombre as text from dev_documento";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<filtros> lista = new List<filtros>();

                while (reader.Read())
                {
                    filtros obj = new filtros();

                    obj.value = reader["id"].ToString();
                    obj.label = reader["id"].ToString() + " - " + reader["text"].ToString();

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