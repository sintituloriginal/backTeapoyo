using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class tipoIngresoModel : Conexion
{
    public object SelectCargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY a.id_tipo_ingreso asc, a.nombre_ingreso asc) as indice,
                             a.id_tipo_ingreso,
                             a.nombre_ingreso
                             FROM dev_tipo_ingreso as a
                             GROUP BY
                             a.id_tipo_ingreso,
                             a.nombre_ingreso";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<tipoIngresoRequest> lista = new List<tipoIngresoRequest>();

            while (reader.Read())
            {
                tipoIngresoRequest obj = new tipoIngresoRequest();

                obj.idIngreso = Convert.ToInt32(reader["id_tipo_ingreso"].ToString());
                obj.nombreIngreso = reader["nombre_ingreso"].ToString();
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

    public bool comprobarCreacionNombre(tipoIngresoRequest datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();

        bool resultado = false;
        try
        {
            string query = @"SELECT COUNT(1) AS result FROM dev_tipo_ingreso e WHERE e.nombre_ingreso = @nombre_ingreso";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("nombre_ingreso", datos.nombreIngreso);
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





    public bool InsertINGRESO(tipoIngresoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"INSERT INTO dev_tipo_ingreso
                                    (nombre_ingreso
                                    )
                                 VALUES
                                    (@nombre_ingreso)";

            cmd.Parameters.AddWithValue("nombre_ingreso", request.nombreIngreso);
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

    public object eliminar(string idIngreso)
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
        string queryIn = "SELECT id_tipo_ingreso ,nombre_ingreso FROM dev_tipo_ingreso where id_tipo_ingreso  in (" + idIngreso + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_tipo_ingreso WHERE id_tipo_ingreso in (" + idIngreso + ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();


            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Ingreso", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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




    public bool UpdateINGRESO(tipoIngresoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_tipo_ingreso
                            SET  nombre_ingreso = @nombre_ingreso
                            WHERE id_tipo_ingreso = @id_tipo_ingreso";


            cmd.Parameters.AddWithValue("nombre_ingreso", request.nombreIngreso);
            cmd.Parameters.AddWithValue("id_tipo_ingreso", request.idIngreso);

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