using System.Data.SqlClient;
using proyecto.objects.dev.gestores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Security.Claims;
using System.Threading;
using System.Data.OleDb;
using System.Data;
using System.Web.Configuration;
using System.Web;
using System.IO;
using System.Globalization;
using proyecto.Models.menu;

namespace proyecto.Models.dev.gestores
{
    public class normaModel : Conexion
    {
        public object cargaNorma(int norma)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select COD_NORMA , VERSION, NOMBRE_NORMA,DESCRIPCION_NORMA, FECHA_ACTUALIZACION,ESTADO from dev_normas where COD_NORMA = "+norma+" ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<datosNorma> lista = new List<datosNorma>();
              
                while (reader.Read())
                {
                    datosNorma obj = new datosNorma();
                    obj.COD_NORMA = reader["COD_NORMA"].ToString();
                    obj.VERSION = reader["VERSION"].ToString();
                    obj.NOMBRE_NORMA = reader["NOMBRE_NORMA"].ToString();
                    obj.DESCRIPCION_NORMA = reader["DESCRIPCION_NORMA"].ToString();
                    obj.FECHA_ACTUALIZACION = reader["FECHA_ACTUALIZACION"].ToString();
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
        public object usuarios(string id_empresa)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = @"select wu.USER_ID as id, wu.FULLNAME +' ('+wu.Email+')' as text  from web_users wu 
                                inner join web_empresa_user weu on weu.USER_ID = wu.USER_ID and weu.ID_EMPRESA = "+id_empresa;


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<responsables> lista = new List<responsables>();

                while (reader.Read())
                {
                    responsables obj = new responsables();
                   
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
        public object datosusuarios()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var GROUP_ID = identity.Claims.Where(c => c.Type == "group_id")
                                   .Select(c => c.Value).SingleOrDefault();


                string query = "select ADMINISTRADOR , AUDITOR from web_groups where  GROUP_ID = "+ GROUP_ID + " ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<datos_perfil> lista = new List<datos_perfil>();

                while (reader.Read())
                {
                    datos_perfil obj = new datos_perfil();

                    obj.ADMINISTRADOR = reader["ADMINISTRADOR"].ToString();
                    obj.AUDITOR = reader["AUDITOR"].ToString();

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

        public Boolean hayDatosDefinitivos(int norma)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = "";
                query = "select count(1) as cantidad from dev_punto_normativo where COD_NORMA = @COD_NORMA";

                SqlCommand cmd2 = new SqlCommand(query);
                cmd2.Parameters.AddWithValue("COD_NORMA", norma);
                cmd2.Connection = conn;
                conn.Open();
                SqlDataReader reader2 = cmd2.ExecuteReader();
                int cantidad = 0;

                while (reader2.Read())
                {
                    cantidad = Convert.ToInt32(reader2["cantidad"].ToString());
                }
                reader2.Close();


                if (cantidad > 0)
                {
                    return true;
                }else
                {
                    return false;
                }
                
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

        public object cargaInicialTemporal(int norma)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "";
                query = "select ID_TEMPORAL_NORMA, COD_NORMA, ID_PUNTO_NORMATIVO, DESC_PUNTO_NORMATIVO, RELEVANCIA from dev_temporal_norma where COD_NORMA = @COD_NORMA ";
                
                SqlCommand cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("COD_NORMA", norma);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<norma_excel> lista = new List<norma_excel>();
                Boolean es_titulo = false;
                Boolean es_evaluable = false;
                
                while (reader.Read())
                {
                    norma_excel obj = new norma_excel();

                    if (reader["RELEVANCIA"].ToString() == "")
                    {
                        es_titulo = true;
                    }else
                    {
                        es_titulo = false;
                    }

                    if (reader["RELEVANCIA"].ToString().ToUpper() == "S" || reader["RELEVANCIA"].ToString().ToUpper() == "N")
                    {
                        es_evaluable = true;
                    }else
                    {
                        es_evaluable = false;
                    }

                    obj.respuesta = "";
                    obj.numero = reader["ID_PUNTO_NORMATIVO"].ToString();
                    obj.requisito = reader["DESC_PUNTO_NORMATIVO"].ToString();
                    obj.v_model = "";
                    obj.relevancia = reader["RELEVANCIA"].ToString().ToUpper();
                    obj.titulo = es_titulo;
                    obj.es_evaluable = es_evaluable;
                    obj.responsable = "";
                    obj.comentario = "";
                    obj.resumen = "";
                    obj.auditor = "";
                    obj.fecha = "";
                    obj.id_auditoria = "";

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
        public object cargaInicial(int norma)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "";
                query= "select count(ID_AUDITORIA) Cantidad , ID_AUDITORIA from dev_auditoria where ESTADO = 'Habilitado' and COD_NORMA = " + norma+ " group by ID_AUDITORIA ";

                SqlCommand cmd2 = new SqlCommand(query);
                cmd2.Connection = conn;
                conn.Open();
                SqlDataReader reader2 = cmd2.ExecuteReader();
                int cantidad = 0;
                string id_auditoria = "";
                
                while (reader2.Read())
                {
                    cantidad = Convert.ToInt32(reader2["Cantidad"].ToString());
                    id_auditoria = reader2["ID_AUDITORIA"].ToString();
                }
                reader2.Close();

                /*query = "select * from (select dpn.INDICE,dpn.ID_PUNTO_NORMATIVO, dpn.COD_NORMA,dpn.DESC_PUNTO_NORMATIVO, ";

                if(cantidad == 0)
                {
                    query += " dpn.ULTIMA_EVALUACION , ";
                }else
                {
                    query += " case when de.EVALUACION IS not null or dpn.ULTIMA_EVALUACION = 'NO' then dpn.ULTIMA_EVALUACION else '' end ULTIMA_EVALUACION , ";
                }
                


                query += " dpn.EVALUABLE, dpn.OBSERVACION AS ULTIMA_OBSERVACION," +
                               " dpn.TITULO, dpn.RELEVANCIA , isnull(de.OBSERVACION, '') OBSERVACION , " +
                               " ISNULL(wu.FULLNAME + ' (' + wu.Email + ')', '') RESPONSABLE, " +
                               "  COUNT(dnw.ID_WORKFLOW)   AS respuesta ," +
                               " convert(varchar(100),dpn.DESC_PUNTO_NORMATIVO) as DESC_RESUMEN, " +
                               " ISNULL(wus.FULLNAME + ' (' + wus.Email + ')', '') auditor, " +
                               " convert(nvarchar(10),de.FECHA, 105) as fecha_evalu ,dv.ID_AUDITORIA " +
                               " from dev_punto_normativo dpn ";

                if (cantidad == 0)
                {
                    query += " left join dev_auditoria dv on dpn.COD_NORMA = dv.COD_NORMA and dv.ESTADO = 'Habilitado' ";
                }
                else
                {
                    query += " LEFT JOIN dev_auditoria dv 	ON dpn.COD_NORMA = dv.COD_NORMA		AND dv.ESTADO = 'Habilitado'  and dv.ID_AUDITORIA = "+ id_auditoria + "   ";
                }



                query += " left join dev_evaluacion de on de.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO and de.ID_AUDITORIA = dv.ID_AUDITORIA " +
                               " left join dev_no_conformidad dnc on dnc.ID_AUDITORIA = dv.ID_AUDITORIA and dnc.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO " +
                               " left join  web_users wu on wu.USER_ID = dnc.USER_RESP " +
                               " left join  web_users wus on wus.USER_ID = de.USER_ID " +
                               " LEFT JOIN dev_nc_workflow dnw on dnw.ID_AUDITORIA =  dv.ID_AUDITORIA and dnw.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO " +
                               " where dpn.COD_NORMA = " + norma + " and dpn.ULTIMA_EVALUACION != 'NO' " +
                               " group by dpn.ID_PUNTO_NORMATIVO " +
                               " ,dpn.COD_NORMA " +
                               " ,dpn.DESC_PUNTO_NORMATIVO " +
                               " ,dpn.ULTIMA_EVALUACION " +
                               " ,dpn.EVALUABLE " +
                               " ,dpn.TITULO ,dpn.OBSERVACION" +
                               " ,dpn.RELEVANCIA " +
                               " ,isnull(de.OBSERVACION, '') " +
                               " ,ISNULL(wu.FULLNAME + ' (' + wu.Email + ')', ''),ISNULL(wus.FULLNAME + ' (' + wus.Email + ')', ''),de.FECHA,de.EVALUACION,dv.ID_AUDITORIA, dpn.INDICE  " +                               
                               " union " +
                               " SELECT dpn.INDICE," +
                               " dpn.ID_PUNTO_NORMATIVO " +
                               " ,dpn.COD_NORMA " +
                               " ,dpn.DESC_PUNTO_NORMATIVO " +
                               " ,dpn.ULTIMA_EVALUACION " +
                               " ,dpn.EVALUABLE ,dpn.OBSERVACION AS ULTIMA_OBSERVACION" +
                               " ,dpn.TITULO " +
                               " ,dpn.RELEVANCIA " +
                               " ,dnc.NO_CONFORMIDAD OBSERVACION " +
                               " , wu.FULLNAME + ' (' + wu.Email + ')' RESPONSABLE " +
                               " , COUNT(dnw.ID_WORKFLOW)  AS respuesta " +
                               " , convert(varchar(100),dpn.DESC_PUNTO_NORMATIVO) as DESC_RESUMEN " +
                               " ,ISNULL(wus.FULLNAME + ' (' + wus.Email + ')', '') auditor " +
                               " ,CONVERT(NVARCHAR(10), de.FECHA, 105) AS fecha_evalu ,dv.ID_AUDITORIA" +
                               " FROM " +
                               " dev_normas dn " +
                               " LEFT JOIN dev_auditoria dv ON dn.COD_NORMA = dv.COD_NORMA AND dv.ESTADO = 'Habilitado'" +
                               " left join dev_evaluacion de on dv.ID_AUDITORIA = de.ID_AUDITORIA " +
                               " LEFT JOIN dev_no_conformidad dnc    ON dnc.ID_AUDITORIA = dv.ID_AUDITORIA and dnc.ESTADO <> 'Aprobado' " +
                               " join dev_punto_normativo dpn on dpn.COD_NORMA = dn.COD_NORMA and de.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO and dnc.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO " +
                               " LEFT JOIN web_users wu  ON wu.USER_ID = dnc.USER_RESP " +
                               " LEFT JOIN web_users wus	ON wus.USER_ID = de.USER_ID " +
                               " LEFT JOIN dev_nc_workflow dnw	ON dnw.ID_AUDITORIA = dv.ID_AUDITORIA	AND dnw.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO " +
                               " WHERE dpn.COD_NORMA = " + norma + " and dpn.ULTIMA_EVALUACION = 'NO' " +
                               " GROUP BY dpn.ID_PUNTO_NORMATIVO " +
                               " ,dpn.COD_NORMA " +
                               " ,dpn.DESC_PUNTO_NORMATIVO " +
                               " ,dpn.ULTIMA_EVALUACION " +
                               " ,dpn.EVALUABLE " +
                               " ,dpn.TITULO,dpn.OBSERVACION " +
                               " ,dpn.RELEVANCIA " +
                               " ,dnc.NO_CONFORMIDAD " +
                               " ,wu.FULLNAME + ' (' + wu.Email + ')',ISNULL(wus.FULLNAME + ' (' + wus.Email + ')', '') ,de.FECHA ,dv.ID_AUDITORIA, dpn.INDICE) tabla ";
                               
                */

                
                if (cantidad > 0)
                {
                    //query += " LEFT JOIN dev_auditoria DV ON DV.COD_NORMA = dn.COD_NORMA   and DV.ID_AUDITORIA = " + id_auditoria + "   ";
                    //query += " left join dev_auditoria dv on dpn.COD_NORMA = dv.COD_NORMA and dv.ESTADO = 'Habilitado' ";
                    query = " SELECT  " +
                            " DPN.INDICE ," +
                            " DPN.ID_PUNTO_NORMATIVO,  " +
                            " DPN.COD_NORMA,  " +
                            " DPN.DESC_PUNTO_NORMATIVO,  " +
                            " CASE WHEN DPN.ULTIMA_EVALUACION = 'NO' THEN " +
                            " DPN.ULTIMA_EVALUACION " +
                            " ELSE " +
                            " DE.EVALUACION END AS ULTIMA_EVALUACION, " +
                            " DPN.EVALUABLE, " +
                            " CASE WHEN DPN.ULTIMA_EVALUACION = 'NO' THEN " +
                            " DPN.OBSERVACION ELSE " +
                            " DE.OBSERVACION END AS ULTIMA_OBSERVACION, " +
                            " DPN.TITULO,  " +
                            " DPN.RELEVANCIA,  " +
                            " DPN.CANTIDAD_NC_ACTIVAS,  " +
                            " DPN.ULTIMA_ID_NC,  " +
                            " DPN.OBSERVACION ULTIMA_OBSERVACION,  " +
                            " convert(varchar(100),dpn.DESC_PUNTO_NORMATIVO) as DESC_RESUMEN,  " +
                            " CONVERT(NVARCHAR(10), DV.FECHA, 105) FECHA_EVALU,  " +
                            " ISNULL(WU.FULLNAME + ' (' + WU.Email + ')', '') RESPONSABLE ,  " +
                            " ISNULL(WU2.FULLNAME + ' (' + WU2.Email + ')',  '') AUDITOR ,  " +
                            " COUNT(dnw.ID_WORKFLOW)  AS RESPUESTA,  " +
                            " convert(varchar(100), dpn.DESC_PUNTO_NORMATIVO) as DESC_RESUMEN,  " +
                            " MAX(dv.ID_AUDITORIA) ID_AUDITORIA " +
                            " FROM dev_normas dn  " +
                            " INNER JOIN dev_punto_normativo DPN ON DPN.COD_NORMA = dn.COD_NORMA " +
                            " LEFT JOIN dev_auditoria DV ON DV.COD_NORMA = dn.COD_NORMA   and DV.ID_AUDITORIA =  " + id_auditoria + "   "+
                            " LEFT JOIN dev_evaluacion DE ON DE.ID_AUDITORIA = DV.ID_AUDITORIA AND DE.ID_PUNTO_NORMATIVO = DPN.ID_PUNTO_NORMATIVO  " +
                            " LEFT JOIN dev_no_conformidad DNC ON DNC.ID_NC = DPN.ULTIMA_ID_NC AND DNC.ESTADO!='Aprobado'  " +
                            " LEFT JOIN web_users WU ON WU.USER_ID = DNC.USER_RESP  " +
                            " LEFT JOIN web_users WU2 ON WU2.USER_ID = DV.USER_ID  " +
                            " LEFT JOIN dev_nc_workflow dnw    ON dnw.ID_NC = dpn.ULTIMA_ID_NC AND dnw.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO AND dnw.ESTADO != 'Aprobado' " +
                            //" LEFT JOIN dev_nc_workflow dnw    ON dnw.ID_NC = dpn.ULTIMA_ID_NC AND dnw.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO and dnw.ID_AUDITORIA=DV.ID_AUDITORIA" +
                            " WHERE dn.COD_NORMA = " + norma + " "+
                            " GROUP BY DPN.INDICE,DPN.ID_PUNTO_NORMATIVO,DPN.COD_NORMA,DPN.DESC_PUNTO_NORMATIVO,DPN.ULTIMA_EVALUACION,DPN.EVALUABLE,DPN.OBSERVACION,DPN.TITULO,DPN.RELEVANCIA,  " +
                            " DPN.CANTIDAD_NC_ACTIVAS,DPN.ULTIMA_ID_NC,WU.FULLNAME,WU.Email,WU2.FULLNAME,WU2.Email , DV.FECHA,DE.EVALUACION,DE.OBSERVACION ORDER BY DPN.INDICE ASC ";
                }
                else
                {
                    query = " SELECT " +
                            " DPN.INDICE ," +
                            " DPN.ID_PUNTO_NORMATIVO, " +
                            " DPN.COD_NORMA, " +
                            " DPN.DESC_PUNTO_NORMATIVO, " +
                            " DPN.ULTIMA_EVALUACION," +
                            " DPN.EVALUABLE," +
                            " DPN.OBSERVACION AS ULTIMA_EVALUACION," +
                            " DPN.TITULO, " +
                            " DPN.RELEVANCIA, " +
                            " DPN.CANTIDAD_NC_ACTIVAS, " +
                            " DPN.ULTIMA_ID_NC, " +
                            " DPN.OBSERVACION ULTIMA_OBSERVACION, " +
                            " convert(varchar(100),dpn.DESC_PUNTO_NORMATIVO) as DESC_RESUMEN, " +
                            " CONVERT(NVARCHAR(10),(SELECT FECHA FROM dev_auditoria WHERE ID_AUDITORIA = MAX(DNC.ID_AUDITORIA)), 105) FECHA_EVALU,  " +
                            " ISNULL(WU.FULLNAME + ' (' + WU.Email + ')', '') RESPONSABLE , " +
                            " ISNULL(WU2.FULLNAME + ' (' + WU2.Email + ')',  '') AUDITOR , " +
                            " COUNT(dnw.ID_WORKFLOW)  AS RESPUESTA, " +
                            " convert(varchar(100), dpn.DESC_PUNTO_NORMATIVO) as DESC_RESUMEN, " +
                            " MAX(dv.ID_AUDITORIA) ID_AUDITORIA, " +
                            " MAX(DNC.ID_AUDITORIA) AS MAX_ID_AUDITORIA "+
                            " FROM dev_normas dn " +
                            " INNER JOIN dev_punto_normativo DPN ON DPN.COD_NORMA = dn.COD_NORMA " +
                            " LEFT JOIN dev_auditoria DV ON DV.COD_NORMA = dn.COD_NORMA AND DV.ID_AUDITORIA = (  SELECT MAX(ID_AUDITORIA) FROM dev_auditoria WHERE COD_NORMA = DN.COD_NORMA AND TIPO_AUDITORIA != 'Proceso' and TIPO_AUDITORIA != 'Reclamo') " +
                            " LEFT JOIN dev_evaluacion DE ON DE.ID_AUDITORIA = DV.ID_AUDITORIA AND DE.ID_PUNTO_NORMATIVO = DPN.ID_PUNTO_NORMATIVO " +
                            " LEFT JOIN dev_no_conformidad DNC ON DNC.ID_NC = DPN.ULTIMA_ID_NC AND DNC.ESTADO!='Aprobado' " +
                            " LEFT JOIN web_users WU ON WU.USER_ID = DNC.USER_RESP " +
                            " LEFT JOIN web_users WU2 ON WU2.USER_ID = DV.USER_ID " +
                            " LEFT JOIN dev_nc_workflow dnw    ON dnw.ID_NC = dpn.ULTIMA_ID_NC AND dnw.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO AND dnw.ESTADO != 'Aprobado' " +
                            " WHERE dn.COD_NORMA = " + norma + " " +
                            " GROUP BY DPN.INDICE, DPN.ID_PUNTO_NORMATIVO,DPN.COD_NORMA,DPN.DESC_PUNTO_NORMATIVO,DPN.ULTIMA_EVALUACION,DPN.EVALUABLE,DPN.OBSERVACION,DPN.TITULO,DPN.RELEVANCIA, " +
                            " DPN.CANTIDAD_NC_ACTIVAS,DPN.ULTIMA_ID_NC,DNC.FECHA,WU.FULLNAME,WU.Email,WU2.FULLNAME,WU2.Email,DNC.ID_AUDITORIA ORDER BY DPN.INDICE ASC ";
                }

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                //conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<norma_excel> lista = new List<norma_excel>();
                
                while (reader.Read())
                {
                    norma_excel obj = new norma_excel();
                    
                    obj.respuesta = reader["RESPUESTA"].ToString();
                    obj.numero = reader["ID_PUNTO_NORMATIVO"].ToString();
                    obj.requisito = reader["DESC_PUNTO_NORMATIVO"].ToString();
                    obj.v_model = reader["ULTIMA_EVALUACION"].ToString(); 
                    obj.relevancia = reader["RELEVANCIA"].ToString();
                    obj.titulo =    Convert.ToBoolean(reader["TITULO"].ToString());
                    obj.es_evaluable = Convert.ToBoolean(reader["EVALUABLE"].ToString());
                    obj.responsable = reader["RESPONSABLE"].ToString();
                    obj.comentario = reader["ULTIMA_OBSERVACION"].ToString();
                    obj.resumen = reader["DESC_RESUMEN"].ToString();
                    obj.auditor = reader["AUDITOR"].ToString();
                    obj.fecha = reader["FECHA_EVALU"].ToString();
                    obj.id_auditoria = reader["ID_AUDITORIA"].ToString();
                    obj.ultima_observacion = reader["ULTIMA_OBSERVACION"].ToString();
                    obj.no_conformidades = reader["CANTIDAD_NC_ACTIVAS"].ToString();
                    obj.id_nc = reader["ULTIMA_ID_NC"].ToString();

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
        public object auditoria(int norma)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select da.ID_AUDITORIA ,da.COD_NORMA, da.TIPO_AUDITORIA, da.RUT_EMPRESA, dec.NOMBRE_EMPRESA,  da.OBSERVACION, "+
                               " case when max(de.FECHA) != null  then convert(nvarchar(10) ,MAX(de.FECHA), 105)  else convert(nvarchar(10) ,da.FECHA, 105)  end as fecha, " +
                               " da.USER_ID, da.ESTADO, wu.FULLNAME " +
                               " from dev_auditoria da "+
                               " LEFT "+
                               " join dev_evaluacion de on de.ID_AUDITORIA = da.ID_AUDITORIA "+
                               " join web_users wu on wu.USER_ID = da.USER_ID" +
                               " left join dev_empresa_certificadora dec on dec.rut = da.rut_empresa"+
                               " WHERE da.COD_NORMA = "+norma+" and da.ESTADO = 'Habilitado' "+
                               " group by da.ID_AUDITORIA, da.COD_NORMA, da.TIPO_AUDITORIA, da.RUT_EMPRESA, da.OBSERVACION, da.USER_ID, da.ESTADO, da.FECHA,  wu.FULLNAME, dec.NOMBRE_EMPRESA  ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<datosAuditoria> lista = new List<datosAuditoria>();
                while (reader.Read())
                {
                    datosAuditoria obj = new datosAuditoria();
                    obj.ID_AUDITORIA = reader["ID_AUDITORIA"].ToString();
                    obj.COD_NORMA = reader["COD_NORMA"].ToString();
                    obj.TIPO_AUDITORIA = reader["TIPO_AUDITORIA"].ToString();
                    obj.RUT_EMPRESA = reader["RUT_EMPRESA"].ToString();
                    obj.NOMBRE_EMPRESA = reader["NOMBRE_EMPRESA"].ToString();
                    obj.FECHA = reader["FECHA"].ToString();
                    obj.OBSERVACION = reader["OBSERVACION"].ToString();
                    obj.USER_ID = reader["USER_ID"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
                    obj.USER = reader["FULLNAME"].ToString();
                    obj.ID_AUDITORIA_ORI = "";

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
        public string get_constante_tooltip()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT " +
                                " VALOR " +
                                " FROM WEB_CONSTANTES " +
                                " WHERE ID_CONSTANTE = 122 ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();


                string texto = "";
                while (reader.Read())
                {
                    texto = reader["VALOR"].ToString();

                }
                reader.Close();
                return texto;
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
        public object ReadExcelFile(norma_data request)
        {
            string cod_norma = request.cod_norma.ToString();

            string directoryName = String.Empty;
            string ruta = String.Empty;

            var path = HttpRuntime.AppDomainAppPath;
            directoryName = System.IO.Path.Combine(path, "file\\temporal");
            ruta = System.IO.Path.Combine(directoryName, request.nombre_archivo.ToString());


            //string ruta = request.ruta;
            //ruta = "C:/inetpub/wwwroot/LFE_SGI/server/norma/Resumen Normas BRCV7 e IFSV666.xls";

            string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ruta + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1'";

            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            
            cmd.Connection = conn;

            try
            {

                

                excelConnection.Open();               

                DataSet dset = new DataSet();

                OleDbDataAdapter dadp = new OleDbDataAdapter("SELECT * FROM [SGI$]", excelConnection);
                                
                dadp.TableMappings.Add("tbl", "Table");

                dadp.Fill(dset);
                DataTable table = dset.Tables[0];

                int filas = table.Rows.Count;

                int columnas = table.Columns.Count;
                
                List<norma_excel> lista = new List<norma_excel>();
                               
                
                for (int i = 0; i < filas; i++)
                {

                    Boolean es_evaluable = false;
                    Boolean es_titulo = false;
                    string relevancia = "";
                    //si la fila tiene datos que guardar
                    if (!(table.Rows[i][0].ToString() == "" && table.Rows[i][1].ToString() == "" && table.Rows[i][2].ToString() == "")) {                        
                        
                        //pregunto si la columna 2 de la fila tiene un valor distinto de vacio
                        if (table.Rows[i][2].ToString() != "")
                        {
                            //pregunto si es S o N
                            if (table.Rows[i][2].ToString().ToUpper() == "S" || table.Rows[i][2].ToString().ToUpper() == "N")
                            {
                                es_evaluable = true;
                                relevancia = table.Rows[i][2].ToString().ToUpper();
                            }
                        }
                        else
                        {
                            es_titulo = true;
                        }
                        norma_excel obj = new norma_excel();
                        obj.responsable = "";
                        obj.numero = table.Rows[i][0].ToString();
                        obj.requisito = table.Rows[i][1].ToString();
                        obj.v_model = "";
                        obj.relevancia = relevancia.ToUpper();
                        obj.titulo = es_titulo;
                        obj.es_evaluable = es_evaluable;
                        lista.Add(obj);

                    }
                }

                


                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
                excelConnection.Close();
                //elimino el archivo de la ruta temporal
                if (System.IO.File.Exists(ruta))
                {
                    try
                    {
                        System.IO.File.Delete(ruta);
                    }
                    catch (System.IO.IOException e)
                    { 
                        throw new Exception(e.Message) ;
                    }
                }
            }
        }


        public object ValidarExcel(norma_data request)
        {
            string cod_norma = request.cod_norma.ToString();

            string directoryName = String.Empty;
            string ruta = String.Empty;

            var path = HttpRuntime.AppDomainAppPath;
            directoryName = System.IO.Path.Combine(path, "file\\temporal");
            ruta = System.IO.Path.Combine(directoryName, request.nombre_archivo.ToString());
            

            string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ruta + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1'";

            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();

            cmd.Connection = conn;

            try
            {

                List<erroresExcel> listado = new List<erroresExcel>();

                excelConnection.Open();

                DataTable dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String[] excelSheetNames = new String[dt.Rows.Count];
                int iii = 0;

                //recorro las hojas del excel validando que exista la hoja SGI
                Boolean existeHojaSGI = false;
                foreach (DataRow row in dt.Rows)
                {
                    excelSheetNames[iii] = row["TABLE_NAME"].ToString();
                    iii++;
                    if (row["TABLE_NAME"].ToString().ToUpper() == "SGI$")
                    {
                        existeHojaSGI = true;
                    }
                }

                if (existeHojaSGI == true)
                {

                    DataSet dset = new DataSet();

                    OleDbDataAdapter dadp = new OleDbDataAdapter("SELECT * FROM [SGI$]", excelConnection);

                    dadp.TableMappings.Add("tbl", "Table");

                    dadp.Fill(dset);
                    DataTable table = dset.Tables[0];

                    int filas = table.Rows.Count;

                    int columnas = table.Columns.Count;
                    if (columnas >= 3){ 

                        List<norma_excel> lista = new List<norma_excel>();

                        string query;

                        query = "delete from dev_temporal_norma where COD_NORMA = " + cod_norma;
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();

                        Boolean es_titulo = false;
                        Boolean es_evaluable = false;

                        for (int i = 0; i < filas; i++)
                        {
                            //si la fila tiene datos que guardar
                            if (!(table.Rows[i][0].ToString() == "" && table.Rows[i][1].ToString() == "" && table.Rows[i][2].ToString() == ""))
                            {
                                //guardo el registro en la BD en la tabla temporal
                                cmd.Parameters.Clear();

                                query = "INSERT INTO dev_temporal_norma(COD_NORMA, ID_PUNTO_NORMATIVO, DESC_PUNTO_NORMATIVO, RELEVANCIA, NUMERO_FILA, TITULO, EVALUABLE) " +
                                    " VALUES (@COD_NORMA,@NUMERO,@REQUISITO,@CUMPLIMIENTO,@NUMERO_FILA, @TITULO, @EVALUABLE) ";


                                if (table.Rows[i][2].ToString() == "")
                                {
                                    es_titulo = true;
                                }
                                else
                                {
                                    es_titulo = false;
                                }

                                if (table.Rows[i][2].ToString().ToUpper() == "S" || table.Rows[i][2].ToString().ToUpper() == "N")
                                {
                                    es_evaluable = true;
                                }
                                else
                                {
                                    es_evaluable = false;
                                }

                                cmd.Parameters.AddWithValue("COD_NORMA", cod_norma);
                                cmd.Parameters.AddWithValue("NUMERO", table.Rows[i][0].ToString());
                                cmd.Parameters.AddWithValue("REQUISITO", table.Rows[i][1].ToString());
                                cmd.Parameters.AddWithValue("CUMPLIMIENTO", table.Rows[i][2].ToString());
                                cmd.Parameters.AddWithValue("NUMERO_FILA", (i + 2));
                                cmd.Parameters.AddWithValue("TITULO", es_titulo.ToString());
                                cmd.Parameters.AddWithValue("EVALUABLE", es_evaluable.ToString());

                                cmd.CommandText = query;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();



                            }
                        }

                        //ejecuto la query para detectar errores
                        string queryErrores = "select tt.motivo, replace(tt.Filas,'.',',') as filas from " +
                                                    "(SELECT Results.motivo, STUFF((SELECT ',' + CONVERT(VARCHAR, S.NUMERO_FILA) as lineas_Error " +
                                                      "FROM( " +
                                                              "SELECT NUMERO_FILA, 'Error con el número del punto normativo' as motivo " +
                                                              "FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA AND " +
                                                              "(ID_PUNTO_NORMATIVO    LIKE '%[^[0,1,2,3,4,5,6,7,8,9, ,.]%' OR  ID_PUNTO_NORMATIVO LIKE '%..%' " +
                                                              "OR LEN(ID_PUNTO_NORMATIVO) = 0 OR ID_PUNTO_NORMATIVO LIKE '%,%' OR ID_PUNTO_NORMATIVO LIKE '%.0.%') " +
                                                              "union all " +
                                                              "SELECT NUMERO_FILA, 'Error con la descripción del punto normativo' as motivo " +
                                                              "FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA  AND(LEN(replace(DESC_PUNTO_NORMATIVO, ' ', '')) = 0) " +
                                                              "union all " +
                                                              "SELECT NUMERO_FILA, 'Valor inválido en la columna de cumplimiento' as motivo " +
                                                              "FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA  AND (RELEVANCIA LIKE '%[^[s,S,n,N, ]%' or len(RELEVANCIA) > 1) " +
                                                              "UNION ALL " +
                                                              "SELECT NUMERO_FILA, 'Número del punto normativo repetido' as motivo FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA AND ID_PUNTO_NORMATIVO != '' AND ID_PUNTO_NORMATIVO IN( " +
                                                              "SELECT ID_PUNTO_NORMATIVO " +
                                                              "FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA  " +
                                                              "GROUP BY ID_PUNTO_NORMATIVO " +
                                                              "HAVING COUNT(1) > 1) " +
                                                       ") AS S " +
                                                       "Where(S.motivo = Results.motivo) " +
                                                       "ORDER BY S.NUMERO_FILA " +
                                                       "FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS Filas " +
                                                       "from( " +
                                                              "SELECT NUMERO_FILA, 'Error con el número del punto normativo' as motivo " +
                                                              "FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA  AND " +
                                                              "(ID_PUNTO_NORMATIVO    LIKE '%[^[0,1,2,3,4,5,6,7,8,9, ,.]%' OR  ID_PUNTO_NORMATIVO LIKE '%..%' " +
                                                              "OR LEN(ID_PUNTO_NORMATIVO) = 0 OR ID_PUNTO_NORMATIVO LIKE '%,%' OR ID_PUNTO_NORMATIVO LIKE '%.0.%') " +
                                                              "union all " +
                                                              "SELECT NUMERO_FILA, 'Error con la descripción del punto normativo' as motivo " +
                                                              "FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA  AND(LEN(replace(DESC_PUNTO_NORMATIVO, ' ', '')) = 0) " +
                                                              "union all " +
                                                              "SELECT NUMERO_FILA, 'Valor inválido en la columna de cumplimiento' as motivo " +
                                                              "FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA  AND (RELEVANCIA LIKE '%[^[s,S,n,N, ]%' or len(RELEVANCIA) > 1) " +
                                                              "UNION ALL " +
                                                              "SELECT NUMERO_FILA, 'Número del punto normativo repetido' as motivo FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA AND ID_PUNTO_NORMATIVO != '' AND ID_PUNTO_NORMATIVO IN( " +
                                                              "SELECT ID_PUNTO_NORMATIVO " +
                                                              "FROM dev_temporal_norma " +
                                                              "WHERE COD_NORMA = @COD_NORMA  " +
                                                              "GROUP BY ID_PUNTO_NORMATIVO " +
                                                              "HAVING COUNT(1) > 1) " +
                                                       ") as Results " +
                                                       "group by Results.motivo) as tt";

                        cmd.Parameters.AddWithValue("COD_NORMA", cod_norma);
                        cmd.CommandText = queryErrores;

                        SqlDataReader reader = cmd.ExecuteReader();



                        while (reader.Read())
                        {
                            erroresExcel obj = new erroresExcel();
                            obj.motivo = reader["motivo"].ToString() + ": ";
                            obj.filas = reader["filas"].ToString();
                            listado.Add(obj);
                        }
                        reader.Close();


                        //si el listado tiene elementos, es decir el excel tiene errores, se elimina el contenido de la tabla temporal para que 
                        //no se cargue la tabla para ningun usuario
                        if (listado.Count() > 0)
                        {
                            query = "delete from dev_temporal_norma where COD_NORMA = " + cod_norma;
                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();
                        }


                    }else
                    {
                        //el documento tiene solo 2 columnas
                        //si la hoja no existe
                        erroresExcel objeto = new erroresExcel();
                        objeto.motivo = "La hoja SGI no tiene las 3 columnas necesarias para su procesamiento";
                        objeto.filas = "Todo el Documento";
                        listado.Add(objeto);
                    }


                }else
                {
                    //si la hoja no existe
                    erroresExcel objeto = new erroresExcel();
                    objeto.motivo = "La hoja SGI no existe en el archivo";
                    objeto.filas = "Todo el Documento";
                    listado.Add(objeto);
                }
                                               
                return listado;

                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
                excelConnection.Close();
            }
        }



        public object doc_asociados(norma_data request)
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

                List<doc_asociado> lista = new List<doc_asociado>();

                while (reader.Read())
                {
                    doc_asociado obj = new doc_asociado();
                    obj.codigo = reader["USER_ID"].ToString();
                    obj.version = reader["FULLNAME"].ToString();
                    obj.tipo = reader["USERNAME"].ToString();
                    obj.nombre = reader["PERFIL"].ToString();
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

        public object responsables(norma_data request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT " +                                
                                " FULLNAME," +
                                " USERNAME " +                                
                                " FROM WEB_USERS ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<responsables> lista = new List<responsables>();

                while (reader.Read())
                {
                    responsables obj = new responsables();
                    obj.value = reader["USERNAME"].ToString();
                    obj.label = reader["USERNAME"].ToString();
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

        public bool guardar(guardarDatos request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();            
            cmd.Connection = conn;


            try
            {
                string query;
                query = "delete from dev_punto_normativo where COD_NORMA = " + request.norma;
                //cmd.Parameters.AddWithValue("COD_NORMA", request.norma);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                JavaScriptSerializer js = new JavaScriptSerializer();
                object[] testObj = (object[])js.Deserialize(request.datos, new object().GetType());
                
                cmd.Parameters.Clear();
                
                    
                query = "INSERT INTO dev_punto_normativo (ID_PUNTO_NORMATIVO, COD_NORMA,DESC_PUNTO_NORMATIVO, ULTIMA_EVALUACION, EVALUABLE,TITULO, RELEVANCIA,OBSERVACION ) " +
                        "SELECT ID_PUNTO_NORMATIVO, COD_NORMA, DESC_PUNTO_NORMATIVO, '', EVALUABLE,TITULO, RELEVANCIA,'' " +
                        "from dev_temporal_norma " +
                        "where COD_NORMA = " + request.norma;
                    
                
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                //elimino los datos de la tabla temporal
                query = "delete from dev_temporal_norma where COD_NORMA = " + request.norma;                
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                query = "Select NOMBRE_NORMA, VERSION FROM dev_normas where COD_NORMA = " + request.norma;
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();

                string nombre_norma = "";
                string version = "";

                while (reader.Read())
                {
                    datosDocumentoPuntoNormativo obj = new datosDocumentoPuntoNormativo();
                    nombre_norma = reader["NOMBRE_NORMA"].ToString();
                    version = reader["VERSION"].ToString();
                }
                reader.Close();
                Tracking t = new Tracking();
                string queryIn = "select COD_NORMA , VERSION, NOMBRE_NORMA, DESCRIPCION_NORMA,FECHA_ACTUALIZACION, ESTADO, " +
                               "  COLORNORMA,COLORCUMPLIMIENTO from dev_normas" +
                               "   where COD_NORMA = '" + request.norma + "'";

                //GUARDAMOS EL TRACKING
                t.guardarTracking("Publicar", "Gestor norma", t.obtenerDatosNuevosyAntiguos(queryIn));
                //genero la notificación de la publicación de la norma
                menuModel modelo = new menuModel();

                int id_notificacion = modelo.guardar_notificacion("Se ha publicado la norma " + nombre_norma + " versión " + version, "", request.id_empresa);


               /* SqlCommand command = new SqlCommand("SP_SGI_ALERTA_PUBLICACION_NORMA", conn);
                command.CommandType = CommandType.StoredProcedure;

                //SqlParameter paramId = new SqlParameter("Id", SqlDbType.Int);
                //paramId.Direction = ParameterDirection.Output;
                //command.Parameters.Add(paramId);
                string destinatarios = modelo.emailsAreaNorma(Int32.Parse(request.norma));
                string url = WebConfigurationManager.AppSettings["baseURL"] + "norma/" + request.norma;

                command.Parameters.AddWithValue("COD_NORMA", request.norma);
                command.Parameters.AddWithValue("emails", destinatarios);
                command.Parameters.AddWithValue("URL", url);

                command.ExecuteNonQuery(); */




                return true;
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
        public object puntoNormativo(datosDocumentoPuntoNormativo request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            //SqlTransaction transaccion = default(SqlTransaction);
            //transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            //cmd.Transaction = transaccion;
            try
            {

                string query = " SELECT ddp.ID_PUNTO_NORMATIVO , ddp.COD_NORMA , ddp.codigo_documento , dd.version, dd.id_empresa as ID_EMPRESA,dtd.tipo,dd.nombre FROM dev_doc_pn ddp " +
                               " INNER JOIN dev_documento dd on dd.codigo_documento = ddp.codigo_documento  " +
                               " and case when dd.caducidad is not null then CONVERT(char(10), GetDate(),126) else CONVERT(char(10), GetDate(),126)  end >= CONVERT(char(10), GetDate(),126)"+
                               " INNER JOIN dev_tipo_documento dtd on dd.tipo = dtd.id  " +
                               " INNER JOIN dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento and dad.version = dd.version and dad.estado = 'Publicado' and dad.id_empresa = @id_empresa "+
                               " INNER JOIN dev_normas dn on dn.COD_NORMA = ddp.COD_NORMA " +
                               " where ddp.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO AND ddp.COD_NORMA = @COD_NORMA ";

                cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("id_empresa", request.ID_EMPRESA);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();

                List<datosDocumentoPuntoNormativo> lista = new List<datosDocumentoPuntoNormativo>();
                
                
                while (reader.Read())
                {
                    datosDocumentoPuntoNormativo obj = new datosDocumentoPuntoNormativo();
                    obj.ID_PUNTO_NORMATIVO = reader["ID_PUNTO_NORMATIVO"].ToString();
                    obj.COD_NORMA = reader["COD_NORMA"].ToString();
                    obj.ID_EMPRESA = reader["ID_EMPRESA"].ToString();
                    obj.codigo_documento = reader["codigo_documento"].ToString();
                    obj.version = reader["version"].ToString();
                    obj.ruta_archivo = get_ruta_archivo(reader["codigo_documento"].ToString());
                    obj.tipo = reader["tipo"].ToString();
                    obj.nombre = reader["nombre"].ToString();
                   
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
        public object documentos(string id_empresa)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = @"select dd.codigo_documento,dd.nombre, dd.version ,dtd.tipo, dd.indice,dad.estado, dd.id_empresa from dev_documento dd 
                               inner join dev_tipo_documento dtd on dtd.id = dd.tipo 
                               inner join  dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento and dad.estado = 'Publicado' and dd.version = dad.version
                               where  dad.id_empresa=@id_empresa ";//and dd.caducidad >= CONVERT(char(10), GetDate(),126) 
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                cmd.Parameters.AddWithValue("id_empresa", id_empresa);
                SqlDataReader reader = cmd.ExecuteReader();
                List<datosDocumentoPuntoNormativo> lista = new List<datosDocumentoPuntoNormativo>();
                while (reader.Read())
                {
                    datosDocumentoPuntoNormativo obj = new datosDocumentoPuntoNormativo();
                    
                    obj.codigo_documento = reader["codigo_documento"].ToString();
                    obj.indice = reader["indice"].ToString();
                    obj.nombre = reader["nombre"].ToString();
                    obj.version = reader["version"].ToString();
                    obj.ruta_archivo = get_ruta_archivo(reader["codigo_documento"].ToString());
                    obj.tipo = reader["tipo"].ToString();
                    obj.estado = reader["estado"].ToString();
                    obj.ID_EMPRESA = reader["id_empresa"].ToString();
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
        public string get_ruta_archivo(string codigo)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select ruta from dev_archivo_documento where estado != 'Historico' and cod_documento = '" + codigo + "' ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();



                string ruta = "";
                while (reader.Read())
                {
                    ruta = reader["ruta"].ToString();
                }
                reader.Close();
                return ruta;
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
        public bool guardarPuntoNormativo(guardarDatos request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                string query;
                JavaScriptSerializer js = new JavaScriptSerializer();
                //object[] testObj = (object[])js.Deserialize(request.datos, new object().GetType());
                //foreach (var item in testObj)
                //{
                cmd.Parameters.Clear();
             //   Dictionary<string, object> Nodo = (Dictionary<string, object>)item;

                query = "INSERT INTO dev_doc_pn (ID_PUNTO_NORMATIVO, COD_NORMA,codigo_documento,version, id_empresa ) " +
                    " VALUES (@ID_PUNTO_NORMATIVO,@COD_NORMA,@codigo_documento, @version, @id_empresa) ";

                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.puntoSeleccionado);
                cmd.Parameters.AddWithValue("COD_NORMA", request.norma);
                cmd.Parameters.AddWithValue("codigo_documento", request.datos);
                cmd.Parameters.AddWithValue("version", request.version);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                   
                    
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                    //query = " INSERT INTO DEV_DURACION_ETAPAS (ID_EMPRESA ETAPA,DURACION) " +
                    //        " (SELECT '" + ID_EMPRESA + "', etapa, '0' FROM DEV_ETAPAS_CSC) ";

                    //cmd.CommandText = query;
                    //cmd.ExecuteNonQuery()
                //}

                //se almacenan los datos del usuario en la tabla user
                //---------------------------------------------------------------------



                transaccion.Commit();
                //TRACKING
                Tracking t = new Tracking();
                string queryIn = "SELECT ID_PUNTO_NORMATIVO, COD_NORMA,DESC_PUNTO_NORMATIVO, ULTIMA_EVALUACION, EVALUABLE,TITULO, RELEVANCIA  FROM dev_punto_normativo da  where COD_NORMA = '"+ request.norma + "' ";
                //GUARDAMOS EL TRACKING
                t.guardarTracking("Crear", "documento y Punto Normativo", t.obtenerDatosNuevosyAntiguos(queryIn));
                return true;
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public bool eliminarPuntoNormativo(guardarDatos request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                string query;
                JavaScriptSerializer js = new JavaScriptSerializer();
                object[] testObj = (object[])js.Deserialize(request.datos, new object().GetType());
                var cod_norma = "";
                foreach (var item in testObj)
                {
                    cmd.Parameters.Clear();
                    Dictionary<string, object> Nodo = (Dictionary<string, object>)item;

                    query = "delete from dev_doc_pn where codigo_documento = @codigo_documento and  ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and COD_NORMA = @COD_NORMA ";

                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", Nodo["ID_PUNTO_NORMATIVO"]);
                    cmd.Parameters.AddWithValue("COD_NORMA", Nodo["COD_NORMA"]);
                    cmd.Parameters.AddWithValue("codigo_documento", Nodo["codigo_documento"]);
                    
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cod_norma = Nodo["COD_NORMA"].ToString();


                }

               
                transaccion.Commit();
                //TRACKING
                Tracking t = new Tracking();
                string queryIn = "SELECT ID_PUNTO_NORMATIVO, COD_NORMA,DESC_PUNTO_NORMATIVO, ULTIMA_EVALUACION, EVALUABLE,TITULO, RELEVANCIA  FROM dev_punto_normativo da  where COD_NORMA = '"+ cod_norma + "' ";
                //GUARDAMOS EL TRACKING
                t.guardarTracking("Crear", "documento y Punto Normativo", t.obtenerDatosNuevosyAntiguos(queryIn));
                return true;
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        public object empresas(string tipo)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select  RUT as id, NOMBRE_EMPRESA as text from dev_empresa_certificadora where TIPO = '"+tipo+"' ";
                
                SqlCommand cmd = new SqlCommand(query);

                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<responsables> lista = new List<responsables>();
                
                
                while (reader.Read())
                {
                    responsables obj = new responsables();


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

        public bool GuardarAuditoria(auditoria request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var userid = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();
                //VERIFICAMOS SI AUN SIGUE PUBLICADA LA NORMA
                string query = "select COD_NORMA from dev_punto_normativo where COD_NORMA='"+ request.COD_NORMA + "'";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                bool publicado = false;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    publicado = true;
                }
                reader.Close();
                if (publicado)
                {
                    query = "";
                    query += "declare @FECHA datetime ";
                   query += "set @FECHA = convert(datetime, '" + request.FECHA + "', 103)";
                    query += "INSERT INTO dev_auditoria (COD_NORMA, TIPO_AUDITORIA, RUT_EMPRESA,FECHA ,OBSERVACION,USER_ID,ESTADO ) " +
                          " VALUES (@COD_NORMA,@TIPO_AUDITORIA,@RUT_EMPRESA,@FECHA,@OBSERVACION,@USER_ID,@ESTADO) ";


                    cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                    cmd.Parameters.AddWithValue("TIPO_AUDITORIA", request.TIPO_AUDITORIA);
                    cmd.Parameters.AddWithValue("RUT_EMPRESA", request.RUT_EMPRESA);
                   // cmd.Parameters.AddWithValue("FECHA", request.FECHA);
                    cmd.Parameters.AddWithValue("OBSERVACION", request.OBSERVACION);
                    cmd.Parameters.AddWithValue("USER_ID", userid);
                    cmd.Parameters.AddWithValue("ESTADO", "Habilitado");


                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();


                    transaccion.Commit();
                    //TRACKING
                    Tracking t = new Tracking();
                    string queryIn = "SELECT ID_AUDITORIA,COD_NORMA, TIPO_AUDITORIA, RUT_EMPRESA,FECHA ,OBSERVACION,USER_ID,ESTADO   FROM dev_auditoria da  where COD_NORMA = '" + request.COD_NORMA + "' ";
                    //GUARDAMOS EL TRACKING
                    t.guardarTracking("Crear", "Auditoria", t.obtenerDatosNuevosyAntiguos(queryIn));
                    return true;
                }
                return false;
                
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public bool GuardarEvaluacion(evaluacion request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var userid = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();
                string query="";
                query += "declare @FECHA datetime ";
                query += "set @FECHA = convert(datetime, '" + request.FECHA + "', 103) ";
                if (request.ID_AUDITORIA == null)
                {
                    query += "INSERT INTO dev_auditoria (COD_NORMA , TIPO_AUDITORIA, RUT_EMPRESA,FECHA, OBSERVACION, USER_ID , ESTADO) values " +
                        "(@COD_NORMA, @TIPO, '' ,GETDATE() ,'' , @USER_ID , 'Cerrado') SELECT CAST(scope_identity() AS int)";


                    

                    //cmd.Parameters.AddWithValue("FECHA", request.FECHA);
                    cmd.Parameters.AddWithValue("USER_ID", userid);
                    cmd.Parameters.AddWithValue("TIPO", request.TIPO);
                    cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                    cmd.CommandText = query;

                    Int32 ID_AUDITORIA = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.Parameters.Clear();
                    query = "";
                    query += "declare @FECHA datetime ";
                    query += "set @FECHA = convert(datetime, '" + request.FECHA + "', 103) ";
                    query += " IF EXISTS (SELECT ID_AUDITORIA FROM dev_evaluacion WHERE ID_AUDITORIA =@ID_AUDITORIA AND ID_PUNTO_NORMATIVO =@ID_PUNTO_NORMATIVO) " +
                         " UPDATE  dev_evaluacion SET   EVALUACION = @EVALUACION , OBSERVACION = @OBSERVACION , FECHA=GETDATE(), USER_ID=@USER_ID, TIPO = @TIPO  " +
                         " ELSE " +
                         " INSERT INTO dev_evaluacion (ID_AUDITORIA, ID_PUNTO_NORMATIVO, EVALUACION,OBSERVACION ,FECHA,USER_ID,TIPO ) " +
                         "  VALUES(@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@EVALUACION,@OBSERVACION,GETDATE(),@USER_ID,@TIPO) ";
                    

                    cmd.Parameters.AddWithValue("ID_AUDITORIA", ID_AUDITORIA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("EVALUACION", request.EVALUACION);
                    cmd.Parameters.AddWithValue("OBSERVACION", request.OBSERVACION == null ? "" : request.OBSERVACION);
                    cmd.Parameters.AddWithValue("USER_ID", userid);
                    cmd.Parameters.AddWithValue("TIPO", request.TIPO);
                    cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                   

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    query = "";
                    query += "update dev_punto_normativo set ULTIMA_EVALUACION = @EVALUACION , OBSERVACION = @OBSERVACION  where ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and COD_NORMA = @COD_NORMA";
                    cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("EVALUACION", request.EVALUACION);
                    cmd.Parameters.AddWithValue("OBSERVACION", request.OBSERVACION == null ? "" : request.OBSERVACION);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    if (request.EVALUACION == "NO")
                    {
                        query = "";
                        query += "declare @FECHA datetime ";
                        query += "set @FECHA = convert(datetime, '" + request.FECHA + "', 103) ";
                        
                        query += " INSERT INTO dev_no_conformidad (ID_AUDITORIA, ID_PUNTO_NORMATIVO, NO_CONFORMIDAD,USER_RESP ,FECHA,ESTADO ) " +
                                 "  VALUES(@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@OBSERVACION,@USER_RESP,GETDATE(),'En revisión') ";

                        cmd.Parameters.AddWithValue("ID_AUDITORIA", ID_AUDITORIA);
                        cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                        cmd.Parameters.AddWithValue("OBSERVACION", request.OBSERVACION == null ? "" : request.OBSERVACION);
                        cmd.Parameters.AddWithValue("USER_RESP", request.USER_RESP);


                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        cmd.Parameters.Clear();

                        //SELECT PARA CONTAR NO CONFORMIDADES
                        //query = "select COUNT(ID_PUNTO_NORMATIVO) AS CANTIDAD FROM dev_no_conformidad  where ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and COD_NORMA = @COD_NORMA";
                        query = " select case when MAX(dnc.ID_NC) is null then 0 else MAX(dnc.ID_NC) end AS ID_NC, COUNT(dnc.ID_NC) AS CANTIDAD   FROM dev_auditoria  da " +
                                " left join dev_no_conformidad dnc on da.ID_AUDITORIA = dnc.ID_AUDITORIA " +
                                " where da.COD_NORMA = @COD_NORMA AND   dnc.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO AND dnc.ESTADO<>'Aprobado'";
                        cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                        cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader = cmd.ExecuteReader();
                        int totalNoconformidades = 0;
                        int idNC = 0;
                        while (reader.Read())
                        {
                            totalNoconformidades = Convert.ToInt32(reader["CANTIDAD"].ToString());
                            idNC = Convert.ToInt32(reader["ID_NC"].ToString());
                        }
                        reader.Close();


                        cmd.Parameters.Clear();
                        query = "update dev_punto_normativo set  CANTIDAD_NC_ACTIVAS = @CANTIDAD_NC_ACTIVAS, ULTIMA_ID_NC=@ULTIMA_ID_NC  where ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and COD_NORMA = @COD_NORMA";
                        cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                        cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                        cmd.Parameters.AddWithValue("CANTIDAD_NC_ACTIVAS", totalNoconformidades);
                        cmd.Parameters.AddWithValue("ULTIMA_ID_NC", idNC);

                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                    }
                    transaccion.Commit();
                    //TRACKING
                    Tracking t = new Tracking();
                    string queryIn = "SELECT ID_AUDITORIA, ID_PUNTO_NORMATIVO, EVALUACION,OBSERVACION ,FECHA,USER_ID,TIPO   FROM dev_evaluacion da  where ID_AUDITORIA = '" + request.ID_AUDITORIA + "' and ID_PUNTO_NORMATIVO = '" + request.ID_PUNTO_NORMATIVO + "' ";

                    //GUARDAMOS EL TRACKING
                    t.guardarTracking("Crear", "evaluacion", t.obtenerDatosNuevosyAntiguos(queryIn));
                }
                else
                {
                    

                    query = "";
                    query += "declare @FECHA datetime ";
                    query += "set @FECHA = convert(datetime, '" + request.FECHA + "', 103) ";
                    query += " IF EXISTS (SELECT ID_AUDITORIA FROM dev_evaluacion WHERE ID_AUDITORIA =@ID_AUDITORIA AND ID_PUNTO_NORMATIVO =@ID_PUNTO_NORMATIVO) " +
                          " UPDATE  dev_evaluacion SET   EVALUACION = @EVALUACION , OBSERVACION = @OBSERVACION , FECHA=@FECHA, USER_ID=@USER_ID, TIPO = @TIPO  " +
                          " WHERE ID_AUDITORIA =@ID_AUDITORIA AND ID_PUNTO_NORMATIVO =@ID_PUNTO_NORMATIVO "+
                          " ELSE " +
                          " INSERT INTO dev_evaluacion (ID_AUDITORIA, ID_PUNTO_NORMATIVO, EVALUACION,OBSERVACION ,FECHA,USER_ID,TIPO ) " +
                          "  VALUES(@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@EVALUACION,@OBSERVACION,@FECHA,@USER_ID,@TIPO) ";


                    cmd.Parameters.AddWithValue("ID_AUDITORIA", request.ID_AUDITORIA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("EVALUACION", request.EVALUACION);
                    cmd.Parameters.AddWithValue("OBSERVACION", request.OBSERVACION == null ? "" : request.OBSERVACION);
                    //cmd.Parameters.AddWithValue("FECHA", request.FECHA);
                    cmd.Parameters.AddWithValue("USER_ID", userid);
                    cmd.Parameters.AddWithValue("TIPO", request.TIPO);
                    cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);


                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    query = "update dev_punto_normativo set ULTIMA_EVALUACION = @EVALUACION , OBSERVACION = @OBSERVACION  where ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and COD_NORMA = @COD_NORMA";
                    cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("EVALUACION", request.EVALUACION);
                    cmd.Parameters.AddWithValue("OBSERVACION", request.OBSERVACION == null ? "" : request.OBSERVACION);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    if (request.EVALUACION == "NO")
                    {
                        query = "";
                        query += "declare @FECHA datetime ";
                        query += "set @FECHA = convert(datetime, '" + request.FECHA + "', 103) ";
                        query += " INSERT INTO dev_no_conformidad (ID_AUDITORIA, ID_PUNTO_NORMATIVO, NO_CONFORMIDAD,USER_RESP ,FECHA,ESTADO ) " +
                                 "  VALUES(@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@OBSERVACION,@USER_RESP,@FECHA,'En revisión') ";

                        cmd.Parameters.AddWithValue("ID_AUDITORIA", request.ID_AUDITORIA);
                        cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                        cmd.Parameters.AddWithValue("OBSERVACION", request.OBSERVACION == null ? "" : request.OBSERVACION);
                        //cmd.Parameters.AddWithValue("FECHA", request.FECHA);
                        cmd.Parameters.AddWithValue("USER_RESP", request.USER_RESP);


                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();

                        
                        cmd.Parameters.Clear();

                        //SELECT PARA CONTAR NO CONFORMIDADES
                        //query = "select COUNT(ID_PUNTO_NORMATIVO) AS CANTIDAD FROM dev_no_conformidad  where ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and COD_NORMA = @COD_NORMA";
                        query = " select case when MAX(dnc.ID_NC) is null then 0 else MAX(dnc.ID_NC) end AS ID_NC, COUNT(dnc.ID_NC) AS CANTIDAD FROM dev_auditoria  da " +
                                " left join dev_no_conformidad dnc on da.ID_AUDITORIA = dnc.ID_AUDITORIA " +
                                " where da.COD_NORMA = @COD_NORMA AND   dnc.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO  AND dnc.ESTADO<>'Aprobado'";
                        cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                        cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        SqlDataReader reader = cmd.ExecuteReader();
                        int totalNoconformidades = 0;
                        int idNC = 0;
                        while (reader.Read())
                        {
                            totalNoconformidades = Convert.ToInt32(reader["CANTIDAD"].ToString());
                            idNC = Convert.ToInt32(reader["ID_NC"].ToString());
                        }
                        reader.Close();
                        cmd.Parameters.Clear();
                        
                        cmd.Parameters.Clear();
                        query = "update dev_punto_normativo set ULTIMA_ID_NC = @ULTIMA_ID_NC , CANTIDAD_NC_ACTIVAS = @CANTIDAD_NC_ACTIVAS  where ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and COD_NORMA = @COD_NORMA";
                        cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                        cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                        cmd.Parameters.AddWithValue("ULTIMA_ID_NC", idNC);
                        cmd.Parameters.AddWithValue("CANTIDAD_NC_ACTIVAS", totalNoconformidades);

                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();


                    }

                    transaccion.Commit();
                    //TRACKING
                    Tracking t = new Tracking();
                    string queryIn = "SELECT ID_AUDITORIA, ID_PUNTO_NORMATIVO, EVALUACION,OBSERVACION ,FECHA,USER_ID,TIPO   FROM dev_evaluacion da  where ID_AUDITORIA = '" + request.ID_AUDITORIA + "' and ID_PUNTO_NORMATIVO = '" + request.ID_PUNTO_NORMATIVO + "' ";

                    //GUARDAMOS EL TRACKING
                    t.guardarTracking("Crear", "evaluacion", t.obtenerDatosNuevosyAntiguos(queryIn));
                }


                //mandamos la notificacion solo cuando sea una NO CONFORMIDAD independiente de que haya o no una Auditoria
                if (request.EVALUACION.ToString() == "NO")
                {
                    query = $@"Select dn.NOMBRE_NORMA, dn.VERSION, dne.ID_EMPRESA, em.NOMBRE FROM dev_normas dn
                            join dev_norma_empresa dne on dne.id_norma = dn.COD_NORMA
							join web_empresa em on em.ID_EMPRESA = dne.ID_EMPRESA
                            where dn.COD_NORMA = '{request.COD_NORMA}'
                            ";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();

                    string nombre_norma = "";
                    string version = "";
                    string id_empresa = "";
                    string nombre_empresa = "";

                    while (reader.Read())
                    {
                        nombre_norma = reader["NOMBRE_NORMA"].ToString();
                        version = reader["VERSION"].ToString();
                        id_empresa = reader["ID_EMPRESA"].ToString();
                        nombre_empresa = reader["NOMBRE"].ToString();
                    }
                    reader.Close();

                    //genero la notificación de la publicación de la norma
                    menuModel modelo = new menuModel();

                    int id_notificacion = modelo.guardar_notificacion("Se generó una NC en " + nombre_norma + " " + version + " " + request.ID_PUNTO_NORMATIVO, request.USER_RESP.ToString(), id_empresa);

                    SqlCommand command = new SqlCommand("SP_SGI_ALERTA_NUEVA_NOCONFORMIDAD", conn);
                    command.CommandType = CommandType.StoredProcedure;

                    //SqlParameter paramId = new SqlParameter("Id", SqlDbType.Int);
                    //paramId.Direction = ParameterDirection.Output;
                    //command.Parameters.Add(paramId);
                    string destinatarios = modelo.emailUnUsuario(request.USER_RESP.ToString());
                    string url = WebConfigurationManager.AppSettings["clientURL"] + "noConformidad";

                    command.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                    command.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                    command.Parameters.AddWithValue("emails", destinatarios);
                    command.Parameters.AddWithValue("URL", url);
                    command.Parameters.AddWithValue("NOMBRE", nombre_empresa);
                    
                    command.ExecuteNonQuery();

                }
                

                return true;
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        public bool cerrarAuditoria(evaluacion request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var userid = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();
                bool respuesta = false;
                if (userid == request.USER_ID)
                {
                    string query;
                    query = "update dev_auditoria set ESTADO ='Cerrado'  where ID_AUDITORIA = @ID_AUDITORIA and COD_NORMA = @COD_NORMA";
                    cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_AUDITORIA", request.ID_AUDITORIA);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    transaccion.Commit();
                    //TRACKING
                    Tracking t = new Tracking();
                    string queryIn = "SELECT *  FROM dev_auditoria da  where ID_AUDITORIA = '" + request.ID_AUDITORIA + "' and  COD_NORMA = '" + request.COD_NORMA + "' ";

                    //GUARDAMOS EL TRACKING
                    t.guardarTracking("Actualizar", "Auditoria", t.obtenerDatosNuevosyAntiguos(queryIn));
                    respuesta = true;
                }
                
                return respuesta;
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        public string esUsuarioLFE()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try {

                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var userid = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();

                string query = "select wg.Usuario as respuesta from web_users wu join web_groups wg on wu.GROUP_ID = wg.GROUP_ID where wu.USER_ID = '"+ userid +"' ";

                SqlCommand cmd = new SqlCommand(query);

                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                string respuesta = "";
                                
                while (reader.Read())
                {                    
                    respuesta = reader["respuesta"].ToString();                   
                }

                reader.Close();

                return respuesta;

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



        public object cargarNormas(string ID_EMPRESA)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT COD_NORMA"+
                    ",VERSION "+
                    ",NOMBRE_NORMA "+
                    ",DESCRIPCION_NORMA "+
                    ",FECHA_ACTUALIZACION "+
                    ",dn.ESTADO "+
                    ",COLORNORMA "+
                    ",COLORCUMPLIMIENTO  , dne.ID_EMPRESA as ID_EMPRESA ,we.NOMBRE as NOMBRE_EMPRESA " +
                    "FROM dev_normas  dn "+
                    "INNER JOIN dev_norma_empresa dne on dne.ID_NORMA = dn.COD_NORMA and dne.ID_EMPRESA = "
                    +"'"+ ID_EMPRESA +"'"+ 
                    " inner join web_empresa we on we.ID_EMPRESA = dne.ID_EMPRESA"+
                    " ORDER BY NOMBRE_NORMA ASC";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<datosNormaMantenedor> lista = new List<datosNormaMantenedor>();

                while (reader.Read())
                {
                    datosNormaMantenedor obj = new datosNormaMantenedor();
                    obj.COD_NORMA = reader["COD_NORMA"].ToString();
                    obj.VERSION = reader["VERSION"].ToString();
                    obj.NOMBRE_NORMA = reader["NOMBRE_NORMA"].ToString();
                    obj.DESCRIPCION_NORMA = reader["DESCRIPCION_NORMA"].ToString();
                    obj.FECHA_ACTUALIZACION = reader["FECHA_ACTUALIZACION"].ToString();
                    obj.COLOR_CUMPLIMIENTO = reader["COLORCUMPLIMIENTO"].ToString();
                    obj.COLOR_NORMA = reader["COLORNORMA"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
                    obj.ID_EMPRESA = reader["ID_EMPRESA"].ToString();
                    obj.NOMBRE_EMPRESA = reader["NOMBRE_EMPRESA"].ToString();
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
        public object crearNorma(datosNormaMantenedor nuevaNorma)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
               
                string query = " select dn.COD_NORMA from dev_normas dn " + 
                " join dev_norma_empresa dne on dne.id_norma = dn.cod_norma and dne.id_empresa = "+nuevaNorma.ID_EMPRESA+" "+
                " where dn.NOMBRE_NORMA=UPPER('"+nuevaNorma.NOMBRE_NORMA+"') AND dn.VERSION=UPPER('"+nuevaNorma.VERSION+"')";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader = cmd.ExecuteReader();
                bool tiene = false;
                while (reader.Read())
                {
                    tiene = true;
                }
                reader.Close();

                if (!tiene)
                {
                    
                    string query2 = "INSERT INTO dev_normas(VERSION, NOMBRE_NORMA, DESCRIPCION_NORMA, FECHA_ACTUALIZACION,ESTADO,COLORNORMA,COLORCUMPLIMIENTO  )" +
                                "VALUES(UPPER(@VERSION), UPPER(@NOMBRE_NORMA), @DESCRIPCION_NORMA, GETDATE(), @ESTADO, @COLORNORMA, @COLORCUMPLIMIENTO )";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("NOMBRE_NORMA", nuevaNorma.NOMBRE_NORMA);
                    cmd.Parameters.AddWithValue("VERSION", nuevaNorma.VERSION);
                    cmd.Parameters.AddWithValue("DESCRIPCION_NORMA", nuevaNorma.DESCRIPCION_NORMA);
                    cmd.Parameters.AddWithValue("ESTADO", nuevaNorma.ESTADO);
                    cmd.Parameters.AddWithValue("COLORNORMA", nuevaNorma.COLOR_NORMA);
                    cmd.Parameters.AddWithValue("COLORCUMPLIMIENTO", nuevaNorma.COLOR_CUMPLIMIENTO);
                    cmd.CommandText = query2;
                    cmd.ExecuteNonQuery();
                    query2 = "select COD_NORMA from dev_normas where NOMBRE_NORMA='" + nuevaNorma.NOMBRE_NORMA + "' AND VERSION='" + nuevaNorma.VERSION + "'";
                    cmd.Parameters.Clear();
                    cmd.CommandText = query2;
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader2 = cmd.ExecuteReader();
                    int id = 0;
                    while (reader2.Read())
                    {
                        id = Convert.ToInt32(reader2["COD_NORMA"].ToString());
                    }
                    reader2.Close();


                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("id_norma", id);
                    cmd.Parameters.AddWithValue("id_empresa", nuevaNorma.ID_EMPRESA);
                    query2 = "INSERT INTO dev_norma_empresa(ID_NORMA, ID_EMPRESA )" +
                            "VALUES(@id_norma, UPPER(@id_empresa) )";
                    cmd.CommandText = query2;
                    cmd.ExecuteNonQuery();

                    transaccion.Commit();
                    Tracking t = new Tracking();
                    string queryIn = "SELECT dn.VERSION,dn.NOMBRE_NORMA, dn.DESCRIPCION_NORMA ,dn.FECHA_ACTUALIZACION, dn.ESTADO, dn.COLORNORMA, dn.COLORCUMPLIMIENTO   FROM dev_normas dn";
                    //GUARDAMOS EL TRACKING
                    t.guardarTracking("Crear", "Norma", t.obtenerDatosNuevosyAntiguos(queryIn));
                    return true;
                }
                transaccion.Commit();
                return false;
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        public object actualizarNorma(datosNormaMantenedor norma)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
               string query = "select COD_NORMA from dev_normas where NOMBRE_NORMA=UPPER('" + norma.NOMBRE_NORMA + "') AND VERSION=UPPER('" + norma.VERSION + "')"+
                                " AND COD_NORMA<>'"+ norma.COD_NORMA + "'";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader = cmd.ExecuteReader();
                bool tiene = false;
                while (reader.Read())
                {
                    tiene = true;
                }
                reader.Close();

                if (!tiene)
                {
                    Tracking t = new Tracking();
                    string queryIn = "SELECT dn.VERSION,dn.NOMBRE_NORMA, dn.DESCRIPCION_NORMA ,dn.FECHA_ACTUALIZACION, dn.ESTADO, dn.COLORNORMA, dn.COLORCUMPLIMIENTO   FROM dev_normas dn where dn.COD_NORMA = '" + norma.COD_NORMA + "'";
                    List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

                    string query2 = "UPDATE dev_normas SET VERSION=UPPER(@VERSION), NOMBRE_NORMA=UPPER(@NOMBRE_NORMA), DESCRIPCION_NORMA=@DESCRIPCION_NORMA, FECHA_ACTUALIZACION=GETDATE(),ESTADO=@ESTADO,COLORNORMA=@COLORNORMA,COLORCUMPLIMIENTO=@COLORCUMPLIMIENTO  " +
                                    "WHERE COD_NORMA=@COD_NORMA";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("COD_NORMA", norma.COD_NORMA);
                    cmd.Parameters.AddWithValue("NOMBRE_NORMA", norma.NOMBRE_NORMA);
                    cmd.Parameters.AddWithValue("VERSION", norma.VERSION);
                    cmd.Parameters.AddWithValue("DESCRIPCION_NORMA", norma.DESCRIPCION_NORMA);
                    cmd.Parameters.AddWithValue("ESTADO", norma.ESTADO);
                    cmd.Parameters.AddWithValue("COLORNORMA", norma.COLOR_NORMA);
                    cmd.Parameters.AddWithValue("COLORCUMPLIMIENTO", norma.COLOR_CUMPLIMIENTO);
                    cmd.CommandText = query2;
                    cmd.ExecuteNonQuery();
                    //Areas 
                    query2 = "DELETE FROM dev_norma_area WHERE COD_NORMA='"+norma.COD_NORMA+"'";
                    cmd.CommandText = query2;
                    cmd.ExecuteNonQuery();
                    transaccion.Commit();
                    //GUARDAMOS EL TRACKING
                    t.guardarTracking("Editar", "Norma", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
                    return true;
                }
                transaccion.Commit();
                return false;
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public object eliminarNorma(int norma)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                bool eliminar = true;
                string query = "select COD_NORMA from dev_auditoria where  COD_NORMA='" + norma + "'";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    eliminar = false;
                }
                reader.Close();
                if ( eliminar )
                {
                    /*query = "delete from dev_evaluacion where ID_AUDITORIA IN(SELECT ID_AUDITORIA from dev_auditoria where COD_NORMA='" + norma + "')";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    query = "delete from dev_nc_workflow where ID_AUDITORIA IN(SELECT ID_AUDITORIA from dev_auditoria where COD_NORMA='" + norma + "')";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    query = "delete from dev_no_conformidad where ID_AUDITORIA IN(SELECT ID_AUDITORIA from dev_auditoria where COD_NORMA='" + norma + "')";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    query = "delete from dev_auditoria where COD_NORMA='" + norma + "'";
                    cmd.CommandText = query;*/
                    cmd.ExecuteNonQuery();
                    query = "delete from dev_temporal_norma where COD_NORMA='" + norma + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    query = "delete from dev_doc_pn where COD_NORMA='" + norma + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    query = "delete from dev_punto_normativo where COD_NORMA='" + norma + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    query = "delete from dev_norma_area where COD_NORMA='" + norma + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    query = "delete from dev_normas where COD_NORMA='" + norma + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    transaccion.Commit();

                 /*   Tracking t = new Tracking();
                    string queryIn = "SELECT dn.NOMBRE_NORMA, dn.COD_NORMA, dn.VERSION, dn.DESCRIPCION_NORMA, dn.FECHA_ACTUALIZACION, dn.ESTADO, dn.COLORNORMA, dn.COLORCUMPLIMIENTO, " +
                         " replace(( SELECT NOM_AREA + ' '  FROM dev_norma_area dna " +
                            " WHERE dn.COD_NORMA = dna.COD_NORMA ORDER BY dn.COD_NORMA " +
                                "FOR XML PATH('') ), ',', ' ') AS AREAS  " +
                        "    FROM dev_normas dn  where dn.COD_NORMA='" + norma + "'";
                    t.guardarTracking("Eliminar", "Norma", t.obtenerDatosNuevosyAntiguos(queryIn));*/
                    
                    return eliminar;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        public object cargarAreas()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select nombre from dev_area where estado='Habilitado'";

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<string> lista = new List<string>();

                while (reader.Read())
                {
                    lista.Add(reader["nombre"].ToString());
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

        public object cargarNormaAreas(int ID)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select NOM_AREA from dev_norma_area where COD_NORMA='"+ ID + "'";

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<string> lista = new List<string>();

                while (reader.Read())
                {
                    lista.Add(reader["NOM_AREA"].ToString());
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

        public object habilitarDeshabilitar(habilitarDeshabilitarNorma request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            //TRACKING
            Tracking t = new Tracking();

            string queryIn = "SELECT  COD_NORMA  FROM dev_normas WHERE COD_NORMA in (" + request.id + ")";
            try
            {
                //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
                List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

                string query = "UPDATE dev_normas SET ESTADO  = '" + request.estado + "' where COD_NORMA  in (" + request.id + ") ";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                transaccion.Commit();

                var accion = request.estado == "Habilitado" ? "Habilitar" : "Deshabilitar";
                //GUARDAMOS EL TRACKING
                t.guardarTracking(accion, "Norma", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

                return true;
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        public object despublicarNorma(int COD_NORMA, string id_empresa)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;

            try
            {
                string query = "";
                //COMPRUEBO SI EL USUARIO ES ADMINISTRADOR
                List<datos_perfil> datosUsuario = (List < datos_perfil >) datosusuarios();
                if(datosUsuario.Count()>0 && datosUsuario.ElementAt(0).ADMINISTRADOR=="true")
                {

                    query = "select ID_AUDITORIA  from dev_auditoria where COD_NORMA='" + COD_NORMA + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader1 = cmd.ExecuteReader();
                    bool tieneAuditoriaAbierta = false;
                    while (reader1.Read())
                    {
                        tieneAuditoriaAbierta = true;
                    }
                    reader1.Close();
                    if (tieneAuditoriaAbierta)
                    {
                        return false;
                    }

                    //AUDITORIAS
                    //QUITAMOS LAS EVALUACIONES DE LA AUDITORIA
                    query = "delete from dev_evaluacion where ID_AUDITORIA IN ( select ID_AUDITORIA from dev_auditoria where COD_NORMA='" + COD_NORMA + "')";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    query = "delete from dev_nc_workflow  where ID_AUDITORIA IN ( select ID_AUDITORIA from dev_auditoria where COD_NORMA='" + COD_NORMA + "')";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    query = "delete from dev_no_conformidad  where ID_AUDITORIA IN ( select ID_AUDITORIA from dev_auditoria where COD_NORMA='" + COD_NORMA + "')";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    //QUITAMOS TODO LO RELACIONADO A LA NORMA
                    query = "delete from dev_auditoria where COD_NORMA='" + COD_NORMA + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    query = "delete from dev_temporal_norma where COD_NORMA='" + COD_NORMA + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    query = "delete from dev_doc_pn where COD_NORMA='" + COD_NORMA + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    query = "delete from dev_punto_normativo where COD_NORMA='" + COD_NORMA + "'";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    transaccion.Commit();


                    //ENVIAR NOTIFICACION
                    query = "Select NOMBRE_NORMA, VERSION FROM dev_normas where COD_NORMA = " + COD_NORMA;
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();

                    string nombre_norma = "";
                    string version = "";

                    while (reader.Read())
                    {
                        datosDocumentoPuntoNormativo obj = new datosDocumentoPuntoNormativo();
                        nombre_norma = reader["NOMBRE_NORMA"].ToString();
                        version = reader["VERSION"].ToString();
                    }
                    reader.Close();

                    Tracking t = new Tracking();
                    string queryIn = "select COD_NORMA , VERSION, NOMBRE_NORMA, DESCRIPCION_NORMA,FECHA_ACTUALIZACION, ESTADO, " +
                                   "  COLORNORMA,COLORCUMPLIMIENTO from dev_normas" +
                                   "   where COD_NORMA = '" + COD_NORMA + "'";

                    //GUARDAMOS EL TRACKING
                    t.guardarTracking("Despublicar", "Gestor norma", t.obtenerDatosNuevosyAntiguos(queryIn));



                    //genero la notificación de la publicación de la norma
                    menuModel modelo = new menuModel();

                    int id_notificacion = modelo.guardar_notificacion("Se ha despublicado la norma " + nombre_norma + " versión " + version, "", id_empresa);
                   /* SqlCommand command = new SqlCommand("SP_SGI_ALERTA_DESPUBLICACION_NORMA", conn);
                    command.CommandType = CommandType.StoredProcedure;

                    string destinatarios = modelo.emailsAreaNorma(COD_NORMA);
                    string url = WebConfigurationManager.AppSettings["baseURL"] + "norma/" + COD_NORMA;
                    command.Parameters.AddWithValue("COD_NORMA", COD_NORMA);
                    command.Parameters.AddWithValue("emails", destinatarios);
                    command.Parameters.AddWithValue("URL", url);
                    command.ExecuteNonQuery();
                    
                    */

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }




        public object editarAuditoria(datosAuditoria request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                Tracking t = new Tracking();
                string queryIn = "SELECT  COD_NORMA, TIPO_AUDITORIA, RUT_EMPRESA, ESTADO, FECHA, OBSERVACION, USER_ID FROM dev_auditoria WHERE ID_AUDITORIA='" + request.ID_AUDITORIA + "'";
                List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);
                string query="";
                query += "declare @FECHA datetime "; 
                query += "set @FECHA = convert(datetime, '" + request.FECHA + "', 103) ";
                query += "UPDATE dev_auditoria set  TIPO_AUDITORIA=@TIPO_AUDITORIA, "+
                               "RUT_EMPRESA=@RUT_EMPRESA, FECHA=@FECHA, OBSERVACION=@OBSERVACION "+
                               " WHERE ID_AUDITORIA=@ID_AUDITORIA";
                cmd.Parameters.AddWithValue("ID_AUDITORIA", request.ID_AUDITORIA);
                cmd.Parameters.AddWithValue("TIPO_AUDITORIA", request.TIPO_AUDITORIA);
                cmd.Parameters.AddWithValue("RUT_EMPRESA", request.RUT_EMPRESA);
                cmd.Parameters.AddWithValue("OBSERVACION", request.OBSERVACION);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                transaccion.Commit();
                t.guardarTracking("Editar", "Auditoría", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
                return true;
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public object anularAuditoria(datosAuditoria request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {

                //COMPROBAR SI SE PUEDE ANULAR
                // string query = " SELECT ID_PUNTO_NORMATIVO FROM dev_no_conformidad WHERE ID_AUDITORIA=@ID_AUDITORIA";

                string query = " SELECT ID_PUNTO_NORMATIVO FROM dev_nc_workflow WHERE ID_AUDITORIA=@ID_AUDITORIA";
                cmd.Parameters.AddWithValue("ID_AUDITORIA", request.ID_AUDITORIA);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                
                SqlDataReader reader = cmd.ExecuteReader();
                bool eliminar = true;
                while (reader.Read())
                {
                    eliminar=false;

                }
                reader.Close();
                if (eliminar)
                {
                    cmd.Parameters.Clear();
                    Tracking t = new Tracking();
                    string queryIn = "SELECT  COD_NORMA, TIPO_AUDITORIA, RUT_EMPRESA, ESTADO, FECHA, OBSERVACION, USER_ID FROM dev_auditoria WHERE ID_AUDITORIA='" + request.ID_AUDITORIA + "'";
                    List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);
                    query = "UPDATE dev_auditoria set  ESTADO=@ESTADO WHERE ID_AUDITORIA=@ID_AUDITORIA";
                    cmd.Parameters.AddWithValue("ID_AUDITORIA", request.ID_AUDITORIA);
                    cmd.Parameters.AddWithValue("ESTADO", "Anulada");
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    transaccion.Commit();
                    t.guardarTracking("Anular", "Auditoría", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public object obtenerNoConformidades(datosRequestNoConformidades request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {
                string query = " SELECT "+
                               " CONVERT(NVARCHAR(10), dv.FECHA, 105) as FECHA_AUDITORIA, "+
                               " dnc.ID_AUDITORIA, "+
                               " dnc.NO_CONFORMIDAD, "+
                               " dnc.ESTADO, "+
                               " dnc.ID_NC, "+
                               " COUNT(dnw.ID_PUNTO_NORMATIVO) as RESPUESTA, "+
                               " CONVERT(NVARCHAR(10), dnc.FECHA, 105) as FECHA, " +
                               " wu.FULLNAME + ' (' + wu.Email + ')' RESPONSABLE FROM dev_no_conformidad dnc " +
                               " left join dev_punto_normativo dpn on dnc.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO " +
                               " left join dev_nc_workflow dnw on dnc.ID_PUNTO_NORMATIVO = dnw.ID_PUNTO_NORMATIVO AND dnc.ID_NC = dnw.ID_NC " +
                               " inner join web_users wu on dnc.USER_RESP = wu.USER_ID " +
                               " left join dev_auditoria dv on dnc.ID_AUDITORIA = dv.ID_AUDITORIA "+
                               " WHERE dnc.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO AND dnc.ESTADO != 'Aprobado' AND dpn.COD_NORMA = @COD_NORMA " +
                               " GROUP BY dnc.ID_NC, dnc.ID_AUDITORIA, dnc.ID_PUNTO_NORMATIVO, dnc.NO_CONFORMIDAD, " +
                               " dnc.FECHA, dnc.ESTADO, wu.FULLNAME + ' (' + wu.Email + ')', dv.FECHA ";
                cmd.Parameters.AddWithValue("COD_NORMA", request.COD_NORMA);
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", request.ID_PUNTO_NORMATIVO);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader = cmd.ExecuteReader();

                List<datosNoConformidadMultiple> lista = new List<datosNoConformidadMultiple>();

                while (reader.Read())
                {
                    datosNoConformidadMultiple obj = new datosNoConformidadMultiple();
                    obj.ID_AUDITORIA = reader["ID_AUDITORIA"].ToString();
                    obj.ID_PUNTO_NORMATIVO = request.ID_PUNTO_NORMATIVO;
                    obj.OBSERVACION = reader["NO_CONFORMIDAD"].ToString();
                    obj.FECHA = reader["FECHA"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
                    obj.RESPONSABLE = reader["RESPONSABLE"].ToString();
                    obj.RESPUESTA = reader["RESPUESTA"].ToString();
                    obj.ID_NC = reader["ID_NC"].ToString();
                    obj.FECHA_AUDITORIA = reader["FECHA_AUDITORIA"].ToString();
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