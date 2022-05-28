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


public class documentoModel : Conexion
{
    public object cargarDatos(string ID_EMPRESA, string CODIGO)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        try
        {
            string query = @"select 
	                            distinct dd.indice,
	                            dd.codigo_documento,
	                            dd.nombre,
	                            dd.version,
	                            dtd.tipo,
	                            CONVERT(nvarchar(10), dd.ult_revision,105) as ult_revision,  
	                            CONVERT(nvarchar(10), dd.caducidad,105) as caducidad,    
	                            wu.FULLNAME,   
	                            dd.imprimir, 
	                            dd.descargar, 
	                            dad.estado , 
	                            dad.ruta, 
	                            we.ID_EMPRESA AS ID_EMPRESA, 
	                            we.NOMBRE as NOMBRE_EMPRESA     
	                        from dev_documento dd    
	                        inner join web_users wu 
	                        on wu.USER_ID = dd.user_id     
	                        inner join dev_archivo_documento dad 
	                        on dad.cod_documento = dd.codigo_documento and dad.version = dd.version and dad.id_empresa = dd.id_empresa  
	                        inner join dev_tipo_documento dtd 
	                        on dtd.id = dd.tipo 
	                        inner join web_empresa we 
	                        on we.ID_EMPRESA = dd.id_empresa and we.ID_EMPRESA = @idEmpresa    
	                        left join dev_doc_area dda on dda.cod_documento = dd.codigo_documento and dda.version = dd.version and dda.id_empresa = dd.id_empresa
                            where 1 =1  ";

            if (CODIGO != "" && CODIGO != null)
            {
                query += " and dd.codigo_documento = '" + CODIGO + "' ";
            }
            cmd.Parameters.AddWithValue("@idEmpresa", ID_EMPRESA);
            cmd.CommandText = query;
            cmd.Connection = conn;
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosDocumento> lista = new List<DatosDocumento>();

