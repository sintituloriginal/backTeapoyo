using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class tipoPlazoModel : Conexion
{
    public object SelectCargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY a.id_plazo asc, a.nombre_plazo asc) as indice,
                             a.id_plazo,
                             a.nombre_plazo
                             FROM dev_tipo_plazo as a
                             GROUP BY
                             a.id_plazo,
                             a.nombre_plazo";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<tipoPlazoRequest> lista = new List<tipoPlazoRequest>();

            while (reader.Read())
            {
                tipoPlazoRequest obj = new tipoPlazoRequest();

                obj.idPlazo = Convert.ToInt32(reader["id_plazo"].ToString());
                obj.nombrePlazo = reader["nombre_plazo"].ToString();
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

    public bool comprobarCreacionNombre(tipoPlazoRequest datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();

        bool resultado = false;
        try
        {
            string query = @"SELECT COUNT(1) AS result FROM dev_tipo_plazo e WHERE e.nombre_plazo = @nombre_plazo";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("nombre_plazo", datos.nombrePlazo);
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



    public bool InsertPLAZO(tipoPlazoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"INSERT INTO dev_tipo_plazo
                                    (nombre_plazo
                                    )
                                 VALUES
                                    (@nombre_plazo)";

            cmd.Parameters.AddWithValue("nombre_plazo", request.nombrePlazo);
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

    public object eliminar(string idPlazo)
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
        string queryIn = "SELECT id_plazo ,nombre_plazo FROM dev_tipo_plazo where id_plazo  in (" + idPlazo + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_tipo_plazo WHERE id_plazo in (" + idPlazo + ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();


            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Plazo", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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




    public bool UpdatePLAZO(tipoPlazoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_tipo_plazo
                            SET  nombre_plazo = @nombre_plazo
                            WHERE id_plazo = @id_plazo";


            cmd.Parameters.AddWithValue("nombre_plazo", request.nombrePlazo);
            cmd.Parameters.AddWithValue("id_plazo", request.idPlazo);

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

    public object SelectCargarPLAZO(tipoPlazoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT
                                e.nombre_plazo,
                            FROM dev_tipo_plazo as e
                            WHERE id_plazo = @id_plazo";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("id_plazo", request.idPlazo);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            tipoPlazoRequest obj = new tipoPlazoRequest();

            while (reader.Read())
            {
                obj.nombrePlazo = reader["nombre_plazo"].ToString();
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