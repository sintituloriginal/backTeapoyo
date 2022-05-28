using System;
using System.Collections.Generic;
using System.Data.SqlClient;

public class bancoModel : Conexion
{
    public object SelectCargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY b.id_banco asc, b.nombre_banco asc) as indice,
                             b.id_banco,
                             b.nombre_banco
                             FROM dev_banco as b
                             GROUP BY
                             b.id_banco,
                             b.nombre_banco";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<tipoBancoRequest> lista = new List<tipoBancoRequest>();

            while (reader.Read())
            {
                tipoBancoRequest obj = new tipoBancoRequest();

                obj.idBanco = Convert.ToInt32(reader["id_banco"].ToString());
                obj.nombreBanco = reader["nombre_banco"].ToString();
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

    public bool comprobarCreacionNombre(tipoBancoRequest datos)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();

        bool resultado = false;
        try
        {
            string query = @"SELECT COUNT(1) AS result FROM dev_banco b WHERE b.nombre_banco = @nombre_banco";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("nombre_banco", datos.nombreBanco);
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



    public bool InsertBANCO(tipoBancoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"INSERT INTO dev_banco
                                    (nombre_banco
                                    )
                                 VALUES
                                    (@nombre_banco)";

            cmd.Parameters.AddWithValue("nombre_banco", request.nombreBanco);
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

    public object eliminar(string idBanco)
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
        string queryIn = "SELECT id_banco ,nombre_banco FROM dev_banco where id_banco  in (" + idBanco + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_banco WHERE id_banco in (" + idBanco + ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();


            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Banco", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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




    public bool UpdateBANCO(tipoBancoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_banco
                            SET  nombre_banco = @nombre_banco
                            WHERE id_banco = @id_banco";


            cmd.Parameters.AddWithValue("nombre_banco", request.nombreBanco);
            cmd.Parameters.AddWithValue("id_banco", request.idBanco);

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