            while (reader.Read())
            {
                DatosDocumento obj = new DatosDocumento();
                obj.indice = reader["indice"].ToString();
                obj.codigo = reader["codigo_documento"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.version = reader["version"].ToString();
                obj.tipo = reader["tipo"].ToString();
                obj.ult_revision = reader["ult_revision"].ToString();
                obj.caducidad = reader["caducidad"].ToString();
                obj.area = get_areas_listado(reader["codigo_documento"].ToString(), reader["ID_EMPRESA"].ToString(), reader["version"].ToString());
                obj.usuario = reader["FULLNAME"].ToString();
                //obj.ruta_archivo = get_ruta_archivo(reader["codigo_documento"].ToString());
                obj.id_empresa = reader["ID_EMPRESA"].ToString();
                obj.nombre_empresa = reader["NOMBRE_EMPRESA"].ToString();
                obj.ruta_archivo = reader["ruta"].ToString();
                obj.imprimir = reader["imprimir"].ToString();
                obj.descargar = reader["descargar"].ToString();
                obj.estado = reader["estado"].ToString();
                obj.palabras_clave = (string)get_palabra_clave_editar(obj.codigo, obj.version, obj.id_empresa);

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

    public object cargarDatos_con_restricciones(string ID_EMPRESA, string CODIGO)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string query = "select distinct dd.indice,dd.codigo_documento , dd.nombre, dd.version, dtd.tipo, " +
                          " CONVERT(nvarchar(10), dd.ult_revision,105) as ult_revision, " +
                          " CONVERT(nvarchar(10), dd.caducidad,105) as caducidad, " +
                          "   wu.FULLNAME, " +
                          "  dd.imprimir, dd.descargar, dad.estado , dad.ruta, we.ID_EMPRESA AS ID_EMPRESA, we.NOMBRE as NOMBRE_EMPRESA  " +
                          "   from dev_documento dd " +
                          "   inner join web_users wu on wu.USER_ID = dd.user_id  " +
                          "   inner join dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento and dad.version =(select max(version) from dev_archivo_documento where cod_documento = dd.codigo_documento) " +
                          "   inner join dev_tipo_documento dtd on dtd.id = dd.tipo " +
                          "inner join web_empresa we on we.ID_EMPRESA = dd.id_empresa and we.ID_EMPRESA = " + "'" + ID_EMPRESA + "' " +
                          "   left join dev_doc_area dda on dda.cod_documento = dd.codigo_documento " +
                          "   where dad.estado = 'Publicado'  ";

            if (CODIGO != "" && CODIGO != null)
            {
                query += " and dd.codigo_documento = '" + CODIGO + "' ";
            }

            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosDocumento> lista = new List<DatosDocumento>();

            while (reader.Read())
            {
                DatosDocumento obj = new DatosDocumento();
                obj.indice = reader["indice"].ToString();
                obj.codigo = reader["codigo_documento"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.version = reader["version"].ToString();
                obj.tipo = reader["tipo"].ToString();
                obj.ult_revision = reader["ult_revision"].ToString();
                obj.caducidad = reader["caducidad"].ToString();
                obj.area = get_areas_listado(reader["codigo_documento"].ToString(), reader["ID_EMPRESA"].ToString(), reader["version"].ToString());

                obj.usuario = reader["FULLNAME"].ToString();
                //obj.ruta_archivo = get_ruta_archivo(reader["codigo_documento"].ToString());
                obj.id_empresa = reader["ID_EMPRESA"].ToString();
                obj.nombre_empresa = reader["NOMBRE_EMPRESA"].ToString();
                obj.ruta_archivo = reader["ruta"].ToString();
                obj.imprimir = reader["imprimir"].ToString();
                obj.descargar = reader["descargar"].ToString();
                obj.estado = reader["estado"].ToString();
                obj.palabras_clave = (string)get_palabra_clave_editar(obj.codigo, obj.version, obj.id_empresa);

                lista.Add(obj);
            }
            reader.Close();
            //-------------ya no existe el campo transversal

            //string query2 = "select distinct dd.codigo_documento , dd.nombre, dd.version, dtd.tipo, " +
            //               " CONVERT(nvarchar(10), dd.ult_revision,105) as ult_revision, " +
            //               " CONVERT(nvarchar(10), dd.caducidad,105) as caducidad, " +
            //               "   wu.FULLNAME, " +
            //               "   dd.imprimir, dd.descargar, dad.estado, dad.ruta " +
            //               "   from dev_documento dd " +
            //               "   inner join web_users wu on wu.USER_ID = dd.user_id  " +
            //               "   inner join dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento and dad.version =(select max(version) from dev_archivo_documento where cod_documento = dd.codigo_documento) " +
            //               "   inner join dev_tipo_documento dtd on dtd.id = dd.tipo " +
            //               "   left join dev_doc_area dda on dda.cod_documento = dd.codigo_documento " +
            //               "   where 1 =1 and dd.transversal = 'Si' ";

            //cmd.Parameters.Clear();
            //cmd.CommandText = query2;
            //cmd.ExecuteNonQuery();


            //SqlDataReader reader2 = cmd.ExecuteReader();

            //while (reader2.Read())
            //{
            //    DatosDocumento obj = new DatosDocumento();
            //    obj.codigo = reader2["codigo_documento"].ToString();
            //    obj.nombre = reader2["nombre"].ToString();
            //    obj.version = reader2["version"].ToString();
            //    obj.tipo = reader2["tipo"].ToString();
            //    obj.ult_revision = reader2["ult_revision"].ToString();
            //    obj.caducidad = reader2["caducidad"].ToString();
            //    obj.area = get_areas_listado(reader2["codigo_documento"].ToString());
            //    obj.usuario = reader2["FULLNAME"].ToString();
            //    //obj.ruta_archivo = get_ruta_archivo(reader2["codigo_documento"].ToString());
            //    obj.ruta_archivo = reader2["ruta"].ToString();
            //    obj.imprimir = reader2["imprimir"].ToString();
            //    obj.descargar = reader2["descargar"].ToString();
            //    obj.estado = reader2["estado"].ToString();


            //    lista.Add(obj);
            //}
            //reader2.Close();

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

    public string get_areas_user(string ID)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = " select " +
                           " area " +
                           " from dev_user_area " +
                           " WHERE user_id = '" + ID + "'";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            string and_query = "";
            int cont = 0;
            while (reader.Read())
            {
                if (cont == 0)
                {
                    cont = 1;
                    and_query += "  '" + reader["area"].ToString() + "' ";
                }
                else
                {
                    and_query += "  ,'" + reader["area"].ToString() + "' ";
                }
            }
            reader.Close();

            return and_query;
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

    public string get_areas_listado(string codigo, string id_empresa, string version)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = " select  id_area  from dev_doc_area"
                        + " WHERE cod_documento ='" + codigo + "' and version='" + version + "' and id_empresa='" + id_empresa + "'";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();





            SqlDataReader reader = cmd.ExecuteReader();

            string and_query = "";
            while (reader.Read())
            {
                and_query += " " + reader["id_area"].ToString();
            }
            reader.Close();

            return and_query;
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
    public object get_areas_user_no_administrador()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                               .Select(c => c.Value).SingleOrDefault();

            string query = " select " +
                           " area " +
                           " from dev_user_area " +
                           " WHERE user_id = '" + user_id + "'";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<filtro_norma> lista = new List<filtro_norma>();

            while (reader.Read())
            {
                filtro_norma obj = new filtro_norma();
                obj.id = reader["area"].ToString();
                obj.text = reader["area"].ToString();
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
    public string es_administrador()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                               .Select(c => c.Value).SingleOrDefault();

            string query = "SELECT wg.administrador " +
                           "  FROM web_users wu " +
                           "  inner join web_groups wg on wg.GROUP_ID = wu.GROUP_ID " +
                           "  where wu.USER_ID = '" + user_id + "' ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            string es_administrador = "";
            while (reader.Read())
            {
                es_administrador = reader["administrador"].ToString();
            }
            reader.Close();
            return es_administrador;
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
    public string es_jefe_area()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                               .Select(c => c.Value).SingleOrDefault();

            string query = "SELECT nombre " +
                           "  FROM dev_area wu " +
                           "  where responsable = '" + user_id + "' ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            string es_jefe_area = "no";
            while (reader.Read())
            {
                es_jefe_area = "si";
            }
            reader.Close();
            return es_jefe_area;
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
    public object filtrar(DatosDocumento datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {
            string query = "";
            if (datos.calendario_inicio != null)
            {
                query += "declare @fechaInicio datetime ";
                query += "set @fechaInicio = convert(datetime, '" + datos.calendario_inicio + "', 103)";
            }
            if (datos.calendario_fin != null)
            {
                query += "declare @fechaFin datetime ";
                query += "set @fechaFin = convert(datetime, '" + datos.calendario_fin + "', 103)";
            }


            query += "select distinct dd.codigo_documento , dd.indice, dd.nombre, dd.version, dtd.tipo, " +
                           " CONVERT(nvarchar(10), dd.ult_revision,105) as ult_revision, " +
                           " CONVERT(nvarchar(10), dd.caducidad,105) as caducidad, " +
                           "   wu.FULLNAME, " +
                           "  dd.imprimir, dd.descargar, dad.estado , dad.ruta, we.ID_EMPRESA AS ID_EMPRESA, we.NOMBRE as NOMBRE_EMPRESA  " +
                           "   from dev_documento dd " +
                           "   inner join web_users wu on wu.USER_ID = dd.user_id  " +
                           "   inner join dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento and dad.version =dd.version and dad.id_empresa= dd.id_empresa" +
                           "   inner join dev_tipo_documento dtd on dtd.id = dd.tipo " +
                           " inner join web_empresa we on we.ID_EMPRESA = dd.id_empresa and we.ID_EMPRESA = " + "'" + datos.nombre_empresa + "' " +
                           "   left join dev_doc_area dda on dda.cod_documento = dd.codigo_documento " +
                           " left join dev_documento_norma ddn on ddn.codigo_documento = dd.codigo_documento " +
                           " left join dev_normas dn on dn.cod_norma = ddn.cod_norma " +
                           " left JOIN dev_doc_pn ddp on dd.codigo_documento = ddp.codigo_documento  " +
                           "   where 1 =1  ";
            if (datos.keywords != "" && datos.keywords != null)
            {
                query += " AND ddpc.palabra_clave IN (" + datos.keywords + ")";
            }
            if (datos.codigo != "" && datos.codigo != null)
            {
                query += " and dd.codigo_documento = '" + datos.codigo + "' ";
            }

            if (datos.documentos == "true")
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                // Get the claims values
                string user_id = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();

                query += " and dd.user_id = '" + user_id + "' ";
            }
            //se deserealiza el objeto json nodos


            if (datos.estado != null)
            {
                query += "and  dad.estado in (" + datos.estado + ")";
            }
            if (datos.normas != null)
            {
                query += "and ddp.COD_NORMA in (" + datos.normas + ") and dad.estado = 'Publicado' ";
            }
            if (datos.area != null)
            {
                query += "and dda.id_area in (" + datos.area + ")";
            }

            if (datos.tipo != null)
            {
                query += " and dd.tipo in (" + datos.tipo + ")";
            }
            if (datos.caducidad != null)
            {
                if (datos.caducidad == "No")
                {
                    query += " and dd.caducidad >= GETDATE()";
                }
                else
                {
                    query += " and dd.caducidad <= GETDATE()";
                }

            }
            if (datos.calendario_inicio != null)
            {
                query += " and dd.ult_revision >= @fechaInicio";
            }
            if (datos.calendario_fin != null)
            {
                query += " and dd.ult_revision <= @fechafin";
            }

            //query += " group by dd.codigo_documento , dd.nombre, dad.version, dtd.tipo,dd.ult_revision, dd.caducidad, "+
            //         "  dd.area,dd.transversal,wu.FULLNAME ";


            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosDocumento> lista = new List<DatosDocumento>();

            while (reader.Read())
            {
                DatosDocumento obj = new DatosDocumento();
                obj.codigo = reader["codigo_documento"].ToString();
                obj.indice = reader["indice"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.version = reader["version"].ToString();
                obj.tipo = reader["tipo"].ToString();
                obj.ult_revision = reader["ult_revision"].ToString();
                obj.caducidad = reader["caducidad"].ToString();
                obj.area = get_areas_listado(reader["codigo_documento"].ToString(), reader["ID_EMPRESA"].ToString(), reader["version"].ToString());
                obj.usuario = reader["FULLNAME"].ToString();
                //obj.ruta_archivo = get_ruta_archivo(reader["codigo_documento"].ToString());
                obj.id_empresa = reader["ID_EMPRESA"].ToString();
                obj.nombre_empresa = reader["NOMBRE_EMPRESA"].ToString();
                obj.ruta_archivo = reader["ruta"].ToString();
                obj.imprimir = reader["imprimir"].ToString();
                obj.descargar = reader["descargar"].ToString();
                obj.estado = reader["estado"].ToString();

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

            string query = "select ruta from dev_archivo_documento where estado != 'Histórico' and cod_documento = '" + codigo + "' ";


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
    public object get_norma(string id_empresa)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = $@" select dn.cod_norma as id, nombre_norma as name from dev_normas dn  
                             inner join dev_norma_empresa dne on dne.id_norma = dn.cod_norma where dne.id_empresa = '{id_empresa}'
                             order by nombre_norma ASC ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<filtro_norma> lista = new List<filtro_norma>();

            while (reader.Read())
            {
                filtro_norma obj = new filtro_norma();
                obj.id = reader["id"].ToString();
                obj.text = reader["name"].ToString();
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
    public object get_tipo_documento()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select id, tipo,codigo  from dev_tipo_documento  order by tipo ASC ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<filtro_tipo_documento> lista = new List<filtro_tipo_documento>();

            while (reader.Read())
            {
                filtro_tipo_documento obj = new filtro_tipo_documento();
                obj.id = reader["id"].ToString();
                obj.text = reader["tipo"].ToString();
                obj.codigo = reader["codigo"].ToString();
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
    public object get_area(string id_empresa)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = " select nombre, codarea from dev_area da " +
                     " where estado = 'Habilitado' " +
                     " and id_empresa = " + id_empresa + " order by nombre asc";




            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();

            List<filtro_area> lista = new List<filtro_area>();

            while (reader.Read())
            {
                filtro_area obj = new filtro_area();
                obj.id = reader["nombre"].ToString();
                obj.text = reader["nombre"].ToString();
                obj.codigo = reader["codarea"].ToString();
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

    public object get_palabra_clave()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select abreviatura from dev_area where estado = 'Habilitado'  group by abreviatura ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<selector_palabra_clave> lista = new List<selector_palabra_clave>();

            while (reader.Read())
            {
                selector_palabra_clave obj = new selector_palabra_clave();
                obj.value = reader["abreviatura"].ToString();
                obj.label = reader["abreviatura"].ToString();
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

    public bool existe_documento(Datos_documento_crear request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select codigo_documento from dev_documento where codigo_documento = '" + request.codigo + "' and version = '" + request.version + "' and id_empresa = '" + request.id_empresa + "' ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();


            bool bandera = false;
            while (reader.Read())
            {
                bandera = true;
            }
            reader.Close();
            return bandera;
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
    public object guardar(Datos_documento_crear request)
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
            object[] archivos_Obj = (object[])js.Deserialize(request.archivo, new object().GetType());

            foreach (var _accion in archivos_Obj)
            {
                //Dictionary<string, object> Nodo = (Dictionary<string, object>)accion;
                string archivo = _accion.ToString();

                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                // Get the claims values
                string user_id = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();
                //crear secuencia codigo

                DatosArea area = new DatosArea();
                if (request.area == "TRANSVERSAL")
                {
                    area.CODAREA = "TR";
                }
                else
                {

                    string get_codarea = @"select codarea from dev_area
                                      where nombre=@nombre and id_empresa=@id_empresa";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("nombre", request.area);
                    cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);

                    cmd.CommandText = get_codarea;
                    cmd.ExecuteNonQuery();
                    SqlDataReader _reader = cmd.ExecuteReader();

                    while (_reader.Read())
                    {
                        area.CODAREA = _reader[0].ToString();
                    }
                    _reader.Close();
                }




                string get_codeTipo = @"select codigo from dev_tipo_documento
                                      where id=@id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id", request.tipo);

                cmd.CommandText = get_codeTipo;
                cmd.ExecuteNonQuery();


                SqlDataReader tipo_reader = cmd.ExecuteReader();
                DatosTipo tipo = new DatosTipo();
                while (tipo_reader.Read())
                {
                    tipo.codigo = tipo_reader[0].ToString();
                }
                tipo_reader.Close();

                // letras del codigo
                string codigo_final;
                string base_codigo = tipo.codigo + area.CODAREA;
                string numero_codigo = "0001";
                // consultar convinaciones existentes
                string get_codigo_existente = @"select codigo_documento
                                              from dev_documento
                                              where id_empresa = @id_empresa";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);

                cmd.CommandText = get_codigo_existente;
                cmd.ExecuteNonQuery();

                SqlDataReader code_reader = cmd.ExecuteReader();

                List<string> codigos_lista = new List<string>();

                while (code_reader.Read())
                {
                    string codigo_existente;
                    codigo_existente = code_reader[0].ToString();
                    codigos_lista.Add(codigo_existente);
                }
                code_reader.Close();
                List<int> numero_existenteList = new List<int>();
                foreach (var item in codigos_lista)
                {
                    string code = item.ToString();

                    if (code.Contains(base_codigo))
                    {
                        int numero_existente = Int32.Parse(code.Substring(6));
                        numero_existenteList.Add(numero_existente);

                    }

                }
                if (numero_existenteList.Count > 0)
                {
                    int maximo = numero_existenteList.Max();
                    maximo++;
                    codigo_final = base_codigo + maximo.ToString().PadLeft(4, '0');
                }
                else
                {
                    codigo_final = base_codigo + numero_codigo;
                }






                string ruta_destino = mover_documento(archivo, codigo_final, request.version, request.id_empresa);
                string query = "";

                //query += "declare @fecha_caducidad datetime ";
                //query += "set @fecha_caducidad = convert(datetime, '" + request.fecha_caducidad + "', 103)";
                query += "insert into dev_documento (codigo_documento, nombre, tipo, ult_revision, user_id, version,  imprimir, descargar, caducidad, id_empresa) " +
                               " values (@codigo_documento, @nombre , @tipo, GETDATE(), @user_id , @version,  @imprimir, @descargar, @fecha_caducidad, @id_empresa) ";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("codigo_documento", codigo_final);
                cmd.Parameters.AddWithValue("nombre", request.nombre);
                cmd.Parameters.AddWithValue("tipo", request.tipo);
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("version", request.version == null ? "" : request.version);
                cmd.Parameters.AddWithValue("imprimir", request.imprimir == null ? "" : request.imprimir);
                cmd.Parameters.AddWithValue("descargar", request.descargar == null ? "" : request.descargar);
                cmd.Parameters.AddWithValue("fecha_caducidad", request.fecha_caducidad == null ? "" : request.fecha_caducidad);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                //fin registrar principal



                string query2 = "insert into dev_archivo_documento (cod_documento, ruta, nombre, version, estado, user_id_creador, observaciones, fecha_ult_revision, id_empresa) " +
                          " values (@codigo, @ruta, @nombre , @version , @estado, @user_id, @observaciones, GETUTCDATE(), @id_empresa) ";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("codigo", codigo_final);
                cmd.Parameters.AddWithValue("ruta", ruta_destino);
                cmd.Parameters.AddWithValue("nombre", request.archivo);
                cmd.Parameters.AddWithValue("version", request.version == null ? "" : request.version);
                cmd.Parameters.AddWithValue("estado", "Guardado");
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                cmd.Parameters.AddWithValue("observaciones", request.observaciones == null ? "" : request.observaciones);
                cmd.CommandText = query2;
                cmd.ExecuteNonQuery();

                //registrar areas asociadas;
                //se deserealiza el objeto json nodos
                // registrar Principal;


                //registrar archivo fisico asociado



                //se insertan las acciones asociadas al screen correspondiente
                //object[] valor = (Object[])Nodo["acciones"];



                string query3 = "INSERT INTO dev_doc_area (id_area, cod_documento, version, id_empresa) VALUES(@id_area, @cod_documento, @version, @id_empresa)";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id_area", request.area);
                cmd.Parameters.AddWithValue("cod_documento", codigo_final);
                cmd.Parameters.AddWithValue("version", request.version);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                cmd.CommandText = query3;
                cmd.ExecuteNonQuery();


                // registrar palabras asociadas
                object[] palabras_Obj = (object[])js.Deserialize(request.palabras_claves, new object().GetType());


                //se insertan las acciones asociadas al screen correspondiente
                //object[] valor = (Object[])Nodo["acciones"];
                foreach (var accion in palabras_Obj)
                {
                    //Dictionary<string, object> Nodo = (Dictionary<string, object>)accion;

                    string palabra_clave = accion.ToString();

                    cmd.Parameters.Clear();
                    string query4 = "INSERT INTO dev_doc_palabras_claves (cod_documento, palabra_clave, version, id_empresa) VALUES(@cod_documento, @palabra_clave, @version, @id_empresa)";
                    cmd.CommandText = query4;
                    cmd.Parameters.AddWithValue("cod_documento", codigo_final);
                    cmd.Parameters.AddWithValue("palabra_clave", palabra_clave);
                    cmd.Parameters.AddWithValue("version", request.version);
                    cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);


                    cmd.ExecuteNonQuery();

                }

                // registrar emmpresas asociadas
                //string asociar_empresa = "insert into dev_documento_empresa (codigo_documento, id_empresa) " +
                //               " values (@codigo, @id_empresa) ";

                //cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("codigo", request.codigo);
                //cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                //cmd.CommandText = asociar_empresa;
                //cmd.ExecuteNonQuery();


                //TRACKING
                Tracking t = new Tracking();
                string queryIn = "select dd.codigo_documento , dd.nombre, dd.version, dtd.tipo,dd.ult_revision, dd.caducidad, " +
                               "   wu.FULLNAME from dev_documento dd " +
                               "   inner join web_users wu on wu.USER_ID = dd.user_id  " +
                               "   inner join dev_tipo_documento dtd on dtd.id = dd.tipo " +
                               "   where dd.codigo_documento = '" + codigo_final + "'";

                request.codigo = codigo_final;
            }



            //GUARDAMOS EL TRACKING
            //t.guardarTracking("Crear", "Gestor documento", t.obtenerDatosNuevosyAntiguos(queryIn));


            //generamos la notificacion correspondiente dependiendo si es transversal o no
            //List<string> listaCorreos = new List<string>();
            //string destinatarios = "";
            menuModel modelo = new menuModel();

            //if (request.transversal.ToString() == "Si")
            //{
            //    query = "select wu.USER_ID from web_users wu join web_groups wg on wu.GROUP_ID = wg.GROUP_ID where wg.ADMINISTRADOR = 'true' " ;
            //    cmd.CommandText = query;
            //    cmd.ExecuteNonQuery();
            //    SqlDataReader reader = cmd.ExecuteReader();

            //    string usuario = "";
            //    List<string> listaNotificaciones = new List<string>();

            //    int id_notificacion;

            //    while (reader.Read())
            //    {                    
            //        usuario = reader["USER_ID"].ToString();

            //        id_notificacion = modelo.guardar_notificacion("Se creó el documento " + request.nombre + ", código " + request.codigo + " " , "No", usuario);

            //        usuario = "";
            //    }
            //    reader.Close();

            int id_notificacion = modelo.guardar_notificacion("Se creó el documento " + request.nombre + ", código " + request.codigo + " ", "", request.id_empresa);

            //}
            //else
            //{
            //-----------------notificaciones TODO
            //foreach (var accion in areas_Obj)
            //{
            //    Dictionary<string, object> Nodo = (Dictionary<string, object>)accion;

            //    string id_area = Nodo["id"].ToString();

            //    cmd.Parameters.Clear();
            //    query = "select responsable from dev_area where nombre = '"+ id_area + "'";
            //    cmd.CommandText = query;
            //    cmd.ExecuteNonQuery();
            //    SqlDataReader reader2 = cmd.ExecuteReader();


            //    menuModel modelo2 = new menuModel();
            //    string usuario2 = "";

            //    //string destinatarios = "";

            //    while (reader2.Read())
            //    {
            //        usuario2 = reader2["user_id"].ToString();

            //        modelo2.guardar_notificacion("Se creó el documento " + request.nombre + ", código " + request.codigo + " ", usuario2);

            //        usuario2 = "";
            //    }
            //    reader2.Close();

            //}


            //}

            //envio los correos
            //obtengo todos los usuarios ADMINISTRADORES_LFE del Sistema
            //query = "select wu.USER_ID from web_users wu join web_groups wg on wu.GROUP_ID = wg.GROUP_ID where wg.ADMINISTRADOR = 'true' ";
            //cmd.CommandText = query;
            //cmd.ExecuteNonQuery();
            //SqlDataReader reader3 = cmd.ExecuteReader();
            //string user;

            //while (reader3.Read())
            //{
            //    user = reader3["USER_ID"].ToString();                
            //    //consulto por el correo del usuario y lo almaceno en una lista
            //    destinatarios = modelo.emailUnUsuario(user);
            //    listaCorreos.Add(destinatarios);

            //    user = "";
            //}
            //reader3.Close();

            ////primero recorro la lista para quitar los correos repetidos
            //string destinatariosOficiales = modelo.quitarCorreosRepetidos(listaCorreos);
            //SqlCommand command22 = new SqlCommand("SP_SGI_ALERTA_NUEVO_DOCUMENTO", conn);
            //command22.CommandType = CommandType.StoredProcedure;

            //string url = WebConfigurationManager.AppSettings["baseURL"] + "documento";

            //command22.Parameters.AddWithValue("CODIGO_DOCUMENTO", request.codigo.ToString());
            //command22.Parameters.AddWithValue("emails", destinatariosOficiales);
            //command22.Parameters.AddWithValue("URL", url);


            //command22.ExecuteNonQuery();
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

    public object datos_editar(string ID)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = " select " +
                           " dd.codigo_documento, " +
                           " dd.nombre, " +
                           " dad.version, " +
                           " dad.ruta, " +
                           " dad.nombre as nombre_archivo, " +
                           " dd.tipo, " +
                           " dd.ult_revision, " +
                           " CONVERT(nvarchar(10), dd.caducidad,105) as caducidad, " +

                           " dd.imprimir, " +
                           " dd.descargar, " +
                           " dd.observaciones " +
                           " from dev_documento dd " +
                           " inner join dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento " +
                           " inner join dev_tipo_documento dtd on dtd.id = dd.tipo" +
                           " WHERE codigo_documento = '" + ID + "' and dad.estado != 'Histórico' ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<Datos_documento_editar> lista = new List<Datos_documento_editar>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            while (reader.Read())
            {
                Datos_documento_editar obj = new Datos_documento_editar();
                obj.codigo = reader["codigo_documento"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.tipo = reader["tipo"].ToString();
                obj.area = get_areas_editar(ID);
                obj.imprimir = reader["imprimir"].ToString();
                obj.descargar = reader["descargar"].ToString();
                obj.fecha_caducidad = reader["caducidad"].ToString();
                // obj.palabras_claves = get_palabra_clave_editar(ID);
                obj.version = reader["version"].ToString();
                obj.observaciones = reader["observaciones"].ToString();
                obj.ruta = reader["ruta"].ToString();
                obj.archivo = reader["nombre_archivo"].ToString();


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

    public object get_areas_editar(string ID)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = " select " +
                           " cod_documento, " +
                           " id_area " +
                           " from dev_doc_area " +
                           " WHERE cod_documento = '" + ID + "'";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<selector_editar> lista = new List<selector_editar>();

            while (reader.Read())
            {
                selector_editar obj = new selector_editar();
                obj.id = reader["id_area"].ToString();
                obj.text = reader["id_area"].ToString();
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

    public object get_palabra_clave_editar(string codigo, string version, string id_empresa)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = " select " +
                           " cod_documento, " +
                           " palabra_clave " +
                           " from dev_doc_palabras_claves " +
                           " WHERE cod_documento =@codigo and version=@version and id_empresa=@id_empresa";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("codigo", codigo);
            cmd.Parameters.AddWithValue("version", version);
            cmd.Parameters.AddWithValue("id_empresa", id_empresa);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<selector_editar> lista = new List<selector_editar>();

            while (reader.Read())
            {
                selector_editar obj = new selector_editar();
                obj.id = reader["palabra_clave"].ToString();
                obj.text = reader["palabra_clave"].ToString();
                lista.Add(obj);
            }
            reader.Close();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string lista_serialized = (string)js.Serialize(lista);
            return lista_serialized;
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

    public object actualizar(Datos_documento_crear request)
    {


        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        //TRACKING
        Tracking t = new Tracking();

        string queryIn = "select dd.codigo_documento , dd.nombre, dd.version, dtd.tipo,dd.ult_revision, dd.caducidad, " +
                           "   wu.FULLNAME from dev_documento dd " +
                           "   inner join web_users wu on wu.USER_ID = dd.user_id  " +
                           //"   inner join dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento " +
                           "   inner join dev_tipo_documento dtd on dtd.id = dd.tipo " +
                           "   where dd.codigo_documento = '" + request.codigo + "' and id_empresa = '" + request.id_empresa + "'";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                               .Select(c => c.Value).SingleOrDefault();


            string obtener_estado = @"select estado from dev_archivo_documento 
                                      where cod_documento=@codigo and version=@version and id_empresa=@id_empresa";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("codigo", request.codigo);
            cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
            cmd.Parameters.AddWithValue("version", request.version);

            cmd.CommandText = obtener_estado;
            cmd.ExecuteNonQuery();


            SqlDataReader _reader = cmd.ExecuteReader();
            List<DatosDocumento> _lista = new List<DatosDocumento>();
            DatosDocumento _obj = new DatosDocumento();
            while (_reader.Read())
            {
                _obj.estado = _reader[0].ToString();
            }
            _reader.Close();


            if (request.cambio_archivo == "true" && _obj.estado == "Publicado")
            {
                //obtener version a incrementar
                string obtener_version = @"select max(version) from dev_documento where codigo_documento = @codigo and id_empresa = @id_empresa";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("codigo", request.codigo);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);

                cmd.CommandText = obtener_version;
                cmd.ExecuteNonQuery();


                SqlDataReader reader = cmd.ExecuteReader();
                List<DatosDocumento> lista = new List<DatosDocumento>();
                DatosDocumento obj = new DatosDocumento();
                while (reader.Read())
                {
                    obj.version = reader[0].ToString();
                }
                reader.Close();
                int nueva_version;

                nueva_version = Int32.Parse(obj.version) + 1;

                string ruta_destino = mover_documento(request.archivo, request.codigo, nueva_version.ToString(), request.id_empresa);


                // * INSERT NUEVA VERSION
                string query = @"insert into dev_documento 
                                (codigo_documento, nombre, tipo, ult_revision, user_id, version,  imprimir, descargar, caducidad, id_empresa) 
                                values (@codigo_documento, @nombre , @tipo, GETDATE(), @user_id , @version,  @imprimir, @descargar, @fecha_caducidad, @empresa) ";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("codigo_documento", request.codigo);
                cmd.Parameters.AddWithValue("nombre", request.nombre);
                cmd.Parameters.AddWithValue("tipo", request.tipo);
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("version", nueva_version);
                cmd.Parameters.AddWithValue("imprimir", request.imprimir == null ? "" : request.imprimir);
                cmd.Parameters.AddWithValue("descargar", request.descargar == null ? "" : request.descargar);
                cmd.Parameters.AddWithValue("fecha_caducidad", request.fecha_caducidad == null ? "" : request.fecha_caducidad);
                cmd.Parameters.AddWithValue("empresa", request.id_empresa);


                //cmd.Parameters.AddWithValue("tipo_dato", datos.tipo_dato);
                //cmd.Parameters.AddWithValue("estado", request.estado);

                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                // * INSERT NUEVA VERSION
                string query2 = @"insert into dev_archivo_documento 
                                (cod_documento, ruta, nombre, version, estado, user_id_creador, observaciones, fecha_ult_revision, id_empresa) 
                                values (@codigo, @ruta, @nombre , @version , @estado, @user_id, @observaciones, GETUTCDATE(), @id_empresa) ";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("codigo", request.codigo);
                cmd.Parameters.AddWithValue("ruta", ruta_destino);
                cmd.Parameters.AddWithValue("nombre", request.archivo);
                cmd.Parameters.AddWithValue("version", nueva_version);
                cmd.Parameters.AddWithValue("estado", "Guardado");
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);

                cmd.Parameters.AddWithValue("observaciones", request.observaciones == null ? "" : request.observaciones);
                cmd.CommandText = query2;
                cmd.ExecuteNonQuery();

                // * UPDATE ANTIGUA VERSION
                string update_estado = @"UPDATE dev_archivo_documento set estado='Publicado' ,fecha_ult_revision= GETUTCDATE()
                                        where cod_documento=@codigo and version=@version and id_empresa=@id_empresa";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("codigo", request.codigo);
                cmd.Parameters.AddWithValue("version", request.version);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                cmd.CommandText = update_estado;
                cmd.ExecuteNonQuery();



                //se insertan las acciones asociadas al screen correspondiente
                //object[] valor = (Object[])Nodo["acciones"];

                string id_area = request.area;

                cmd.Parameters.Clear();
                string insertar_area = "INSERT INTO dev_doc_area (id_area, cod_documento, version, id_empresa) VALUES(@id_area, @cod_documento, @version, @id_empresa)";
                cmd.CommandText = insertar_area;
                cmd.Parameters.AddWithValue("id_area", id_area);
                cmd.Parameters.AddWithValue("cod_documento", request.codigo);
                cmd.Parameters.AddWithValue("version", nueva_version);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);

                cmd.ExecuteNonQuery();


                // registrar palabras asociadas
                JavaScriptSerializer js = new JavaScriptSerializer();
                object[] palabras_Obj = (object[])js.Deserialize(request.palabras_claves, new object().GetType());


                //se insertan las acciones asociadas al screen correspondiente
                //object[] valor = (Object[])Nodo["acciones"];
                foreach (var accion in palabras_Obj)
                {
                    //Dictionary<string, object> Nodo = (Dictionary<string, object>)accion;

                    string palabra_clave = accion.ToString();

                    cmd.Parameters.Clear();
                    string query4 = @"INSERT INTO dev_doc_palabras_claves (cod_documento, palabra_clave, version, id_empresa) VALUES(@cod_documento, @palabra_clave, @version, @id_empresa)";
                    cmd.CommandText = query4;
                    cmd.Parameters.AddWithValue("cod_documento", request.codigo);
                    cmd.Parameters.AddWithValue("palabra_clave", palabra_clave);
                    cmd.Parameters.AddWithValue("version", nueva_version);
                    cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);

                    cmd.ExecuteNonQuery();

                }

                // registrar emmpresas asociadas
                //string asociar_empresa = "insert into dev_documento_empresa (codigo_documento, id_empresa, version) " +
                //               " values (@codigo, @id_empresa, @version) ";

                //cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("codigo", request.codigo);
                //cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                //cmd.Parameters.AddWithValue("version", nueva_version);

                //cmd.CommandText = asociar_empresa;
                //cmd.ExecuteNonQuery();


                //TRACKING



            }
            else
            {
                if (request.cambio_archivo == "true")
                {
                    string ruta_destino = mover_documento(request.archivo, request.codigo, request.version, request.id_empresa);

                    string query = @"UPDATE dev_documento SET nombre=@nombre,  ult_revision=GETDATE(), user_id=@user_id,  
                                     imprimir=@imprimir, descargar=@descargar, caducidad=@fecha_caducidad 
                                     where codigo_documento=@codigo_documento and version=@version and id_empresa=@id_empresa
                                    ";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("codigo_documento", request.codigo);
                    cmd.Parameters.AddWithValue("nombre", request.nombre);
                    cmd.Parameters.AddWithValue("user_id", user_id);
                    cmd.Parameters.AddWithValue("version", request.version == null ? "" : request.version);
                    cmd.Parameters.AddWithValue("imprimir", request.imprimir == null ? "" : request.imprimir);
                    cmd.Parameters.AddWithValue("descargar", request.descargar == null ? "" : request.descargar);
                    cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                    cmd.Parameters.AddWithValue("fecha_caducidad", request.fecha_caducidad == null ? "" : request.fecha_caducidad);
                    cmd.Parameters.AddWithValue("observaciones", request.observaciones == null ? "" : request.observaciones);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    //cod_documento, ruta, nombre, version, estado, user_id_creador, observaciones, fecha_ult_revision, id_empresa
                    string update_archivo = @"update dev_archivo_documento set 
                                            ruta=@ruta, nombre=@nombre, fecha_ult_revision =GETDATE()
                                            where cod_documento=@codigo and version=@version and id_empresa=@id_empresa
                                            ";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("codigo", request.codigo);
                    cmd.Parameters.AddWithValue("nombre", request.nombre);
                    cmd.Parameters.AddWithValue("ruta", ruta_destino);
                    cmd.Parameters.AddWithValue("version", request.version == null ? "" : request.version);
                    cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                    cmd.CommandText = update_archivo;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string query = @"UPDATE dev_documento SET nombre=@nombre,  ult_revision=GETDATE(), user_id=@user_id,  
                                     imprimir=@imprimir, descargar=@descargar, caducidad=@fecha_caducidad 
                                     where codigo_documento=@codigo_documento and version=@version and id_empresa=@id_empresa
                                    ";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("codigo_documento", request.codigo);
                    cmd.Parameters.AddWithValue("nombre", request.nombre);
                    cmd.Parameters.AddWithValue("user_id", user_id);
                    cmd.Parameters.AddWithValue("version", request.version == null ? "" : request.version);
                    cmd.Parameters.AddWithValue("imprimir", request.imprimir == null ? "" : request.imprimir);
                    cmd.Parameters.AddWithValue("descargar", request.descargar == null ? "" : request.descargar);
                    cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);
                    cmd.Parameters.AddWithValue("fecha_caducidad", request.fecha_caducidad == null ? "" : request.fecha_caducidad);
                    cmd.Parameters.AddWithValue("observaciones", request.observaciones == null ? "" : request.observaciones);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                string delete_palabras = "delete from dev_doc_palabras_claves " +
                 " where cod_documento=@codigo_documento and version=@version and id_empresa=@id_empresa";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("codigo_documento", request.codigo);
                cmd.Parameters.AddWithValue("version", request.version);
                cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);

                cmd.CommandText = delete_palabras;
                cmd.ExecuteNonQuery();

                JavaScriptSerializer js = new JavaScriptSerializer();
                object[] palabras_Obj = (object[])js.Deserialize(request.palabras_claves, new object().GetType());


                //se insertan las acciones asociadas al screen correspondiente
                //object[] valor = (Object[])Nodo["acciones"];
                foreach (var accion in palabras_Obj)
                {
                    //Dictionary<string, object> Nodo = (Dictionary<string, object>)accion;

                    string palabra_clave = accion.ToString();

                    cmd.Parameters.Clear();
                    string query4 = "INSERT INTO dev_doc_palabras_claves (cod_documento, palabra_clave, version, id_empresa) VALUES(@cod_documento, @palabra_clave, @version, @id_empresa)";
                    cmd.CommandText = query4;
                    cmd.Parameters.AddWithValue("cod_documento", request.codigo);
                    cmd.Parameters.AddWithValue("palabra_clave", palabra_clave);
                    cmd.Parameters.AddWithValue("version", request.version);
                    cmd.Parameters.AddWithValue("id_empresa", request.id_empresa);

                    cmd.ExecuteNonQuery();

                }
            }



            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            //t.guardarTracking("Editar", "Gestor documento", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

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

    public object publicar(string ID)
    {


        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        //TRACKING
        Tracking t = new Tracking();
        string queryIn = "select cod_documento , ruta, nombre, version, estado " +
                         " from dev_archivo_documento " +
                         " where cod_documento = '" + ID + "'";

        try
        {

            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                               .Select(c => c.Value).SingleOrDefault();

            string query = "UPDATE dev_archivo_documento SET estado='Publicado', user_id_publicador=@user_id " +
                           " where cod_documento=@codigo_documento  and estado != 'Eliminado'";

            cmd.Parameters.AddWithValue("codigo_documento", ID);
            cmd.Parameters.AddWithValue("user_id", user_id);

            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            int rowsAffected = cmd.ExecuteNonQuery();


            Boolean data = false;

            //string destinatarios = "";

            if (rowsAffected == 1)
            {
                data = true;

            }


            // buscar_datos_notificacion(ID);

            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Publicar", "Gestor documento", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

            return data;
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

    public object despublicar(DatosDocumento_historico datos)
    {


        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        //TRACKING
        Tracking t = new Tracking();
        string queryIn = "select cod_documento , ruta, nombre, version, estado " +
                         " from dev_archivo_documento " +
                         " where cod_documento = '" + datos.codigo + "' and id_empresa = '" + datos.id_empresa + "' " +
                         " and version =  '" + datos.version + "'";

        try
        {

            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string user_id = identity.Claims.Where(c => c.Type == "user_id")
                               .Select(c => c.Value).SingleOrDefault();

            string query = @"UPDATE dev_archivo_documento SET estado='Guardado', user_id_publicador=@user_id 
                            where cod_documento=@codigo_documento  and id_empresa=@id_empresa and version =@version
                            and estado = 'Publicado'";

            cmd.Parameters.AddWithValue("codigo_documento", datos.codigo);
            cmd.Parameters.AddWithValue("id_empresa", datos.id_empresa);
            cmd.Parameters.AddWithValue("version", datos.version);
            cmd.Parameters.AddWithValue("user_id", user_id);

            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            //int rowsAffected = cmd.ExecuteNonQuery();


            Boolean data = true;

            //string destinatarios = "";

            //   if (rowsAffected == 1)
            //   {
            //  data = true;

            // }

            // buscar_datos_notificacion(ID);

            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Despublicar", "Gestor documento", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

            menuModel modelo = new menuModel();
            int id_notificacion = modelo.guardar_notificacion("Se despublicó el documento " + datos.nombre + ", código " + datos.codigo, "", datos.id_empresa);

            return data;
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

    public object buscar_datos_notificacion(string id_documento)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {
            menuModel modelo = new menuModel();

            string query = $@" select  
                            nombre  
                            id_empresa
                            from dev_documento  
                            WHERE codigo_documento = '{id_documento}' ";


            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            SqlDataReader reader = cmd.ExecuteReader();

            string nombre = "";
            string id_empresa = "";
            while (reader.Read())
            {
                nombre = reader["nombre"].ToString();
                id_empresa = reader["id_empresa"].ToString();
            }
            reader.Close();

            int id_notificacion = modelo.guardar_notificacion("Se publicó el documento " + nombre + ", código " + id_documento, "", id_empresa);

            List<string> correosEncargadoAreas = new List<string>();

            //saca el area(s) a la que esta asociado el documento creado
            string query_areas = " select " +
                           " id_area " +
                           " from dev_doc_area " +
                           " WHERE cod_documento = '" + id_documento + "' ";


            cmd.Parameters.Clear();
            cmd.CommandText = query_areas;
            cmd.ExecuteNonQuery();

            SqlDataReader reader2 = cmd.ExecuteReader();

            List<selector_area> lista = new List<selector_area>();
            while (reader2.Read())
            {
                selector_area obj = new selector_area();
                obj.value = reader2["id_area"].ToString();
                //envío el correo                     
                lista.Add(obj);


                //envio el correo al encargado del area
                SqlCommand cmd2 = conn.CreateCommand();
                cmd2.Connection = conn;
                cmd2.Parameters.Clear();

                //obtengo el correo del encargado del area mas todos los usuarios de esa misma area
                query = "select wu.Email from dev_area da " +
                           "inner join web_users wu on da.user_id = wu.USER_ID " +
                           "where nombre = '" + reader2["id_area"].ToString() + "' " +
                           "union all " +
                           "select wu.Email from dev_user_area dua " +
                           "inner join web_users wu on dua.user_id = wu.USER_ID " +
                           "where area = '" + reader2["id_area"].ToString() + "' ";

                cmd2.CommandText = query;
                cmd2.ExecuteNonQuery();
                SqlDataReader reader3 = cmd2.ExecuteReader();


                menuModel modelo2 = new menuModel();
                string usuario2 = "";

                while (reader3.Read())
                {
                    usuario2 = reader3["Email"].ToString();
                    //añado a la lista todos los correos del area
                    correosEncargadoAreas.Add(usuario2);

                    usuario2 = "";
                }
                reader3.Close();

            }
            reader2.Close();

            modelo.guardar_notificacion_area(id_notificacion, lista);

            //mando el correo a todas las areas que correspondan
            SqlCommand command = new SqlCommand("SP_SGI_ALERTA_DOCUMENTO_PUBLICADO", conn);
            command.CommandType = CommandType.StoredProcedure;

            string url = WebConfigurationManager.AppSettings["baseURL"] + "documento";

            command.Parameters.AddWithValue("CODIGO_DOCUMENTO", id_documento);
            command.Parameters.AddWithValue("emails", modelo.quitarCorreosRepetidos(correosEncargadoAreas));
            command.Parameters.AddWithValue("URL", url);

            command.ExecuteNonQuery();





            //if (transversal == "Si")
            //{
            //    //aca le envío el correo a todo el mundo                

            //    SqlCommand command = new SqlCommand("SP_SGI_ALERTA_DOCUMENTO_PUBLICADO", conn);
            //    command.CommandType = CommandType.StoredProcedure;

            //    //SqlParameter paramId = new SqlParameter("Id", SqlDbType.Int);
            //    //paramId.Direction = ParameterDirection.Output;
            //    //command.Parameters.Add(paramId);
            //    string destinatarios = modelo.todosLosEmails();
            //    string url = WebConfigurationManager.AppSettings["baseURL"] + "documento";

            //    command.Parameters.AddWithValue("CODIGO_DOCUMENTO", id_documento);
            //    command.Parameters.AddWithValue("emails", destinatarios);
            //    command.Parameters.AddWithValue("URL", url);

            //    command.ExecuteNonQuery();
            //}

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

    public object eliminar(string codigo, string id_empresa, string version, string indice)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = default(SqlTransaction);
        transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        //TRACKING
        Tracking t = new Tracking();
        string queryIn = "select dd.codigo_documento , dd.nombre, dd.version, dtd.tipo,dd.ult_revision, dd.caducidad, " +
                           "  wu.FULLNAME from dev_documento dd " +
                           "   inner join web_users wu on wu.USER_ID = dd.user_id  " +
                           //"   inner join dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento " +
                           "   inner join dev_tipo_documento dtd on dtd.id = dd.tipo " +
                           "   where dd.indice = '" + indice + "'";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            // List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            // bool elimino_repositorio = delete_documento(ID);

            string query;

            // query = " DELETE FROM dev_doc_area WHERE cod_documento = @codigo_documento ";
            // cmd.CommandText = query;
            // cmd.Parameters.AddWithValue("codigo_documento", ID);
            // cmd.ExecuteNonQuery();

            // cmd.Parameters.Clear();

            // query = " DELETE FROM dev_doc_palabras_claves WHERE cod_documento = @codigo_documento ";
            // cmd.CommandText = query;
            // cmd.Parameters.AddWithValue("codigo_documento", ID);
            // cmd.ExecuteNonQuery();

            // cmd.Parameters.Clear();

            // query = " DELETE FROM dev_archivo_documento WHERE cod_documento = @codigo_documento ";
            // cmd.CommandText = query;
            // cmd.Parameters.AddWithValue("codigo_documento", ID);
            // cmd.ExecuteNonQuery();

            // cmd.Parameters.Clear();

            // query = "DELETE FROM dev_documento WHERE codigo_documento = @codigo_documento ";
            // cmd.CommandText = query;
            // cmd.Parameters.AddWithValue("codigo_documento", ID);
            // cmd.ExecuteNonQuery();



            // //GUARDAMOS EL TRACKING
            // t.guardarTracking("Eliminar", "Gestor documento", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

            query = @"update dev_archivo_documento set estado= 'Eliminado' where cod_documento = @codigo_documento 
                    and id_empresa = @id_empresa and version = @version and estado in ('Histórico','Guardado')";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("codigo_documento", codigo);
            cmd.Parameters.AddWithValue("version", version);
            cmd.Parameters.AddWithValue("id_empresa", id_empresa);
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


    public bool es_publicadoD(string codigo, string id_empresa, string version)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select estado from dev_archivo_documento where cod_documento = '" + codigo + "' and estado = 'Publicado' " +
                            " and id_empresa = '" + id_empresa + "' and version = '" + version + "'";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();


            bool bandera = true;
            while (reader.Read())
            {
                bandera = false;
            }
            reader.Close();
            return bandera;
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

    public bool es_publicado(string ID)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select estado from dev_archivo_documento where cod_documento = '" + ID + "' and estado = 'Publicado' ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();


            bool bandera = true;
            while (reader.Read())
            {
                bandera = false;
            }
            reader.Close();
            return bandera;
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

    public bool estaPublicado(datosDocumentoPublicar request)
    {
        SqlConnection conn = dbproyecto.openConnection();

        try
        {
            SqlCommand cmd = new SqlCommand();
            string query = "select estado from dev_archivo_documento where cod_documento = @codigoDocumento and version = @versionDocumento and id_empresa = @idEmpresaDoc and estado = 'Publicado'";
            cmd.Parameters.AddWithValue("@codigoDocumento", request.idDocumento);
            cmd.Parameters.AddWithValue("@versionDocumento", request.versionDocumento);
            cmd.Parameters.AddWithValue("@idEmpresaDoc", request.idEmpresaDocumento);

            cmd.CommandText = query;
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            bool bandera = false;
            while (reader.Read())
            {
                bandera = true;
            }
            reader.Close();
            return bandera;
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

    public object cargarDatos_historial(DatosDocumento_historico request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {
            string query = "select dd.codigo_documento, dad.fecha_ult_revision, dd.nombre, dad.version, " +
                           "   wu.FULLNAME as usuario_creador,wus.FULLNAME as usuario_publicador, " +
                           "   dad.ruta, dad.nombre as nombre_archivo, dad.estado, dad.observaciones " +
                           "   from dev_documento dd " +
                           " inner join dev_archivo_documento dad on dad.cod_documento = dd.codigo_documento " +
                           " inner join web_users wu on wu.USER_ID = dad.user_id_creador " +
                           " left join web_users wus on wus.USER_ID = dad.user_id_publicador   " +
                           "   where dd.codigo_documento = '" + request.codigo + "' ";



            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            SqlDataReader reader = cmd.ExecuteReader();
            List<DatosDocumento_historico> lista = new List<DatosDocumento_historico>();

            while (reader.Read())
            {
                DatosDocumento_historico obj = new DatosDocumento_historico();
                obj.revision = reader["fecha_ult_revision"].ToString();
                obj.codigo = reader["codigo_documento"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.version = reader["version"].ToString();
                obj.observacion = reader["observaciones"].ToString();
                obj.usuario_creador = reader["usuario_creador"].ToString();
                obj.usuario_publicador = reader["usuario_publicador"].ToString();
                obj.estado = reader["estado"].ToString();
                obj.ruta = reader["ruta"].ToString();
                obj.nombre_archivo = reader["nombre_archivo"].ToString();

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

    public string mover_documento(string archivo, string codigo_documento, string version, string empresa)
    {
        try
        {

            string rutaFisicaDocumento = WebConfigurationManager.AppSettings["rutaFisicaDocumento"];
            string tempDocUrl = WebConfigurationManager.AppSettings["DocumentosUrl"];

            string sourceFile = rutaFisicaDocumento + "temporal" + "\\" + archivo;
            string destinationFile = rutaFisicaDocumento + "documento" + "\\" + empresa + "\\" + codigo_documento + "\\" + version + "\\" + archivo;
            bool documento_existe = System.IO.File.Exists(destinationFile);
            if (documento_existe)
            {
                File.Delete(destinationFile);
            }
            string ruta_final = tempDocUrl + "documento" + "/" + empresa + "/" + codigo_documento + "/" + version + "/" + archivo; //ruta final para guardar como resgistro en la tabla dev_archivo_documento

            string directoryName = String.Empty;
            string directoryName2 = String.Empty;

            var path = HttpRuntime.AppDomainAppPath;


            directoryName = System.IO.Path.Combine(path, "file\\documento\\" + empresa + "\\" + codigo_documento);
            bool a = System.IO.Directory.Exists(directoryName);
            //bool a = Directory.Exists(directoryName);
            if (a == false)
            {
                Directory.CreateDirectory(directoryName);
            }
            directoryName2 = System.IO.Path.Combine(path, "file\\documento\\" + empresa + "\\" + codigo_documento + "\\" + version);
            bool b = System.IO.Directory.Exists(directoryName2);
            if (b == false)
            {
                Directory.CreateDirectory(directoryName2);
            }
            else
            {
                directoryName2 = System.IO.Path.Combine(path, "file\\documento\\" + empresa + "\\" + codigo_documento + "\\" + version);
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

    public bool delete_documento(string codigo_documento)
    {
        try
        {

            string rutaFisicaDocumento = WebConfigurationManager.AppSettings["rutaFisicaDocumento"];


            string destinationFile = rutaFisicaDocumento + "documento" + "\\" + codigo_documento;



            // To move a file or folder to a new location:

            Directory.Delete(@"" + destinationFile, true);

            return true;
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public string get_fecha_minima()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "SELECT CONVERT(nvarchar(10), GetDate(),105) as fecha_minima ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            string fecha_minima = "";
            while (reader.Read())
            {

                fecha_minima = reader["fecha_minima"].ToString();

            }
            reader.Close();
            return fecha_minima;
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

    public object publicarDocumento(datosDocumentoPublicar request)
    {


        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {


            string query = @"UPDATE dev_archivo_documento SET estado = 'Publicado' 
                        WHERE cod_documento = @idDocumento and version = @versionDocumento  and id_empresa = @idEmpresaDocumento";

            cmd.Parameters.AddWithValue("idDocumento", request.idDocumento);
            cmd.Parameters.AddWithValue("idEmpresaDocumento", request.idEmpresaDocumento);
            cmd.Parameters.AddWithValue("versionDocumento", request.versionDocumento);

            cmd.CommandText = query;
            cmd.ExecuteNonQuery();





            string update_historicos = @"UPDATE dev_archivo_documento SET estado = 'Histórico' 
                        WHERE cod_documento = @idDocumento   and id_empresa = @idEmpresaDocumento and version != @versionDocumento and estado != 'Eliminado'";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("idDocumento", request.idDocumento);
            cmd.Parameters.AddWithValue("idEmpresaDocumento", request.idEmpresaDocumento);
            cmd.Parameters.AddWithValue("versionDocumento", request.versionDocumento);

            cmd.CommandText = update_historicos;
            cmd.ExecuteNonQuery();

            menuModel modelo = new menuModel();

            int id_notificacion = modelo.guardar_notificacion("Se publicó el documento " + request.nombre + ", código " + request.idDocumento, "", request.idEmpresaDocumento);



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

    public object gurdarRegistroActividad(registroActividad request)
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

            request.codigo = request.codigo.TrimEnd(' ');
            string query = "";
            // query += "declare @fecha_caducidad datetime ";
            // query += "set @fecha_caducidad = convert(datetime, '" + request.fecha_caducidad + "', 103)";
            query += "insert into web_log_vizualizacion_documento (id_indice, tipo_actividad, id_usuario, fecha_revision) " +
                    " values (@indice, @tipo_actividad, @id_usuario, getdate()) ";

            cmd.Parameters.AddWithValue("indice", request.codigo);
            cmd.Parameters.AddWithValue("tipo_actividad", request.tipo_actividad);
            cmd.Parameters.AddWithValue("id_usuario", user_id);

            cmd.CommandText = query;
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

}
