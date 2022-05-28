using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;

public class ingresoModel : Conexion
{

    public object SelectCargarTotal()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"SELECT                       
                             ISNULL(abs(SUM(monto)),0) AS TOTAL
                             FROM dev_gestion_ingreso as i
                             where i.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<ingresoRequest> lista = new List<ingresoRequest>();

            while (reader.Read())
            {
                ingresoRequest obj = new ingresoRequest();

                obj.monto = Convert.ToInt32(reader["TOTAL"]);
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

    public object SelectCargarTotalAdmin(ingresoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT                       
                             ISNULL(abs(SUM(monto)),0) AS TOTAL
                             FROM dev_gestion_ingreso as i
                             INNER JOIN web_users as w
                             ON w.USER_ID = i.USER_ID
                             WHERE 1=1";

            if (request.fechaNacimiento != null)
            {
                query += @" AND FECHA_NACIMIENTO = @FECHA_NACIMIENTO";
            }
            if (request.sexo != null)
            {
                query += @" AND SEXO = @SEXO";
            }
            SqlCommand cmd = new SqlCommand(query);

            if (request.fechaNacimiento != null)
            {
                cmd.Parameters.AddWithValue("FECHA_NACIMIENTO", request.fechaNacimiento);
            }

            if (request.sexo != null)
            {
                cmd.Parameters.AddWithValue("SEXO", request.sexo);
            }
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<ingresoRequest> lista = new List<ingresoRequest>();

            while (reader.Read())
            {
                ingresoRequest obj = new ingresoRequest();

                obj.monto = Convert.ToInt32(reader["TOTAL"]);
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


    public object SelectCargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY i.id_ingreso asc, b.nombre_ingreso asc, i.monto asc, i.descripcion asc, i.fecha_creacion asc) as indice,
                             i.id_ingreso,
                             b.nombre_ingreso,
                             REPLACE(CONVERT(VARCHAR,CAST(i.monto as money),3),'.00','') as monto,
                             i.descripcion,
                             FORMAT(i.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_gestion_ingreso as i
                             INNER JOIN dev_tipo_ingreso as b
                             ON b.id_tipo_ingreso = i.id_tipo_ingreso
                             where i.user_id = @user_id
                             GROUP BY
                             i.id_ingreso,
                             b.nombre_ingreso,
                             i.monto,
                             i.descripcion,
                             i.fecha_creacion";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<ingresoRequest> lista = new List<ingresoRequest>();

            while (reader.Read())
            {
                ingresoRequest obj = new ingresoRequest();

                obj.idIngreso = Convert.ToInt32(reader["id_ingreso"].ToString());
                obj.nombreTipoIngreso = reader["nombre_ingreso"].ToString();
                obj.montoCarga = "$" + reader["monto"].ToString();
                obj.descripcion = reader["descripcion"].ToString();
                obj.fechaCreacion = reader["fecha_creacion"].ToString();
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

    public object FiltrarTabla(ingresoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY i.id_ingreso asc, b.nombre_ingreso asc, i.monto asc, i.descripcion asc, i.fecha_creacion asc) as indice,
                             i.id_ingreso,
							 w.FULLNAME,
							 w.UserName,
                             b.nombre_ingreso,
                             REPLACE(CONVERT(VARCHAR,CAST(i.monto as money),3),'.00','') as monto,
                             i.descripcion,
                             FORMAT(i.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_gestion_ingreso as i
                             INNER JOIN dev_tipo_ingreso as b
                             ON b.id_tipo_ingreso = i.id_tipo_ingreso
							 INNER JOIN web_users as w 
							 ON w.USER_ID = i.USER_ID
                             WHERE 1 = 1";

            if(request.fechaNacimiento != null)
            {
                query += @" AND w.FECHA_NACIMIENTO = @FECHA_NACIMIENTO";
            }

            if(request.sexo != null)
            {
                query += @" AND w.SEXO = @SEXO";
            }

                   query +=  @" GROUP BY
                             i.id_ingreso,
							 w.FULLNAME,
							 w.UserName,
                             b.nombre_ingreso,
                             i.monto,
                             i.descripcion,
                             i.fecha_creacion";



            SqlCommand cmd = new SqlCommand(query);

            if (request.fechaNacimiento != null)
            {
                cmd.Parameters.AddWithValue("FECHA_NACIMIENTO", request.fechaNacimiento);
            }
            if (request.sexo != null)
            {
                cmd.Parameters.AddWithValue("SEXO", request.sexo);
            }
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<ingresoRequest> lista = new List<ingresoRequest>();

            while (reader.Read())
            {
                ingresoRequest obj = new ingresoRequest();

                obj.idIngreso = Convert.ToInt32(reader["id_ingreso"].ToString());
                obj.nombreTipoIngreso = reader["nombre_ingreso"].ToString();
                obj.username = reader["UserName"].ToString();
                obj.fullname = reader["FULLNAME"].ToString();
                obj.montoCarga = "$" + reader["monto"].ToString();
                obj.descripcion = reader["descripcion"].ToString();
                obj.fechaCreacion = reader["fecha_creacion"].ToString();
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

    public object SelectCargarDatosAdmin()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY i.id_ingreso asc, b.nombre_ingreso asc, i.monto asc, i.descripcion asc, i.fecha_creacion asc) as indice,
                             i.id_ingreso,
							 w.FULLNAME,
							 w.UserName,
                             b.nombre_ingreso,
                             REPLACE(CONVERT(VARCHAR,CAST(i.monto as money),3),'.00','') as monto,
                             i.descripcion,
                             FORMAT(i.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_gestion_ingreso as i
                             INNER JOIN dev_tipo_ingreso as b
                             ON b.id_tipo_ingreso = i.id_tipo_ingreso
							 INNER JOIN web_users as w 
							 ON w.USER_ID = i.USER_ID
                             GROUP BY
                             i.id_ingreso,
							 w.FULLNAME,
							 w.UserName,
                             b.nombre_ingreso,
                             i.monto,
                             i.descripcion,
                             i.fecha_creacion";



            SqlCommand cmd = new SqlCommand(query); 
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<ingresoRequest> lista = new List<ingresoRequest>();

            while (reader.Read())
            {
                ingresoRequest obj = new ingresoRequest();

                obj.idIngreso = Convert.ToInt32(reader["id_ingreso"].ToString());
                obj.nombreTipoIngreso = reader["nombre_ingreso"].ToString();
                obj.username = reader["UserName"].ToString();
                obj.fullname = reader["FULLNAME"].ToString();
                obj.montoCarga = "$" + reader["monto"].ToString();
                obj.descripcion = reader["descripcion"].ToString();
                obj.fechaCreacion = reader["fecha_creacion"].ToString();
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


    public bool InsertINGRESO(ingresoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"INSERT INTO dev_gestion_ingreso
                                    (id_tipo_ingreso,
                                     monto,
                                     user_id,
                                     descripcion,
                                     fecha_creacion
                                    )
                                 VALUES
                                    (@id_tipo_ingreso,
                                     @monto,
                                     @user_id,
                                     @descripcion,
                                     GETDATE())";

            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Parameters.AddWithValue("id_tipo_ingreso", request.idTipoIngreso);
            cmd.Parameters.AddWithValue("monto", request.monto);
            cmd.Parameters.AddWithValue("descripcion", request.descripcion);
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
        string queryIn = "SELECT id_tipo_ingreso ,id_tipo_ingreso, monto, user_id FROM dev_gestion_ingreso where id_ingreso  in (" + idIngreso + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_gestion_ingreso WHERE id_ingreso in (" + idIngreso + ")";
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




    public bool UpdateINGRESO(ingresoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_gestion_ingreso
                            SET  monto = @monto
                            ,descripcion = @descripcion  
                            WHERE id_ingreso = @id_ingreso";

            cmd.Parameters.AddWithValue("id_ingreso", request.idIngreso);
            cmd.Parameters.AddWithValue("monto", request.monto);
            cmd.Parameters.AddWithValue("descripcion", request.descripcion);

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

    public object SelectCargarIngresoSelector()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT 
                                nombre_ingreso as label,
                                id_tipo_ingreso as value
                            FROM dev_tipo_ingreso
                            group by id_tipo_ingreso, nombre_ingreso";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<SelectorINGRESO> lista = new List<SelectorINGRESO>();

            while (reader.Read())
            {
                SelectorINGRESO obj = new SelectorINGRESO();
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



    public object SelectCargarINGRESO(ingresoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT
                                m.descripcion,
                                m.monto,
                            FROM dev_gestion_ingreso as m
                            WHERE id_ingreso = @id_ingreso";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("id_ingreso", request.idIngreso);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            ingresoRequest obj = new ingresoRequest();

            while (reader.Read())
            {
                obj.descripcion = reader["descripcion"].ToString();
                obj.monto= Convert.ToInt32(reader["monto"].ToString());
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