using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


//agregar para crear lo basico
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Globalization;

public class logEventoModel : Conexion
{
    public object tablaLogDeEvento(logEventoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        SqlCommand cmd = new SqlCommand();
        try
        {

            string query = " SELECT " +
                            " l.id_logEvento AS id," +
                            " l.fecha," +
                            " l.modulo   AS module," +
                            " l.accion   AS action," +
                            " g.name     AS perfil," +
                            " u.username AS usuario," +
                            " l.json" +
                            " FROM web_logEvento l" +
                            " INNER JOIN web_users u  ON u.user_id = l.id_usuario" +
                            " INNER JOIN web_groups g  ON g.group_id = u.group_id" +
                            " WHERE 1=1";

           

            if (request.modulo != null && request.modulo != "" && request.modulo != "0")
            {
                query += " AND l.modulo = @modulo ";
                cmd.Parameters.AddWithValue("modulo", request.modulo);
            }

            if (request.user_id != "0" && request.user_id != "" && request.user_id != null)
            {
                query += " AND u.user_id = @user_id";
                cmd.Parameters.AddWithValue("user_id", request.user_id);
            }

            if (request.perfil != "0" && request.perfil != "" && request.perfil != null)
            {
                query += " AND g.group_id = @perfil";
                cmd.Parameters.AddWithValue("perfil", request.perfil);
            }
            //query += " )vista  WHERE 1=1";

            //if (request.fecha_inicio != "" && request.fecha_inicio != null && request.fecha_inicio != "01-01-1970")
            //{
            //    query += " AND TO_CHAR(l.fecha,'DD/MM/RRRR') >= TO_DATE(@fecha_inicio,'DD/MM/RRRR')";
            //    cmd.Parameters.AddWithValue("fecha_inicio", request.fecha_inicio);
            //}
            //if (request.fecha_fin != "" && request.fecha_fin != null && request.fecha_fin != "01-01-1970")
            //{
            //    query += " AND TO_CHAR(l.fecha,'DD/MM/RRRR') <= TO_DATE(@fecha_termino,'DD/MM/RRRR')";
            //    cmd.Parameters.AddWithValue("fecha_termino", request.fecha_fin);
            //}


            if (request.fecha_inicio != "" && request.fecha_inicio != null)
            {
                query += " AND l.fecha >= @fecha_ini ";
                try
                {
                    DateTime dateInicio = DateTime.ParseExact(request.fecha_inicio,
                                         "dd-MM-yyyy",
                                         CultureInfo.InvariantCulture);
                    string fechaInicio = dateInicio.ToString("yyyy-MM-dd");
                    cmd.Parameters.AddWithValue("fecha_ini", fechaInicio+" 00:00:00");
                }
                catch (FormatException)
                {
                    cmd.Parameters.AddWithValue("fecha_ini", request.fecha_inicio + " 00:00:00");
                }
                //cmd.Parameters.AddWithValue("fecha_ini", request.fecha_inicio);
            }
            else
            {
                query += " AND l.fecha >= @fecha_ini ";
                cmd.Parameters.AddWithValue("fecha_ini", DateTime.Today.ToString("yyyy-MM-dd") + " 00:00:00");
            }
            if (request.fecha_fin != "" && request.fecha_fin != null)
            {
                query += " AND l.fecha <= @fecha_fin ";
                try
                {
                    DateTime dateFin = DateTime.ParseExact(request.fecha_fin,
                                         "dd-MM-yyyy",
                                         CultureInfo.InvariantCulture);
                    string fechaFin = dateFin.ToString("yyyy-MM-dd");
                    cmd.Parameters.AddWithValue("fecha_fin", fechaFin + " 23:59:59");
                }
                catch (FormatException)
                {
                    cmd.Parameters.AddWithValue("fecha_fin", request.fecha_fin + " 23:59:59");
                }
                //cmd.Parameters.AddWithValue("fecha_fin", request.fecha_fin);
            }
            else
            {
                query += " AND l.fecha <= @fecha_fin ";
                cmd.Parameters.AddWithValue("fecha_fin", DateTime.Today.ToString("yyyy-MM-dd") + " 23:59:59");
            }


            cmd.CommandText = query;
            cmd.Connection = conn;

            

            

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<data> lista = new List<data>();



            while (reader.Read())
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                
                //RootObject[,] deserialized = JsonConvert.DeserializeObject<RootObject[,]>(reader["json"].ToString());
                data obj = new data();
                obj.id = reader["id"].ToString();
                obj.fecha = reader["fecha"].ToString();
                obj.modulo = reader["module"].ToString();
                obj.action = reader["action"].ToString();
                obj.perfil = reader["perfil"].ToString();
                obj.usuario = reader["usuario"].ToString();
                obj.json = reader["json"].ToString();
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


    public object selectorModulo()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = "SELECT MODULO FROM WEB_LOGEVENTO GROUP BY MODULO ";
            SqlCommand cmd = new SqlCommand(query);

            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<string> lista = new List<string>();

            while (reader.Read())
            {
                string obj = "";
                obj = reader["MODULO"].ToString();

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
    public List<perfilData> selectorPerfil()
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = "SELECT group_id as id,name FROM web_groups ";
            SqlCommand cmd = new SqlCommand(query);

            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<perfilData> lista = new List<perfilData>();

            while (reader.Read())
            {
                perfilData obj = new perfilData();
                obj.codigo = reader["id"].ToString();
                obj.text = reader["name"].ToString();
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
    public object usuariosPorPerfil(perfilRequestPorUsuario request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            string query = "SELECT ROW_NUMBER() OVER (ORDER BY u.username) as q,u.user_id AS id,u.username FROM web_users u INNER JOIN web_groups g ON g.group_id = u.group_id WHERE UPPER(username) like UPPER('%" + request.term + "%')";

            if (request.perfil != "0" && request.perfil != "" && request.perfil != null)
            {
                query += " AND g.group_id = @perfil ";
            }

            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();

            if (request.perfil != "0" && request.perfil != "" && request.perfil != null)
            {
                cmd.Parameters.AddWithValue("perfil", request.perfil);
            }
            SqlDataReader reader = cmd.ExecuteReader();
            List<perfilDataPorUsuario> lista = new List<perfilDataPorUsuario>();

            while (reader.Read())
            {
                perfilDataPorUsuario obj = new perfilDataPorUsuario();
                obj.id = reader["id"].ToString();
                obj.text = reader["username"].ToString();
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


    public object getDatosSelectPerfil(obj_fecha_filtro request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            request.fecha_inicio = request.fecha_inicio.Replace("/", "-");
            request.fecha_fin = request.fecha_fin.Replace("/", "-");

            string query = @"
                        SELECT 
	                        wg.GROUP_ID as value,
	                        UPPER(wg.NAME) as label	
                        FROM web_logevento wle
                        inner join web_users wu on wle.ID_USUARIO = wu.USER_ID
                        inner join web_groups wg on wu.GROUP_ID = wg.GROUP_ID
                        where CONVERT(varchar, wle.FECHA, 23) BETWEEN @fecha_inicio AND @fecha_fin
                        group by wg.GROUP_ID, wg.NAME
                ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("fecha_inicio", request.fecha_inicio);
            cmd.Parameters.AddWithValue("fecha_fin", request.fecha_fin);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<perfilSelect> lista = new List<perfilSelect>();

            while (reader.Read())
            {
                perfilSelect obj = new perfilSelect();
                obj.value = reader["value"].ToString();
                obj.label = reader["label"].ToString();
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

    public object getDatosSelectUsuario(obj_fecha_filtro request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            request.fecha_inicio = request.fecha_inicio.Replace("/", "-");
            request.fecha_fin = request.fecha_fin.Replace("/", "-");

            string query = @"
                    SELECT 
	                    wle.ID_USUARIO as value,
	                    wu.UserName as label
                    FROM web_logevento wle
                    inner join web_users wu on wle.ID_USUARIO = wu.USER_ID
                    where CONVERT(varchar, wle.FECHA, 23) BETWEEN @fecha_inicio AND @fecha_fin
                    group by wle.ID_USUARIO, wu.UserName
                ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("fecha_inicio", request.fecha_inicio);
            cmd.Parameters.AddWithValue("fecha_fin", request.fecha_fin);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<usuarioSelect> lista = new List<usuarioSelect>();

            while (reader.Read())
            {
                usuarioSelect obj = new usuarioSelect();
                obj.value = reader["value"].ToString();
                obj.label = reader["label"].ToString();
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


    public object getDatosSelectModulo(obj_fecha_filtro request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            request.fecha_inicio = request.fecha_inicio.Replace("/", "-");
            request.fecha_fin = request.fecha_fin.Replace("/", "-");

            string query = @"
                        SELECT 
	                        MODULO as value,
	                        MODULO as label
                        FROM web_logevento 
                        where CONVERT(varchar, FECHA, 23) BETWEEN @fecha_inicio AND @fecha_fin
                        group by MODULO
                ";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("fecha_inicio", request.fecha_inicio);
            cmd.Parameters.AddWithValue("fecha_fin", request.fecha_fin);
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<moduloSelect> lista = new List<moduloSelect>();

            while (reader.Read())
            {
                moduloSelect obj = new moduloSelect();
                obj.value = reader["value"].ToString();
                obj.label = reader["label"].ToString();
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