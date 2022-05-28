using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using proyecto.libraries;
using System.Data;
using System.Web.Script.Serialization;
using proyecto.objects;
using proyecto.objects.dev.mantenedores.Usuario;
using System.Web.Configuration;
using System.Security.Claims;
using System.Threading;

namespace proyecto.Models.web.mantenedores
{
    public class usuarioModel : Conexion
    {
        public object cargarDatos()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = @"SELECT u.USER_ID, 
                                 u.FULLNAME, 
                                 u.USERNAME, 
                                 g.NAME as PERFIL, 
                                 u.EMAIL, 
								 EMPRESA = STUFF(( 
											SELECT ', '  WE.NOMBRE 
											FROM WEB_USERS wu 
											left OUTER JOIN WEB_EMPRESA_USER WEU ON WEU.USER_ID = U.USER_ID 
											left OUTER JOIN WEB_EMPRESA WE ON WE.ID_EMPRESA = WEU.ID_EMPRESA 
											WHERE u.USER_ID = wu.USER_ID
											FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''),                      
                                 u.ESTADO 
                                 FROM WEB_USERS u 
                                 INNER JOIN WEB_GROUPS g ON g.GROUP_ID = u.GROUP_ID
                                 ORDER BY u.fullname ASC";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<Data> lista = new List<Data>();

                while (reader.Read())
                {
                    Data obj = new Data();
                    obj.ID = reader["USER_ID"].ToString();
                    obj.FULLNAME = reader["FULLNAME"].ToString();
                    obj.USERNAME = reader["USERNAME"].ToString();
                    obj.PERFIL = reader["PERFIL"].ToString();
                    obj.EMAIL = reader["EMAIL"].ToString();
                    obj.EMPRESA = reader["EMPRESA"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
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
        public object nombresUsuarios()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select  USERNAME as name , USER_ID as code from WEB_USERS ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<selector> lista = new List<selector>();

                while (reader.Read())
                {
                    selector obj = new selector();

                    obj.name = reader["name"].ToString();
                    obj.code = reader["code"].ToString();

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
        public object areas()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "select  nombre as name , nombre as code from dev_area where estado = 'Habilitado' ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<selector> lista = new List<selector>();

                while (reader.Read())
                {
                    selector obj = new selector();

                    obj.name = reader["name"].ToString();
                    obj.code = reader["code"].ToString();

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

        public object cargaUsuarioRecupera()
        {
            SqlConnection conn = dbproyecto.openConnection();
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var userid = identity.Claims.Where(c => c.Type == "user_id")
                .Select(c => c.Value).SingleOrDefault();

            try
            {
                string query = @"SELECT *from web_users where user_id = @user_id";

                SqlCommand cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("user_id", userid);

                cmd.Connection = conn;
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                List<UsuarioFullData> lista = new List<UsuarioFullData>();


                reader.Close();
                return lista;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public object perfiles()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT GROUP_ID AS code,NAME AS name, MULTI_EMPRESA,ADMINISTRADOR,AUDITOR FROM WEB_GROUPS ";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<selectorPerfil> lista = new List<selectorPerfil>();

                while (reader.Read())
                {
                    selectorPerfil obj = new selectorPerfil();

                    obj.name = reader["name"].ToString();
                    obj.code = reader["code"].ToString();
                    obj.administrador = reader["administrador"].ToString();
                    obj.auditor = reader["auditor"].ToString();
                    obj.multi_empresa = reader["multi_empresa"].ToString();
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
        public object empresas()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT ID_EMPRESA AS code,NOMBRE AS name FROM WEB_EMPRESA ORDER BY NOMBRE ASC";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<selector> lista = new List<selector>();

                while (reader.Read())
                {
                    selector obj = new selector();

                    obj.name = reader["name"].ToString();
                    obj.code = reader["code"].ToString();

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
        public bool guardarUserOld(UsuarioFullData request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                string query;
                //se almacenan los datos del usuario en la tabla user
                //---------------------------------------------------------------------
                Encrypt encr = new Encrypt();
                query = "INSERT INTO web_users (username, PASSWORDHASH, fullname, email, avatar, estado, group_id) " +
                        " VALUES (@username,@pass,@nombre,@email,@avatar,@estado,@perfil) ";
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("username", request.nombreUsuario.ToUpper());
                cmd.Parameters.AddWithValue("pass", encr.encriptar(request.contraseña));
                cmd.Parameters.AddWithValue("nombre", request.nombre == null ? "" : request.nombre);
                cmd.Parameters.AddWithValue("email", request.email);
                cmd.Parameters.AddWithValue("avatar", string.IsNullOrWhiteSpace(request.avatar) ? "" : request.avatar);
                cmd.Parameters.AddWithValue("estado", request.estado == null ? "" : request.estado);
                cmd.Parameters.AddWithValue("perfil", request.perfil == null ? "" : request.perfil);



                //SqlParameter outputParameter = new SqlParameter("user_id", SqlDbType.Int);
                //outputParameter.Direction = ParameterDirection.Output;
                //cmd.Parameters.Add(outputParameter);
                cmd.ExecuteNonQuery();

                //decimal USER_ID = Convert.ToDecimal(cmd.Parameters["user_id"].Value);
                int USER_ID = Convert.ToInt32(cmd.ExecuteScalar());
                //se insertan las diferentes EMPRESAS del usuario

                JavaScriptSerializer js = new JavaScriptSerializer();
                selector[] empresaUser = js.Deserialize<selector[]>(request.empresa);

                foreach (selector empresa in empresaUser)
                {
                    cmd.Parameters.Clear();
                    string ID_EMPRESA = empresa.code;
                    string DEFECTO = "";
                    if (ID_EMPRESA == request.empresa_defecto)
                    {
                        DEFECTO = "SI";
                    }
                    query = "INSERT INTO WEB_EMPRESA_USER (USER_ID,ID_EMPRESA,DEFECTO) VALUES (@USER_ID,@ID_EMPRESA,@DEFECTO )";
                    cmd.Parameters.AddWithValue("USER_ID", USER_ID);
                    cmd.Parameters.AddWithValue("ID_EMPRESA", ID_EMPRESA);
                    cmd.Parameters.AddWithValue("DEFECTO", DEFECTO);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                transaccion.Commit();
                conn.Close();

                return true;
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        public bool correoRecuperaPass(string email, string codigo, string caso, string userName)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {

                var procedure = new Dictionary<string, string>
                 {
                     {"recuperar", "SP_SGI_RECOVERY_PASS"},
                     {"crear", "SP_SGI_CREAR_PASS"},
                 };

                cmd.CommandText = procedure[caso];
                string url = WebConfigurationManager.AppSettings["URLrecuperacion"];


                url += codigo;
                if (caso == "crear")
                {
                    cmd.Parameters.AddWithValue("userName", caso == "crear" ? userName : "");
                }
                cmd.Parameters.AddWithValue("emails", email);
                cmd.Parameters.AddWithValue("URL", url);
                cmd.ExecuteNonQuery();


                return true;
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion

                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }

        }

        public bool guardarUser(ApplicationUser request, string empresas, string empresa_defecto, string email, string pass)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                string queryUs;
                string query;
                queryUs = @"INSERT INTO web_user_ingreso (USER_ID_INGRESO, INGRESO)
                          VALUES (@USER_ID, 'NO')";

                //se almacenan los datos del usuario en la tabla user
                //---------------------------------------------------------------------
                // Encrypt encr = new Encrypt();
                // query = "INSERT INTO web_users (USER_ID, username, PASSWORDHASH, fullname, email, avatar, estado, group_id) " +
                //         " VALUES (@USER_ID,@username,@pass,@nombre,@email,@avatar,@estado,@perfil) ";
                cmd.Parameters.AddWithValue("USER_ID", request.Id);
                cmd.CommandText = queryUs;

                // cmd.Parameters.AddWithValue("USER_ID", request.Id);
                // cmd.Parameters.AddWithValue("username", request.UserName.ToUpper());
                // cmd.Parameters.AddWithValue("pass", encr.encriptar(request.PasswordHash));
                // cmd.Parameters.AddWithValue("nombre", request.FULLNAME == null ? "" : request.FULLNAME);
                // cmd.Parameters.AddWithValue("email", request.Email);
                // cmd.Parameters.AddWithValue("avatar", string.IsNullOrWhiteSpace(request.AVATAR) ? "" : request.AVATAR);
                // cmd.Parameters.AddWithValue("estado", request.ESTADO == null ? "" : request.ESTADO);
                // cmd.Parameters.AddWithValue("perfil", request.GROUP_ID == null ? "" : request.GROUP_ID);

                cmd.ExecuteNonQuery();

                // int USER_ID = Convert.ToInt32(cmd.ExecuteScalar());

                //se insertan las diferentes EMPRESAS del usuario

                JavaScriptSerializer js = new JavaScriptSerializer();
                selector[] empresaUser = js.Deserialize<selector[]>(empresas);


                foreach (selector empresa in empresaUser)
                {
                    cmd.Parameters.Clear();
                    string ID_EMPRESA = empresa.code;
                    string DEFECTO = "";
                    if (ID_EMPRESA == empresa_defecto)
                    {
                        DEFECTO = "SI";
                    }
                    query = "INSERT INTO WEB_EMPRESA_USER (USER_ID,ID_EMPRESA,DEFECTO) VALUES (@USER_ID,@ID_EMPRESA,@DEFECTO )";
                    cmd.Parameters.AddWithValue("USER_ID", request.Id);
                    cmd.Parameters.AddWithValue("ID_EMPRESA", ID_EMPRESA);
                    cmd.Parameters.AddWithValue("DEFECTO", DEFECTO);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                transaccion.Commit();

                conn.Close();

                return true;
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        public bool habilitarDeshabilitar(habilitarDeshabilitar request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;


            //TRACKING
            Tracking t = new Tracking();
            string queryIn = " SELECT u.username," +
                          " u.fullname," +
                          " u.email," +
                          " u.avatar," +
                          " u.rut," +
                          " u.sexo," +
                          " u.telefono," +
                          " u.celular," +
                          " u.direccion," +
                          " u.fecha_nacimiento," +
                          " wp.nombre AS empresa," +
                          " u.estado" +
                        " FROM web_users u" +
                        " LEFT JOIN web_groups g" +
                        " ON g.group_id = u.group_id" +
                        " LEFT OUTER JOIN WEB_EMPRESA_USER WPU" +
                        " ON WPU.USER_ID = U.USER_ID" +
                        " LEFT OUTER JOIN WEB_EMPRESA WP" +
                        " ON WP.ID_EMPRESA = WPU.ID_EMPRESA" +
                        " WHERE u.user_id in (" + request.ID + ") ";

            try
            {
                //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
                List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

                string query = "";

                query = "UPDATE web_users SET estado = '" + request.estado + "' WHERE USER_ID IN (" + request.ID + ") ";


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                transaccion.Commit();
                var accion = request.estado == "Habilitado" ? "Habilitar" : "Deshabilitar";
                //GUARDAMOS EL TRACKING
                t.guardarTracking(accion, "Usuario", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));


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
        public object cargarEditar(string ID)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = "SELECT " +
                                " u.USER_ID," +
                                " u.FULLNAME," +
                                " u.USERNAME," +
                                " u.AVATAR," +
                                " u.GROUP_ID as id_perfil," +
                                " g.NAME as perfil," +
                                " g.MULTI_EMPRESA," +
                                " g.administrador," +
                                " g.auditor," +
                                " u.EMAIL," +
                                " u.ESTADO" +
                                " FROM WEB_USERS u" +
                                " INNER JOIN WEB_GROUPS g ON g.GROUP_ID = u.GROUP_ID" +
                                " WHERE u.USER_ID = '" + ID + "'";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<UsuarioFullData> lista = new List<UsuarioFullData>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                while (reader.Read())
                {
                    UsuarioFullData obj = new UsuarioFullData();
                    obj.nombre = reader["FULLNAME"].ToString();
                    obj.nombreUsuario = reader["USERNAME"].ToString();
                    obj.perfil = reader["perfil"].ToString();
                    obj.id_perfil = reader["id_perfil"].ToString();
                    obj.multi_empresa = reader["MULTI_EMPRESA"].ToString();
                    obj.administrador = reader["administrador"].ToString();
                    obj.auditor = reader["auditor"].ToString();
                    obj.email = reader["EMAIL"].ToString();
                    obj.estado = reader["ESTADO"].ToString();
                    obj.avatar = reader["AVATAR"].ToString();
                    obj.empresa = js.Serialize(empresasUser(ID));
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
        public object empresasUser(string ID)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = "SELECT pu.ID_EMPRESA, pu.DEFECTO,p.NOMBRE FROM WEB_EMPRESA_USER pu " +
                               "INNER JOIN WEB_EMPRESA p ON p.ID_EMPRESA = pu.ID_EMPRESA WHERE pu.USER_ID = '" + ID + "' ORDER BY pu.ID_EMPRESA ASC";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<empresasUser> lista = new List<empresasUser>();

                while (reader.Read())
                {
                    empresasUser obj = new empresasUser();

                    obj.code = reader["ID_EMPRESA"].ToString();
                    obj.name = reader["NOMBRE"].ToString();
                    obj.defecto = reader["DEFECTO"].ToString();

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

        public object eliminar(string ID)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = default(SqlTransaction);
            transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                string query;
                //se eliminan los datos del usuario en la tabla user y empresa_user
                //---------------------------------------------------------------------              

                query = "DELETE FROM WEB_EMPRESA_USER WHERE USER_ID in (" + ID + ")";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();

                query = "DELETE FROM web_users WHERE USER_ID in (" + ID + ") ";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                query = "DELETE FROM web_user_ingreso WHERE USER_ID_INGRESO in (" + ID + ")";
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

        public object getByUser(string username)
        {
            try
            {
                SqlConnection conn = dbproyecto.openConnection();
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;

                string query = "SELECT wu.user_id from web_users wu where wu.username = @username";


                cmd.Parameters.AddWithValue("username", username);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();

                UsuarioFullData user = new UsuarioFullData();
                while (reader.Read())
                {

                    user.ID = reader["user_id"].ToString();

                }
                reader.Close();
                return user.ID;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public object resetear(ChangePasswordBindingModel request)
        {


            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            try
            {

                SqlCommand command = new SqlCommand("SP_SGI_RANDOM_PASS", conn);
                command.CommandType = CommandType.StoredProcedure;

                string url = WebConfigurationManager.AppSettings["baseURL"];

                command.Parameters.AddWithValue("password", request.NewPassword);
                command.Parameters.AddWithValue("emails", request.Email);
                command.Parameters.AddWithValue("URL", url);
                command.ExecuteNonQuery();


                return true;
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

        public object editar(UsuarioFullData request)
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
            string queryIn = " SELECT u.username," +
                          " u.fullname," +
                          " u.email," +
                          " u.avatar," +
                          " u.rut," +
                          " u.sexo," +
                          " u.telefono," +
                          " u.celular," +
                          " u.direccion," +
                          " u.fecha_nacimiento," +
                          " wp.nombre AS empresa," +
                          " u.estado" +
                        " FROM web_users u" +
                        " LEFT JOIN web_groups g" +
                        " ON g.group_id = u.group_id" +
                        " LEFT OUTER JOIN WEB_EMPRESA_USER WPU" +
                        " ON WPU.USER_ID = U.USER_ID" +
                        " LEFT OUTER JOIN WEB_EMPRESA WP" +
                        " ON WP.ID_EMPRESA = WPU.ID_EMPRESA" +
                        " WHERE u.user_id = '" + request.ID + "' ";
            try
            {
                //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
                List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

                string query = "select USERNAME from WEB_USERS where UPPER(USERNAME)=UPPER(@username) AND USER_ID!=@user_id";

                cmd.Parameters.AddWithValue("username", request.nombreUsuario);
                cmd.Parameters.AddWithValue("user_id", request.ID);
                cmd.CommandText = query;
                //comprueba si existe otro username igual
                SqlDataReader reader = cmd.ExecuteReader();
                int afectada = 0;
                while (reader.Read())
                {
                    afectada = 1;
                }
                reader.Close();
                cmd.Parameters.Clear();
                if (afectada == 0)
                {

                    //se almacenan los datos del usuario en la tabla user
                    //---------------------------------------------------------------------

                    query = "UPDATE web_users SET ";
                    if (!string.IsNullOrWhiteSpace(request.nombreUsuario))
                    {
                        query += "username = @username ,";
                    }

                    if (!string.IsNullOrWhiteSpace(request.nombre))
                    {
                        query += " fullname = @nombre,";
                    }
                    query += " email =@email, avatar=@avatar, estado=@estado, group_id=@perfil WHERE USER_ID=@ID ";
                    cmd.CommandText = query;

                    //decimal USER_ID = Convert.ToDecimal(request.ID);
                    if (!string.IsNullOrWhiteSpace(request.nombreUsuario))
                    {
                        cmd.Parameters.AddWithValue("username", request.nombreUsuario.ToUpper());
                    }


                    if (!string.IsNullOrWhiteSpace(request.nombre))
                    {
                        cmd.Parameters.AddWithValue("nombre", request.nombre == null ? "" : request.nombre);
                    }
                    cmd.Parameters.AddWithValue("email", string.IsNullOrWhiteSpace(request.email) ? "" : request.email);
                    cmd.Parameters.AddWithValue("avatar", string.IsNullOrWhiteSpace(request.avatar) ? "" : request.avatar);
                    cmd.Parameters.AddWithValue("estado", request.estado == null ? "" : request.estado);
                    cmd.Parameters.AddWithValue("perfil", request.perfil == null ? "" : request.perfil);
                    cmd.Parameters.AddWithValue("ID", request.ID);

                    cmd.ExecuteNonQuery();

                    //se eliminan las empresas del usuario
                    cmd.Parameters.Clear();

                    query = "DELETE FROM WEB_EMPRESA_USER  WHERE USER_ID=@USER_ID";
                    cmd.Parameters.AddWithValue("USER_ID", request.ID);
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();


                    //se insertan las diferentes EMPRESAS del usuario
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    selector[] empresaUser = js.Deserialize<selector[]>(request.empresa);

                    foreach (selector empresa in empresaUser)
                    {
                        cmd.Parameters.Clear();
                        string ID_EMPRESA = empresa.code;
                        string DEFECTO = "";
                        if (ID_EMPRESA == request.empresa_defecto)
                        {
                            DEFECTO = "SI";
                        }
                        query = "INSERT INTO WEB_EMPRESA_USER (USER_ID,ID_EMPRESA,DEFECTO) VALUES (@USER_ID,@ID_EMPRESA,@DEFECTO )";
                        cmd.Parameters.AddWithValue("USER_ID", request.ID);
                        cmd.Parameters.AddWithValue("ID_EMPRESA", ID_EMPRESA);
                        cmd.Parameters.AddWithValue("DEFECTO", DEFECTO);
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }


                    transaccion.Commit();

                    //GUARDAMOS EL TRACKING
                    t.guardarTracking("Editar", "Usuario", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //deshacemos la transaccion
                transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public object cargarNombre(String nombre)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {

                string query = "select  USERNAME from WEB_USERS where UPPER(USERNAME) like UPPER('%" + nombre + "%')";

                cmd.Parameters.AddWithValue("nombre", nombre);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader = cmd.ExecuteReader();
                List<String> lista = new List<String>();

                while (reader.Read())
                {
                    String obj;

                    obj = reader["USERNAME"].ToString();


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
        public object cargarPerfil(String perfil)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {

                String query = "SELECT NAME FROM WEB_GROUPS where UPPER(NAME) like UPPER('%" + perfil + "%') GROUP BY NAME";


                cmd.Parameters.AddWithValue("perfil", perfil);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader = cmd.ExecuteReader();
                List<string> lista = new List<string>();

                while (reader.Read())
                {
                    String obj;

                    obj = reader["NAME"].ToString();
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
        public object filtrar(Data datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {

                string query = " SELECT u.USER_ID, " +
                                "  u.FULLNAME, " +
                                 " u.USERNAME, " +
                                 " g.NAME as PERFIL, " +
                                 " u.EMAIL, " +
                                 " EMPRESA = STUFF(( " +
                                     "		SELECT ', ' + WE.NOMBRE " +
                                         "	FROM WEB_USERS wu " +
                                         "	left OUTER JOIN WEB_EMPRESA_USER WEU ON WEU.USER_ID = U.USER_ID " +
                                         "	left OUTER JOIN WEB_EMPRESA WE ON WE.ID_EMPRESA = WEU.ID_EMPRESA " +
                                         "	WHERE u.USER_ID = wu.USER_ID" +
                                         "	FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, ''), " +
                                "  u.ESTADO " +
                                "  FROM WEB_USERS u " +
                                "  INNER JOIN WEB_GROUPS g ON g.GROUP_ID = u.GROUP_ID where 1=1  ";
                if (datos.USERNAME != null)
                {
                    query += " and u.USERNAME = @username ";
                    cmd.Parameters.AddWithValue("username", datos.USERNAME);

                }
                if (datos.PERFIL != null)
                {
                    query += " and g.NAME = @perfil ";
                    cmd.Parameters.AddWithValue("perfil", datos.PERFIL);
                }
                if (datos.ESTADO != null)
                {
                    query += " and ESTADO = @estado ";
                    cmd.Parameters.AddWithValue("estado", datos.ESTADO);

                }
                query += "  ORDER BY u.fullname ASC";

                //cmd.Parameters.AddWithValue("username", datos.USERNAME);
                //cmd.Parameters.AddWithValue("perfil", datos.PERFIL);
                //cmd.Parameters.AddWithValue("estado", datos.ESTADO);


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();

                List<Data> lista = new List<Data>();

                while (reader.Read())
                {
                    Data obj = new Data();
                    obj.ID = reader["USER_ID"].ToString();
                    obj.FULLNAME = reader["FULLNAME"].ToString();
                    obj.USERNAME = reader["USERNAME"].ToString();
                    obj.PERFIL = reader["PERFIL"].ToString();
                    obj.EMAIL = reader["EMAIL"].ToString();
                    obj.EMPRESA = reader["EMPRESA"].ToString();
                    obj.ESTADO = reader["ESTADO"].ToString();
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

        public object comprobarPassword(string user_id, string new_password)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            try
            {
                string OBTENER_PASS_ANTIGUOS = @" SELECT user_id , password FROM DEV_PASS_HiSTORY where user_id = @user_id";
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.CommandText = OBTENER_PASS_ANTIGUOS;
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();

                List<UsuarioPass> oldPasswords = new List<UsuarioPass>();

                while (reader.Read())
                {
                    UsuarioPass usuarioPass = new UsuarioPass();
                    usuarioPass.user_id = reader["USER_ID"].ToString();
                    usuarioPass.password = reader["PASSWORD"].ToString();
                    oldPasswords.Add(usuarioPass);
                }
                reader.Close();

                bool existe = oldPasswords.Exists(x => x.password == new_password);

                return existe;
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

        public object passHistory(string user_id, string new_password)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {
                // 1 get passHistory 
                string GET_PASS_HISTORY = @"SELECT password, created_at from DEV_PASS_HiSTORY WHERE user_id = @user_id";
                cmd.CommandText = GET_PASS_HISTORY;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("user_id", user_id);
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                List<UsuarioPass> oldUserPass = new List<UsuarioPass>();
                while (reader.Read())
                {
                    UsuarioPass obj = new UsuarioPass();
                    obj.password = reader["password"].ToString();
                    //obj.creted_at = reader["creted_at"].ToString();


                    oldUserPass.Add(obj);
                }
                reader.Close();
                string REGISTRAR_PASS;
                // 2 si <= 4 => insert

                if (oldUserPass.Count <= 4)
                {
                    REGISTRAR_PASS = @"insert into DEV_PASS_HiSTORY (user_id, password, created_at)
                                        values(@user_id, @password, GETUTCDATE())";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("user_id", user_id);
                    cmd.Parameters.AddWithValue("password", new_password);
                    cmd.CommandText = REGISTRAR_PASS;
                    cmd.ExecuteNonQuery();

                }
                // 3 si == 4 => insert && delete oldest.
                if (oldUserPass.Count == 4)
                {
                    string OLD_HISTORY = @"SELECT TOP 1 password
                                                  ,MAX(created_at)
                                                  ,id
                                              FROM  DEV_PASS_HISTORY
                                              where user_id = @user_id
                                              group by password,id
                                              order by 2 asc";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("user_id", user_id);
                    cmd.CommandText = OLD_HISTORY;
                    cmd.ExecuteNonQuery();
                    SqlDataReader leer = cmd.ExecuteReader();

                    List<UsuarioPass> usersPass = new List<UsuarioPass>();
                    while (leer.Read())
                    {
                        UsuarioPass usuarioPass = new UsuarioPass();
                        usuarioPass.ID = leer["id"].ToString();
                        //----delete
                        usersPass.Add(usuarioPass);

                    }
                    leer.Close();
                    foreach (var item in usersPass)
                    {
                        cmd.CommandText = @"DELETE
                                              FROM  DEV_PASS_HiSTORY
                                              where ID = @ID
                                             ";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("ID", item.ID);
                        cmd.ExecuteNonQuery();
                    }

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

        public object buscarEmail(string email)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = @"SELECT wu.USER_ID, wu.USERNAME, wu.FULLNAME, wu.EMAIL
                                 FROM web_users wu where wu.EMAIL = @email";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<Data> lista = new List<Data>();

                while (reader.Read())
                {
                    Data obj = new Data();
                    obj.ID = reader["USER_ID"].ToString();
                    obj.USERNAME = reader["USERNAME"].ToString();
                    obj.FULLNAME = reader["FULLNAME"].ToString();
                    obj.EMAIL = reader["EMAIL"].ToString();


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
}