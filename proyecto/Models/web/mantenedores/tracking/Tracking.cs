using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Dynamic;
using System.Security.Claims;
using System.Threading;

public class Tracking : Conexion
{
    public bool guardarTracking(string accion, string modulo, string json)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {
            if (json == "SIN CAMBIOS")
            {
                return true;
            }
            else
            {
                string query = "";

                //PARA OBTENER LOS DATOS DEL USUARIO, SE DEFINEN EN IDENTITY MODEL COMO CLAIMS, SETEAR EL TIPO PARA OBTENER EL QUE QUIERAS
                //Get the current claims principal

                // Get the claims values
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var userid = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();

                //query = " DECLARE";
                //query += " JSON VARCHAR(32000);";
                //query += " BEGIN";
                //query += " JSON := '" + json + "';";
                query += " INSERT INTO web_logEvento (FECHA,JSON,ID_USUARIO,ACCION,MODULO) VALUES(GETDATE(), @JSON,@user_id,@ACCION,@MODULO)";
                // query += " END;";
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("ACCION", accion);
                cmd.Parameters.AddWithValue("MODULO", modulo);
                cmd.Parameters.AddWithValue("user_id", userid);
                cmd.Parameters.AddWithValue("JSON", json);

                cmd.ExecuteNonQuery();
                transaccion.Commit();
                return true;
            }
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


    public List<IDictionary<string, object>> obtenerAntiguo(string queryIn)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;

        try
        {
            cmd.CommandText = queryIn;
            SqlDataReader reader = cmd.ExecuteReader();
            List<IDictionary<string, object>> lista = new List<IDictionary<string, object>>();

            var columns = new List<string>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }

            while (reader.Read())
            {
                dynamic objExpando = new ExpandoObject();
                IDictionary<string, object> dict = (IDictionary<string, object>)objExpando;
                for (int i = 0; i < columns.Count; i++)
                {
                    dict[columns[i]] = reader[columns[i]].ToString();
                }
                lista.Add(dict);
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

    public string obtenerDatosNuevosyAntiguos(string queryIn, List<IDictionary<string, object>> antiguo = null)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        SqlTransaction transaccion = conn.BeginTransaction();
        cmd.Connection = conn;
        cmd.Transaction = transaccion;

        try
        {
            if (antiguo == null) antiguo = new List<IDictionary<string, object>>() {  };

            cmd.CommandText = queryIn;

            SqlDataReader reader = cmd.ExecuteReader();

            var columns = new List<string>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }

            bool flag = false;

            List<IDictionary<string, object>> lista = new List<IDictionary<string, object>>();

            var newOld = new { nuevo = "", antiguo = "" };

            List<IDictionary<string, object>> listaReader = new List<IDictionary<string, object>>();
            while (reader.Read())
            {
                dynamic o = new ExpandoObject();
                IDictionary<string, object> d = (IDictionary<string, object>)o;
                for (int i = 0; i < columns.Count; i++)
                {
                    d[columns[i]] = reader[columns[i]].ToString();
                }
                listaReader.Add(d);
            }
            reader.Close();


            if (antiguo.Count > 0 && listaReader.Count > 0 )
            {
                if (listaReader.Count == antiguo.Count)
                {
                    for (int i = 0; i < antiguo.Count; i++)
                    {
                        bool hayCambios = false;
                        dynamic objExpando = new ExpandoObject();
                        IDictionary<string, object> dict = (IDictionary<string, object>)objExpando;
                        for (int y = 0; y < columns.Count; y++)
                        {
                            if (listaReader[i][columns[y]].ToString() != antiguo[i][columns[y]].ToString())
                            {//si algún dato se cambió , guardo el tracking, si no , no es necesario.
                                hayCambios = true;
                                flag = true;
                            }

                            newOld = new { nuevo = listaReader[i][columns[y]].ToString(), antiguo = antiguo[i][columns[y]].ToString() };
                            dict[columns[y]] = newOld;

                        }
                        if (dict.Count > 0 && hayCambios)
                            lista.Add(dict);
                    }
                }else
                {
                    for (int i = 0; i < listaReader.Count; i++)
                    {

                        dynamic objExpando = new ExpandoObject();
                        IDictionary<string, object> dict = (IDictionary<string, object>)objExpando;
                        for (int y = 0; y < columns.Count; y++)
                        {
                            flag = true;
                            newOld = new { nuevo = listaReader[i][columns[y]].ToString(), antiguo = "" };
                            dict[columns[y]] = newOld;

                        }
                        if (dict.Count > 0)
                            lista.Add(dict);
                    }
                }
            }
            

            if (listaReader.Count > 0 && antiguo.Count == 0)
            {
                for (int i = 0; i < listaReader.Count; i++)
                {

                    dynamic objExpando = new ExpandoObject();
                    IDictionary<string, object> dict = (IDictionary<string, object>)objExpando;
                    for (int y = 0; y < columns.Count; y++)
                    {
                        flag = true;
                        newOld = new { nuevo = listaReader[i][columns[y]].ToString(), antiguo = "" };
                        dict[columns[y]] = newOld;
                        
                    }
                    if (dict.Count > 0)
                        lista.Add(dict);
                }
            }

            if (listaReader.Count == 0 && antiguo.Count > 0)
            {
                for (int i = 0; i < antiguo.Count; i++)
                {

                    dynamic objExpando = new ExpandoObject();
                    IDictionary<string, object> dict = (IDictionary<string, object>)objExpando;
                    for (int y = 0; y < columns.Count; y++)
                    {
                        flag = true;
                        newOld = new { nuevo = "", antiguo = antiguo[i][columns[y]].ToString() };
                        dict[columns[y]] = newOld;

                    }
                    if (dict.Count > 0)
                        lista.Add(dict);
                }
            }

            
            string json = "";
            if (flag)
            {
                JavaScriptSerializer jsonserializer = new JavaScriptSerializer();
                jsonserializer.MaxJsonLength = 500000000;
                json = jsonserializer.Serialize(lista);

            }
            else
            {
                json = "SIN CAMBIOS";
            }

            return json;
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