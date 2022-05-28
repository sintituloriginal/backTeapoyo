using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;

public class financieraModel : Conexion
{
    public object SelectTotalActualCortoPlazo()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"select ISNULL(sum(f.saldo_deuda),0)as totalActualCortoPlazo from dev_carga_financiera as f where f.id_tipo_deuda != '15' and f.id_tipo_plazo = '1'
                             and f.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["totalActualCortoPlazo"]);
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

    public object SelectTotalActualCortoPlazoAdmin(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
   
        try
        {
            string query = @"select ISNULL(sum(f.saldo_deuda),0)as totalActualCortoPlazo 
                                from dev_carga_financiera as f 
                                INNER JOIN web_users as w
                                ON w.USER_ID = f.USER_ID
                                where f.id_tipo_deuda != '15' and f.id_tipo_plazo = '1'";

            if(request.fechaNacimiento != null)
            {
                query += @" AND FECHA_NACIMIENTO = @FECHA_NACIMIENTO";
            }
            if(request.sexo != null)
            {
                query += @" AND SEXO = @SEXO";
            }
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

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["totalActualCortoPlazo"]);
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

    public object SelectCargarTotal()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"select ISNULL(sum(i.pago_mensual),0) as total from dev_carga_financiera as i
                             where i.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["TOTAL"]);
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

    public object SelectCargarTotalAdmin(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();

        try
        {
            string query = @"select ISNULL(sum(i.pago_mensual),0) as total 
                             from dev_carga_financiera as i
                             INNER JOIN web_users as w
                             ON w.USER_ID = i.USER_ID
                             where 1=1";


            if (request.fechaNacimiento != null)
            {
                query += @" AND w.FECHA_NACIMIENTO = @FECHA_NACIMIENTO";
            }
            if (request.sexo != null)
            {
                query += @" AND w.SEXO = @SEXO";
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

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["TOTAL"]);
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

    public object SelectCargarTotalConsumoFuturo()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"select ISNULL(sum(f.saldo_deuda),0) as ConsumoFuturo from dev_carga_financiera as f where f.id_tipo_deuda = '15' and f.id_tipo_plazo = '1'
                             and f.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["ConsumoFuturo"]);
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

    public object SelectCargarTotalConsumoFuturoAdmin(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"select ISNULL(sum(f.saldo_deuda),0) as ConsumoFuturo 
                             from dev_carga_financiera as f
                             INNER JOIN web_users as w
                             ON w.USER_ID = f.USER_ID
                             where f.id_tipo_deuda = '15' and f.id_tipo_plazo = '1'";

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

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["ConsumoFuturo"]);
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


    public object SelectCargarTotalActualLargoPlazo()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"select ISNULL(sum(f.saldo_deuda),0) as totalActualLargoPLazo from dev_carga_financiera as f where f.id_tipo_deuda != '16' and f.id_tipo_plazo = '2'
                             and f.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["totalActualLargoPlazo"]);
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

    public object SelectCargarTotalActualLargoPlazoAdmin(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();

        try
        {
            string query = @"select ISNULL(sum(f.saldo_deuda),0) as totalActualLargoPLazo 
                             from dev_carga_financiera as f 
                             INNER JOIN web_users as w
                             ON w.USER_ID = f.USER_ID
                             where f.id_tipo_deuda != '16' and f.id_tipo_plazo = '2'";


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

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["totalActualLargoPlazo"]);
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


    public object SelectCargarTotalHiptecarioFuturo()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"select ISNULL(sum(f.saldo_deuda),0) as HipotecarioFuturo from dev_carga_financiera as f where f.id_tipo_deuda = '16' and f.id_tipo_plazo = '2'
                             and f.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["HipotecarioFuturo"]);
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

    public object SelectCargarTotalHiptecarioFuturoAdmin(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();

        try
        {
            string query = @"select ISNULL(sum(f.saldo_deuda),0) as HipotecarioFuturo 
                             from dev_carga_financiera as f 
                             INNER JOIN web_users as w
                             ON w.USER_ID = f.USER_ID
                             where f.id_tipo_deuda = '16' and f.id_tipo_plazo = '2'";

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

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["HipotecarioFuturo"]);
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

    public object SelectCargarPagoMensualCortoPLazo()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"select ISNULL(sum(f.pago_mensual),0) as totalPagoMensualCortoPlazo from dev_carga_financiera as f where f.id_tipo_plazo = '1'
                             and f.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["totalPagoMensualCortoPlazo"]);
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

    public object SelectCargarPagoMensualCortoPLazoAdmin(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();

        try
        {
            string query = @"select ISNULL(sum(f.pago_mensual),0) as totalPagoMensualCortoPlazo 
                             from dev_carga_financiera as f
                             INNER JOIN web_users as w  ON w.USER_ID = f.USER_ID
                             where f.id_tipo_plazo = '1'";

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

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["totalPagoMensualCortoPlazo"]);
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


    public object SelectCargarPagoMensualLargoPlazo()
    {
        SqlConnection conn = dbproyecto.openConnection();
        var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
        var userid = identity.Claims.Where(c => c.Type == "user_id")
                          .Select(c => c.Value).SingleOrDefault();
        try
        {
            string query = @"select ISNULL(sum(f.pago_mensual),0) as totalPagoMensualLargoPlazo from dev_carga_financiera as f where f.id_tipo_plazo = '2'
                             and f.user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["totalPagoMensualLargoPlazo"]);
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

    public object SelectCargarPagoMensualLargoPlazoAdmin(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"select ISNULL(sum(f.pago_mensual),0) as totalPagoMensualLargoPlazo 
                            from dev_carga_financiera as f 
                            INNER JOIN web_users as w
                            ON w.USER_ID = f.USER_ID
                            where f.id_tipo_plazo = '2'";

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

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.total = Convert.ToInt32(reader["totalPagoMensualLargoPlazo"]);
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
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY f.id_carga_financiera asc, f.nombre_deuda asc, t.nombre_deuda, f.saldo_deuda asc, f.cupo asc, f.pago_mensual asc, p.nombre_plazo asc, f.valor_cuota asc, b.nombre_banco asc, f.fecha_creacion asc) as indice,
                             f.id_carga_financiera,
							 f.nombre_deuda, 
							 t.nombre_deuda as tipo_deuda, 
							 REPLACE(CONVERT(VARCHAR,CAST(f.saldo_deuda as money),3),'.00','') as saldo_deuda, 
							 REPLACE(CONVERT(VARCHAR,CAST(f.cupo as money),3),'.00','') as cupo, 
							 REPLACE(CONVERT(VARCHAR,CAST(f.pago_mensual as money),3),'.00','') as pago_mensual, 
							 p.nombre_plazo, 
							 REPLACE(CONVERT(VARCHAR,CAST(f.valor_cuota as money),3),'.00','') as valor_cuota,
                             b.nombre_banco,
                             FORMAT(f.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_carga_financiera as f
                             INNER JOIN dev_tipo_deuda as t
                             ON f.id_tipo_deuda = t.id_deuda
							 INNER JOIN dev_tipo_plazo as p
							 ON f.id_tipo_plazo = p.id_plazo
                             INNER JOIN  dev_banco as b
                             ON f.id_banco = b.id_banco
                             where f.user_id = @user_id
                             GROUP BY
                             f.id_carga_financiera,
							 f.nombre_deuda, 
							 t.nombre_deuda, 
							 f.saldo_deuda, 
							 f.cupo, 
							 f.pago_mensual, 
							 p.nombre_plazo, 
							 f.valor_cuota,
                             b.nombre_banco,
                             f.fecha_creacion";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.idFinanciera = Convert.ToInt32(reader["id_carga_financiera"].ToString());
                obj.nombreDeuda = reader["nombre_deuda"].ToString();
                obj.TipoDeduda =  reader["tipo_deuda"].ToString();
                obj.saldoDeuda = "$" + reader["saldo_deuda"].ToString();
                obj.cupoTotal = "$" + reader["cupo"].ToString();
                obj.pagoMensual ="$" + reader["pago_mensual"].ToString();
                obj.nombrePlazo = reader["nombre_plazo"].ToString();
                obj.valorCuota = "$" + reader["valor_cuota"].ToString();
                obj.nombreBanco = reader["nombre_banco"].ToString();
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
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY f.id_carga_financiera asc, f.nombre_deuda asc, t.nombre_deuda, f.saldo_deuda asc, f.cupo asc, f.pago_mensual asc, p.nombre_plazo asc, f.valor_cuota asc, b.nombre_banco asc, f.fecha_creacion asc) as indice,
                             f.id_carga_financiera,
							 w.UserName,
							 w.FULLNAME,
							 f.nombre_deuda, 
							 t.nombre_deuda as tipo_deuda, 
							 f.saldo_deuda, 
							 f.cupo, 
							 f.pago_mensual, 
							 p.nombre_plazo, 
							 f.valor_cuota,
                             b.nombre_banco,
                             FORMAT(f.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_carga_financiera as f
                             INNER JOIN dev_tipo_deuda as t
                             ON f.id_tipo_deuda = t.id_deuda
							 INNER JOIN dev_tipo_plazo as p
							 ON f.id_tipo_plazo = p.id_plazo
                             INNER JOIN  dev_banco as b
                             ON f.id_banco = b.id_banco
							 INNER JOIN web_users as w
							 ON w.USER_ID = f.USER_ID
                             GROUP BY
                             f.id_carga_financiera,
							 w.UserName,
							 w.FULLNAME,
							 f.nombre_deuda, 
							 t.nombre_deuda, 
							 f.saldo_deuda, 
							 f.cupo, 
							 f.pago_mensual, 
							 p.nombre_plazo, 
							 f.valor_cuota,
                             b.nombre_banco,
                             f.fecha_creacion";



            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.idFinanciera = Convert.ToInt32(reader["id_carga_financiera"].ToString());
                obj.nombreDeuda = reader["nombre_deuda"].ToString();
                obj.TipoDeduda = reader["tipo_deuda"].ToString();
                obj.saldoDeuda ="$" + reader["saldo_deuda"].ToString();
                obj.cupoTotal ="$" + reader["cupo"].ToString();
                obj.pagoMensual ="$" + reader["pago_mensual"].ToString();
                obj.nombrePlazo = reader["nombre_plazo"].ToString();
                obj.valorCuota = "$" + reader["valor_cuota"].ToString();
                obj.nombreBanco = reader["nombre_banco"].ToString();
                obj.fechaCreacion = reader["fecha_creacion"].ToString();
                obj.username = reader["UserName"].ToString();
                obj.fullname = reader["FULLNAME"].ToString();
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


    public object FiltrarTabla(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT ROW_NUMBER() OVER (ORDER BY f.id_carga_financiera asc, f.nombre_deuda asc, t.nombre_deuda, f.saldo_deuda asc, f.cupo asc, f.pago_mensual asc, p.nombre_plazo asc, f.valor_cuota asc, b.nombre_banco asc, f.fecha_creacion asc) as indice,
                             f.id_carga_financiera,
							 w.UserName,
							 w.FULLNAME,
							 f.nombre_deuda, 
							 t.nombre_deuda as tipo_deuda, 
							 f.saldo_deuda, 
							 f.cupo, 
							 f.pago_mensual, 
							 p.nombre_plazo, 
							 f.valor_cuota,
                             b.nombre_banco,
                             FORMAT(f.fecha_creacion, 'dd/MM/yyyy HH:mm:ss') as fecha_creacion
                             FROM dev_carga_financiera as f
                             INNER JOIN dev_tipo_deuda as t
                             ON f.id_tipo_deuda = t.id_deuda
							 INNER JOIN dev_tipo_plazo as p
							 ON f.id_tipo_plazo = p.id_plazo
                             INNER JOIN  dev_banco as b
                             ON f.id_banco = b.id_banco
							 INNER JOIN web_users as w
							 ON w.USER_ID = f.USER_ID
                             WHERE 1=1";

                if(request.fechaNacimiento != null)
                {
                query += @" AND FECHA_NACIMIENTO = @FECHA_NACIMIENTO";
                }
                if(request.sexo != null)
                {
                query += @" AND SEXO = @SEXO";
                }

                query+=     @" GROUP BY
                             f.id_carga_financiera,
							 w.UserName,
							 w.FULLNAME,
							 f.nombre_deuda, 
							 t.nombre_deuda, 
							 f.saldo_deuda, 
							 f.cupo, 
							 f.pago_mensual, 
							 p.nombre_plazo, 
							 f.valor_cuota,
                             b.nombre_banco,
                             f.fecha_creacion";



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

            List<financieraRequest> lista = new List<financieraRequest>();

            while (reader.Read())
            {
                financieraRequest obj = new financieraRequest();

                obj.idFinanciera = Convert.ToInt32(reader["id_carga_financiera"].ToString());
                obj.nombreDeuda = reader["nombre_deuda"].ToString();
                obj.TipoDeduda = reader["tipo_deuda"].ToString();
                obj.saldoDeuda = "$" + reader["saldo_deuda"].ToString();
                obj.cupoTotal = "$" + reader["cupo"].ToString();
                obj.pagoMensual = "$" + reader["pago_mensual"].ToString();
                obj.nombrePlazo = reader["nombre_plazo"].ToString();
                obj.valorCuota = "$" + reader["valor_cuota"].ToString();
                obj.nombreBanco = reader["nombre_banco"].ToString();
                obj.fechaCreacion = reader["fecha_creacion"].ToString();
                obj.username = reader["UserName"].ToString();
                obj.fullname = reader["FULLNAME"].ToString();
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


    public object SelectCargarUsuarioSelector()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT 
                                FULLNAME as label,
                                USER_ID as value
                            FROM web_users
                            group by FULLNAME, USER_ID";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<SelectorUSUARIO> lista = new List<SelectorUSUARIO>();

            while (reader.Read())
            {
                SelectorUSUARIO obj = new SelectorUSUARIO();
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


    public object SelectCargarUserSelector()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT 
                                UserName label,
                                USER_ID as value
                            FROM web_users
                            group by UserName, USER_ID";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<SelectorUSUARIONOMBRE> lista = new List<SelectorUSUARIONOMBRE>();

            while (reader.Read())
            {
                SelectorUSUARIONOMBRE obj = new SelectorUSUARIONOMBRE();
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

            List<SelectorBANCO> lista = new List<SelectorBANCO>();

            while (reader.Read())
            {
                SelectorBANCO obj = new SelectorBANCO();
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

    public bool InsertFINANCIERA(financieraRequest request)
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
            string query = @"INSERT INTO dev_carga_financiera
                                    (nombre_deuda,
                                     id_tipo_deuda,
                                     saldo_deuda,
                                     pago_mensual,
                                     id_tipo_plazo,
                                     user_id,
                                     valor_cuota,
                                     id_banco,
                                     cupo,
                                     fecha_creacion
                                    )
                                 VALUES
                                    (@nombre_deuda,
                                     @id_tipo_deuda,
                                     @saldo_deuda,
                                     @pago_mensual,
                                     @id_tipo_plazo,
                                     @user_id,
                                     @valor_cuota,
                                     @id_banco,
                                     @cupo,
                                     GETDATE())";

            cmd.Parameters.AddWithValue("user_id", userid);
            cmd.Parameters.AddWithValue("nombre_deuda", request.nombreDeuda);
            cmd.Parameters.AddWithValue("id_tipo_deuda", request.idTipoDeuda);
            cmd.Parameters.AddWithValue("saldo_deuda", request.saldoDeuda);
            cmd.Parameters.AddWithValue("cupo", request.cupoTotal);
            cmd.Parameters.AddWithValue("pago_mensual", request.pagoMensual);
            cmd.Parameters.AddWithValue("id_tipo_plazo", request.idPlazo);
            cmd.Parameters.AddWithValue("valor_cuota", request.valorCuota);
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

    public object eliminar(string idFinanciera)
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
        string queryIn = "SELECT id_carga_financiera ,nombre_deuda, id_tipo_deuda, saldo_deuda, cupo, pago_mensual, id_tipo_plazo, user_id, valor_cuota, id_banco FROM dev_carga_financiera where id_carga_financiera  in (" + idFinanciera + ")";

        try
        {
            //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
            List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

            string query;

            query = " DELETE FROM dev_carga_financiera WHERE id_carga_financiera in (" + idFinanciera+ ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();


            transaccion.Commit();

            //GUARDAMOS EL TRACKING
            t.guardarTracking("Eliminar", "Financiera", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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




    public bool UpdateFINANCIERA(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;
        try
        {
            string query = @"UPDATE dev_carga_financiera
                            SET nombre_deuda = @nombre_deuda,
                                saldo_deuda = @saldo_deuda,
                                cupo = @cupo,
                                valor_cuota =  @valor_cuota
                            WHERE id_carga_financiera = @id_carga_financiera";

            cmd.Parameters.AddWithValue("id_carga_financiera", request.idFinanciera);
            cmd.Parameters.AddWithValue("id_tipo_deuda", request.idTipoDeuda);
            cmd.Parameters.AddWithValue("cupo", request.cupoTotal);
            cmd.Parameters.AddWithValue("saldo_deuda", request.saldoDeuda);
            cmd.Parameters.AddWithValue("valor_cuota", request.valorCuota);
            cmd.Parameters.AddWithValue("nombre_deuda", request.nombreDeuda);

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

            List<SelectorDEUDA> lista = new List<SelectorDEUDA>();

            while (reader.Read())
            {
                SelectorDEUDA obj = new SelectorDEUDA();
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


    public object SelectCargarFINANCIERA(financieraRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = @"SELECT
                                f.nombre_deuda,
                                d.nombre_deuda,
                                f.saldo,
                                f.cupo,
                                p.nombre_plazo,
                                b.nombre_banco
                            FROM dev_carga_financiera as f
                            INNER JOIN dev_tipo_deuda as d
                            ON f.id_tipo_deuda = d.id_deuda
                            INNER JOIN dev_tipo_plazo as p
                            ON f.id_tipo_plazo on f_id_tipo_plazo = p.id_plazo
                            INNER JOIN dev_banco as b
                            ON f.id_banco = b.id_banco
                            WHERE id_carga_financiera = @id_carga_financiera";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("id_carga_financiera", request.idFinanciera);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            financieraRequest obj = new financieraRequest();

            while (reader.Read())
            {
                obj.nombreDeuda = reader["nombre_deuda"].ToString();
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