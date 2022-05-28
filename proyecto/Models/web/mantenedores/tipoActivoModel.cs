using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class tipoActivoModel : Conexion
{
    public object SelectCargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY a.id_activo asc, a.nombre_activo asc) as indice,
                             a.id_activo,
                             a.nombre_activo
                             FROM dev_tipo_activo as a
                             WHERE a.nombre_activo <> ''
                             GROUP BY
                             a.id_activo,
                             a.nombre_activo";
                            


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<tipoActivoRequest> lista = new List<tipoActivoRequest>();

            while (reader.Read())
            {
                tipoActivoRequest obj = new tipoActivoRequest();

                obj.idActivo = Convert.ToInt32(reader["id_activo"].ToString());
                obj.nombreActivo = reader["nombre_activo"].ToString();
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

    public bool comprobarCreacionNombre(tipoActivoRequest datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();

        bool resultado = false;
        try
        {
            string query = @"SELECT COUNT(1) AS result FROM dev_tipo_activo e WHERE e.nombre_activo = @nombre_activo";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("nombre_activo", datos.nombreActivo);
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

   

    public bool InsertACTIVO(tipoActivoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"INSERT INTO dev_tipo_activo
                                    (nombre_activo
                                    )
                                 VALUES
                                    (@nombre_activo)";

            cmd.Parameters.AddWithValue("nombre_activo", request.nombreActivo);
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

    public object eliminar(string idActivo)
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
        string queryIn = "SELECT id_activo ,nombre_activo FROM dev_tipo_activo where id_activo  in (" + idActivo + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_tipo_activo WHERE id_activo in (" + idActivo + ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();


            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Activo", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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




    public bool UpdateACTIVO(tipoActivoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_tipo_activo
                            SET  nombre_activo = @nombre_activo
                            WHERE id_activo = @id_activo";


            cmd.Parameters.AddWithValue("nombre_activo", request.nombreActivo);
            cmd.Parameters.AddWithValue("id_activo", request.idActivo);

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

    public object SelectCargarACTIVO(tipoActivoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT
                                e.nombre_activo,
                            FROM dev_tipo_activo as e
                            WHERE id_activo = @id_activo";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("id_activo", request.idActivo);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            tipoActivoRequest obj = new tipoActivoRequest();

            while (reader.Read())
            {
                obj.nombreActivo = reader["nombre_activo"].ToString();
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