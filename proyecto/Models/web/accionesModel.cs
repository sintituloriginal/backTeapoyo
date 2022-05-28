using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


//agregar para crear lo basico
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Collections;
using proyecto.objects;
using System.Security.Claims;
using System.Threading;

public class accionesModel : Conexion
{
    public object acciones(rutaRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {

            string query = " SELECT  " +
                            " web_screen.link, " +
                            " web_action.name as action, " +
                            " web_screen_action.id_action, " +
                            " web_screen.vista_por_defecto, " +
                            " web_accion_generica.cmd, " +
                            " web_accion_generica.uiIcon " +
                            " FROM web_screen " +
                            " INNER JOIN web_action ON cast(web_action.link as varchar(250)) = cast( web_screen.link as varchar(250)) " +
                            " INNER JOIN web_screen_action on web_screen_action.id_action=web_action.id_action and web_screen_action.id_screen = web_screen.id_screen " +
                            " INNER JOIN web_groups on web_groups.group_id=web_screen.group_id " +
                            " INNER JOIN web_accion_generica on web_accion_generica.nombre = web_action.name " +
                            " WHERE web_screen.group_id= @groups and web_screen.link= @perfil ORDER BY cast(web_screen.link as varchar(250)) desc, web_accion_generica.ordenmostrar asc";


            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = conn;
            conn.Open();



            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string group = identity.Claims.Where(c => c.Type == "group_id")
                               .Select(c => c.Value).SingleOrDefault();

            request.group_id = group;
            cmd.Parameters.AddWithValue("groups", request.group_id);
            cmd.Parameters.AddWithValue("perfil", request.ruta);
            SqlDataReader reader = cmd.ExecuteReader();

            List<acciones> lista = new List<acciones>();

            while (reader.Read())
            {
                acciones obj = new acciones();
                obj.action = reader["action"].ToString();
                obj.link = reader["link"].ToString();
                obj.cmd = reader["cmd"].ToString();
                obj.uiIcon = reader["uiIcon"].ToString();
                obj.vista_por_defecto = reader["vista_por_defecto"].ToString();
                lista.Add(obj);
            }
            reader.Close();


            var accionEliminar = new Dictionary<string, string>();
            var group_id = identity.Claims.Where(c => c.Type == "group_id")
                               .Select(c => c.Value).SingleOrDefault();
            var user_id = identity.Claims.Where(c => c.Type == "user_id")
                               .Select(c => c.Value).SingleOrDefault();

            query = "";
            query += "select  ADMINISTRADOR  from web_groups where GROUP_ID =  " + group_id + " ";
            cmd.Parameters.Clear();
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            reader = cmd.ExecuteReader();
            var ADMINISTRADOR = "";
            while (reader.Read())
            {
                ADMINISTRADOR = reader["ADMINISTRADOR"].ToString();
            }
           //duplica el eliminar
            // if (ADMINISTRADOR == "true")
            // {
            //     acciones obj = new acciones();
            //     obj.action = "Eliminar";
            //     obj.cmd = "eliminar";
            //     obj.link = "/noConformidad";
            //     obj.uiIcon = "delete";
            //     obj.vista_por_defecto = "";
            //     lista.Add(obj);
            // }
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

    public object validarPermisosAcceso(validacionesAccesoRequest request)
    {
        SqlConnection conn = dbproyecto.openConnection();
        try
        {
            //'se diferencia las consultas dependiendo si es una pantalla raiz o si es una pantalla
            //'que se cargo a partir de una accion
            string query = "";
            int registrosPermisos = 0;

            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            // Get the claims values
            string group = identity.Claims.Where(c => c.Type == "group_id")
                               .Select(c => c.Value).SingleOrDefault();


            request.id_perfil = group;
            if (request.accion == "" || request.accion == null)
            {
                query = " select " +
                    "     COUNT(1) as cantidad " +
                    " from web_screen s " +
                    " where " +
                    " s.status = 'true'" +
                    " and s.group_id = @group_id " +
                    " and cast(s.link as varchar(250)) = @link ";
            }
            else
            {
                query = "  select COUNT(1) as cantidad from web_screen_action sa" +
                " inner join web_screen s on s.id_screen = sa.id_screen " +
                " inner join web_action a on sa.id_action = a.id_action " +
                " where " +
                " s.status = 'true'" +
                " and s.group_id = @group_id " +
                " and cast(s.link as varchar(250)) = @link " +
                " and a.name = @accion ";
            }

            SqlCommand cmd = new SqlCommand(query);
            if (request.accion == "" || request.accion == null)
            {
                cmd.Parameters.AddWithValue("group_id", request.id_perfil);
                cmd.Parameters.AddWithValue("link", request.link);
            }
            else
            {
                cmd.Parameters.AddWithValue("accion", request.accion);
                cmd.Parameters.AddWithValue("group_id", request.id_perfil);
                cmd.Parameters.AddWithValue("link", request.link);
            }
            cmd.Connection = conn;
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                registrosPermisos = Convert.ToInt32(reader["cantidad"].ToString());
            }
            reader.Close();
            if (registrosPermisos >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
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