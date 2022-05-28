using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using proyecto.objects.web;
using proyecto.objects;
using System.Security.Claims;
using System.Threading;
using System.Globalization;

namespace proyecto.Models.menu
{
    public class menuModel : Conexion
    {
        public List<Branch> branch(int group_id)
        {
            SqlConnection conn;
            conn = dbproyecto.openConnection();
            try
            {
                string query;
                query = "SELECT * FROM web_branch" +
                        " INNER JOIN web_screen ON cast(web_screen.link as varchar(250))=cast(web_branch.link as varchar(250))" +
                        " WHERE web_screen.group_id=@groups" +
                        " AND web_screen.status='true'" +
                        " ORDER BY web_branch.orden ASC";

                //cramos el comando
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                // seteamos los valores de la consulta
                // TODO VHT propuesta modificacion un valor declarado int nunca podrá ser null, provocará error antes de llegar a esta validacion 
                cmd.Parameters.AddWithValue("groups", group_id == null ? 0 : group_id);
                SqlDataReader reader;

                //abrimos la conexion a la base de datos
                conn.Open();
                reader = cmd.ExecuteReader();


                var lista = new List<Branch>();

                // comprobamos los datos            
                while (reader.Read())
                {
                    Branch obj = new Branch();
                    obj.id_branch = Convert.ToInt32(reader["id_branch"].ToString());
                    obj.title = reader["name"].ToString();
                    obj.href = reader["link"].ToString();
                    obj.icon = reader["icon"].ToString();
                    obj.source = reader["source"].ToString();
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
        public List<Branch> branchConSource(int group_id)
        {
            SqlConnection conn;
            conn = dbproyecto.openConnection();
            try
            {
                string query;
                query = " SELECT id_branch,name,web_branch.link,icon,source, " +
                        " case when cast(source as varchar(250)) != '' " +
                        " then 1 " +
                        " else 0 " +
                        " end as source1,orden,id_screen,group_id,status,fecha " +
                        " FROM web_branch " +
                        " INNER JOIN web_screen ON cast(web_screen.link as varchar(250))=cast(web_branch.link as varchar(250)) " +
                        " WHERE web_screen.group_id=@groups " +
                        " AND web_screen.status='true' and id_branch IN (select id_branch from web_sheet) ";

                //cramos el comando
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                // seteamos los valores de la consulta
                // TODO propuesta modificacion un valor declarado int nunca podrá ser null, provocará error antes de llegar a esta validacion 

                cmd.Parameters.AddWithValue("groups", group_id == null ? 0 : group_id);
                SqlDataReader reader;

                //abrimos la conexion a la base de datos
                conn.Open();
                reader = cmd.ExecuteReader();


                var lista = new List<Branch>();

                // comprobamos los datos            
                while (reader.Read())
                {
                    Branch obj = new Branch();
                    obj.id_branch = Convert.ToInt32(reader["id_branch"].ToString());
                    obj.title = reader["name"].ToString();
                    obj.href = reader["link"].ToString();
                    obj.icon = reader["icon"].ToString();
                    obj.source = reader["source"].ToString();
                    obj.bandera = Convert.ToInt32(reader["source1"].ToString());
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

        public List<Sheet> sheet(int group_id)
        {
            SqlConnection conn;
            conn = dbproyecto.openConnection();
            try
            {
                string query;
                query = "SELECT * FROM web_sheet" +
                        " INNER JOIN web_screen on cast(web_screen.link as varchar(250)) = cast(web_sheet.link as varchar(250))" +
                        " WHERE web_screen.group_id=@groups" +
                        " AND web_screen.status='true' AND Web_Sheet.Id_Branch != '27' and web_screen.link != '/norma' ORDER BY web_sheet.orden ASC";

                //cramos el comando
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                // seteamos los valores de la consulta
                cmd.Parameters.AddWithValue("groups", group_id);
                SqlDataReader reader;

                //abrimos la conexion a la base de datos
                conn.Open();
                reader = cmd.ExecuteReader();


                var lista = new List<Sheet>();

                // comprobamos los datos            
                while (reader.Read())
                {
                    Sheet obj = new Sheet();
                    obj.id_sheet = Convert.ToInt32(reader["id_sheet"].ToString());
                    obj.title = reader["name"].ToString();
                    obj.href = reader["link"].ToString();
                    obj.icon = reader["icon"].ToString();
                    obj.id_branch = Convert.ToInt32(reader["id_branch"].ToString());
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



        //PADRES
        public List<Branch> branchPadre(int group_id)
        {
            SqlConnection conn;
            conn = dbproyecto.openConnection();
            try
            {
                string query;
                query = "SELECT * FROM web_branch" +
                        " INNER JOIN web_screen ON cast(web_screen.link as varchar(250))=cast(web_branch.link as varchar(250))" +
                        " WHERE web_branch.SOURCE IS NULL" +
                        " AND web_screen.group_id=@groups" +
                        " AND web_screen.status='true'" +
                        " ORDER BY web_branch.orden ASC";

                //cramos el comando
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                // seteamos los valores de la consulta
                // TODO propuesta modificacion un valor declarado int nunca podrá ser null, provocará error antes de llegar a esta validacion 

                cmd.Parameters.AddWithValue("groups", group_id == null ? 0 : group_id);
                SqlDataReader reader;

                //abrimos la conexion a la base de datos
                conn.Open();
                reader = cmd.ExecuteReader();


                var lista = new List<Branch>();

                // comprobamos los datos            
                while (reader.Read())
                {
                    Branch obj = new Branch();
                    obj.id_branch = Convert.ToInt32(reader["id_branch"].ToString());
                    obj.title = reader["name"].ToString();
                    obj.href = reader["link"].ToString();
                    obj.icon = reader["icon"].ToString();
                    obj.source = reader["source"].ToString();
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
        public List<Branch> branchPadresHijos(int group_id)
        {
            SqlConnection conn;
            conn = dbproyecto.openConnection();
            try
            {
                string query;
                query = "SELECT * FROM web_branch" +
                        " INNER JOIN web_screen ON cast(web_screen.link as varchar(250))=cast(web_branch.link as varchar(250))" +
                        " WHERE web_branch.SOURCE IS NOT NULL" +
                        " AND web_screen.group_id=@groups" +
                        " AND web_screen.status='true'" +
                        " ORDER BY web_branch.orden ASC";

                //cramos el comando
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                // seteamos los valores de la consulta
                // TODO propuesta modificacion un valor declarado int nunca podrá ser null, provocará error antes de llegar a esta validacion 
                cmd.Parameters.AddWithValue("groups", group_id == null ? 0 : group_id);
                SqlDataReader reader;

                //abrimos la conexion a la base de datos
                conn.Open();
                reader = cmd.ExecuteReader();


                var lista = new List<Branch>();

                // comprobamos los datos            
                while (reader.Read())
                {
                    Branch obj = new Branch();
                    obj.id_branch = Convert.ToInt32(reader["id_branch"].ToString());
                    obj.title = reader["name"].ToString();
                    obj.href = reader["link"].ToString();
                    obj.icon = reader["icon"].ToString();
                    obj.source = reader["source"].ToString();
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

        public List<datosUsuario> datosUsuario(string user_id)
        {
            SqlConnection conn;
            conn = dbproyecto.openConnection();
            try
            {
                string query;
                query = @" SELECT wu.USER_ID,wu.EMAIL,wu.USERNAME,wu.GROUP_ID,wu.FULLNAME, 
                          wu.AVATAR, wg.ADMINISTRADOR, wg.AUDITOR
                          FROM WEB_USERS wu 
                          INNER JOIN web_groups wg on wg.GROUP_ID = wu.GROUP_ID 
                          WHERE wu.USER_ID = @user_id ";

                //cramos el comando
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                // seteamos los valores de la consulta
                cmd.Parameters.AddWithValue("user_id", user_id == null ? "" : user_id);
                SqlDataReader reader;

                //abrimos la conexion a la base de datos
                conn.Open();
                reader = cmd.ExecuteReader();


                var lista = new List<datosUsuario>();

                // comprobamos los datos            
                while (reader.Read())
                {
                    datosUsuario obj = new datosUsuario();
                    obj.user_id = reader["USER_ID"].ToString();
                    obj.email = reader["email"].ToString();
                    obj.username = reader["username"].ToString();
                    obj.group_id = reader["group_id"].ToString();
                    obj.fullname = reader["fullname"].ToString();
                    obj.avatar = reader["avatar"].ToString();
                    obj.administrador = reader["administrador"].ToString();
                    obj.auditor = reader["auditor"].ToString();
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

        public object get_areas_user(string ID)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = " select " +
                               " area " +
                               " from dev_user_area " +
                               " WHERE user_id = '" + ID + "'";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<selector_editar> lista = new List<selector_editar>();

                while (reader.Read())
                {
                    selector_editar obj = new selector_editar();
                    obj.id = reader["area"].ToString();
                    obj.text = reader["area"].ToString();
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

        public List<datosUsuario> datosUsuarios(string[] usuarios)
        {

            try
            {
                var lista = new List<datosUsuario>();
                foreach (var item in usuarios)
                {
                    SqlConnection conn;
                    conn = dbproyecto.openConnection();
                    try
                    {

                        string query;
                        query = " SELECT EMAIL,USERNAME,GROUP_ID,FULLNAME,AVATAR, USER_ID FROM" +
                                " WEB_USERS WHERE USER_ID = '" + item + "'";

                        //cramos el comando
                        SqlCommand cmd = new SqlCommand(query);
                        cmd.Connection = conn;

                        // seteamos los valores de la consulta
                        SqlDataReader reader;

                        //abrimos la conexion a la base de datos
                        conn.Open();
                        reader = cmd.ExecuteReader();




                        // comprobamos los datos            
                        while (reader.Read())
                        {
                            datosUsuario obj = new datosUsuario();
                            obj.email = reader["email"].ToString();
                            obj.username = reader["username"].ToString();
                            obj.group_id = reader["group_id"].ToString();
                            obj.fullname = reader["fullname"].ToString();
                            obj.avatar = reader["avatar"].ToString();
                            obj.user_id = reader["user_id"].ToString();
                            lista.Add(obj);
                        }
                        reader.Close();
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
                return lista;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<datosUsuario> empresasUsuario(string user_id)
        {
            SqlConnection conn;
            conn = dbproyecto.openConnection();
            try
            {
                string query;
                query = " SELECT " +
                         " *FROM" +
                         " WEB_USERS U" +
                         " WHERE U.USER_ID = @user_id";

                //cramos el comando
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                // seteamos los valores de la consulta
                cmd.Parameters.AddWithValue("user_id", user_id == null ? "" : user_id);
                SqlDataReader reader;

                //abrimos la conexion a la base de datos
                conn.Open();
                reader = cmd.ExecuteReader();


                var lista = new List<datosUsuario>();

                // comprobamos los datos            
                while (reader.Read())
                {
                    datosUsuario obj = new datosUsuario();
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

        public object get_notificaciones(string user_id)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {
                //string query_areas = " select " +
                //               " area " +
                //               " from dev_user_area " +
                //               " WHERE user_id = '" + user_id + "'";

                //cmd.CommandText = query_areas;
                //cmd.ExecuteNonQuery();

                //SqlDataReader reader = cmd.ExecuteReader();

                //List<areas> areas_Obj = new List<areas>();
                //while (reader.Read())
                //{
                //    areas obj = new areas();
                //    obj.id = reader["area"].ToString();
                //    areas_Obj.Add(obj);
                //}
                //reader.Close();




                //string query = " select "+
                //               " wn.ID_NOTIFICACION, CONVERT(char(10), wn.FECHA,126) as FECHA, wn.NOTIFICACION " +
                //               " from web_notificacion wn " +
                //               " left join web_notificacion_area wna on wna.ID_NOTIFICACION = wn.ID_NOTIFICACION " +
                //               " left outer join web_not_user_view wnuv on wnuv.ID_NOTIFICACION = wn.ID_NOTIFICACION "+
                //               " where wn.user_id = '" + user_id + "' and wnuv.ID_NOTIFICACION is null ";

                //bool bandera = true;
                //decimal contador = areas_Obj.Count();
                //foreach (var accion in areas_Obj)
                //{

                //    string area = accion.id.ToString();
                //    if (bandera == true)
                //    {
                //        query += " AND ( ";
                //        bandera = false;
                //    }
                //    query += " wna.area = '" + area + "'";

                //    if (contador > 1)
                //    {
                //        query += " or ";
                //        contador--;
                //    }
                //}
                //if (bandera == false)
                //{
                //    query += " ) ";
                //}

                //query += " group by wn.ID_NOTIFICACION, wn.FECHA, wn.NOTIFICACION ";

                string query = $@"select we.id_empresa from web_empresa  we
                                join web_empresa_user weu on weu.ID_EMPRESA = we.id_empresa 
                                and weu.USER_ID = '{user_id}' and weu.DEFECTO= 'SI'";

                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();

                string empresa = "";
                while (reader.Read())
                {
                    empresa = reader["id_empresa"].ToString();
                }
                reader.Close();

                query = $@" select
                                wn.ID_NOTIFICACION,
                                CONVERT(nvarchar(10), wn.FECHA, 105) as FECHA,
                                wn.NOTIFICACION
                            from
                                web_notificacion wn
                                LEFT outer join web_not_user_view wnuv on wnuv.ID_NOTIFICACION = wn.ID_NOTIFICACION
                                AND WNUV.ID_EMPRESA = '{empresa}'
                            WHERE
                                wnuv.ID_NOTIFICACION is null
                                AND wn.ID_EMPRESA = '{empresa}'
                            union
                            select
                                wn.ID_NOTIFICACION,
                                CONVERT(nvarchar(10), wn.FECHA, 105) as FECHA,
                                wn.NOTIFICACION
                            from
                                web_notificacion wn
                                LEFT outer join web_not_user_view wnuv on wnuv.ID_NOTIFICACION = wn.ID_NOTIFICACION
                                AND WNUV.ID_EMPRESA = '{empresa}'
                            WHERE
                                wnuv.ID_NOTIFICACION is null
                                and wn.ID_EMPRESA = ''";

                cmd.Parameters.Clear();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                List<notificaciones> lista = new List<notificaciones>();

                SqlDataReader reader2 = cmd.ExecuteReader();

                while (reader2.Read())
                {
                    notificaciones obj = new notificaciones();
                    obj.id_notificacion = reader2["ID_NOTIFICACION"].ToString();
                    obj.fecha = reader2["FECHA"].ToString();
                    obj.notificacion = reader2["NOTIFICACION"].ToString();


                    lista.Add(obj);
                }
                reader2.Close();

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

        public int guardar_notificacion(string notificacion, string user_id, string id_empresa)
        {


            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;


            try
            {
                string query_notificacion = @"insert into web_notificacion (notificacion, user_id, fecha,id_empresa)
                                values (@notificacion,  @user_id, GETDATE(),@id_empresa) SELECT CAST(scope_identity() AS int) ";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("notificacion", notificacion);
                cmd.Parameters.AddWithValue("user_id", user_id == null ? "" : user_id);
                cmd.Parameters.AddWithValue("id_empresa", id_empresa);


                cmd.CommandText = query_notificacion;

                object result = cmd.ExecuteScalar();
                result = (result == DBNull.Value) ? null : result;
                int id_notificacion = Convert.ToInt32(result);

                transaccion.Commit();

                return id_notificacion;
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
        public object guardar_notificacion_area(int id_notificacion, List<selector_area> areas)
        {


            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;


            try
            {
                foreach (var accion in areas)
                {
                    string id_area = accion.value.ToString();

                    cmd.Parameters.Clear();
                    string query = "insert into web_notificacion_area (id_notificacion, area) " +
                                   " values (@id_notificacion, @area) ";

                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("id_notificacion", id_notificacion);
                    cmd.Parameters.AddWithValue("area", id_area);

                    cmd.ExecuteNonQuery();

                }

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

        public bool notificacion_visto(string id_notificacion, string user_id, string id_empresa)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;

            try
            {
                string query = "insert into web_not_user_view (id_notificacion, user_id, fecha, id_empresa) " +
                                " values (@id_notificacion, @user_id, SYSDATETIME(), @id_empresa) ";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("id_notificacion", id_notificacion);
                cmd.Parameters.AddWithValue("user_id", user_id == null ? "" : user_id);
                cmd.Parameters.AddWithValue("id_empresa", id_empresa);


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



        public string todosLosEmails()
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {

                string query = "SELECT " +
                               "STUFF((SELECT '; ' + wu.Email " +
                                "FROM[dbo].[web_users] wu " +
                                "where wu.ESTADO = 'Habilitado' " +
                                "group by wu.Email " +
                                "FOR XML PATH(''), TYPE) " +
                                ".value('.', 'NVARCHAR(MAX)'),1,2,' ') as correos ";

                cmd.Parameters.Clear();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                string correos = "";

                SqlDataReader reader2 = cmd.ExecuteReader();

                while (reader2.Read())
                {
                    correos = reader2["correos"].ToString();
                }

                reader2.Close();

                return correos;
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


        public List<string> emailsAdministradores()
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {
                string query = "select Email from web_users wu where GROUP_ID IN (select GROUP_ID from web_groups where ADMINISTRADOR='true')   ";
                cmd.Parameters.Clear();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                List<string> correos = new List<string>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    correos.Add(reader["Email"].ToString());
                }
                reader.Close();
                return correos;
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


        public string emailsAreaNorma(int COD_NORMA)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {
                List<string> correos = emailsAdministradores();
                string query = " select wu.Email from dev_norma_area dna" +
                                 " inner join dev_user_area dua on dna.NOM_AREA = dua.area " +
                                 " inner join web_users wu on dua.user_id = wu.USER_ID" +
                                 " where dna.COD_NORMA = '" + COD_NORMA + "'";

                cmd.Parameters.Clear();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    correos.Add(reader["Email"].ToString());
                }
                reader.Close();
                return quitarCorreosRepetidos(correos);
                // return correos;
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

        public string emailUnUsuario(string user_id)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {

                string query = "SELECT Email from web_users where USER_ID = '" + user_id + "'";

                cmd.Parameters.Clear();
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                string correos = "";

                SqlDataReader reader2 = cmd.ExecuteReader();

                while (reader2.Read())
                {
                    correos = reader2["Email"].ToString();
                }

                reader2.Close();

                return correos;
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


        public string quitarCorreosRepetidos(List<string> listaCorreos)
        {
            string correosSinRepetir = "";
            List<string> listaAux = new List<string>();

            for (int i = 0; i < listaCorreos.Count; i++)
            {
                if (!(listaAux.Contains(listaCorreos[i])))
                {
                    correosSinRepetir = correosSinRepetir + listaCorreos[i] + ";";
                    listaAux.Add(listaCorreos[i]);
                }
            }

            return correosSinRepetir.Substring(0, correosSinRepetir.Length - 1);
        }



    }
}