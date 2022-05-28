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

public class constantesModel : Conexion
{
    public object cargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select id_constante, nombre, descripcion, valor,tipoDato, estado from web_constantes ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosConstantes> lista = new List<DatosConstantes>();

            while (reader.Read())
            {
                DatosConstantes obj = new DatosConstantes();
                obj.id_constante = reader["id_constante"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.descripcion = reader["descripcion"].ToString();
                obj.valor = reader["valor"].ToString();
                obj.tipoDato = reader["tipoDato"].ToString();
                obj.value = reader["nombre"].ToString();
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

    public object nombresCosntantes()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select  nombre as name , nombre as code from web_constantes ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosConstantes> lista = new List<DatosConstantes>();

            while (reader.Read())
            {
                DatosConstantes obj = new DatosConstantes();

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
    
    public object editar(int id_constante)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "select id_constante, nombre, descripcion, valor,tipoDato ,estado from web_constantes where id_constante = " + id_constante + " ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosConstantes> lista = new List<DatosConstantes>();

            while (reader.Read())
            {
                DatosConstantes obj = new DatosConstantes();
                obj.id_constante = reader["id_constante"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.descripcion = reader["descripcion"].ToString();
                obj.valor = reader["valor"].ToString();
                obj.tipoDato = reader["tipoDato"].ToString();
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
    public object eliminar(int id_constantes)
    {


        //string query = "delete from web_constantes where id_constante = '(" + datos.id_constantes + ")' ";
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            //string query = "delete from web_constantes where id_constante (:id_constante) ";
            
            //cmd.Parameters.AddWithValue("id_constante", id_constantes);
            //cmd.CommandText = query;
            //cmd.ExecuteNonQuery();
            //transaccion.Commit();


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
    public object guardar(constantesRequest datos)
    {
       

            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;

            try
            {

                string query = "insert into web_constantes (nombre, descripcion, valor,tipoDato,estado) values (@nombre , @descripcion , @valor, @tipo_dato , @estado ) ";
                cmd.Parameters.AddWithValue("nombre", datos.nombre);
                cmd.Parameters.AddWithValue("descripcion", datos.descripcion);
                cmd.Parameters.AddWithValue("valor", datos.valor);
                cmd.Parameters.AddWithValue("tipo_dato", datos.tipo_dato);
                cmd.Parameters.AddWithValue("estado", datos.estado);
                               
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
        
   
    public object actualizar(constantesRequest datos)
    {
       
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        
        //TRACKING
        Tracking t = new Tracking();
        string queryIn = "SELECT * FROM web_constantes where id_constante = " + datos.id_constante;
        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string,object>> antiguo = t.obtenerAntiguo(queryIn);

            string query = "update  web_constantes set nombre = @nombre , descripcion = @descripcion , valor = @valor , " +
                         "  tipoDato = @tipo_dato ,estado =  @estado " +
                          " where id_constante = @id_constante";

            cmd.Parameters.AddWithValue("nombre", datos.nombre);
            cmd.Parameters.AddWithValue("descripcion", datos.descripcion);
            cmd.Parameters.AddWithValue("valor", datos.valor);
            cmd.Parameters.AddWithValue("tipo_dato", datos.tipo_dato);
            cmd.Parameters.AddWithValue("estado", datos.estado);
            cmd.Parameters.AddWithValue("id_constante", datos.id_constante);
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            transaccion.Commit();

            
            //GUARDAMOS EL TRACKING
            t.guardarTracking("Editar", "Constantes", t.obtenerDatosNuevosyAntiguos(queryIn,antiguo));
            
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
    public object habilitarDeshabilitar(constantesRequest datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {

            string query = "UPDATE web_constantes SET ESTADO  = @estado where id_constante  in (@id_constante) ";
            cmd.Parameters.AddWithValue("estado", datos.estado);
            cmd.Parameters.AddWithValue("id_constante", datos.id_constante);
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


    public List<DatosConstantes> generarExcel(constantesRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = "SELECT c.id_constante, c.nombre,c.descripcion,c.valor,c.tipoDato from web_constantes c ";
            if (request.id_constante != null && request.id_constante != "")
            {
                query += "where c.id_constante = @id_constante ";
            }


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            if (request.id_constante != null && request.id_constante != "")
            {
                cmd.Parameters.AddWithValue("id_constante", request.id_constante);
            }

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<DatosConstantes> lista = new List<DatosConstantes>();

            while (reader.Read())
            {
                DatosConstantes obj = new DatosConstantes();
                obj.id_constante = reader["id_constante"].ToString();
                obj.nombre = reader["nombre"].ToString();
                obj.descripcion = reader["descripcion"].ToString();
                obj.valor = reader["valor"].ToString();
                obj.tipoDato = reader["tipoDato"].ToString();
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