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

public class areaModel : Conexion
{
    public object cargarDatos(string id_empresa)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = @"SELECT da.ID_AREA,da.NOMBRE,da.CODAREA,wu.FULLNAME,da.ESTADO ,da.RESPONSABLE FROM dev_area da  
                            inner join web_users wu on wu.USER_ID = da.RESPONSABLE  
                            where da.id_empresa = "+id_empresa;
           
            
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosArea> lista = new List<DatosArea>();

            while (reader.Read())
            {
                DatosArea obj = new DatosArea();
                obj.ID_AREA = (int)reader["ID_AREA"];
                obj.NOMBRE = reader["NOMBRE"].ToString();
                obj.CODAREA = reader["CODAREA"].ToString();
                obj.nombreUsuario = reader["FULLNAME"].ToString();
                obj.ESTADO = reader["ESTADO"].ToString();
                obj.RESPONSABLE = reader["RESPONSABLE"].ToString();

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
    public object cargarUsuarios()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select USER_ID as id ,FULLNAME +' (' +Email+')' as text from web_users";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<selectorArea> lista = new List<selectorArea>();

            while (reader.Read())
            {
                selectorArea obj = new selectorArea();
                obj.text = reader["text"].ToString();
                obj.id = reader["id"].ToString();
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
    public bool existe(areaRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = "SELECT NOMBRE FROM dev_area where UPPER(CODAREA) = @codarea and id_empresa = @id_empresa ";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("codarea", request.CODAREA.ToUpper());
            cmd.Parameters.AddWithValue("id_empresa", request.ID_EMPRESA);
            //cmd.Parameters.AddWithValue("nombre", request.empresa.ToUpper());

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            bool existe = false;

            while (reader.Read())
            {
                existe = true;
            }
            reader.Close();
            return existe;
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
    public bool areaEnUso(string ID)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = "SELECT user_id FROM dev_user_area  where UPPER(area) = @area ";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("area", ID);
            //cmd.Parameters.AddWithValue("nombre", request.empresa.ToUpper());

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            bool existe = false;

            while (reader.Read())
            {
                existe = true;
            }
            reader.Close();
            return existe;
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
    public bool crear(areaRequest request)
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
            //se almacenan los datos del usuario en la tabla user
            //---------------------------------------------------------------------

            query = "INSERT INTO dev_area (NOMBRE, CODAREA,RESPONSABLE, ESTADO ,ID_EMPRESA) " +
                    " VALUES (UPPER(@nombre),UPPER(@abreviatura),@user_id,@estado,@id_empresa) ";
            

            cmd.Parameters.AddWithValue("nombre", request.NOMBRE == null ? "" : request.NOMBRE);
            cmd.Parameters.AddWithValue("abreviatura", request.CODAREA == null ? "" : request.CODAREA);
            cmd.Parameters.AddWithValue("user_id", request.RESPONSABLE == null ? "" : request.RESPONSABLE);
            cmd.Parameters.AddWithValue("estado", request.ESTADO == null ? "" : request.ESTADO);
            cmd.Parameters.AddWithValue("id_empresa", request.ID_EMPRESA);
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            //query = " INSERT INTO DEV_DURACION_ETAPAS (ID_EMPRESA,ETAPA,DURACION) " +
            //        " (SELECT '" + ID_EMPRESA + "', etapa, '0' FROM DEV_ETAPAS_CSC) ";

            //cmd.CommandText = query;
            //cmd.ExecuteNonQuery();

            transaccion.Commit();
            //TRACKING
            Tracking t = new Tracking();
            string queryIn = "SELECT da.NOMBRE,da.CODAREA, da.RESPONSABLE ,da.ESTADO  FROM dev_area da  where da.NOMBRE = '" + request.NOMBRE + "'";
            //GUARDAMOS EL TRACKING
            t.guardarTracking("Crear", "Área", t.obtenerDatosNuevosyAntiguos(queryIn));
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

    public object cargarDatosEditar(areaRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        try
        {

            string query = "SELECT da.NOMBRE,da.CODAREA, da.RESPONSABLE ,da.ESTADO  FROM dev_area da " +
                          " where da.NOMBRE = @area";

          
            cmd.Parameters.AddWithValue("area", request.NOMBRE);
          

            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosArea> lista = new List<DatosArea>();

            while (reader.Read())
            {
                DatosArea obj = new DatosArea();

                obj.NOMBRE = reader["NOMBRE"].ToString();
                obj.CODAREA = reader["CODAREA"].ToString();
                obj.RESPONSABLE = reader["RESPONSABLE"].ToString();
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
    public bool actualizar(areaRequest request)
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
        string queryIn = "SELECT da.NOMBRE,da.CODAREA, da.RESPONSABLE ,da.ESTADO  FROM dev_area da  where da.ID_AREA = '" + request.ID_AREA + "'";
        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " UPDATE dev_area SET  CODAREA =UPPER(@CODAREA),NOMBRE=@NOMBRE, RESPONSABLE =@RESPONSABLE, ESTADO =@ESTADO  " +
                    " WHERE ID_AREA=@ID ";
            cmd.CommandText = query;

            
            cmd.Parameters.AddWithValue("NOMBRE", request.NOMBRE == null ? "" : request.NOMBRE);
            cmd.Parameters.AddWithValue("CODAREA", request.CODAREA == null ? "" : request.CODAREA);
            cmd.Parameters.AddWithValue("RESPONSABLE", request.RESPONSABLE == null ? "" : request.RESPONSABLE);
            cmd.Parameters.AddWithValue("ESTADO", request.ESTADO == null ? "" : request.ESTADO);
            cmd.Parameters.AddWithValue("ID", request.ID_AREA);


            cmd.ExecuteNonQuery();

            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Editar", "Área", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

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
    public bool eliminar(string ID)
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
        string queryIn = "SELECT da.NOMBRE,da.CODAREA, da.RESPONSABLE ,da.ESTADO  FROM dev_area da  where da.ID_AREA = '" + ID + "'";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_area WHERE ID_AREA = @ID_AREA ";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("ID_AREA", ID);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Area", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
            return true;
        }
        catch (SqlException ex)
        {
            String mensage = ex.Message.ToUpper();
            String deleteErrorES = "THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT";
            String deleteErrorEN = "THE DELETE STATEMENT CONFLICTED WITH THE REFERENCE CONSTRAINT";
            //EL AREA NO PUEDE SER ELIMINADA, YA QUE, ESTA SIENDO UTILIZADA EN EL SISTEMA.
            if (mensage.Contains(deleteErrorES) || mensage.Contains(deleteErrorEN))
            {
                transaccion.Rollback();
                return false;
            }
            transaccion.Rollback();
            throw new Exception(ex.Message);
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
    public object habilitarDeshabilitar(habilitarDeshabilitarArea request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        //TRACKING
        Tracking t = new Tracking();
        
        string queryIn = "SELECT da.NOMBRE,da.CODAREA, da.RESPONSABLE ,da.ESTADO  FROM dev_area da  where da.ID_AREA in (" + request.ID + ")";
        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query = "UPDATE dev_area SET estado  = '" + request.ESTADO + "' where ID_AREA  in (" + request.ID + ") ";
            
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            transaccion.Commit();

            var accion = request.ESTADO == "Habilitado" ? "Habilitar" : "Deshabilitar";
            //GUARDAMOS EL TRACKING
            t.guardarTracking(accion, "Area", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

            return "ok";
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