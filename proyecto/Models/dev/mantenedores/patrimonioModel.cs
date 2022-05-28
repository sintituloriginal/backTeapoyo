using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;

public class patrimonioModel : Conexion
{

    public object SelectCargarTotal()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"select SUM(CAST(p.total AS BIGINT)) as total from dev_patrimonio as p
                             where p.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<patrimonioRequest> lista = new List<patrimonioRequest>();

            while (reader.Read())
            {
                patrimonioRequest obj = new patrimonioRequest();

                obj.totalGeneral= reader["TOTAL"].ToString();
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

    public object SelectCargarTotalAdmin(patrimonioRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();

        try
        {
            string query = @"select SUM(CAST(p.total AS BIGINT)) as total 
                             from dev_patrimonio as p
                             INNER JOIN web_users as w
                             ON w.USER_ID = p.USER_ID
                             where 1=1";

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

            List<patrimonioRequest> lista = new List<patrimonioRequest>();

            while (reader.Read())
            {
                patrimonioRequest obj = new patrimonioRequest();

                obj.totalGeneral = reader["TOTAL"].ToString();
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

    public object SelectCargarDatosAdministrador()
    {
        SqlConnection conn = dbproyecto.openConnection();
        
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY p.id_patrimonio asc,  p.nombre_item asc, a.nombre_activo asc, p.monto_activo asc, v.nombre_valorizacion asc, d.nombre_deuda asc, p.total asc, b.nombre_banco asc, p.fecha_creacion asc) as indice,
                             p.id_patrimonio,
							 w.FULLNAME,
							 w.UserName,
                             p.nombre_item,
                             a.nombre_activo,
                            REPLACE(CONVERT(VARCHAR,CAST(p.monto_activo as money),3),'.00','') as monto_activo,
                             v.nombre_valorizacion,
                             d.nombre_deuda,
                             b.nombre_banco,
                              REPLACE(CONVERT(VARCHAR,CAST(p.total as money),3),'.00','') as total,
                             FORMAT(p.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_patrimonio as p
                             INNER JOIN dev_tipo_activo as a
                             ON p.id_tipo_activo = a.id_activo
                             INNER JOIN dev_tipo_valorizacion as v
                             ON p.id_valorizacion = v.id_valorizacion
                             INNER JOIN dev_tipo_deuda as d
                             ON p.id_deuda = d.id_deuda
							 INNER JOIN web_users as w 
							 ON w.USER_ID = p.USER_ID
                             LEFT JOIN dev_banco as b
                             ON p.id_banco = b.id_banco
                             GROUP BY
                             p.id_patrimonio,
							 w.FULLNAME,
							 W.UserName,
                             p.nombre_item,
                             a.nombre_activo,
                             p.monto_activo,
                             v.nombre_valorizacion,
                             d.nombre_deuda,
                             b.nombre_banco,
                             p.total,
                             p.fecha_creacion";



            SqlCommand cmd = new SqlCommand(query);     
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<patrimonioRequest> lista = new List<patrimonioRequest>();

            while (reader.Read())
            {
                patrimonioRequest obj = new patrimonioRequest();

                obj.idPatrimonio = Convert.ToInt32(reader["id_patrimonio"].ToString());
                obj.nombreItem = reader["nombre_item"].ToString();
                obj.nombreActivo = reader["nombre_activo"].ToString();
                obj.montoActivo = '$' + reader["monto_activo"].ToString();
                obj.nombreValorizacion = reader["nombre_valorizacion"].ToString();
                obj.nobmreDeuda = reader["nombre_deuda"].ToString();
                obj.nombreBanco = reader["nombre_banco"].ToString();
                obj.total = '$' + reader["total"].ToString();
                obj.fechaCreacion = reader["fecha_creacion"].ToString();
                obj.fullname = reader["FULLNAME"].ToString();
                obj.username = reader["UserName"].ToString();
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


    public object filtrarNombreCarga(patrimonioRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();

        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY p.id_patrimonio asc,  p.nombre_item asc, a.nombre_activo asc, p.monto_activo asc, v.nombre_valorizacion asc, d.nombre_deuda asc, p.total asc, b.nombre_banco asc, p.fecha_creacion asc) as indice,
                             p.id_patrimonio,
							 w.FULLNAME,
							 w.UserName,
                             p.nombre_item,
                             a.nombre_activo,
                            REPLACE(CONVERT(VARCHAR,CAST(p.monto_activo as money),3),'.00','') as monto_activo,
                             v.nombre_valorizacion,
                             d.nombre_deuda,
                             b.nombre_banco,
                              REPLACE(CONVERT(VARCHAR,CAST(p.total as money),3),'.00','') as total,
                             FORMAT(p.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_patrimonio as p
                             INNER JOIN dev_tipo_activo as a
                             ON p.id_tipo_activo = a.id_activo
                             INNER JOIN dev_tipo_valorizacion as v
                             ON p.id_valorizacion = v.id_valorizacion
                             INNER JOIN dev_tipo_deuda as d
                             ON p.id_deuda = d.id_deuda
							 INNER JOIN web_users as w 
							 ON w.USER_ID = p.USER_ID
                             LEFT JOIN dev_banco as b
                             ON p.id_banco = b.id_banco
                             where 1 = 1";
            if (request.fechaNacimiento != null)
            {
                query += @" AND w.FECHA_NACIMIENTO = @FECHA_NACIMIENTO";
            }
            if (request.sexo != null)
            {
                query += @" AND w.SEXO = @SEXO";
            }

            query +=  @" GROUP BY
                             p.id_patrimonio,
							 w.FULLNAME,
							 W.UserName,
                             p.nombre_item,
                             a.nombre_activo,
                             p.monto_activo,
                             v.nombre_valorizacion,
                             d.nombre_deuda,
                             b.nombre_banco,
                             p.total,
                             p.fecha_creacion";


            SqlCommand cmd = new SqlCommand(query);
            if(request.fechaNacimiento != null)
            {
                cmd.Parameters.AddWithValue("FECHA_NACIMIENTO", request.fechaNacimiento);
            }
            if(request.sexo != null)
            {
                cmd.Parameters.AddWithValue("SEXO", request.sexo);
            }
           

            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<patrimonioRequest> lista = new List<patrimonioRequest>();

            while (reader.Read())
            {
                patrimonioRequest obj = new patrimonioRequest();

                obj.idPatrimonio = Convert.ToInt32(reader["id_patrimonio"].ToString());
                obj.nombreItem = reader["nombre_item"].ToString();
                obj.nombreActivo = reader["nombre_activo"].ToString();
                obj.montoActivo = '$' + reader["monto_activo"].ToString();
                obj.nombreValorizacion = reader["nombre_valorizacion"].ToString();
                obj.nobmreDeuda = reader["nombre_deuda"].ToString();
                obj.nombreBanco = reader["nombre_banco"].ToString();
                obj.total = '$' + reader["total"].ToString();
                obj.fechaCreacion = reader["fecha_creacion"].ToString();
                obj.fullname = reader["FULLNAME"].ToString();
                obj.username = reader["UserName"].ToString();
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


    public object SelectCargarDatos()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY p.id_patrimonio asc,  p.nombre_item asc, a.nombre_activo asc, p.monto_activo asc, v.nombre_valorizacion asc, d.nombre_deuda asc, p.total asc, b.nombre_banco asc, p.fecha_creacion asc) as indice,
                             p.id_patrimonio,
                             p.nombre_item,
                             a.nombre_activo,
                             REPLACE(CONVERT(VARCHAR,CAST(p.monto_activo as money),3),'.00','') as monto_activo,
                             v.nombre_valorizacion,
                             d.nombre_deuda,
                             b.nombre_banco,
                              REPLACE(CONVERT(VARCHAR,CAST(p.total as money),3),'.00','') as total,
                             FORMAT(p.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_patrimonio as p
                             INNER JOIN dev_tipo_activo as a
                             ON p.id_tipo_activo = a.id_activo
                             INNER JOIN dev_tipo_valorizacion as v
                             ON p.id_valorizacion = v.id_valorizacion
                             INNER JOIN dev_tipo_deuda as d
                             ON p.id_deuda = d.id_deuda
                             LEFT JOIN dev_banco as b
                             ON p.id_banco = b.id_banco
                             where p.user_id = @user_id
                             GROUP BY
                             p.id_patrimonio,
                             p.nombre_item,
                             a.nombre_activo,
                             p.monto_activo,
                             v.nombre_valorizacion,
                             d.nombre_deuda,
                             b.nombre_banco,
                             p.total,
                             p.fecha_creacion";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<patrimonioRequest> lista = new List<patrimonioRequest>();

            while (reader.Read())
            {
                patrimonioRequest obj = new patrimonioRequest();

                obj.idPatrimonio = Convert.ToInt32(reader["id_patrimonio"].ToString());
                obj.nombreItem = reader["nombre_item"].ToString();
                obj.nombreActivo = reader["nombre_activo"].ToString();
                obj.montoActivo = '$' + reader["monto_activo"].ToString();
                obj.nombreValorizacion = reader["nombre_valorizacion"].ToString();
                obj.nobmreDeuda = reader["nombre_deuda"].ToString();
                obj.nombreBanco = reader["nombre_banco"].ToString();
                obj.total = '$' + reader["total"].ToString();
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

    public object SelectCargarBancoSelector()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT 
                                nombre_banco as label,
                                id_banco as value
                            FROM dev_banco
                            group by id_banco, nombre_banco";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<SelectorBANCOP> lista = new List<SelectorBANCOP>();

            while (reader.Read())
            {
                SelectorBANCOP obj = new SelectorBANCOP();
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



    public bool InsertPATRIMONIO(patrimonioRequest request)
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
            string query = @"INSERT INTO dev_patrimonio
                                    (nombre_item,
                                     id_tipo_activo,
                                     monto,
                                     user_id,
                                     id_valorizacion,
                                     id_deuda,
                                     monto_activo,
                                     id_banco,
                                     total,
                                     fecha_creacion
                                    )
                                 VALUES
                                    (@nombre_item,
                                     @id_tipo_activo,
                                     @monto,
                                     @user_id,
                                     @id_valorizacion,
                                     @id_deuda,
                                     @monto_activo,
                                     @id_banco,
                                     @total,
                                     GETDATE())";

            cmd.Parameters.AddWithValue("nombre_item", request.nombreItem);
            cmd.Parameters.AddWithValue("id_tipo_activo", request.idActivo);
            cmd.Parameters.AddWithValue("monto", request.montoActivo);
            cmd.Parameters.AddWithValue("monto_activo", request.montoActivo);
            cmd.Parameters.AddWithValue("id_valorizacion", request.idValorizacion);
            cmd.Parameters.AddWithValue("id_deuda", request.idDeuda);
            cmd.Parameters.AddWithValue("id_banco", request.idBanco);
            cmd.Parameters.AddWithValue("total", request.total);
            cmd.Parameters.AddWithValue("user_id", userid);
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

    public object eliminar(string idPatrimonio)
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
        string queryIn = "SELECT nombre_item ,id_tipo_activo, id_banco, id_deuda, monto, user_id, id_valorizacion, total FROM dev_patrimonio where id_patrimonio  in (" + idPatrimonio + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_patrimonio WHERE id_patrimonio in (" + idPatrimonio + ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();


            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Patrimonio", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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




    public bool UpdatePATRIMONIO(patrimonioRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_patrimonio
                            SET  nombre_item = @nombre_item
                            , monto = @monto
                            WHERE id_patrimonio = @id_patrimonio";

            cmd.Parameters.AddWithValue("id_patrimonio", request.idPatrimonio);
            cmd.Parameters.AddWithValue("nombre_item", request.nombreItem);
            cmd.Parameters.AddWithValue("monto", request.montoActivo);

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


    public object SelectCargarDeudaSelector()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT 
                                nombre_deuda as label,
                                id_deuda as value
                            FROM dev_tipo_deuda
                            group by id_deuda, nombre_deuda";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<SelectorDEUDAP> lista = new List<SelectorDEUDAP>();

            while (reader.Read())
            {
                SelectorDEUDAP obj = new SelectorDEUDAP();
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


    public object SelectCargarActivoSelector()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT 
                                nombre_activo as label,
                                id_activo as value
                            FROM dev_tipo_activo
                            group by id_activo, nombre_activo";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<SelectorACTIVO> lista = new List<SelectorACTIVO>();

            while (reader.Read())
            {
                SelectorACTIVO obj = new SelectorACTIVO();
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


    public object SelectCargarValorizacionSelector()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT 
                                nombre_valorizacion as label,
                                id_valorizacion as value
                            FROM dev_tipo_valorizacion
                            group by id_valorizacion, nombre_valorizacion";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<SelectorVALORIZACION> lista = new List<SelectorVALORIZACION>();

            while (reader.Read())
            {
                SelectorVALORIZACION obj = new SelectorVALORIZACION();
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



    public object SelectCargarPATRIMONIO(patrimonioRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT
                                p.nombre_item,
                                p.monto,
                            FROM dev_patrimonio as m
                            INNER JOIN dev_tipo_ingreso as e
                            WHERE id_patrimonio = @id_patrimonio";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("id_patrimonio", request.idPatrimonio);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            patrimonioRequest obj = new patrimonioRequest();

            while (reader.Read())
            {
                obj.nombreItem = reader["nombre_item"].ToString();
                obj.montoActivo = reader["monto"].ToString();
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