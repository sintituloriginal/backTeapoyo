using System.Data.SqlClient;

using System;
using System.Collections.Generic;
using proyecto.objects.dev.reportes;
using System.Globalization;

namespace proyecto.Models.dev.reportes
{
    public class informeAuditoriaModel : Conexion
    {
       public object cargarDatos(datosInformeAuditoria datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = "";
                if (datos.fechaInicio != null)
                {
                    query += "declare @fechaInicio datetime ";
                    query += "set @fechaInicio = convert(datetime, '" + datos.fechaInicio + "', 103) ";
                }
                //else
                //{
                //    DateTime FirstDay = new DateTime(DateTime.Now.Year, 1, 1);
                //    query += "declare @fechaInicio datetime ";
                //    query += "set @fechaInicio = convert(datetime, '" + FirstDay.ToString("dd-MM-yyyy") + "', 103) ";
                //}
                if (datos.fechaFin != null)
                {
                    query += "declare @fechaFin datetime ";
                    query += "set @fechaFin = convert(datetime, '" + datos.fechaFin + "', 103) ";
                }
                //else
                //{
                //    DateTime LastDay = new DateTime(DateTime.Now.Year, 12, 31);
                //    query += "declare @fechaFin datetime ";
                //    query += "set @fechaFin = convert(datetime, '" + LastDay.ToString("dd-MM-yyyy") + "', 103) ";
                //}
                /* query += "select da.ID_AUDITORIA, dn.COD_NORMA , dn.VERSION , dn.NOMBRE_NORMA , convert(nvarchar(10),da.FECHA, 105) FECHA, wu.FULLNAME ,de.EVALUACION, de.OBSERVACION ,de.ID_PUNTO_NORMATIVO " +
                                " ,dpn.DESC_PUNTO_NORMATIVO " +
                                " , de.TIPO, dn.COD_NORMA from dev_auditoria da " +
                                " inner join dev_normas dn on dn.COD_NORMA = da.COD_NORMA " +
                                " inner join dev_evaluacion de on de.ID_AUDITORIA = da.ID_AUDITORIA " +
                                " inner join dev_punto_normativo dpn on dpn.ID_PUNTO_NORMATIVO = de.ID_PUNTO_NORMATIVO and dpn.COD_NORMA = dn.COD_NORMA " +
                                " inner join web_users wu on wu.USER_ID = da.USER_ID " +
                                " where 1 =1  ";*/
                    query += "select da.ID_AUDITORIA, dn.COD_NORMA , dn.VERSION , dn.NOMBRE_NORMA , convert(nvarchar(10),da.FECHA, 105) FECHA, wu.FULLNAME ,de.EVALUACION, de.ID_PUNTO_NORMATIVO, " +
                               " CASE WHEN de.EVALUACION = 'SI' THEN " +
                               " de.OBSERVACION " +
                               " ELSE " +
                               " dnc.NO_CONFORMIDAD END as OBSERVACION " +
                               " ,dpn.DESC_PUNTO_NORMATIVO " +
                               " ,de.TIPO, dn.COD_NORMA from dev_auditoria da " +
                               " inner join dev_normas dn on dn.COD_NORMA = da.COD_NORMA " +
                               " inner join dev_norma_empresa dne on dne.id_norma = dn.COD_NORMA and dne.id_empresa =  "+datos.id_empresa +" "+
                               " inner join dev_evaluacion de on de.ID_AUDITORIA = da.ID_AUDITORIA " +
                               " inner join dev_punto_normativo dpn on dpn.ID_PUNTO_NORMATIVO = de.ID_PUNTO_NORMATIVO and dpn.COD_NORMA = dn.COD_NORMA " +
                               " inner join web_users wu on wu.USER_ID = da.USER_ID " +
                               " left join dev_no_conformidad dnc on dpn.ID_PUNTO_NORMATIVO = dnc.ID_PUNTO_NORMATIVO " +
                               " and da.ID_AUDITORIA = dnc.ID_AUDITORIA and de.EVALUACION!='SI' " +
                               " where 1 =1  ";
                

                if (datos.norma != null)
                {
                    query += " AND da.COD_NORMA in " + datos.norma + " ";
                }
                if (datos.tipo != null && datos.tipo != "Todo")
                {
                    query += " AND da.TIPO_AUDITORIA in  " + datos.tipo + "";
                }
                if (datos.auditoria != null)
                {
                    query += " AND da.ID_AUDITORIA in " + datos.auditoria + "";
                }
                if (datos.auditor != null)
                {
                    query += " AND da.USER_ID  in " + datos.auditor + "";
                }

                if (datos.fechaInicio != null)
                {
                    query += " AND da.FECHA >= @fechaInicio ";
                }
                if (datos.fechaFin != null) {
                    query += " AND da.FECHA <= @fechaFin ";
                }
                /*query += " group by da.ID_AUDITORIA, dn.NOMBRE_NORMA , dn.VERSION , da.FECHA, wu.FULLNAME ,de.EVALUACION, de.OBSERVACION  ,de.ID_PUNTO_NORMATIVO ,dpn.DESC_PUNTO_NORMATIVO , dn.COD_NORMA , de.TIPO " +
                     " order by da.FECHA asc,CONVERT(INT,REPLACE(de.ID_PUNTO_NORMATIVO,'.','')) asc";*/
                query += " group by da.ID_AUDITORIA, dn.NOMBRE_NORMA , dn.VERSION , da.FECHA, wu.FULLNAME ,de.EVALUACION, de.ID_PUNTO_NORMATIVO ,dpn.DESC_PUNTO_NORMATIVO , dn.COD_NORMA , de.TIPO " +
                         " , dnc.NO_CONFORMIDAD, de.OBSERVACION " +
                        " order by da.FECHA asc,CONVERT(INT,REPLACE(de.ID_PUNTO_NORMATIVO,'.','')) asc";
                

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<respuestaInformeAuditoria> lista = new List<respuestaInformeAuditoria>();

                while (reader.Read())
                {
                    respuestaInformeAuditoria obj = new respuestaInformeAuditoria();
                    obj.ID_AUDITORIA = reader["ID_AUDITORIA"].ToString();
                    obj.NOMBRE_NORMA = reader["NOMBRE_NORMA"].ToString();
                    obj.COD_NORMA = reader["COD_NORMA"].ToString();
                    obj.FECHA = reader["FECHA"].ToString();
                    obj.FULLNAME = reader["FULLNAME"].ToString();
                    obj.EVALUACION = reader["EVALUACION"].ToString();
                    obj.OBSERVACION = reader["OBSERVACION"].ToString();
                    obj.ID_PUNTO_NORMATIVO = reader["ID_PUNTO_NORMATIVO"].ToString();
                    obj.DESC_PUNTO_NORMATIVO = reader["DESC_PUNTO_NORMATIVO"].ToString();
                    obj.TIPO = reader["TIPO"].ToString();
                    obj.VERSION = reader["VERSION"].ToString();
                    obj.PERTENECE = "";

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
       public object titulos(datosInformeAuditoria datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select dpn.COD_NORMA , dpn.ID_PUNTO_NORMATIVO, dpn.DESC_PUNTO_NORMATIVO from dev_punto_normativo dpn"+
                               " inner join dev_norma_empresa dne on dne.id_norma = dpn.COD_NORMA and dne.id_empresa =  "+datos.id_empresa +" "+
                                " where TITULO = 'true'";

                if (datos.norma != null)
                {
                    query += " AND COD_NORMA in " + datos.norma + " ";
                }

                query += "  order by COD_NORMA";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<respuestaInformeAuditoria> lista = new List<respuestaInformeAuditoria>();

                while (reader.Read())
                {
                    respuestaInformeAuditoria obj = new respuestaInformeAuditoria();
                    
                    obj.ID_PUNTO_NORMATIVO = reader["ID_PUNTO_NORMATIVO"].ToString();
                    obj.DESC_PUNTO_NORMATIVO = reader["DESC_PUNTO_NORMATIVO"].ToString();
                    obj.COD_NORMA = reader["COD_NORMA"].ToString();
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

       public object auditorias(datosInformeAuditoria datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = " select da.ID_AUDITORIA , da.COD_NORMA , convert(nvarchar(10),da.FECHA, 105) FECHA, wu.FULLNAME , dn.NOMBRE_NORMA, da.OBSERVACION  from dev_auditoria da " +
                               " inner join dev_normas dn on dn.COD_NORMA = da.COD_NORMA " +
                               " inner join dev_norma_empresa dne on dne.id_norma = dn.COD_NORMA and dne.id_empresa =  "+datos.id_empresa +" "+
                               " inner join dev_evaluacion de on de.ID_AUDITORIA = da.ID_AUDITORIA "+
                               " inner join web_users wu on wu.USER_ID = da.USER_ID " +
                               "  where TIPO_AUDITORIA != 'Reclamo' and TIPO_AUDITORIA != 'Proceso' and da.ESTADO<>'Anulada' ";

                if (datos.tipo != null)
                {
                    query += " AND da.TIPO_AUDITORIA in " + datos.tipo + " ";
                }
                if (datos.norma != null)
                {
                    query += " AND da.COD_NORMA in " + datos.norma + " ";
                }
                if (datos.auditoria != null)
                {
                    query += " AND da.ID_AUDITORIA in " + datos.auditoria + " ";
                }
                query += "group by da.ID_AUDITORIA , da.COD_NORMA ,da.FECHA, da.OBSERVACION, wu.FULLNAME , dn.NOMBRE_NORMA order by  da.ID_AUDITORIA ";
                

               SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<respuestaInformeAuditoria> lista = new List<respuestaInformeAuditoria>();

                while (reader.Read())
                {
                    respuestaInformeAuditoria obj = new respuestaInformeAuditoria();

                    obj.ID_AUDITORIA = reader["ID_AUDITORIA"].ToString();
                    obj.FECHA = reader["FECHA"].ToString();
                    obj.COD_NORMA = reader["COD_NORMA"].ToString();
                    obj.FULLNAME = reader["FULLNAME"].ToString();
                    obj.NOMBRE_NORMA = reader["NOMBRE_NORMA"].ToString();
                    obj.OBSERVACION = reader["OBSERVACION"].ToString();
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
       public object normas(datosInformeAuditoria datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = " select dn.COD_NORMA as id , dn.NOMBRE_NORMA as text from dev_normas dn "+
                            " inner join dev_norma_empresa dne on dne.id_norma = dn.COD_NORMA and dne.id_empresa =  "+datos.id_empresa ;

                

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<filtrosReporte> lista = new List<filtrosReporte>();

                while (reader.Read())
                {
                    filtrosReporte obj = new filtrosReporte();

                    obj.id = reader["id"].ToString();
                    obj.text = reader["text"].ToString();
                    obj.value = false;
                    
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
        public object auditor(datosInformeAuditoria datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = " select wu.USER_ID as id , wu.FULLNAME  as text from web_users wu "+
                            " inner join web_groups wg on wg.GROUP_ID = wu.GROUP_ID and(wg.ADMINISTRADOR = 'true' or wg.ADMINISTRADOR = 'true') "+
                            " inner join web_empresa_user weu on weu.user_id = wu.USER_ID and weu.id_empresa =  "+datos.id_empresa ;



                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<filtrosReporte> lista = new List<filtrosReporte>();

                while (reader.Read())
                {
                    filtrosReporte obj = new filtrosReporte();

                    obj.id = reader["id"].ToString();
                    obj.text = reader["text"].ToString();
                    obj.value = false;

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

        public object auditoriasFiltro(datosInformeAuditoria datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                //string query = " select  da.ID_AUDITORIA as id ,CONCAT(da.ID_AUDITORIA, ' (', convert(varchar(10),da.FECHA)  ,')') as text from dev_auditoria da "+
                //       " join dev_evaluacion de on de.ID_AUDITORIA = da.ID_AUDITORIA where COD_NORMA =  " + datos+ " AND DA.TIPO_AUDITORIA != 'Reclamo' AND DA.TIPO_AUDITORIA != 'Proceso'  group by da.ID_AUDITORIA,da.FECHA";
                string query = " select  da.ID_AUDITORIA as id ,CONCAT(da.ID_AUDITORIA, ' (', convert(nvarchar(10),da.FECHA, 105)  ,')') as text  from dev_auditoria da " +
                       " join dev_evaluacion de on de.ID_AUDITORIA = da.ID_AUDITORIA "+
                       " inner join dev_norma_empresa dne on dne.id_norma = da.COD_NORMA and dne.id_empresa =  "+datos.id_empresa +" "+
                       " where DA.TIPO_AUDITORIA != 'Reclamo' AND DA.TIPO_AUDITORIA != 'Proceso' ";

                if (datos.norma != "" && datos.norma != null)
                {
                    query += " and da.COD_NORMA in  " + datos.norma + " ";
                }
                if (datos.tipo != "" && datos.tipo != null)
                {
                    query += " and da.TIPO_AUDITORIA  in " + datos.tipo + " ";
                }
                query += "group by da.ID_AUDITORIA,da.FECHA";

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<filtrosReporte> lista = new List<filtrosReporte>();

                while (reader.Read())
                {
                    filtrosReporte obj = new filtrosReporte();

                    obj.id = reader["id"].ToString();
                    obj.text = reader["text"].ToString();
                    obj.value = false;
                    
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