using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class tipoDeudasModel : Conexion
{
    public object SelectCargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY a.id_deuda asc, a.nombre_deuda asc, nombre_plazo) as indice,
                             a.id_deuda,
                             a.nombre_deuda,
                             e.id_plazo,
                             e.nombre_plazo
                             FROM dev_tipo_deuda as a
                             INNER JOIN dev_tipo_plazo as e
                             ON e.id_plazo = a.id_tipo_plazo
                             GROUP BY
                             a.id_deuda,
                             a.nombre_deuda,
                             e.id_plazo,
                             e.nombre_plazo";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<tipoDeudasRequest> lista = new List<tipoDeudasRequest>();

            while (reader.Read())
            {
                tipoDeudasRequest obj = new tipoDeudasRequest();

                obj.idDeuda = Convert.ToInt32(reader["id_deuda"].ToString());
                obj.nombreDeuda = reader["nombre_deuda"].ToString();
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

    public bool comprobarCreacionNombre(tipoDeudasRequest datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();

        bool resultado = false;
        try
        {
            string query = @"SELECT COUNT(1) AS result FROM dev_tipo_deuda e WHERE e.nombre_deuda = @nombre_deuda";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("nombre_deuda", datos.nombreDeuda);
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



    public bool InsertDEUDA(tipoDeudasRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"INSERT INTO dev_tipo_deuda
                                    (nombre_deuda,
                                     id_tipo_plazo
                                    )
                                 VALUES
                                    (@nombre_deuda,
                                     @id_tipo_plazo)";

            cmd.Parameters.AddWithValue("nombre_deuda", request.nombreDeuda);
            cmd.Parameters.AddWithValue("id_tipo_plazo", request.idPlazo);
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

    public object eliminar(string idDeuda)
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
        string queryIn = "SELECT id_deuda ,nombre_deuda, id_tipo_plazo FROM dev_tipo_deuda where id_deuda  in (" + idDeuda + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_tipo_deuda WHERE id_deuda in (" + idDeuda + ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();


            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Deuda", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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




    public bool UpdateDEUDA(tipoDeudasRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_tipo_deuda
                            SET  nombre_deuda = @nombre_deuda
                            , id_tipo_plazo = @id_tipo_plazo    
                            WHERE id_deuda = @id_deuda";


            cmd.Parameters.AddWithValue("id_deuda", request.idDeuda);
            cmd.Parameters.AddWithValue("nombre_deuda", request.nombreDeuda);
            cmd.Parameters.AddWithValue("id_tipo_plazo", request.idPlazo);

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

    public object SelectCargarPlazoSelector()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT 
                                nombre_plazo as label,
                                id_plazo as value
                            FROM dev_tipo_plazo
                            group by id_plazo, nombre_plazo";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<SelectorPLAZO> lista = new List<SelectorPLAZO>();

            while (reader.Read())
            {
                SelectorPLAZO obj = new SelectorPLAZO();
                obj.label = reader["label"].ToString();
                obj.value = reader["value"].ToString();
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


    public object SelectCargarDEUDAS(tipoDeudasRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT
                                e.nombre_deuda,
                                m.nombre_plazo
                            FROM dev_tipo_activo as e
                            INNER JOIN dev_tipo_plazo as m
                            ON m.id_plazo = e.id_tipo_plazo
                            WHERE id_deuda = @id_deuda";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("id_deuda", request.idDeuda);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            tipoDeudasRequest obj = new tipoDeudasRequest();

            while (reader.Read())
            {
                obj.nombreDeuda = reader["nombre_deuda"].ToString();
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