using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


//agregar para crear lo basico
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Collections;

public class PerfilesModel : Conexion
{
    public object cargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select group_id, name, description,fecha_created, fecha_updated   from web_groups ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<Datosperfiles> lista = new List<Datosperfiles>();

            while (reader.Read())
            {
                Datosperfiles obj = new Datosperfiles();
                obj.group_id = reader["group_id"].ToString();
                obj.nombre = reader["name"].ToString();
                obj.descripcion = reader["description"].ToString();
                obj.fecha_created = reader["fecha_created"].ToString();
                obj.fecha_updated = reader["fecha_updated"].ToString();

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

    public object nombres()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select  name as name , name as code from web_groups ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<Datosperfiles> lista = new List<Datosperfiles>();

            while (reader.Read())
            {
                Datosperfiles obj = new Datosperfiles();

                obj.name = reader["name"].ToString();
                obj.code = reader["code"].ToString();

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

    public object branch()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "SELECT b.name,b.link,b.source,b.id_branch, count(s.ID_SHEET) as hijos FROM web_branch b "
+ " left join web_sheet s on s.ID_BRANCH = b.ID_BRANCH " +
                           " group by b.name,b.link,b.source,b.id_branch,b.orden order by b.orden ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<Datosbranch> lista = new List<Datosbranch>();

            while (reader.Read())
            {
                Datosbranch obj = new Datosbranch();
                obj.link = reader["link"].ToString();
                obj.nombre = reader["name"].ToString();
                obj.source = reader["source"].ToString();
                obj.id_branch = reader["id_branch"].ToString();
                obj.hijos = reader["hijos"].ToString();



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

    public object sheet()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = " SELECT s.ID_SHEET, s.name,s.link,b.name as namePadre,s.id_branch FROM web_sheet s " +
                           " INNER JOIN web_branch b ON b.id_branch = s.id_branch ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<Datossheet> lista = new List<Datossheet>();

            while (reader.Read())
            {
                Datossheet obj = new Datossheet();
                obj.link = reader["link"].ToString();
                obj.nombre = reader["name"].ToString();
                obj.namePadre = reader["namePadre"].ToString();
                obj.id_branch = reader["id_branch"].ToString();
                obj.id_sheet = reader["ID_SHEET"].ToString();
                obj.codigo = reader["ID_SHEET"].ToString();
                obj.estado = reader["ID_SHEET"].ToString();


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
    public object guardar(perfilesRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {
            string query = " select NAME from  WEB_GROUPS where UPPER(NAME)=UPPER(@nombre)";

            cmd.Parameters.AddWithValue("nombre", request.nombre);

            cmd.CommandText = query;

            SqlDataReader reader = cmd.ExecuteReader();
            int afectada = 0;
            while (reader.Read())
            {
                afectada = 1;
            }
            reader.Close();
            cmd.Parameters.Clear();
            if (afectada == 0)
            {

                //se deserealiza el objeto json nodos
                JavaScriptSerializer js = new JavaScriptSerializer();
                object[] testObj = (object[])js.Deserialize(request.nodos, new object().GetType());


                //se actualizan los datos del perfil

                query = "INSERT INTO WEB_GROUPS(NAME,DESCRIPTION,USUARIO,ADMINISTRADOR,AUDITOR, fecha_created, fecha_updated)" +
                " VALUES(@nombre,@descripcion,@usuario,@administrador,@auditor ,FORMAT(GETDATE() , 'dd/MM/yyyy HH:mm:ss'), FORMAT(GETDATE() , 'dd/MM/yyyy HH:mm:ss')) SELECT CAST(scope_identity() AS int) ";

                cmd.Parameters.AddWithValue("nombre", request.nombre);
                cmd.Parameters.AddWithValue("descripcion", request.descripcion);
                cmd.Parameters.AddWithValue("usuario", request.multiempresa);
                cmd.Parameters.AddWithValue("administrador", request.administrador);
                cmd.Parameters.AddWithValue("auditor", request.auditor);


                //SqlParameter outputParameter = new SqlParameter("GROUP_ID", SqlType.Int32);
                //outputParameter.Direction = ParameterDirection.Output;
                //cmd.Parameters.Add(outputParameter);

                cmd.CommandText = query;
                //cmd.ExecuteNonQuery();


                //decimal group_id = Convert.ToDecimal(cmd.Parameters["GROUP_ID"].Value);
                Int32 group_id = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Parameters.Clear();
                //se almacenan los screen y sus acciones correspondientes al perfil señalado
                //---------------------------------------------------------------------
                //decimal id_screen;
                ArrayList arreglo = new ArrayList();
                foreach (var item in testObj)
                {
                    cmd.Parameters.Clear();
                    Dictionary<string, object> Nodo = (Dictionary<string, object>)item;
                    query = "INSERT INTO WEB_SCREEN(LINK,GROUP_ID,STATUS,FECHA,VISTA_POR_DEFECTO) VALUES('" + Nodo["link"] + "',@group_id,'" + Nodo["status"] + "',GETDATE(), @vista_por_defecto) SELECT CAST(scope_identity() AS int) ";

                    if (Nodo["link"].ToString() == "/dashboard")
                    {
                        cmd.Parameters.AddWithValue("vista_por_defecto", Nodo["vista_por_defecto"]);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("vista_por_defecto", "");
                    }
                    cmd.Parameters.AddWithValue("group_id", group_id);

                    //Creamos el comando 

                    //SqlParameter outputParameter2 = new SqlParameter("ID_SCREEN", SqlType.Int32);
                    //outputParameter2.Direction = ParameterDirection.Output;
                    //cmd.Parameters.Add(outputParameter2);
                    cmd.CommandText = query;
                    //cmd.ExecuteNonQuery();

                    //id_screen = Convert.ToDecimal(cmd.Parameters["ID_SCREEN"].Value);
                    int id_screen = Convert.ToInt32(cmd.ExecuteScalar());

                    object[] actObj = (object[])js.Deserialize(request.acciones, new object().GetType());

                    //se insertan las acciones asociadas al screen correspondiente
                    //object[] valor = (Object[])Nodo["acciones"];
                    foreach (var accion in actObj)
                    {
                        Dictionary<string, object> val = (Dictionary<string, object>)accion;
                        string link = Nodo["link"].ToString();
                        string linkaccion = val["link"].ToString();
                        if (link == linkaccion)
                        {
                            cmd.Parameters.Clear();
                            string query2 = "INSERT INTO WEB_SCREEN_ACTION(ID_SCREEN,ID_ACTION,FECHA) VALUES(@id_screen,'" + val["id_action"] + "',GETDATE())";
                            cmd.CommandText = query2;
                            cmd.Parameters.AddWithValue("id_screen", id_screen);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                transaccion.Commit();


                //TRACKING
                Tracking t = new Tracking();
                string queryIn = " SELECT g.name,g.description,s.link,case when s.status = 'true' then 'Activado' else 'Desactivado' end status ,a.name as nameaction FROM web_groups g" +
            " INNER JOIN web_screen s ON s.group_id = g.group_id" +
            " LEFT JOIN web_screen_action sa ON sa.id_screen = s.id_screen" +
            " LEFT JOIN web_action a ON a.id_action = sa.id_action" +
            " WHERE g.name is not null and g.group_id =" + group_id + " order by S.LINK asc,a.name asc";

                //GUARDAMOS EL TRACKING
                t.guardarTracking("Crear", "Perfiles", t.obtenerDatosNuevosyAntiguos(queryIn));





                return true;
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

    public object actualizar(perfilesRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        string query = "";
        query = " SELECT g.name,g.description,s.link,case when s.status = 'true' then 'Activado' else 'Desactivado' end status ,a.name as nameaction FROM web_groups g" +
            " INNER JOIN web_screen s ON s.group_id = g.group_id" +
            " LEFT JOIN web_screen_action sa ON sa.id_screen = s.id_screen" +
            " LEFT JOIN web_action a ON a.id_action = sa.id_action" +
            " WHERE g.name is not null and g.group_id =" + request.group_id + " order by S.LINK asc,a.name asc";
        //TRACKING
        Tracking t = new Tracking();
        string queryIn = query;
        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            query = " select NAME from  WEB_GROUPS where UPPER(NAME)=UPPER(@nombre) AND group_id!=@group_id";

            cmd.Parameters.AddWithValue("nombre", request.nombre);
            cmd.Parameters.AddWithValue("group_id", request.group_id);

            cmd.CommandText = query;

            SqlDataReader reader = cmd.ExecuteReader();
            int afectada = 0;
            while (reader.Read())
            {
                afectada = 1;
            }
            reader.Close();
            cmd.Parameters.Clear();
            if (afectada == 0)
            {
                decimal id_screen;
                //se deserealiza el objeto json nodos
                JavaScriptSerializer js = new JavaScriptSerializer();
                object[] testObj = (object[])js.Deserialize(request.nodos, new object().GetType());

                //se actualizan los datos del perfil
                query = "UPDATE web_groups SET name= @name , description= @description , USUARIO = @multiempresa,ADMINISTRADOR = @administrador,AUDITOR = @auditor , fecha_updated =  FORMAT(GETDATE() , 'dd/MM/yyyy HH:mm:ss') WHERE group_id= @group_id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("name", request.nombre);
                cmd.Parameters.AddWithValue("description", request.descripcion);
                cmd.Parameters.AddWithValue("multiempresa", request.multiempresa);
                cmd.Parameters.AddWithValue("administrador", request.administrador);
                cmd.Parameters.AddWithValue("auditor", request.auditor);
                cmd.Parameters.AddWithValue("group_id", request.group_id);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                //se recorren todos los nodos

                ArrayList arreglo = new ArrayList();
                foreach (var item in testObj)
                {
                    Dictionary<string, object> Nodo = (Dictionary<string, object>)item;
                    cmd.Parameters.Clear();
                    query = "SELECT id_screen FROM web_screen WHERE CAST(link as varchar(250)) ='" + Nodo["link"] + "' AND group_id= @group_id";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("group_id", request.group_id);
                    id_screen = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.Parameters.Clear();
                    if (id_screen == 0)
                    {
                        query = "INSERT INTO web_screen (LINK, GROUP_ID, STATUS, FECHA, vista_por_defecto)  " +
                                "VALUES ('" + Nodo["link"] + "',@group_id,'" + Nodo["status"] + "',(SELECT CONVERT (date, GETDATE()) ), @vista_por_defecto)";
                        cmd.CommandText = query;
                        if (Nodo["link"].ToString() == "/dashboard")
                        {
                            cmd.Parameters.AddWithValue("vista_por_defecto", Nodo["vista_por_defecto"]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("vista_por_defecto", "");
                        }

                        cmd.Parameters.AddWithValue("group_id", request.group_id);
                        id_screen = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        cmd.Parameters.Clear();
                        query = "UPDATE web_screen SET status='" + Nodo["status"] + "', fecha=(SELECT CONVERT (date, GETDATE()) ), vista_por_defecto= @vista_por_defecto WHERE id_screen= @id_screen ";
                        cmd.CommandText = query;
                        if (Nodo["link"].ToString() == "/dashboard")
                        {
                            cmd.Parameters.AddWithValue("vista_por_defecto", Nodo["vista_por_defecto"]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("vista_por_defecto", "");
                        }

                        cmd.Parameters.AddWithValue("id_screen", id_screen);
                        cmd.ExecuteNonQuery();
                    }


                    //'se borran las acciones asociadas al screen determinado
                    cmd.Parameters.Clear();
                    query = "DELETE FROM web_screen_action WHERE id_screen= @id_screen ";

                    cmd.Parameters.AddWithValue("id_screen", id_screen);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();


                    //se insertan las acciones asociadas al screen correspondiente
                    object[] actObj = (object[])js.Deserialize(request.acciones, new object().GetType());

                    //se insertan las acciones asociadas al screen correspondiente
                    //object[] valor = (Object[])Nodo["acciones"];
                    foreach (var accion in actObj)
                    {
                        Dictionary<string, object> val = (Dictionary<string, object>)accion;
                        string link = Nodo["link"].ToString();
                        string linkaccion = val["link"].ToString();
                        string estado = val["estadoPerfil"].ToString();
                        if (link == linkaccion && estado == "True")
                        {
                            cmd.Parameters.Clear();
                            string query2 = "INSERT INTO WEB_SCREEN_ACTION(ID_SCREEN,ID_ACTION,FECHA) VALUES(@id_screen,'" + val["id_action"] + "',GETDATE())";
                            cmd.CommandText = query2;
                            cmd.Parameters.AddWithValue("id_screen", id_screen);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                transaccion.Commit();

                //GUARDAMOS EL TRACKING
                t.guardarTracking("Editar", "Perfiles", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

                return true;
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


    public object acciones()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "SELECT a.id_action, a.name, a.link, wag.cmd " +
                            " FROM web_action a " +
                            " INNER JOIN web_accion_generica wag on wag.nombre = a.name ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            //cmd.Parameters.AddWithValue("link", link);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            List<ActionResponse> lista = new List<ActionResponse>();

            while (reader.Read())
            {
                ActionResponse obj = new ActionResponse();
                obj.id_action = reader["id_action"].ToString();
                obj.estado = reader["id_action"].ToString();
                obj.name = reader["name"].ToString();
                obj.link = reader["link"].ToString();
                obj.cmd = reader["cmd"].ToString();
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


    public object obtenerDatosPerfil(int group_id)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = "SELECT G.GROUP_ID, G.NAME, G.DESCRIPTION ,G.USUARIO, s.VISTA_POR_DEFECTO,G.ADMINISTRADOR,G.AUDITOR " +
                            " FROM WEB_GROUPS G " +
                            " INNER JOIN web_screen s on s.GROUP_ID = G.GROUP_ID " +
                            "WHERE G.GROUP_ID = @perfil and s.LINK = '/dashboard'";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("perfil", group_id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<perfilesRequest> lista = new List<perfilesRequest>();

            while (reader.Read())
            {
                perfilesRequest obj = new perfilesRequest();
                obj.group_id = reader["GROUP_ID"].ToString();
                obj.nombre = reader["NAME"].ToString();
                obj.descripcion = reader["DESCRIPTION"].ToString();
                obj.multiempresa = reader["USUARIO"].ToString();
                obj.administrador = reader["ADMINISTRADOR"].ToString();
                obj.auditor = reader["AUDITOR"].ToString();
                obj.vista_por_defecto = reader["VISTA_POR_DEFECTO"].ToString();
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

    public object obtenerScreen(int group_id)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = "SELECT S.ID_SCREEN , S.GROUP_ID , S.STATUS ,S.FECHA,S.LINK ,isnull(case when ws.id_branch is null then wb.id_branch else ws.id_branch end ,0)id_branch, ISNULL(WS.ID_SHEET,0)ID_SHEET FROM WEB_SCREEN S " +
                           " left join web_sheet ws on s.link = ws.link" +
                            " left join web_branch wb on wb.link = s.link " +
                           " WHERE S.GROUP_ID = @perfil ";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("perfil", group_id);
            ////'Seteamos los valores de la consulta

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<ScreenResponse> lista = new List<ScreenResponse>();

            while (reader.Read())
            {
                ScreenResponse obj = new ScreenResponse();
                obj.id_screen = reader["ID_SCREEN"].ToString();
                obj.link = reader["LINK"].ToString();
                obj.group_id = reader["GROUP_ID"].ToString();
                obj.status = reader["STATUS"].ToString();
                obj.fecha = reader["FECHA"].ToString();
                obj.id_branch = reader["ID_BRANCH"].ToString();
                obj.id_sheet = reader["ID_SHEET"].ToString();
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

    public object obtenerAccionesScreenPerfil(int group_id)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = "SELECT distinct a.id_action, a.name, a.link,group_id,sa.ID_ACTION, " +
                           " case when sa.ID_ACTION is null then 'false' else 'true' end estado, " +
                           " wag.cmd " +
                           " FROM web_action a " +
                           " left join web_screen s ON s.LINK = a.LINK  and s.GROUP_ID = @perfil " +
                           " left join web_screen_action sa on sa.ID_SCREEN = s.ID_SCREEN and sa.ID_ACTION = a.ID_ACTION " +
                           " INNER JOIN web_accion_generica wag on wag.nombre = a.name ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            ////'Seteamos los valores de la consulta
            //cmd.Parameters.AddWithValue("@start", request.inicio);
            //cmd.Parameters.AddWithValue("@end", request.final);
            cmd.Parameters.AddWithValue("perfil", group_id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<ActionResponse> lista = new List<ActionResponse>();

            while (reader.Read())
            {
                ActionResponse obj = new ActionResponse();
                obj.id_action = reader["id_action"].ToString();
                obj.estadoPerfil = Convert.ToBoolean(reader["estado"]);
                obj.name = reader["name"].ToString();
                obj.link = reader["link"].ToString();
                obj.cmd = reader["cmd"].ToString();
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


    public object eliminar(Datosperfiles id_perfil)
    {
        //creamos una nueva conección y la transaccion asociada
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        //TRACKING
        Tracking t = new Tracking();
        string queryIn = " SELECT g.name,g.description,s.link,case when s.status = 'true' then 'Activado' else 'Desactivado' end status ,a.name as nameaction FROM web_groups g" +
                    " INNER JOIN web_screen s ON s.group_id = g.group_id" +
                    " LEFT JOIN web_screen_action sa ON sa.id_screen = s.id_screen" +
                    " LEFT JOIN web_action a ON a.id_action = sa.id_action" +
                    " WHERE g.name is not null and g.group_id  in (" + id_perfil.group_id + ") order by S.LINK asc,a.name asc";


        string query = "  select * from web_groups wg " +
                       " inner join web_users wu on wu.GROUP_ID = wg.GROUP_ID where wg.GROUP_ID in (" + id_perfil.group_id + ") ";

        cmd.CommandText = query;
        cmd.ExecuteNonQuery();
        SqlDataReader reader = cmd.ExecuteReader();

        int encontrado = 0;
        int afectada = 0;
        while (reader.Read())
        {
            encontrado = 1;
        }
        reader.Close();
        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            if (encontrado == 0)
            {
                List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);
                query = "DELETE FROM WEB_SCREEN_ACTION WHERE ID_SCREEN IN (SELECT ID_SCREEN FROM WEB_SCREEN WHERE GROUP_ID in( " + id_perfil.group_id + " ) )";
                cmd.CommandText = query;
                //cmd.Parameters.AddWithValue("group_id", id_perfil);
                cmd.Parameters.Clear();

                query = "DELETE FROM WEB_SCREEN WHERE GROUP_ID in( " + id_perfil.group_id + ") ";
                cmd.CommandText = query;
                //cmd.Parameters.AddWithValue("group_id", id_perfil);
                cmd.Parameters.Clear();

                query = "DELETE FROM WEB_GROUPS WHERE GROUP_ID in(" + id_perfil.group_id + ")";
                cmd.CommandText = query;
                //cmd.Parameters.AddWithValue("group_id", id_perfil);
                afectada = cmd.ExecuteNonQuery();

                transaccion.Commit();

                //GUARDAMOS EL TRACKING
                t.guardarTracking("Eliminar", "Perfiles", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

                conn.Close();
            }

            if (afectada > 0)
            {
                return true;
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

}