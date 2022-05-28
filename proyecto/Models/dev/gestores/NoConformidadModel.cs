using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


//agregar para crear lo basico
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Security.Claims;
using System.Threading;
using System.Web.Configuration;
using System.IO;
using System.Globalization;

public class NoConformidadModel : Conexion
{
    public object cargarDatos(Cargar datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {
            string query = "";
            query += "select  ADMINISTRADOR , AUDITOR from web_groups where GROUP_ID =  " + datos.GROUP_ID + " ";

            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            SqlDataReader reader2 = cmd.ExecuteReader();
            var ADMINISTRADOR = "";
            var AUDITOR = "";
            while (reader2.Read())
            {
                ADMINISTRADOR = reader2["ADMINISTRADOR"].ToString();
                AUDITOR = reader2["AUDITOR"].ToString();

            }
            reader2.Close();
            query = "";
            string desde = datos.FECHA_FILTRO_DESDE;
            string hasta = datos.FECHA_FILTRO_HASTA;
            if (desde != "" && desde != null)
            {
                query += "declare @desde datetime ";
                query += "set @desde = convert(datetime, '" + desde + "', 103)";
            }
            if (hasta != "" && hasta != null)
            {
                query += "declare @hasta datetime ";
                query += "set @hasta = convert(datetime, '" + hasta + "', 103)";
            }
            query += "select dn.COD_NORMA , CONCAT(dn.NOMBRE_NORMA,' ver. ',dn.VERSION) as NOMBRE_NORMA , dnc.ID_PUNTO_NORMATIVO, dnc.NO_CONFORMIDAD , wu.FULLNAME AS AUDITOR,convert(nvarchar(10) ,dnc.FECHA, 105) as FECHA , " +
                    " max(isnull(convert(nvarchar(10) ,dnw.FECHA_COMPROMISO, 105),'') ) as FECHA_COMPROMISO,da.TIPO_AUDITORIA,wun.FULLNAME AS RESPONSABLE, dnc.ESTADO, convert(nvarchar(10) ,da.FECHA, 105) as FECHA_AUDITORIA " +
                    " ,convert(varchar(100),dpn.DESC_PUNTO_NORMATIVO) as DESC_PUNTO_NORMATIVO , dnc.ID_AUDITORIA,wunc.FULLNAME as CreadorNoConformidad" +
                    " ,convert(nvarchar(10) ,de.FECHA, 105) as FECHAEVALUACION " +
                    " , dnc.ID_NC " +
                    " from dev_no_conformidad dnc " +
                    " inner join dev_auditoria da on da.ID_AUDITORIA = dnc.ID_AUDITORIA "+
                    " inner join dev_evaluacion de on de.ID_AUDITORIA = dnc.ID_AUDITORIA  and de.ID_PUNTO_NORMATIVO = dnc.ID_PUNTO_NORMATIVO "+
                    " inner join dev_punto_normativo dpn on dpn.COD_NORMA = da.COD_NORMA and dpn.ID_PUNTO_NORMATIVO = dnc.ID_PUNTO_NORMATIVO" +
                    " inner join dev_normas dn on dn.COD_NORMA = da.COD_NORMA "+
                    " inner join dev_norma_empresa dne on dne.id_norma = dn.COD_NORMA and dne.id_empresa =  "+datos.ID_EMPRESA+" "+
                    " inner join web_users wu on wu.USER_ID = da.USER_ID "+
                    " left join dev_nc_workflow dnw on dnw.ID_AUDITORIA = dnc.ID_AUDITORIA and dnw.ID_PUNTO_NORMATIVO = dnc.ID_PUNTO_NORMATIVO and " +
                    " dnw.FECHA_COMPROMISO is not null and dnw.ESTADO !='Rechazado' and dnw.TIPO = 'PA' and dnc.ID_NC=dnw.ID_NC " +
                    " inner join web_users wun on wun.USER_ID = dnc.USER_RESP " +
                    " inner join web_users wunc on wunc.USER_ID = de.USER_ID " +
                    " where  1=1 ";

            if (datos.ESTADO != "" && datos.ESTADO != null && datos.ESTADO != ")")
            {
                query += " and dnc.ESTADO in "+ datos.ESTADO + " ";
                cmd.Parameters.AddWithValue("estado", datos.ESTADO);
            }
            if (desde != "" && desde != null)
            {
                query += " and dnc.FECHA >= @desde ";
            }
            if (hasta != "" && hasta != null)
            {
                query += " and dnc.FECHA <= @hasta ";
            }
            if (datos.TIPO_AUDITORIA != "" && datos.TIPO_AUDITORIA != null && datos.TIPO_AUDITORIA != ")")
            {
                query += " and da.TIPO_AUDITORIA in "+ datos.TIPO_AUDITORIA + " ";
            }

            if (ADMINISTRADOR == "false")
            {
                if (AUDITOR == "false")
                {
                    query += " and dnc.USER_RESP = @user_id ";
                    cmd.Parameters.AddWithValue("user_id", datos.USER_ID);
                }

            }


            query += " group by de.ID_PUNTO_NORMATIVO, dn.COD_NORMA, dn.NOMBRE_NORMA, dn.VERSION, dnc.ID_PUNTO_NORMATIVO, "+
                    "dnc.NO_CONFORMIDAD, wu.FULLNAME,dnc.ESTADO, wun.FULLNAME ,dnc.FECHA, da.TIPO_AUDITORIA,da.FECHA,dpn.DESC_PUNTO_NORMATIVO,dnc.ID_AUDITORIA,wunc.FULLNAME,de.FECHA, dnc.ID_NC ";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            List<noConformidadRequest> lista = new List<noConformidadRequest>();
            
            while (reader.Read())
            {
                noConformidadRequest obj = new noConformidadRequest();

                obj.COD_NORMA = reader["COD_NORMA"].ToString();
                obj.NOMBRE_NORMA = reader["NOMBRE_NORMA"].ToString();
                obj.ID_PUNTO_NORMATIVO = reader["ID_PUNTO_NORMATIVO"].ToString();
                obj.NO_CONFORMIDAD = reader["NO_CONFORMIDAD"].ToString();
                obj.AUDITOR = reader["AUDITOR"].ToString();
                obj.FECHA = reader["FECHA"].ToString();
                obj.FECHA_AUDITORIA = reader["FECHA_AUDITORIA"].ToString();
                obj.DESC_PUNTO_NORMATIVO = reader["DESC_PUNTO_NORMATIVO"].ToString();
                obj.ID_AUDITORIA = reader["ID_AUDITORIA"].ToString();
                obj.FECHA_COMPROMISO = reader["FECHA_COMPROMISO"].ToString();
                obj.TIPO_AUDITORIA = reader["TIPO_AUDITORIA"].ToString();
                obj.RESPONSABLE = reader["RESPONSABLE"].ToString();
                obj.ESTADO = reader["ESTADO"].ToString();
                obj.CreadorNoConformidad = reader["CreadorNoConformidad"].ToString();
                //obj.AREA_RESPONSABLE = reader["AREA_RESPONSABLE"].ToString();
                obj.FECHAEVALUACION = reader["FECHAEVALUACION"].ToString();
                obj.ID_NC = reader["ID_NC"].ToString();
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

    public bool guardar(Guardar datos)
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

            JavaScriptSerializer js = new JavaScriptSerializer();
            object[] testObj = (object[])js.Deserialize(datos.datos, new object().GetType());

            int id = 0;
            string query = " select top 3  ID_WORKFLOW,TIPO,convert(varchar(10) ,FECHA) as FECHA,RESPONSABLE,OBSERVACION,FECHA_COMPROMISO,ADJUNTO " +
                           " from dev_nc_workflow " +
                           " where ID_AUDITORIA = @ID_AUDITORIA and ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and ESTADO = 'En revisión' AND ID_NC=@ID_NC" +
                           " order by ID_WORKFLOW desc ";


            cmd.Parameters.AddWithValue("ID_AUDITORIA", datos.ID_AUDITORIA);
            cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
            cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);
            cmd.Parameters.AddWithValue("ESTADO", "En revisión");


            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

           
            // TODO propuesto eliminar por desuso
            
            string archivo = "";

            cmd.Parameters.Clear();
            
            foreach (var item in testObj)
            {
                Dictionary<string, object> Nodo = (Dictionary<string, object>)item;
                
                string tipo = Nodo["TIPO"].ToString();
               
                if (Nodo["TIPO"].ToString() == "AC")
                {
                    cmd.Parameters.Clear();
                    query = "";
                    query += "INSERT INTO dev_nc_workflow (ID_AUDITORIA, ID_PUNTO_NORMATIVO, TIPO,FECHA ,RESPONSABLE,OBSERVACION,FECHA_COMPROMISO,ADJUNTO,ESTADO, ID_NC ) " +
                    " VALUES (@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@TIPO, GETDATE(),@RESPONSABLE,@OBSERVACION,GETDATE(),@ADJUNTO,@ESTADO, @ID_NC) ";

                    cmd.Parameters.AddWithValue("ID_AUDITORIA", datos.ID_AUDITORIA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);
                    cmd.Parameters.AddWithValue("TIPO", Nodo["TIPO"]);
                    cmd.Parameters.AddWithValue("RESPONSABLE", Nodo["RESPONSABLE"]);
                    cmd.Parameters.AddWithValue("OBSERVACION", Nodo["OBSERVACION"]);
                    cmd.Parameters.AddWithValue("ADJUNTO", Nodo["ADJUNTO"]);
                    cmd.Parameters.AddWithValue("ESTADO", "En revisión");

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    archivo = Nodo["ADJUNTO"].ToString();
                        
                    query = "";
                    query += " UPDATE dev_no_conformidad set ESTADO = @ESTADO WHERE ID_AUDITORIA = @ID_AUDITORIA and " +
                    " ID_PUNTO_NORMATIVO=@ID_PUNTO_NORMATIVO  and ID_NC=@ID_NC";
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
                if (Nodo["TIPO"].ToString() == "PA")
                {
                    cmd.Parameters.Clear();
                    query = "";
                    query += "declare @FECHA_COMPROMISO datetime ";
                    query += "set @FECHA_COMPROMISO = convert(datetime, '" + Nodo["FECHA_COMPROMISO"] + "', 103) ";
                    query += "INSERT INTO dev_nc_workflow (ID_AUDITORIA, ID_PUNTO_NORMATIVO, TIPO,FECHA ,RESPONSABLE,OBSERVACION,FECHA_COMPROMISO,ADJUNTO,ESTADO, ID_NC ) " +
                    " VALUES (@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@TIPO, GETDATE(),@RESPONSABLE,@OBSERVACION,@FECHA_COMPROMISO,@ADJUNTO,@ESTADO,@ID_NC) ";

                    cmd.Parameters.AddWithValue("ID_AUDITORIA", datos.ID_AUDITORIA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("TIPO", Nodo["TIPO"]);
                    cmd.Parameters.AddWithValue("RESPONSABLE", Nodo["RESPONSABLE"]);
                    cmd.Parameters.AddWithValue("OBSERVACION", Nodo["OBSERVACION"]);
                    cmd.Parameters.AddWithValue("ADJUNTO", Nodo["ADJUNTO"]);
                    cmd.Parameters.AddWithValue("ESTADO", "En revisión");
                    cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    archivo = Nodo["ADJUNTO"].ToString();

                    query = "";
                    query += " UPDATE dev_no_conformidad set ESTADO = @ESTADO WHERE ID_AUDITORIA = @ID_AUDITORIA and " +
                    " ID_PUNTO_NORMATIVO=@ID_PUNTO_NORMATIVO  and ID_NC=@ID_NC";

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                if (Nodo["TIPO"].ToString() == "AI")
                {
                    cmd.Parameters.Clear();
                    //query += "declare @FECHA_COMPROMISO datetime ";
                    //query += "set @FECHA = convert(datetime, '" + Nodo["FECHA"] + "', 103)";
                    //query += "set @FECHA_COMPROMISO = convert(datetime, '" + Nodo["FECHA_COMPROMISO"] + "', 103)";
                    query = "";
                    query += "INSERT INTO dev_nc_workflow (ID_AUDITORIA, ID_PUNTO_NORMATIVO, TIPO,FECHA ,RESPONSABLE,OBSERVACION,FECHA_COMPROMISO,ADJUNTO,ESTADO, ID_NC ) " +
                        " VALUES (@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@TIPO,GETDATE(),@RESPONSABLE,@OBSERVACION,GETDATE(),@ADJUNTO,@ESTADO,@ID_NC) ";

                    cmd.Parameters.AddWithValue("ID_AUDITORIA", datos.ID_AUDITORIA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);
                    cmd.Parameters.AddWithValue("TIPO", Nodo["TIPO"]);
                    //cmd.Parameters.AddWithValue("FECHA", Nodo["FECHA"]);
                    cmd.Parameters.AddWithValue("RESPONSABLE", Nodo["RESPONSABLE"]);
                    cmd.Parameters.AddWithValue("OBSERVACION", Nodo["OBSERVACION"]);
                    //cmd.Parameters.AddWithValue("FECHA_COMPROMISO", Nodo["FECHA_COMPROMISO"]);
                    cmd.Parameters.AddWithValue("ADJUNTO", Nodo["ADJUNTO"]);
                    cmd.Parameters.AddWithValue("ESTADO", "En revisión");

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    archivo = Nodo["ADJUNTO"].ToString();
                }

            }
            if (archivo != "")
            {
                cmd.Parameters.Clear();
                query = "select   max(ID_WORKFLOW) as ID_WORKFLOW   from dev_nc_workflow where TIPO = 'PA'  and ID_AUDITORIA= @ID_AUDITORIA and ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and ID_NC=@ID_NC";


                //cmd.Parameters.AddWithValue("empresa", empresa);

                cmd.Parameters.AddWithValue("ID_AUDITORIA", datos.ID_AUDITORIA);
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader2 = cmd.ExecuteReader();

                while (reader2.Read())
                {

                    id = Convert.ToInt32(reader2["ID_WORKFLOW"].ToString());

                }
                reader2.Close();

                //ACTUALIZAR EL PUNTO NORMATIVO LA ULTIMA NC A


                string ruta_destino = mover_documento(archivo, id);
            }
           
            transaccion.Commit();
            //TRACKING
            Tracking t = new Tracking();
            //string queryIn = "SELECT ID_AUDITORIA,COD_NORMA, TIPO_AUDITORIA, RUT_EMPRESA,FECHA ,OBSERVACION,USER_ID,ESTADO   FROM dev_auditoria da  where COD_NORMA = '" + request.COD_NORMA + "' ";
            //GUARDAMOS EL TRACKING
            //t.guardarTracking("Crear", "Auditoria", t.obtenerDatosNuevosyAntiguos(queryIn));
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


    public object cargarInicio(inicial data)
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
            string idAuditoria = "";
            if (data.ID_AUDITORIA_ORIG != null)
            {
               idAuditoria = data.ID_AUDITORIA_ORIG;
                
            }else
            {
                idAuditoria = data.ID_AUDITORIA;
            }
            
            string query = "";
            if (data.GESTOR == "norma")
            {

                query = " select top 4  ID_WORKFLOW,TIPO,convert(varchar(10) ,FECHA) as FECHA,RESPONSABLE,OBSERVACION,convert(varchar(10) ,FECHA_COMPROMISO) FECHA_COMPROMISO,ADJUNTO " +
                           " from dev_nc_workflow " +
                           " where ID_AUDITORIA = @ID_AUDITORIA and ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and ESTADO = 'Aprobado' AND ID_NC = @ID_NC " +
                           " order by ID_WORKFLOW desc ";

           
                cmd.Parameters.AddWithValue("ID_AUDITORIA", idAuditoria);
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", data.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("ID_NC", data.ID_NC);
                cmd.Parameters.AddWithValue("ESTADO", "En revisión");


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

            }else
            {
                if (data.ID_AUDITORIA == null)
                {
                     query =
                         " SELECT " +
                            " dnw.ID_WORKFLOW " +
                            " ,dnw.TIPO " +
                            " ,CONVERT(NVARCHAR(10), dnw.FECHA, 105) AS FECHA " +
                            " , dnw.RESPONSABLE " +
                            " ,dnw.OBSERVACION  " +
                            " ,CONVERT(NVARCHAR(10), dnw.FECHA_COMPROMISO, 105) FECHA_COMPROMISO  " +
                            " ,dnw.ADJUNTO  " +
                            " FROM dev_nc_workflow dnw  " +
                            " WHERE  DNW.ID_WORKFLOW IN ( "+
                           "  SELECT max(dnw.ID_WORKFLOW)FROM dev_auditoria DA "+
                           " JOIN dev_evaluacion DE ON DE.ID_AUDITORIA = DA.ID_AUDITORIA and de.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO "+
                           " JOIN dev_nc_workflow dnw on dnw.ID_AUDITORIA = de.ID_AUDITORIA and dnw.ID_PUNTO_NORMATIVO = de.ID_PUNTO_NORMATIVO and dnw.ESTADO = 'En revisión' "+
                           " where da.COD_NORMA = @COD_NORMA AND dnw.ID_NC = @ID_NC GROUP BY dnw.TIPO) "+
                           " union "+
                           " SELECT "+
                           " dnw.ID_WORKFLOW " +
                            " ,dnw.TIPO " +
                            " ,CONVERT(NVARCHAR(10), dnw.FECHA, 105) AS FECHA " +
                            " , dnw.RESPONSABLE " +
                            " ,dnw.OBSERVACION  " +
                            " ,CONVERT(NVARCHAR(10), dnw.FECHA_COMPROMISO, 105) FECHA_COMPROMISO  " +
                            " ,dnw.ADJUNTO  " +
                            " FROM dev_nc_workflow dnw  " +
                            " WHERE  DNW.ID_WORKFLOW IN ( SELECT max(dnw.ID_WORKFLOW)FROM dev_auditoria DA " +
                            " JOIN dev_evaluacion DE ON DE.ID_AUDITORIA = DA.ID_AUDITORIA and de.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO " +
                            " JOIN dev_nc_workflow dnw on dnw.ID_AUDITORIA = de.ID_AUDITORIA and dnw.ID_PUNTO_NORMATIVO = de.ID_PUNTO_NORMATIVO " +
                            " and dnw.ESTADO = 'Rechazado' and dnw.TIPO='SA' " +
                            " where da.COD_NORMA = @COD_NORMA AND dnw.ID_NC = @ID_NC GROUP BY dnw.TIPO) ";
                    ;

                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", data.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("COD_NORMA", data.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_NC", data.ID_NC);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                }
                else
                {
                    query = 
                    " SELECT " +
                    " dnw.ID_WORKFLOW " +
                    " ,dnw.TIPO " +
                    " ,CONVERT(NVARCHAR(10), dnw.FECHA, 105) AS FECHA " +
                    " , dnw.RESPONSABLE " +
                    " ,dnw.OBSERVACION  " +
                    " ,CONVERT(NVARCHAR(10), dnw.FECHA_COMPROMISO, 105) FECHA_COMPROMISO  " +
                    " ,dnw.ADJUNTO  " +
                    " FROM dev_nc_workflow dnw  " +
                    " WHERE  dnw.ID_NC = @ID_NC and DNW.ID_WORKFLOW IN (SELECT MAX(ID_WORKFLOW) AS MAX FROM dev_nc_workflow WHERE ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO AND  ID_NC=@ID_NC AND ID_AUDITORIA = @ID_AUDITORIA AND (ESTADO = 'En revisión' or ESTADO = 'Aprobado' ) GROUP BY TIPO) "+
                    " union "+
                    " SELECT " +
                    " dnw.ID_WORKFLOW " +
                    " ,dnw.TIPO " +
                    " ,CONVERT(NVARCHAR(10), dnw.FECHA, 105) AS FECHA " +
                    " , dnw.RESPONSABLE " +
                    " ,dnw.OBSERVACION  " +
                    " ,CONVERT(NVARCHAR(10), dnw.FECHA_COMPROMISO, 105) FECHA_COMPROMISO  " +
                    " ,dnw.ADJUNTO  " +
                    " FROM dev_nc_workflow dnw  " +
                    " WHERE  dnw.ID_WORKFLOW IN (SELECT MAX(ID_WORKFLOW) AS MAX " +
                    " FROM dev_nc_workflow  WHERE ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO AND  ID_NC=@ID_NC " +
                    " AND  dnw.TIPO='SA' GROUP BY TIPO) ";

                    cmd.Parameters.AddWithValue("ID_AUDITORIA", idAuditoria);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", data.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("COD_NORMA", data.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_NC", data.ID_NC);
                    cmd.Parameters.AddWithValue("ESTADO", "En revisión");

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }

            
            
            SqlDataReader reader = cmd.ExecuteReader();

            List<inicioModal> lista = new List<inicioModal>();

            while (reader.Read())
            {
                inicioModal obj = new inicioModal();
                obj.date = reader["FECHA"].ToString();
                obj.responsable = reader["RESPONSABLE"].ToString();
                obj.observacion = reader["OBSERVACION"].ToString();
                obj.datecompromiso = reader["FECHA_COMPROMISO"].ToString();
                obj.archivo = reader["ADJUNTO"].ToString();
                obj.tipo = reader["TIPO"].ToString();
                obj.ruta = WebConfigurationManager.AppSettings["DocumentosUrl"] + "\\" + "no_conformidad" + "\\" + reader["ID_WORKFLOW"].ToString() + "\\" + reader["ADJUNTO"].ToString();
                
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

    public bool guardarRespuesta(Guardar datos)
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

            string query;
            query = " SELECT  " +
                    " dv.ID_AUDITORIA " +
                    " FROM " +
                    " dev_normas dn " +
                    " LEFT JOIN dev_auditoria dv ON dn.COD_NORMA = dv.COD_NORMA " +
                    " left join dev_evaluacion de on dv.ID_AUDITORIA = de.ID_AUDITORIA " +
                    " LEFT JOIN dev_no_conformidad dnc    ON dnc.ID_AUDITORIA = dv.ID_AUDITORIA and dnc.ESTADO != 'Aprobado' " +
                    " join dev_punto_normativo dpn on dpn.COD_NORMA = dn.COD_NORMA and de.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO and "+
                    " dnc.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO  and dpn.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO " +
                    " LEFT JOIN web_users wu  ON wu.USER_ID = dnc.USER_RESP " +
                    " LEFT JOIN web_users wus    ON wus.USER_ID = de.USER_ID " +
                    " LEFT JOIN dev_nc_workflow dnw  ON dnw.ID_AUDITORIA = dv.ID_AUDITORIA   AND dnw.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO " +
                    " WHERE dpn.COD_NORMA = @COD_NORMA and dpn.ULTIMA_EVALUACION = 'NO' " +
                    " GROUP BY dv.ID_AUDITORIA "; 

                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("COD_NORMA", datos.COD_NORMA);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    

                    SqlDataReader reader = cmd.ExecuteReader();

                    string auditoria = "";

                    while (reader.Read())
                    {
                        auditoria = reader["ID_AUDITORIA"].ToString();

                    }
                    reader.Close();


            if (datos.ESTADO == "Rechazado")
            {
                cmd.Parameters.Clear();
                query = "INSERT INTO dev_nc_workflow (ID_AUDITORIA, ID_PUNTO_NORMATIVO, TIPO,FECHA ,OBSERVACION,ESTADO,USER_ID, ID_NC ) " +
                        " VALUES (@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@TIPO,GETDATE(),@OBSERVACION,@ESTADO,@USER_ID, @ID_NC) ";


                cmd.Parameters.AddWithValue("ID_AUDITORIA",  auditoria );
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("TIPO", datos.TIPO);
                cmd.Parameters.AddWithValue("OBSERVACION", datos.OBSERVACION);
                cmd.Parameters.AddWithValue("ESTADO", datos.ESTADO);
                cmd.Parameters.AddWithValue("USER_ID", userid);
                cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);

                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                
                cmd.Parameters.Clear();

                query = "UPDATE dev_nc_workflow set ESTADO = @ESTADO WHERE ID_NC = @ID_NC and " +
                        "  ESTADO =  'En revisión'  and TIPO != 'AI' ";

                cmd.Parameters.AddWithValue("ID_AUDITORIA",  auditoria );
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);
                cmd.Parameters.AddWithValue("ESTADO", datos.ESTADO);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                // query = " UPDATE dev_no_conformidad set ESTADO = @ESTADO WHERE ID_AUDITORIA = @ID_AUDITORIA and " +
                //       " ID_PUNTO_NORMATIVO=@ID_PUNTO_NORMATIVO and ID_NC = @ID_NC   ";
                query = " UPDATE dev_no_conformidad set ESTADO = @ESTADO WHERE  ID_NC = @ID_NC   ";
                cmd.Parameters.AddWithValue("ID_AUDITORIA",  auditoria );
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);
                cmd.Parameters.AddWithValue("ESTADO", datos.ESTADO);

                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

            }
            else
            {
                // TODO propuesta eliminar por desuso
                //string nuevaAuditoria = "";

                int cantidadNcActivas = 0;
                //REVISAR CUANTAS CONFORMIDADES ACTIVAS
                query = "SELECT CANTIDAD_NC_ACTIVAS AS CANTIDAD FROM dev_punto_normativo WHERE COD_NORMA = @COD_NORMA and " +
                        " ID_PUNTO_NORMATIVO=@ID_PUNTO_NORMATIVO ";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("COD_NORMA", datos.COD_NORMA);
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cantidadNcActivas = Convert.ToInt32( reader["CANTIDAD"].ToString() );
                }
                reader.Close();

                if (cantidadNcActivas==1)
                {
                    if (datos.ID_AUDITORIA != datos.ID_AUDITORIA_ORIG && datos.ID_AUDITORIA != "0")
                    {
                        cmd.Parameters.Clear();
                        query = "INSERT INTO dev_evaluacion (ID_AUDITORIA , ID_PUNTO_NORMATIVO, EVALUACION,  OBSERVACION , USER_ID, TIPO ) " +
                         " VALUES (@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,'SI',@OBSERVACION,@USER_ID,@TIPO) ";


                        cmd.Parameters.AddWithValue("ID_AUDITORIA", datos.ID_AUDITORIA);
                        cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                        cmd.Parameters.AddWithValue("TIPO", datos.TIPO);
                        cmd.Parameters.AddWithValue("OBSERVACION", datos.OBSERVACION);
                        cmd.Parameters.AddWithValue("USER_ID", userid);


                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        auditoria = datos.ID_AUDITORIA_ORIG;


                    }

                    cmd.Parameters.Clear();
                    query = "INSERT INTO dev_nc_workflow (ID_AUDITORIA, ID_PUNTO_NORMATIVO, TIPO,FECHA ,OBSERVACION,ESTADO,USER_ID, ID_NC ) " +
                     " VALUES (@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@TIPO,GETDATE(),@OBSERVACION,@ESTADO,@USER_ID,@ID_NC) ";


                    cmd.Parameters.AddWithValue("ID_AUDITORIA", auditoria);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("TIPO", datos.TIPO);
                    cmd.Parameters.AddWithValue("OBSERVACION", datos.OBSERVACION);
                    cmd.Parameters.AddWithValue("ESTADO", datos.ESTADO);
                    cmd.Parameters.AddWithValue("USER_ID", userid);
                    cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);



                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();

                    query = " UPDATE dev_nc_workflow set ESTADO = @ESTADO WHERE " +
                            "  ESTADO =  'En revisión' AND ID_NC = @ID_NC  ";
                    cmd.Parameters.AddWithValue("ID_AUDITORIA", auditoria);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("ESTADO", datos.ESTADO);
                    cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    query = " UPDATE dev_evaluacion set EVALUACION = 'SI', OBSERVACION = @OBSERVACION   WHERE ID_AUDITORIA = @ID_AUDITORIA and " +
                            " ID_PUNTO_NORMATIVO=@ID_PUNTO_NORMATIVO  ";
                    cmd.Parameters.AddWithValue("ID_AUDITORIA", auditoria);
                    cmd.Parameters.AddWithValue("OBSERVACION", datos.OBSERVACION);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    query = " UPDATE dev_no_conformidad set ESTADO = @ESTADO WHERE  ID_NC = @ID_NC   ";
                    cmd.Parameters.AddWithValue("ID_AUDITORIA", auditoria);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("ESTADO", datos.ESTADO);
                    cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();


                    query = " UPDATE dev_punto_normativo set ULTIMA_EVALUACION = 'SI' , OBSERVACION = @OBSERVACION, "+
                            " CANTIDAD_NC_ACTIVAS=@CANTIDAD_NC_ACTIVAS WHERE COD_NORMA = @COD_NORMA and " +
                            " ID_PUNTO_NORMATIVO=@ID_PUNTO_NORMATIVO ";
                    cmd.Parameters.AddWithValue("COD_NORMA", datos.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("OBSERVACION", datos.OBSERVACION);
                    cmd.Parameters.AddWithValue("CANTIDAD_NC_ACTIVAS", 0);


                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                else
                {
                    //ACTUALIZAMOS LA ULTIMA_ID_NC Y CANTIDAD_NC_ACTIVAS
                    cmd.Parameters.Clear();
                    query = "INSERT INTO dev_nc_workflow (ID_AUDITORIA, ID_PUNTO_NORMATIVO, TIPO,FECHA ,OBSERVACION,ESTADO,USER_ID, ID_NC ) " +
                     " VALUES (@ID_AUDITORIA,@ID_PUNTO_NORMATIVO,@TIPO,GETDATE(),@OBSERVACION,@ESTADO,@USER_ID,@ID_NC) ";

                    cmd.Parameters.AddWithValue("ID_AUDITORIA", auditoria);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("TIPO", datos.TIPO);
                    cmd.Parameters.AddWithValue("OBSERVACION", datos.OBSERVACION);
                    cmd.Parameters.AddWithValue("ESTADO", datos.ESTADO);
                    cmd.Parameters.AddWithValue("USER_ID", userid);
                    cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    query = " UPDATE dev_no_conformidad set ESTADO = @ESTADO WHERE  ID_NC = @ID_NC   ";
                    cmd.Parameters.AddWithValue("ID_NC", datos.ID_NC);
                    cmd.Parameters.AddWithValue("ESTADO", datos.ESTADO);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    query = " SELECT MAX(dnc.ID_NC) MAX FROM dev_auditoria da "+
                            " INNER JOIN dev_no_conformidad dnc on da.ID_AUDITORIA = dnc.ID_AUDITORIA "+
                            " and dnc.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and dnc.ESTADO != 'Aprobado' " +
                            " WHERE da.COD_NORMA = @COD_NORMA ";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("COD_NORMA", datos.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    reader = cmd.ExecuteReader();
                    int idUltimaNC = 0;
                    while (reader.Read())
                    {
                        idUltimaNC = Convert.ToInt32(reader["MAX"].ToString());
                    }
                    reader.Close();
                    cmd.Parameters.Clear();

                    query = " UPDATE dev_punto_normativo set  "+
                            " CANTIDAD_NC_ACTIVAS=@CANTIDAD_NC_ACTIVAS, "+
                            " ULTIMA_ID_NC = @ULTIMA_ID_NC "+
                            " WHERE COD_NORMA = @COD_NORMA and " +
                            " ID_PUNTO_NORMATIVO=@ID_PUNTO_NORMATIVO ";
                    cmd.Parameters.AddWithValue("COD_NORMA", datos.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                    cmd.Parameters.AddWithValue("OBSERVACION", datos.OBSERVACION);
                    cmd.Parameters.AddWithValue("CANTIDAD_NC_ACTIVAS", cantidadNcActivas-1);
                    cmd.Parameters.AddWithValue("ULTIMA_ID_NC", idUltimaNC);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            transaccion.Commit();
//-------------------------
        if (datos.ESTADO == "Rechazado")
        {               query = $@"SELECT
                            wu.email
                            FROM dev_no_conformidad dnc
                            join web_users wu on wu.USER_ID = dnc.USER_RESP and wu.ESTADO = 'Habilitado'
                            where dnc.ID_NC = {datos.ID_NC}
                            ";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            SqlDataReader readers = cmd.ExecuteReader();

            string email = "";

            while (readers.Read())
            {
                email = readers["email"].ToString();
            }
            readers.Close();

            //genero el correo
            if (email != "")
            {
                string url = WebConfigurationManager.AppSettings["clientURL"] + "noConformidad";
                SqlCommand command = new SqlCommand("SP_SGI_ALERTA_RECHAZO_NOCONFORMIDAD", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("COD_NORMA", datos.COD_NORMA);
                command.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", datos.ID_PUNTO_NORMATIVO);
                command.Parameters.AddWithValue("emails", email);
                command.Parameters.AddWithValue("URL", url);
                command.ExecuteNonQuery();
            }
        }



            //TRACKING
            Tracking t = new Tracking();
            //string queryIn = "SELECT ID_AUDITORIA,COD_NORMA, TIPO_AUDITORIA, RUT_EMPRESA,FECHA ,OBSERVACION,USER_ID,ESTADO   FROM dev_auditoria da  where COD_NORMA = '" + request.COD_NORMA + "' ";
            //GUARDAMOS EL TRACKING
            //t.guardarTracking("Crear", "Auditoria", t.obtenerDatosNuevosyAntiguos(queryIn));
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
    public object cargarHistorial(inicial data)
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
            string query ="";
            if (data.ID_AUDITORIA == null)
            {
                query=  " SELECT " +
                " CONCAT( CONVERT(NVARCHAR(10), dnw.FECHA, 105), CONVERT(NVARCHAR(8), dnw.FECHA, 108) ) FECHA, dnw.ID_WORKFLOW, dnw.ADJUNTO, " +
                " CASE dnw.TIPO  " +
                " WHEN 'AI' THEN 'Análisis inmediata' " +
                " WHEN 'AC' THEN 'Análisis de causa' " +
                " WHEN 'PA' THEN 'Plan de acción' " +
                " WHEN 'SA' THEN 'Seguimiento' " +
                " END TIPO " +
                " , isnull(dnw.RESPONSABLE, WU.FULLNAME) RESPONSABLE " +
                " ,dnw.OBSERVACION " +
                " FROM dev_nc_workflow dnw " +
                " LEFT JOIN web_users wu  ON WU.USER_ID = dnw.USER_ID " +
                " inner join dev_auditoria da on da.COD_NORMA = @COD_NORMA  and da.ID_AUDITORIA = dnw.ID_AUDITORIA " +
                " WHERE " +
                " dnw.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO  AND dnw.ID_NC=@ID_NC";

                
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", data.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("COD_NORMA", data.COD_NORMA);
                cmd.Parameters.AddWithValue("ID_NC", data.ID_NC);

                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
            else
            {
                query = " SELECT  " +
                        " dv.ID_AUDITORIA " +
                        " FROM " +
                        " dev_normas dn " +
                        " LEFT JOIN dev_auditoria dv ON dn.COD_NORMA = dv.COD_NORMA " +
                        " left join dev_evaluacion de on dv.ID_AUDITORIA = de.ID_AUDITORIA " +
                        " LEFT JOIN dev_no_conformidad dnc    ON dnc.ID_AUDITORIA = dv.ID_AUDITORIA and dnc.ESTADO != 'Aprobado' " +
                        " join dev_punto_normativo dpn on dpn.COD_NORMA = dn.COD_NORMA and de.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO and " +
                        " dnc.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO  and dpn.ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO " +
                        " LEFT JOIN web_users wu  ON wu.USER_ID = dnc.USER_RESP " +
                        " LEFT JOIN web_users wus    ON wus.USER_ID = de.USER_ID " +
                        " LEFT JOIN dev_nc_workflow dnw  ON dnw.ID_AUDITORIA = dv.ID_AUDITORIA   AND dnw.ID_PUNTO_NORMATIVO = dpn.ID_PUNTO_NORMATIVO AND dnw.ID_NC=@ID_NC " +
                        " WHERE dpn.COD_NORMA = @COD_NORMA and dpn.ULTIMA_EVALUACION = 'NO' " +
                        " GROUP BY dv.ID_AUDITORIA ";

                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", data.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("COD_NORMA", data.COD_NORMA);
                cmd.Parameters.AddWithValue("ID_NC", data.ID_NC);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();


                SqlDataReader reader2 = cmd.ExecuteReader();

                string auditoria = "";

                while (reader2.Read())
                {
                    auditoria = reader2["ID_AUDITORIA"].ToString();

                }
                reader2.Close();
                cmd.Parameters.Clear();

                query = " select  CONCAT( CONVERT(NVARCHAR(10), dnw.FECHA, 105), ' ',CONVERT(NVARCHAR(8), dnw.FECHA, 108) ) FECHA ,dnw.ID_WORKFLOW , dnw.ADJUNTO,case dnw.TIPO when 'AI' then 'Análisis inmediata' when 'AC' then 'Análisis de causa' when 'PA' then 'Plan de acción' " +
                                " when 'SA' then 'Seguimiento' end TIPO, isnull(dnw.RESPONSABLE, WU.FULLNAME)RESPONSABLE  , dnw.OBSERVACION from dev_nc_workflow dnw " +
                                " left join web_users wu on WU.USER_ID = dnw.USER_ID  " +
                                " WHERE dnw.ID_PUNTO_NORMATIVO =@ID_PUNTO_NORMATIVO and dnw.ID_NC=@ID_NC";
                //cmd.Parameters.AddWithValue("ID_AUDITORIA", auditoria == "" ? data.ID_AUDITORIA : auditoria);
                cmd.Parameters.AddWithValue("ID_AUDITORIA", data.ID_AUDITORIA );
                cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", data.ID_PUNTO_NORMATIVO);
                cmd.Parameters.AddWithValue("ID_NC", data.ID_NC);

                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }

            SqlDataReader reader = cmd.ExecuteReader();

            List<inicioModal> lista = new List<inicioModal>();

            while (reader.Read())
            {
                inicioModal obj = new inicioModal();
                obj.date = reader["FECHA"].ToString();
                obj.responsable = reader["RESPONSABLE"].ToString();
                obj.observacion = reader["OBSERVACION"].ToString();
                obj.tipo = reader["TIPO"].ToString();
                obj.ruta = WebConfigurationManager.AppSettings["DocumentosUrl"] + "\\" + "no_conformidad" + "\\" + reader["ID_WORKFLOW"].ToString() + "\\" + reader["ADJUNTO"].ToString();
                obj.archivo = reader["ADJUNTO"].ToString();
                obj.date = reader["FECHA"].ToString();

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
    public string mover_documento(string archivo, int codigo_documento)
    {
        try
        {

            string rutaFisicaDocumento = WebConfigurationManager.AppSettings["rutaFisicaDocumento"];
            string tempDocUrl = WebConfigurationManager.AppSettings["DocumentosUrl"];

            string sourceFile = rutaFisicaDocumento + "temporal" + "\\" + archivo;
            string destinationFile = rutaFisicaDocumento + "no_conformidad" + "\\" + codigo_documento + "\\" + archivo;

            string ruta_final = tempDocUrl + "no_conformidad" + "/" + codigo_documento +  "/" + archivo; //ruta final para guardar como resgistro en la tabla dev_archivo_documento

            string directoryName = String.Empty;
            //string directoryName2 = String.Empty;
            var path = HttpRuntime.AppDomainAppPath;
            directoryName = System.IO.Path.Combine(path, "file\\no_conformidad\\" + codigo_documento);
            Directory.CreateDirectory(@directoryName);

            //directoryName2 = System.IO.Path.Combine(path, "file\\no_conformidad\\" + codigo_documento );
            //Directory.CreateDirectory(@directoryName2);

            // To move a file or folder to a new location:

            System.IO.File.Move(@"" + sourceFile, @"" + destinationFile);

            //System.IO.Directory.Move(@"C:\Users\Public\public\test\", @"C:\Users\Public\private");

            return ruta_final;
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public bool eliminar(List<noConformidadRequest>  noConformidadesEliminar)
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
            //TRACKING
            Tracking t = new Tracking();

            string queryIn = "select ID_AUDITORIA , ID_PUNTO_NORMATIVO, NO_CONFORMIDAD," +
                                " USER_RESP,FECHA, ESTADO " +
                               "  from dev_no_conformidad  ";
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);
            bool datosEliminados = true;
            foreach (noConformidadRequest noConformidad in noConformidadesEliminar)
            {
                string query = "";
                query = $@" SELECT  
                    ID_PUNTO_NORMATIVO  
                FROM 
                    dev_nc_workflow  
                WHERE  
                    ID_PUNTO_NORMATIVO = '{noConformidad.ID_PUNTO_NORMATIVO}' 
                    and ID_AUDITORIA = {noConformidad.ID_AUDITORIA} 
                    and ID_NC = {noConformidad.ID_NC} ";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader = cmd.ExecuteReader();

                List<inicioModal> lista = new List<inicioModal>();
                bool datosAsociados = false;
                while (reader.Read())
                {
                    datosAsociados = true;
                }
                reader.Close();
                if (!datosAsociados)
                {
                    //ELIMINAMOS LA NO CONFORMIDAD 
                    cmd.Parameters.Clear();
                    query = $@"DELETE FROM
                                dev_no_conformidad
                            WHERE
                                ID_PUNTO_NORMATIVO = '{noConformidad.ID_PUNTO_NORMATIVO}' 
                                and ID_AUDITORIA = {noConformidad.ID_AUDITORIA} 
                                and ID_NC = {noConformidad.ID_NC} ";

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    //ELIMINAMOS LAS MARCAS QUE LE AGREGO AL PUNTO NORMATIVO
                    cmd.Parameters.Clear();
                    query = " UPDATE dev_punto_normativo SET ULTIMA_EVALUACION = '', EVALUABLE = 'True', OBSERVACION = ''" +
                            "WHERE  ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO  and COD_NORMA = @COD_NORMA";

                    cmd.Parameters.AddWithValue("COD_NORMA", noConformidad.COD_NORMA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", noConformidad.ID_PUNTO_NORMATIVO);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                    //ELIMINAMOS LAS EVALUACIONES 
                    cmd.Parameters.Clear();
                    query = " DELETE " +
                    " FROM dev_evaluacion " +
                    " WHERE " +
                    " ID_PUNTO_NORMATIVO = @ID_PUNTO_NORMATIVO and ID_AUDITORIA = @ID_AUDITORIA ";

                    cmd.Parameters.AddWithValue("ID_AUDITORIA", noConformidad.ID_AUDITORIA);
                    cmd.Parameters.AddWithValue("ID_PUNTO_NORMATIVO", noConformidad.ID_PUNTO_NORMATIVO);

                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();

                }
                else
                {
                    datosEliminados = false;
                }
                cmd.Parameters.Clear();
            }
            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Gestor no conformidad", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
            return datosEliminados;
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