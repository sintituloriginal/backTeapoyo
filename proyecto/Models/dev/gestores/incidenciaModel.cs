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
using proyecto.Models.menu;
using System.Globalization;
using proyecto.objects.dev.mantenedores.Usuario;

public class incidenciaModel : Conexion
{
    public object cargarDatos(string ID_EMPRESA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string user_logged_info = @"
                                        select 
                                        g.NAME as PERFIL
                                        from web_users wu
                                        INNER JOIN WEB_GROUPS g ON g.GROUP_ID = wu.GROUP_ID
                                        where wu.USER_ID = @user_id                                            
                                        ";

            cmd.Parameters.AddWithValue("user_id", user_id);
            cmd.CommandText = user_logged_info;
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            UsuarioFullData user = new UsuarioFullData();
            while (reader.Read())
            {
                
                user.perfil = reader["PERFIL"].ToString();
               
            }
            reader.Close();

            List<incidenciaRequest> lista = new List<incidenciaRequest>();
            incidenciaRequest obj = new incidenciaRequest();


            if (user.perfil == "Analista")
            {
                string incidencias_analista = @"select
                                di.ID_INCIDENCIA,
                                convert(varchar, di.FECHA_INCIDENCIA, 103) as FECHA_INCIDENCIA,
								convert(varchar, di.HORA_INCIDENCIA, 100) as HORA_INCIDENCIA,
                                di.TIPO_EVENTO,
                                di.CRITERIO_AFECTADO,
                                da.NOMBRE as AREA,
                                di.REPORTADO_POR,
                                wu.FULLNAME as RESPONS_ANALISIS,
                                di.ESTADO,
                                di.DESCRIPCION
                            from
                                dev_incidencia di
                                inner join web_empresa we on we.id_empresa = di.id_empresa and we.id_empresa = @idEmpresa
                                inner join dev_area da on da.ID_AREA = di.ID_AREA
                                inner join web_users wu on wu.USER_ID = di.RESPONS_ANALISIS and wu.USER_ID= @user
                            where
                                di.ESTADO = 'En Análisis'
                                or di.ESTADO = 'Ingresado'
                                or di.ESTADO = 'Analizado                                                                                                               '  
                            ";
                cmd.Parameters.AddWithValue("idEmpresa", ID_EMPRESA);
                cmd.Parameters.AddWithValue("user", user_id);
                cmd.CommandText = incidencias_analista;
                cmd.ExecuteNonQuery();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    obj = new incidenciaRequest();
                    obj.ID_INCIDENCIA = reader["ID_INCIDENCIA"].ToString();
                    obj.FECHA_HORA_EVENTO = reader["FECHA_INCIDENCIA"].ToString() + " - " + reader["HORA_INCIDENCIA"].ToString();
                    obj.TIPO_EVENTO = reader["TIPO_EVENTO"].ToString();
                    obj.CRITERIO_AFECTADO = reader["CRITERIO_AFECTADO"].ToString();
                    obj.AREA = reader["AREA"].ToString();
                    obj.REPORTADO_POR = reader["REPORTADO_POR"].ToString();
                    obj.RESPONS_ANALISIS = reader["RESPONS_ANALISIS"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
                    obj.DESCRIPCION = reader["DESCRIPCION"].ToString();
                    obj.analista = true;
                    lista.Add(obj);
                }
                reader.Close();
                return lista;
            }
              else
            {
                string incidencias_user = @"select
                                di.ID_INCIDENCIA,
                                convert(varchar, di.FECHA_INCIDENCIA, 103) as FECHA_INCIDENCIA,
								convert(varchar, di.HORA_INCIDENCIA, 100) as HORA_INCIDENCIA,
                                di.TIPO_EVENTO,
                                di.CRITERIO_AFECTADO,
                                da.NOMBRE as AREA,
                                di.REPORTADO_POR,
                                wu.FULLNAME as RESPONS_ANALISIS,
                                di.ESTADO,
                                di.DESCRIPCION
                               
                                
                            from
                                dev_incidencia di
                                inner join dev_area da on da.ID_AREA = di.ID_AREA
                                inner join web_users wu on wu.USER_ID = di.RESPONS_ANALISIS
                               
                            where
                                di.ID_EMPRESA = @idEmpresa
                                and di.USER_ID = @user
                            ";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@idEmpresa", ID_EMPRESA);
                cmd.Parameters.AddWithValue("@user", user_id);
                cmd.CommandText = incidencias_user;
                cmd.ExecuteNonQuery();
                reader = cmd.ExecuteReader();               

                while (reader.Read())
                {
                    obj = new incidenciaRequest();
                    obj.ID_INCIDENCIA = reader["ID_INCIDENCIA"].ToString();
                    obj.FECHA_HORA_EVENTO = reader["FECHA_INCIDENCIA"].ToString() + " - " + reader["HORA_INCIDENCIA"].ToString();
                    obj.TIPO_EVENTO = reader["TIPO_EVENTO"].ToString();
                    obj.CRITERIO_AFECTADO = reader["CRITERIO_AFECTADO"].ToString();
                    obj.AREA = reader["AREA"].ToString();
                    obj.REPORTADO_POR = reader["REPORTADO_POR"].ToString();
                    obj.RESPONS_ANALISIS = reader["RESPONS_ANALISIS"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
                    obj.DESCRIPCION = reader["DESCRIPCION"].ToString();
                    obj.analista = false;
                    lista.Add(obj);
                }
                reader.Close();
            }

            transaccion.Commit();

            return lista;
            
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

     public object cargarDatosExport(filtrarPor request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            

            string user_logged_info = @"
                                        select 
                                        g.NAME as PERFIL
                                        from web_users wu
                                        INNER JOIN WEB_GROUPS g ON g.GROUP_ID = wu.GROUP_ID
                                        where wu.USER_ID = @user_id                                            
                                        ";

            cmd.Parameters.AddWithValue("user_id", request.USER_ID);
            cmd.CommandText = user_logged_info;
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            UsuarioFullData user = new UsuarioFullData();
            while (reader.Read())
            {
                
                user.perfil = reader["PERFIL"].ToString();
               
            }
            reader.Close();

            List<incidenciaExport> lista = new List<incidenciaExport>();
            incidenciaExport obj = new incidenciaExport();


            if (user.perfil == "Analista")
            {
                string incidencias_analista = @"select
                                di.ID_INCIDENCIA,
                                convert(varchar, di.FECHA_INCIDENCIA, 103) as FECHA_INCIDENCIA,
								convert(varchar, di.HORA_INCIDENCIA, 100) as HORA_INCIDENCIA,
                                di.TIPO_EVENTO,
                                di.CRITERIO_AFECTADO,
                                da.NOMBRE as AREA,
                                di.REPORTADO_POR,
                                wu.FULLNAME as RESPONS_ANALISIS,
                                di.ESTADO,
                                di.DESCRIPCION,
                                dda.OBSERVACION as OBSERVACION_ANALISIS,
                                ddc.OBSERVACION as OBSERVACION_CIERRE,
                                ddc.actualizar_documento,
                                dd.nombre as nombre_documento,
                                ddc.revision_riesgos,
                                ddc.detalle_riesgos
                                
                            from
                                dev_incidencia di
                                inner join web_empresa we on we.ID_EMPRESA = di.ID_EMPRESA and we.ID_EMPRESA = @idEmpresa
                                inner join dev_area da on da.ID_AREA = di.ID_AREA
                                inner join web_users wu on wu.USER_ID = di.RESPONS_ANALISIS
                                left join dev_detalle_analisis dda on dda.ID_INCIDENCIA = di.ID_INCIDENCIA
                                left join dev_detalle_cierre ddc on ddc.ID_INCIDENCIA = di.ID_INCIDENCIA
                                left join dev_documento dd on dd.indice = ddc.id_documento
                            where
                                di.USER_ID = @user
                                and di.ESTADO = 'Cerrado'
                                or di.ESTADO = 'Ingresado'
                                or di.ESTADO = 'Analizado'                                                                                                              '  
                            ";
                cmd.Parameters.AddWithValue("idEmpresa", request.ID_EMPRESA);
                cmd.Parameters.AddWithValue("user", request.USER_ID);
                cmd.CommandText = incidencias_analista;
                cmd.ExecuteNonQuery();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    obj = new incidenciaExport();
                    obj.ID_INCIDENCIA = reader["ID_INCIDENCIA"].ToString();
                    obj.FECHA_HORA_EVENTO = reader["FECHA_INCIDENCIA"].ToString() + " - " + reader["HORA_INCIDENCIA"].ToString();
                    obj.TIPO_EVENTO = reader["TIPO_EVENTO"].ToString();
                    obj.CRITERIO_AFECTADO = reader["CRITERIO_AFECTADO"].ToString();
                    obj.AREA = reader["AREA"].ToString();
                    obj.REPORTADO_POR = reader["REPORTADO_POR"].ToString();
                    obj.RESPONS_ANALISIS = reader["RESPONS_ANALISIS"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
                    obj.DESCRIPCION = reader["DESCRIPCION"].ToString();
                    obj.OBSERVACION_ANALISIS = reader["OBSERVACION_ANALISIS"].ToString();
                    obj.OBSERVACION_CIERRE = reader["OBSERVACION_CIERRE"].ToString();
                    obj.ACTUALIZAR_DOCUMENTO = reader["ACTUALIZAR_DOCUMENTO"].ToString();
                    obj.NOMBRE_DOCUMENTO = reader["NOMBRE_DOCUMENTO"].ToString();
                    obj.REVISION_RIESGOS = reader["REVISION_RIESGOS"].ToString();
                    obj.DETALLE_RIESGOS = reader["DETALLE_RIESGOS"].ToString();
                    
                    
                    lista.Add(obj);
                }
                reader.Close();
                return lista;
            }
              else
            {
                string incidencias_user = @"select
                                di.ID_INCIDENCIA,
                                convert(varchar, di.FECHA_INCIDENCIA, 103) as FECHA_INCIDENCIA,
								convert(varchar, di.HORA_INCIDENCIA, 100) as HORA_INCIDENCIA,
                                di.TIPO_EVENTO,
                                di.CRITERIO_AFECTADO,
                                da.NOMBRE as AREA,
                                di.REPORTADO_POR,
                                wu.FULLNAME as RESPONS_ANALISIS,
                                di.ESTADO,
                                di.DESCRIPCION,
                                dda.OBSERVACION as OBSERVACION_ANALISIS,
                                ddc.OBSERVACION as OBSERVACION_CIERRE,
                                ddc.actualizar_documento,
                                dd.nombre as nombre_documento,
                                ddc.revision_riesgos,
                                ddc.detalle_riesgos
                                
                            from
                                 dev_incidencia di
                                inner join web_empresa we on we.ID_EMPRESA = di.ID_EMPRESA and we.ID_EMPRESA = @idEmpresa
                                inner join dev_area da on da.ID_AREA = di.ID_AREA
                                inner join web_users wu on wu.USER_ID = di.RESPONS_ANALISIS
                                left join dev_detalle_analisis dda on dda.ID_INCIDENCIA = di.ID_INCIDENCIA
                                left join dev_detalle_cierre ddc on ddc.ID_INCIDENCIA = di.ID_INCIDENCIA
                                left join dev_documento dd on dd.indice = ddc.id_documento
                            where
                                di.USER_ID = @user
                                and di.ESTADO = 'Cerrado'
                                or di.ESTADO = 'Ingresado'
                                or di.ESTADO = 'Analizado'     
                            ";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@idEmpresa", request.ID_EMPRESA);
                cmd.Parameters.AddWithValue("@user", request.USER_ID);
                cmd.CommandText = incidencias_user;
                cmd.ExecuteNonQuery();
                reader = cmd.ExecuteReader();               

                while (reader.Read())
                {
                    obj = new incidenciaExport();
                    obj.ID_INCIDENCIA = reader["ID_INCIDENCIA"].ToString();
                    obj.FECHA_HORA_EVENTO = reader["FECHA_INCIDENCIA"].ToString() + " - " + reader["HORA_INCIDENCIA"].ToString();
                    obj.TIPO_EVENTO = reader["TIPO_EVENTO"].ToString();
                    obj.CRITERIO_AFECTADO = reader["CRITERIO_AFECTADO"].ToString();
                    obj.AREA = reader["AREA"].ToString();
                    obj.REPORTADO_POR = reader["REPORTADO_POR"].ToString();
                    obj.RESPONS_ANALISIS = reader["RESPONS_ANALISIS"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
                    obj.DESCRIPCION = reader["DESCRIPCION"].ToString();
                    obj.OBSERVACION_ANALISIS = reader["OBSERVACION_ANALISIS"].ToString();
                    obj.OBSERVACION_CIERRE = reader["OBSERVACION_CIERRE"].ToString();
                    obj.ACTUALIZAR_DOCUMENTO = reader["ACTUALIZAR_DOCUMENTO"].ToString();
                    obj.NOMBRE_DOCUMENTO = reader["NOMBRE_DOCUMENTO"].ToString();
                    obj.REVISION_RIESGOS = reader["REVISION_RIESGOS"].ToString();
                    obj.DETALLE_RIESGOS = reader["DETALLE_RIESGOS"].ToString();
                    lista.Add(obj);
                }
                reader.Close();
            }

            transaccion.Commit();

            return lista;
            
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

    public object filtrarDatos(filtrarPor datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        try
        {
            string query = "";
            if (datos.FECHA_INICIO != null)
            {
                query += "declare @fechaInicio datetime ";
                query += "set @fechaInicio = convert(datetime, '" + datos.FECHA_INICIO + "', 103)";
            }
            if (datos.FECHA_FIN != null)
            {
                query += "declare @fechaFin datetime ";
                query += "set @fechaFin = convert(datetime, '" + datos.FECHA_FIN + "', 103)";
            }
            query += @" select
                                di.ID_INCIDENCIA,
                                convert(varchar, di.FECHA_INCIDENCIA, 103) as FECHA_INCIDENCIA,
								convert(varchar, di.HORA_INCIDENCIA, 8) as HORA_INCIDENCIA,
                                di.TIPO_EVENTO,
                                di.CRITERIO_AFECTADO,
                                da.NOMBRE as AREA,
                                di.REPORTADO_POR,
                                wu.FULLNAME as RESPONS_ANALISIS,
                                di.ESTADO,
                                di.DESCRIPCION
                            from
                                dev_incidencia di
                                inner join dev_area da on da.ID_AREA = di.ID_AREA
                                inner join web_users wu on wu.USER_ID = di.RESPONS_ANALISIS
                            where
                                di.ID_EMPRESA = @idEmpresa
                            ";

            if (datos.ESTADO != null)
            {
                query += " and  UPPER(di.ESTADO) in (" + datos.ESTADO + ")";
            }
            if (datos.CRITERIO_AFECTADO != null)
            {
                query += " and  UPPER(di.CRITERIO_AFECTADO) in (" + datos.CRITERIO_AFECTADO + ")";
            }
            if (datos.TIPO_EVENTO != null)
            {
                query += " and  UPPER(di.TIPO_EVENTO) in (" + datos.TIPO_EVENTO + ")";
            }
            if (datos.AREA != null)
            {
                query += " and  di.ID_AREA in (" + datos.AREA + ")";
            }
            if (datos.RESPONS_ANALISIS != null)
            {
                query += " and  di.RESPONS_ANALISIS in (" + datos.RESPONS_ANALISIS + ")";
            }
            if (datos.FECHA_INICIO != null)
            {
                //convert(datetime,di.FECHA_INCIDENCIA , 103)
                query += " and convert(datetime,di.FECHA_INCIDENCIA , 103) >= @fechaInicio";
            }
            if (datos.FECHA_FIN != null)
            {
                query += " and convert(datetime,di.FECHA_INCIDENCIA , 103) <= @fechafin";
            }


            cmd.Parameters.AddWithValue("@idEmpresa", datos.ID_EMPRESA);
            cmd.CommandText = query;
            cmd.Connection = conn;
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<incidenciaRequest> lista = new List<incidenciaRequest>();

            while (reader.Read())
            {
                incidenciaRequest obj = new incidenciaRequest();
                obj.ID_INCIDENCIA = reader["ID_INCIDENCIA"].ToString();
                obj.FECHA_HORA_EVENTO = reader["FECHA_INCIDENCIA"].ToString() + " - " + reader["HORA_INCIDENCIA"].ToString().Insert(5, " ");
                obj.TIPO_EVENTO = reader["TIPO_EVENTO"].ToString();
                obj.CRITERIO_AFECTADO = reader["CRITERIO_AFECTADO"].ToString();
                obj.AREA = reader["AREA"].ToString();
                obj.REPORTADO_POR = reader["REPORTADO_POR"].ToString();
                obj.RESPONS_ANALISIS = reader["RESPONS_ANALISIS"].ToString();
                obj.ESTADO = reader["ESTADO"].ToString();
                obj.DESCRIPCION = reader["DESCRIPCION"].ToString();
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
    public object getAreas(string ID_EMPRESA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = " select nombre, id_area from dev_area da " +
                     " where estado = 'Habilitado' " +
                     " and id_empresa = " + ID_EMPRESA + " order by nombre asc";




            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            List<filtro_area> lista = new List<filtro_area>();

            while (reader.Read())
            {
                filtro_area obj = new filtro_area();
                obj.id = reader["id_area"].ToString();
                obj.text = reader["nombre"].ToString();
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
    public object getAnalistas(string ID_EMPRESA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = " select wu.USER_ID as id , wu.FULLNAME  as text from web_users wu " +
                        " inner join web_groups wg on wg.GROUP_ID = wu.GROUP_ID and  wg.GROUP_ID = 413 " +
                        " inner join web_empresa_user weu on weu.user_id = wu.USER_ID and weu.id_empresa =  " + ID_EMPRESA;



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
    public object crearIncidencia(crearIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"
                                            INSERT INTO dev_incidencia 
                                             (FECHA_INCIDENCIA
                                            ,HORA_INCIDENCIA
                                            ,TIPO_EVENTO
                                            ,CRITERIO_AFECTADO
                                            ,ID_AREA
                                            ,REPORTADO_POR
                                            ,RESPONS_ANALISIS
                                            ,ESTADO
                                            ,FECHA_CREATED
                                            ,FECHA_UPDATED
                                            ,DESCRIPCION
                                            ,ID_EMPRESA
                                            ,USER_ID
                                            )values
                                            
                                            (
                                            @fechaIncidencia
                                            ,@horaIncidencia
                                            ,@tipoEvento
                                            ,@criterioAfectado
                                            ,@idArea
                                            ,@reportadoPor
                                            ,@responsAnalisis
                                            ,@estado
                                            ,GETUTCDATE()
                                            ,GETUTCDATE()
                                            ,@descripcion
                                            ,@idEmpresa
                                            ,@user_id
                                            )
                                             SELECT SCOPE_IDENTITY()
                                             ";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("fechaIncidencia", request.fecha_incidencia!=null? request.fecha_incidencia : "" );
            cmd.Parameters.AddWithValue("horaIncidencia", request.hora_incidencia!=null? request.hora_incidencia : "" );
            cmd.Parameters.AddWithValue("tipoEvento", request.tipo_evento!=null? request.tipo_evento : "");
            cmd.Parameters.AddWithValue("criterioAfectado", request.criterio_afectado!=null? request.criterio_afectado : "");
            cmd.Parameters.AddWithValue("idArea", request.id_area!=null? request.id_area.ToString() : "001");
            cmd.Parameters.AddWithValue("reportadoPor", request.reportado_por!=null? request.reportado_por : "");
            cmd.Parameters.AddWithValue("responsAnalisis", request.id_analista!=null? request.id_analista : user_id);
            cmd.Parameters.AddWithValue("estado", "Guardado");
            
            cmd.Parameters.AddWithValue("descripcion", request.descripcion!=null? request.descripcion : "");
            cmd.Parameters.AddWithValue("idEmpresa", request.id_empresa.ToString());
            cmd.Parameters.AddWithValue("user_id", user_id);

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            string id = cmd.ExecuteScalar().ToString();

            JavaScriptSerializer js = new JavaScriptSerializer();
            object[] archivos_Obj = (object[])js.Deserialize(request.nombre_documento, new object().GetType());

            foreach (var archivo in archivos_Obj)
            {
                string ruta = mover_documento( archivo.ToString(), request.fecha_incidencia, request.id_area.ToString(), request.id_empresa.ToString());

                string REGISTRAR_DOCUMENTO = @"INSERT INTO dev_archivo_incidencia
                                                    (
                                                    nombre
                                                    ,ruta
                                                    ,id_incidencia
                                                    )
                                                VALUES
                                                    (
                                                    @nombre,
                                                    @ruta,
                                                    @id_incidencia
                                                    )";
                cmd.CommandText = REGISTRAR_DOCUMENTO;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("nombre", archivo.ToString());
                cmd.Parameters.AddWithValue("ruta", ruta);
                cmd.Parameters.AddWithValue("id_incidencia", Int32.Parse(id));

                cmd.CommandText = REGISTRAR_DOCUMENTO;
                cmd.ExecuteNonQuery();
                // validate REGISTRAR_INCIDENCIA and REGISTRAR_DOCUMENTO insert correctly an commit transaction



            }

            transaccion.Commit();

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

    public string mover_documento(string archivo, string fecha_registro, string area, string empresa)
    {
        try
        {

            string rutaFisicaDocumento = WebConfigurationManager.AppSettings["rutaFisicaDocumento"];
            string tempDocUrl = WebConfigurationManager.AppSettings["DocumentosUrl"];

            string sourceFile = rutaFisicaDocumento + "temporal" + "\\" + archivo;
            string destinationFile = rutaFisicaDocumento + "incidencias" + "\\" + empresa + "\\" + area + "\\" + fecha_registro + "\\" + archivo;
            bool documento_existe = System.IO.File.Exists(destinationFile);
            if (documento_existe)
            {
                File.Delete(destinationFile);
            }
            string ruta_final = tempDocUrl + "incidencias" + "/" + empresa + "/" + area + "/" + fecha_registro + "/" + archivo; //ruta final para guardar como resgistro en la tabla dev_archivo_incidencias

            string directoryName = String.Empty;
            string directoryName2 = String.Empty;

            var path = HttpRuntime.AppDomainAppPath;


            directoryName = System.IO.Path.Combine(path, "file\\incidencias\\" + empresa + "\\" + area);
            bool a = System.IO.Directory.Exists(directoryName);
            //bool a = Directory.Exists(directoryName);
            if (a == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            directoryName2 = System.IO.Path.Combine(path, "file\\incidencias\\" + empresa + "\\" + area + "\\" + fecha_registro);
            bool b = System.IO.Directory.Exists(directoryName2);
            if (b == false)
            {
                Directory.CreateDirectory(directoryName2);
            }
            else
            {
                directoryName2 = System.IO.Path.Combine(path, "file\\incidencias\\" + empresa + "\\" + area + "\\" + fecha_registro);
            }




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
    public object getDataIncidencia(string ID_INCIDENCIA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = @"select
                                di.ID_INCIDENCIA,
                                convert(varchar, di.FECHA_INCIDENCIA, 103) as FECHA_INCIDENCIA,
								convert(varchar, di.HORA_INCIDENCIA, 100) as HORA_INCIDENCIA,
                                di.TIPO_EVENTO,
                                di.CRITERIO_AFECTADO,
                                da.NOMBRE as AREA,
                                di.REPORTADO_POR,
                                wu.FULLNAME as RESPONS_ANALISIS,
                                di.ESTADO,
                                di.DESCRIPCION
                            from
                                dev_incidencia di
                                inner join dev_area da on da.ID_AREA = di.ID_AREA
                                inner join web_users wu on wu.USER_ID = di.RESPONS_ANALISIS
                            where
                                di.id_incidencia = @id_incidencia";




            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", ID_INCIDENCIA);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            crearIncidenciaReq obj = new crearIncidenciaReq();

            while (reader.Read())
            {
                
                obj.id = Int32.Parse(reader["ID_INCIDENCIA"].ToString());
                obj.fecha_incidencia = reader["FECHA_INCIDENCIA"].ToString();
                obj.hora_incidencia = reader["HORA_INCIDENCIA"].ToString();
                obj.reportado_por = reader["REPORTADO_POR"].ToString();
                obj.responsable_analisis = reader["RESPONS_ANALISIS"].ToString();
                obj.id_area = reader["AREA"].ToString();
                obj.descripcion = reader["DESCRIPCION"].ToString();
                obj.estado = reader["ESTADO"].ToString();
                obj.tipo_evento = reader["tipo_evento"].ToString();
                obj.criterio_afectado = reader["criterio_afectado"].ToString();

            }
            reader.Close();
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

    public object getDocumentos_reg(string ID_INCIDENCIA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = @"SELECT id_archivo, nombre, ruta FROM dev_archivo_incidencia where id_incidencia = @id_incidencia";




            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", ID_INCIDENCIA);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            List<archivoIncidencia> lista = new List<archivoIncidencia>();

            while (reader.Read())
            {
                archivoIncidencia obj = new archivoIncidencia();
                obj.id_archivo = reader["id_archivo"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.ruta = reader["ruta"].ToString(); ;
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
    
    public object getDocumentos_analisis(string ID_INCIDENCIA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = @"SELECT id_archivo, nombre, ruta FROM dev_archivo_analisis daa
                            inner join dev_detalle_analisis dda on dda.id_analisis= daa.id_analisis 
                            and  
                            id_incidencia = @id_incidencia";




            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia",ID_INCIDENCIA);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            List<archivoIncidencia> lista = new List<archivoIncidencia>();

            while (reader.Read())
            {
                archivoIncidencia obj = new archivoIncidencia();
                obj.id_archivo = reader["id_archivo"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.ruta = reader["ruta"].ToString(); ;
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

    public object getDocumentos_cierre(string ID_INCIDENCIA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = @"SELECT id_archivo, nombre, ruta FROM dev_archivo_cierre daa
                            inner join dev_detalle_cierre dda on dda.id_cierre= daa.id_cierre 
                            and  
                            id_incidencia = @id_incidencia";




            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", ID_INCIDENCIA);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            List<archivoIncidencia> lista = new List<archivoIncidencia>();

            while (reader.Read())
            {
                archivoIncidencia obj = new archivoIncidencia();
                obj.id_archivo = reader["id_archivo"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.ruta = reader["ruta"].ToString(); ;
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

    public object actualizarIncidencia(crearIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                            FECHA_UPDATED = GETUTCDATE(),
                                            FECHA_INCIDENCIA=@FECHA_INCIDENCIA,
                                            HORA_INCIDENCIA=@HORA_INCIDENCIA,
                                            DESCRIPCION = @DESCRIPCION,
                                            RESPONS_ANALISIS = @RESPONS_ANALISIS,
                                            TIPO_EVENTO = @TIPO_EVENTO,
                                            CRITERIO_AFECTADO = @CRITERIO_AFECTADO,
                                            ID_AREA = @ID_AREA,
                                            REPORTADO_POR = @REPORTADO_POR
                                            WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id );
            cmd.Parameters.AddWithValue("DESCRIPCION", request.descripcion ?? "");
            cmd.Parameters.AddWithValue("RESPONS_ANALISIS", request.responsable_analisis ?? user_id);
            cmd.Parameters.AddWithValue("TIPO_EVENTO", request.tipo_evento ?? "");
            cmd.Parameters.AddWithValue("CRITERIO_AFECTADO", request.criterio_afectado ?? "");
            cmd.Parameters.AddWithValue("ID_AREA", request.id_area ?? "001");
            cmd.Parameters.AddWithValue("REPORTADO_POR", request.reportado_por ?? "");
            cmd.Parameters.AddWithValue("FECHA_INCIDENCIA", request.fecha_incidencia ?? "");
            cmd.Parameters.AddWithValue("HORA_INCIDENCIA", request.hora_incidencia ?? "");
            

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            cmd.ExecuteNonQuery();

            JavaScriptSerializer js = new JavaScriptSerializer();

            string GET_INCIDENCIA = @"SELECT fecha_incidencia,id_area,id_empresa  from dev_incidencia 
                                      where id_incidencia=@id_incidencia
                                      ";
            cmd.CommandText = GET_INCIDENCIA;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", request.id);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();
            crearIncidenciaReq obj = new crearIncidenciaReq();

            while (reader.Read())
            {
                
                obj.fecha_incidencia = reader["fecha_incidencia"].ToString();
                obj.id_area = reader["id_area"].ToString();
                obj.id_empresa = reader["id_empresa"].ToString(); ;

            }
            reader.Close();


            bool existen_docs = request.nombre_documento != null && request.nombre_documento != "";
            if (existen_docs)
            {
                object[] archivos_Obj = (object[])js.Deserialize(request.nombre_documento, new object().GetType());

                foreach (var archivo in archivos_Obj)
                {
                    string ruta = mover_documento(archivo.ToString(), obj.fecha_incidencia, obj.id_area.ToString(), obj.id_empresa.ToString());

                    string REGISTRAR_DOCUMENTO = @"INSERT INTO dev_archivo_incidencia
                                                    (
                                                    nombre
                                                    ,ruta
                                                    ,id_incidencia
                                                    )
                                                VALUES
                                                    (
                                                    @nombre,
                                                    @ruta,
                                                    @id_incidencia
                                                    )";
                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("nombre", archivo.ToString());
                    cmd.Parameters.AddWithValue("ruta", ruta);
                    cmd.Parameters.AddWithValue("id_incidencia", request.id);

                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.ExecuteNonQuery();
                    // validate REGISTRAR_INCIDENCIA and REGISTRAR_DOCUMENTO insert correctly an commit transaction



                }

            }

            transaccion.Commit();

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

    public object publicarIncidencia(crearIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,estado = 'Ingresado'
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id);

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            cmd.ExecuteNonQuery();

            

            transaccion.Commit();

             String queryEmail = $@"select Email from web_users where USER_ID = '{request.responsable_analisis}'";

            cmd.CommandText = queryEmail;
            SqlDataReader reader = cmd.ExecuteReader();

            string email = "";

            while (reader.Read())
            {
                email = reader["Email"].ToString();
                
            }

            SqlCommand command = new SqlCommand("SP_SGI_ALERTA_NUEVA_INCIDENCIA", conn);
            command.CommandType = CommandType.StoredProcedure;

            string url = WebConfigurationManager.AppSettings["clientURL"] + "incidencia";

            command.Parameters.AddWithValue("registro_incidencia", request.id);
            command.Parameters.AddWithValue("hora_incidencia", request.hora_incidencia);
            command.Parameters.AddWithValue("emails", email);
            command.Parameters.AddWithValue("URL", url);
            command.Parameters.AddWithValue("NOMBRE", request.id_empresa);
            command.ExecuteNonQuery();

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

    //análisis
    public object analizarIncidencia(crearIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"INSERT INTO dev_detalle_analisis 
             (
            id_incidencia
            ,observacion
            ,FECHA_CREATED
            ,FECHA_UPDATED
            )values
            
            (
            @id_incidencia
            ,@observacion
            ,GETUTCDATE()
            ,GETUTCDATE()
            )
             SELECT SCOPE_IDENTITY()
           ";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", request.id);
            cmd.Parameters.AddWithValue("observacion", request.descripcion?? "");

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            string id = cmd.ExecuteScalar().ToString();

            JavaScriptSerializer js = new JavaScriptSerializer();
            string GET_INCIDENCIA = @"SELECT fecha_incidencia,id_area,id_empresa  from dev_incidencia 
                                      where id_incidencia=@id_incidencia
                                      ";
            cmd.CommandText = GET_INCIDENCIA;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", request.id);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();
            crearIncidenciaReq obj = new crearIncidenciaReq();

            while (reader.Read())
            {

                obj.fecha_incidencia = reader["fecha_incidencia"].ToString();
                obj.id_area = reader["id_area"].ToString();
                obj.id_empresa = reader["id_empresa"].ToString(); ;

            }
            reader.Close();


            bool existen_docs = request.nombre_documento != null && request.nombre_documento != "";
            if (existen_docs)
            {
                object[] archivos_Obj = (object[])js.Deserialize(request.nombre_documento, new object().GetType());

                foreach (var archivo in archivos_Obj)
                {
                    string ruta = mover_documento(archivo.ToString(), obj.fecha_incidencia, obj.id_area.ToString(), obj.id_empresa.ToString());

                    string REGISTRAR_DOCUMENTO = @"INSERT INTO dev_archivo_analisis
                                                    (
                                                    nombre
                                                    ,ruta
                                                    ,id_analisis
                                                    )
                                                VALUES
                                                    (
                                                    @nombre,
                                                    @ruta,
                                                    @id_analisis
                                                    )";
                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("nombre", archivo.ToString());
                    cmd.Parameters.AddWithValue("ruta", ruta);
                    cmd.Parameters.AddWithValue("id_analisis", Int32.Parse(id));

                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.ExecuteNonQuery();
                    // validate REGISTRAR_INCIDENCIA and REGISTRAR_DOCUMENTO insert correctly an commit transaction



                }

            }
            //actualizando estado
            string UPDATE_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,tipo_evento = @tipo_evento
                                              ,criterio_afectado = @criterio_afectado
                                              ,estado = 'En Análisis'
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id);
            cmd.Parameters.AddWithValue("tipo_evento", request.tipo_evento);
            cmd.Parameters.AddWithValue("criterio_afectado", request.criterio_afectado);

            cmd.CommandText = UPDATE_INCIDENCIA;
            cmd.ExecuteNonQuery();

            transaccion.Commit();

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

    
    public object getDataAnalisis(string ID_INCIDENCIA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = @"select
                               observacion
                               ,fecha_created
                               ,fecha_updated
                            from
                                dev_detalle_analisis
                            where
                                id_incidencia = @id_incidencia";




            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", ID_INCIDENCIA);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            crearIncidenciaReq obj = new crearIncidenciaReq();

            while (reader.Read())
            {

                obj.descripcion = reader["observacion"].ToString();
                obj.fecha_created = reader["fecha_created"].ToString();
                obj.fecha_updated = reader["fecha_updated"].ToString();

            }
            reader.Close();
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
    public object actualizarAnalisis(crearIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"UPDATE dev_detalle_analisis
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,observacion = @descripcion
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id);
            cmd.Parameters.AddWithValue("descripcion", request.descripcion?? "");

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            cmd.ExecuteNonQuery();

            JavaScriptSerializer js = new JavaScriptSerializer();

            string GET_INCIDENCIA = @"SELECT da.fecha_incidencia,da.id_area,da.id_empresa, dda.id_analisis  from dev_incidencia da 
                                      inner join dev_detalle_analisis dda on dda.id_incidencia = da.id_incidencia
                                      where da.id_incidencia=@id_incidencia
                                      ";
            cmd.CommandText = GET_INCIDENCIA;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", request.id);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();
            crearIncidenciaReq obj = new crearIncidenciaReq();

            while (reader.Read())
            {
                obj.id = Int32.Parse(reader["id_analisis"].ToString());
                obj.fecha_incidencia = reader["fecha_incidencia"].ToString();
                obj.id_area = reader["id_area"].ToString();
                obj.id_empresa = reader["id_empresa"].ToString(); ;

            }
            reader.Close();


            bool existen_docs = request.nombre_documento != null && request.nombre_documento != "";
            if (existen_docs)
            {
                object[] archivos_Obj = (object[])js.Deserialize(request.nombre_documento, new object().GetType());

                foreach (var archivo in archivos_Obj)
                {
                    string ruta = mover_documento(archivo.ToString(), obj.fecha_incidencia, obj.id_area.ToString(), obj.id_empresa.ToString());

                    string REGISTRAR_DOCUMENTO = @"INSERT INTO dev_archivo_analisis
                                                    (
                                                    nombre
                                                    ,ruta
                                                    ,id_analisis
                                                    )
                                                VALUES
                                                    (
                                                    @nombre,
                                                    @ruta,
                                                    @id_analisis
                                                    )";
                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("nombre", archivo.ToString());
                    cmd.Parameters.AddWithValue("ruta", ruta);
                    cmd.Parameters.AddWithValue("id_analisis", obj.id);

                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.ExecuteNonQuery();
                    // validate REGISTRAR_INCIDENCIA and REGISTRAR_DOCUMENTO insert correctly an commit transaction



                }

            }
            string UPDATE_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,tipo_evento = @tipo_evento
                                              ,criterio_afectado = @criterio_afectado
                                              ,estado = 'En Análisis'
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id);
            cmd.Parameters.AddWithValue("tipo_evento", request.tipo_evento);
            cmd.Parameters.AddWithValue("criterio_afectado", request.criterio_afectado);

            cmd.CommandText = UPDATE_INCIDENCIA;
            cmd.ExecuteNonQuery();

            transaccion.Commit();

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
    public object publicarAnalisis(crearIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,estado = 'Analizado'
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id);

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            cmd.ExecuteNonQuery();



            transaccion.Commit();

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

    //cierre
    public object cerrarIncidencia(cerrarIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"INSERT INTO dev_detalle_cierre 
             (
            id_incidencia
            ,observacion
            ,FECHA_CREATED
            ,FECHA_UPDATED
            ,actualizar_documento
            ,id_documento
            ,revision_riesgos
            ,detalle_riesgos
            )values
            
            (
            @id_incidencia
            ,@observacion
            ,GETUTCDATE()
            ,GETUTCDATE()
            ,@actualizar_documento
            ,@id_documento
            ,@revision_riesgos
            ,@detalle_riesgos
            )
             SELECT SCOPE_IDENTITY()
           ";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", request.id_incidencia);
            cmd.Parameters.AddWithValue("observacion", request.observacion?? "");
            cmd.Parameters.AddWithValue("actualizar_documento", request.actualizar_documento ?? "");
            cmd.Parameters.AddWithValue("id_documento", request.id_documento ?? "0");
            cmd.Parameters.AddWithValue("revision_riesgos", request.revision_riesgos ?? "");
            cmd.Parameters.AddWithValue("detalle_riesgos", request.detalle_riesgos ?? "n/a" );

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            string id = cmd.ExecuteScalar().ToString();

            JavaScriptSerializer js = new JavaScriptSerializer();
            string GET_INCIDENCIA = @"SELECT fecha_incidencia,id_area,id_empresa  from dev_incidencia 
                
                                      where id_incidencia=@id_incidencia
                                      ";
            cmd.CommandText = GET_INCIDENCIA;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", request.id_incidencia);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();
            crearIncidenciaReq obj = new crearIncidenciaReq();

            while (reader.Read())
            {

                obj.fecha_incidencia = reader["fecha_incidencia"].ToString();
                obj.id_area = reader["id_area"].ToString();
                obj.id_empresa = reader["id_empresa"].ToString(); ;

            }
            reader.Close();


            bool existen_docs = request.nombre_documento != null && request.nombre_documento != "";
            if (existen_docs)
            {
                object[] archivos_Obj = (object[])js.Deserialize(request.nombre_documento, new object().GetType());

                foreach (var archivo in archivos_Obj)
                {
                    string ruta = mover_documento(archivo.ToString(), obj.fecha_incidencia, obj.id_area.ToString(), obj.id_empresa.ToString());

                    string REGISTRAR_DOCUMENTO = @"INSERT INTO dev_archivo_cierre
                                                    (
                                                    nombre
                                                    ,ruta
                                                    ,id_cierre
                                                    )
                                                VALUES
                                                    (
                                                    @nombre,
                                                    @ruta,
                                                    @id_cierre
                                                    )";
                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("nombre", archivo.ToString());
                    cmd.Parameters.AddWithValue("ruta", ruta);
                    cmd.Parameters.AddWithValue("id_cierre", Int32.Parse(id));

                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.ExecuteNonQuery();
                    // validate REGISTRAR_INCIDENCIA and REGISTRAR_DOCUMENTO insert correctly an commit transaction



                }

            }
            string UPDATE_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,estado = 'Cerrando'
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id_incidencia);

            cmd.CommandText = UPDATE_INCIDENCIA;
            cmd.ExecuteNonQuery();

            transaccion.Commit();

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
    public object actualizarCierre(cerrarIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"UPDATE dev_detalle_cierre
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,observacion = @observacion
                                                ,actualizar_documento = @actualizar_documento
                                                ,id_documento = @id_documento
                                                ,revision_riesgos = @revision_riesgos
                                                ,detalle_riesgos = @detalle_riesgos

                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id_incidencia);
            cmd.Parameters.AddWithValue("observacion", request.observacion ?? "");
            cmd.Parameters.AddWithValue("actualizar_documento", request.actualizar_documento ?? "");
            cmd.Parameters.AddWithValue("id_documento", request.id_documento ?? "0");
            cmd.Parameters.AddWithValue("revision_riesgos", request.revision_riesgos ?? "");
            cmd.Parameters.AddWithValue("detalle_riesgos", request.detalle_riesgos ?? "n/a");

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            cmd.ExecuteNonQuery();

            JavaScriptSerializer js = new JavaScriptSerializer();

            string GET_INCIDENCIA = @"SELECT da.fecha_incidencia,da.id_area,da.id_empresa, dda.id_cierre  from dev_incidencia da 
                                      inner join dev_detalle_cierre dda on dda.id_incidencia = da.id_incidencia
                                      where da.id_incidencia=@id_incidencia
                                      ";
            cmd.CommandText = GET_INCIDENCIA;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", request.id_incidencia);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();
            crearIncidenciaReq obj = new crearIncidenciaReq();

            while (reader.Read())
            {
                obj.id = Int32.Parse(reader["id_cierre"].ToString());
                obj.fecha_incidencia = reader["fecha_incidencia"].ToString();
                obj.id_area = reader["id_area"].ToString();
                obj.id_empresa = reader["id_empresa"].ToString(); ;

            }
            reader.Close();


            bool existen_docs = request.nombre_documento != null && request.nombre_documento != "";
            if (existen_docs)
            {
                object[] archivos_Obj = (object[])js.Deserialize(request.nombre_documento, new object().GetType());

                foreach (var archivo in archivos_Obj)
                {
                    string ruta = mover_documento(archivo.ToString(), obj.fecha_incidencia, obj.id_area.ToString(), obj.id_empresa.ToString());

                    string REGISTRAR_DOCUMENTO = @"INSERT INTO dev_archivo_cierre
                                                    (
                                                    nombre
                                                    ,ruta
                                                    ,id_cierre
                                                    )
                                                VALUES
                                                    (
                                                    @nombre,
                                                    @ruta,
                                                    @id_cierre
                                                    )";
                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("nombre", archivo.ToString());
                    cmd.Parameters.AddWithValue("ruta", ruta);
                    cmd.Parameters.AddWithValue("id_cierre", obj.id);

                    cmd.CommandText = REGISTRAR_DOCUMENTO;
                    cmd.ExecuteNonQuery();
                    // validate REGISTRAR_INCIDENCIA and REGISTRAR_DOCUMENTO insert correctly an commit transaction



                }

            }
            string UPDATE_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,estado = 'Cerrando'
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id_incidencia);

            cmd.CommandText = UPDATE_INCIDENCIA;
            cmd.ExecuteNonQuery();

            transaccion.Commit();

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

    public object publicarCierre(cerrarIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                            .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,estado = 'Cerrado'
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id_incidencia);

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            cmd.ExecuteNonQuery();



            transaccion.Commit();

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
    public object rechazarAnalisis(cerrarIncidenciaReq request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault();

            string REGISTRAR_INCIDENCIA = @"UPDATE dev_incidencia
                                            SET 
                                              FECHA_UPDATED = GETUTCDATE()
                                              ,estado = 'En Análisis'
                                              WHERE id_incidencia = @id_incidencia
                                           ";
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("id_incidencia", request.id_incidencia);

            cmd.CommandText = REGISTRAR_INCIDENCIA;
            cmd.ExecuteNonQuery();



            transaccion.Commit();

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
    public object getDataCierre(string ID_INCIDENCIA)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = @"select
                               observacion
                               ,fecha_created
                               ,fecha_updated
,actualizar_documento
,id_documento
,revision_riesgos
,detalle_riesgos
                            from
                                dev_detalle_cierre
                            where
                                id_incidencia = @id_incidencia";




            cmd.CommandText = query;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("id_incidencia", ID_INCIDENCIA);
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            cerrarIncidenciaReq obj = new cerrarIncidenciaReq();

            while (reader.Read())
            {

                obj.observacion = reader["observacion"].ToString();
                obj.actualizar_documento = reader["actualizar_documento"].ToString();
                obj.id_documento = reader["id_documento"].ToString();
                obj.revision_riesgos = reader["revision_riesgos"].ToString();
                obj.detalle_riesgos = reader["detalle_riesgos"].ToString();
                obj.fecha_created = reader["fecha_created"].ToString();
                obj.fecha_updated = reader["fecha_updated"].ToString();

            }
            reader.Close();
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
}