using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class valorizacionModel : Conexion
{
    public object SelectCargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY a.id_valorizacion asc, a.nombre_valorizacion asc) as indice,
                             a.id_valorizacion,
                             a.nombre_valorizacion
                             FROM dev_tipo_valorizacion as a
                             GROUP BY
                             a.id_valorizacion,
                             a.nombre_valorizacion";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<tipoValorizacionRequest> lista = new List<tipoValorizacionRequest>();

            while (reader.Read())
            {
                tipoValorizacionRequest obj = new tipoValorizacionRequest();

                obj.idValorizacion = Convert.ToInt32(reader["id_valorizacion"].ToString());
                obj.nombreValorizacion = reader["nombre_valorizacion"].ToString();
                obj.indice = reader["indice"].ToString();
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

    public bool comprobarCreacionNombre(tipoValorizacionRequest datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();

        bool resultado = false;
        try
        {
            string query = @"SELECT COUNT(1) AS result FROM dev_tipo_valorizacion e WHERE e.nombre_valorizacion = @nombre_valorizacion";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("nombre_valorizacion", datos.nombreValorizacion);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (Int32.Parse(reader["result"].ToString()) > 0)
                {
                    resultado = true;
                }
            }
            reader.Close();
            return resultado;

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



    public bool InsertVALORIZACION(tipoValorizacionRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"INSERT INTO dev_tipo_valorizacion
                                    (nombre_valorizacion
                                    )
                                 VALUES
                                    (@nombre_valorizacion)";

            cmd.Parameters.AddWithValue("nombre_valorizacion", request.nombreValorizacion);
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

    public object eliminar(string idValorizacion)
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
        string queryIn = "SELECT id_valorizacion ,nombre_valorizacion FROM dev_tipo_valorizacion where id_valorizacion in (" + idValorizacion + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_tipo_valorizacion WHERE id_valorizacion in (" + idValorizacion + ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();


            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Valorizacion", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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




    public bool UpdateVALORIZACION(tipoValorizacionRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_tipo_valorizacion
                            SET  nombre_valorizacion = @nombre_valorizacion
                            WHERE id_valorizacion = @id_valorizacion";


            cmd.Parameters.AddWithValue("nombre_valorizacion", request.nombreValorizacion);
            cmd.Parameters.AddWithValue("id_valorizacion", request.idValorizacion);

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

    public object SelectCargarVALORIZACION(tipoValorizacionRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT
                                e.nombre_valorizacion,
                            FROM dev_tipo_valorizacion as e
                            WHERE id_valorizacion = @id_valorizacion";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("id_valorizacion", request.idValorizacion);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            tipoValorizacionRequest obj = new tipoValorizacionRequest();

            while (reader.Read())
            {
                obj.nombreValorizacion = reader["nombre_valorizacion"].ToString();
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