using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using proyecto.libraries;
using System.Data;
using System.Web.Script.Serialization;

namespace proyecto.Models.web.mantenedores
{
    public class EmpresaModel : Conexion
    {
        public object cargarDatos()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {

                string query = " SELECT  ID_IDENTITY, ID_EMPRESA, NOMBRE, DESCRIPCION, ESTADO  ,FECHA_CREATE  ,FECHA_UPDATE   " +
                               " FROM WEB_EMPRESA ORDER BY ID_EMPRESA, NOMBRE ASC ";

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<DatosEmpresa> lista = new List<DatosEmpresa>();

                while (reader.Read())
                {
                    DatosEmpresa obj = new DatosEmpresa();
                    obj.id = Int32.Parse(reader["ID_IDENTITY"].ToString());
                    obj.codigo = reader["ID_EMPRESA"].ToString();
                    obj.empresa = reader["NOMBRE"].ToString();
                    obj.descripcion = reader["DESCRIPCION"].ToString();
                    obj.fecha_create = reader["FECHA_CREATE"].ToString();
                    obj.fecha_update = reader["FECHA_UPDATE"].ToString();
                    obj.estado = reader["ESTADO"].ToString();
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

        public bool existeEmpresa(empresaFullData request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = "SELECT id_empresa FROM web_EMPRESA where UPPER(id_empresa) = @id_empresa ";

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("id_empresa", request.codigo.ToUpper());
                //cmd.Parameters.AddWithValue("nombre", request.empresa.ToUpper());

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                bool existe = false;

                while (reader.Read())
                {
                    existe = true;
                }
                reader.Close();
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
        public bool guardarEmpresa(empresaFullData request)
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

                query = "INSERT INTO web_EMPRESA (id_empresa, nombre, fecha_create, fecha_update, estado) " +
                        " VALUES (@id_empresa,@nombre,FORMAT(GETDATE() , 'dd/MM/yyyy HH:mm:ss'), FORMAT(GETDATE() , 'dd/MM/yyyy HH:mm:ss'), @estado) ";
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("id_empresa", request.codigo == null ? "" : request.codigo);
                cmd.Parameters.AddWithValue("nombre", request.empresa == null ? "" : request.empresa);
                cmd.Parameters.AddWithValue("estado", request.estado == null ? "" : request.estado);

                cmd.ExecuteNonQuery();

                string ID_EMPRESA = request.codigo;
                cmd.Parameters.Clear();

                //query = " INSERT INTO DEV_DURACION_ETAPAS (ID_EMPRESA,ETAPA,DURACION) " +
                //        " (SELECT '" + ID_EMPRESA + "', etapa, '0' FROM DEV_ETAPAS_CSC) ";

                //cmd.CommandText = query;
                //cmd.ExecuteNonQuery();

                transaccion.Commit();
                //TRACKING
                Tracking t = new Tracking();
                string queryIn = "SELECT id_empresa,nombre, descripcion, estado FROM web_EMPRESA where id_empresa = '" + request.codigo + "'";
                //GUARDAMOS EL TRACKING
                t.guardarTracking("Crear", "Empresa", t.obtenerDatosNuevosyAntiguos(queryIn));
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
        public object eliminar(string ID, string codigo)
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
            string queryIn = "SELECT id_empresa,nombre, descripcion, estado FROM web_EMPRESA where id_identity  in (" + ID + ")";

            try
            {
                //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
                List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

                string query;

                query = " DELETE FROM WEB_EMPRESA_USER WHERE ID_EMPRESA in (" + codigo + ")";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();

                query = "DELETE FROM WEB_EMPRESA WHERE id_identity in (" + ID + ")";
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                transaccion.Commit();

                //GUARDAMOS EL TRACKING
                t.guardarTracking("Eliminar", "Empresa", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));
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
                string query = "SELECT ID_IDENTITY, " +
                                " ID_EMPRESA," +
                                " NOMBRE," +
                                " DESCRIPCION," +
                                " ESTADO " +
                                " FROM WEB_EMPRESA " +
                                " WHERE ID_EMPRESA = '" + ID + "'";


                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<empresaFullData> lista = new List<empresaFullData>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                while (reader.Read())
                {
                    empresaFullData obj = new empresaFullData();
                    obj.id_empresa = Int32.Parse(reader["ID_IDENTITY"].ToString());
                    obj.codigo = reader["ID_EMPRESA"].ToString();
                    obj.empresa = reader["NOMBRE"].ToString();
                    obj.descripcion = reader["DESCRIPCION"].ToString();
                    obj.estado = reader["ESTADO"].ToString();
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

        public bool existeEmpresaEditar(empresaFullData request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = "SELECT id_empresa, id_identity FROM web_EMPRESA where UPPER(id_empresa) = @id_empresa ";

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("id_empresa", request.codigo.ToUpper());

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<empresaFullData> lista = new List<empresaFullData>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                while (reader.Read())
                {
                    empresaFullData obj = new empresaFullData();
                    obj.id_empresa = Int32.Parse(reader["ID_IDENTITY"].ToString());
                    obj.codigo = reader["ID_EMPRESA"].ToString();
                    lista.Add(obj);
                }
                reader.Close();

                bool existe = false;
                foreach (var e in lista)
                {
                    if (e.id_empresa == request.id_empresa)
                    {
                        existe = false;
                    }
                    else if (e.id_empresa != request.id_empresa)
                    {
                        existe = true;
                    }

                }

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


        public object habilitarDeshabilitar(habilitarDeshabiltiar_empresa request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            //TRACKING
            Tracking t = new Tracking();
            string queryIn = "SELECT id_empresa,nombre, descripcion, estado FROM web_EMPRESA where id_empresa in (" + request.ID + ")";
            try
            {
                //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
                List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

                string query = "UPDATE WEB_EMPRESA SET ESTADO  = '" + request.estado + "' where ID_EMPRESA  in (" + request.ID + ") ";
                //cmd.Parameters.AddWithValue("estado", request.estado);
                //cmd.Parameters.AddWithValue("id_empresa", request.ID);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                transaccion.Commit();

                var accion = request.estado == "Habilitado" ? "Habilitar" : "Deshabilitar";
                //GUARDAMOS EL TRACKING
                t.guardarTracking(accion, "Empresa", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

                return "ok";
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

        public object SelectCargarEmpresaSelector()
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                string query = @"SELECT NOMBRE as label,
                                    ID_EMPRESA as value
                                    FROM web_empresa
                                    group by ID_EMPRESA, NOMBRE";

                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = conn;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<SelectorEMPRESA> lista = new List<SelectorEMPRESA>();

                while (reader.Read())
                {
                    SelectorEMPRESA obj = new SelectorEMPRESA();
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

        public object cargar_empresa(string empresa)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {

                string query = "select nombre from web_EMPRESA where nombre like UPPER('%" + empresa + "%')";


                //cmd.Parameters.AddWithValue("empresa", empresa);
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader = cmd.ExecuteReader();
                List<String> lista = new List<String>();

                while (reader.Read())
                {
                    String obj;
                    obj = reader["nombre"].ToString();
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

        public object filtrar(empresaFullData datos)
        {
            SqlConnection conn = dbproyecto.openConnection();
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            SqlTransaction transaccion = conn.BeginTransaction();
            cmd.Connection = conn;
            cmd.Transaction = transaccion;
            try
            {

                string query = " SELECT  ID_IDENTITY, ID_EMPRESA, NOMBRE, DESCRIPCION, ESTADO  ,FECHA_CREATE  ,FECHA_UPDATE   " +
                              " FROM WEB_EMPRESA  where 1 = 1";

                if (datos.empresa != null)
                {
                    query += " and nombre = @empresa ";
                    cmd.Parameters.AddWithValue("empresa", datos.empresa);

                }
                if (datos.estado != null)
                {
                    query += " and estado = @estado ";
                    cmd.Parameters.AddWithValue("estado", datos.estado);

                }
                query += " ORDER BY ID_EMPRESA, NOMBRE ASC  ";


                cmd.CommandText = query;
                cmd.ExecuteNonQuery();

                SqlDataReader reader = cmd.ExecuteReader();
                List<DatosEmpresa> lista = new List<DatosEmpresa>();

                while (reader.Read())
                {
                    DatosEmpresa obj = new DatosEmpresa();
                    obj.id = Int32.Parse(reader["ID_IDENTITY"].ToString());
                    obj.codigo = reader["ID_EMPRESA"].ToString();
                    obj.empresa = reader["NOMBRE"].ToString();
                    obj.descripcion = reader["DESCRIPCION"].ToString();
                    obj.fecha_create = reader["FECHA_CREATE"].ToString();
                    obj.fecha_update = reader["FECHA_UPDATE"].ToString();
                    obj.estado = reader["ESTADO"].ToString();
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

        public object listaEmpresasUsuario(String userID)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string query = @"SELECT we.ID_EMPRESA as idEmpresa, we.NOMBRE as nombreEmpresa
                                    FROM web_empresa_user weu
                                    JOIN web_empresa we
                                    ON we.ID_EMPRESA = weu.ID_EMPRESA
                                where weu.USER_ID = @userID";

                cmd.Parameters.AddWithValue("@userID", userID);

                cmd.CommandText = query;
                cmd.Connection = conn;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<menuListaEmpresas> lista = new List<menuListaEmpresas>();

                while (reader.Read())
                {
                    menuListaEmpresas obj = new menuListaEmpresas();

                    obj.value = reader["idEmpresa"].ToString();
                    obj.label = reader["nombreEmpresa"].ToString();

                    lista.Add(obj);
                }
                reader.Close();
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

        public bool editar(empresaFullData request)
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
            string queryIn = "SELECT id_empresa,nombre, descripcion, estado FROM web_EMPRESA where id_empresa = '" + request.codigo + "'";
            try
            {
                //OBTENGO DATOS ANTIGUO REGISTRO TRACKING
                List<IDictionary<string, object>> antiguo = t.obtenerAntiguo(queryIn);

                string query;

                query = " UPDATE WEB_EMPRESA SET ID_EMPRESA =@codigo, NOMBRE =@nombre, fecha_update = FORMAT(GETDATE() , 'dd/MM/yyyy HH:mm:ss'), ESTADO =@estado  " +
                        " WHERE ID_IDENTITY=@ID ";
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("nombre", request.empresa == null ? "" : request.empresa);
                cmd.Parameters.AddWithValue("estado", request.estado == null ? "" : request.estado);
                cmd.Parameters.AddWithValue("codigo", request.codigo);
                cmd.Parameters.AddWithValue("ID", request.id_empresa);

                cmd.ExecuteNonQuery();

                transaccion.Commit();

                //GUARDAMOS EL TRACKING
                t.guardarTracking("Editar", "Empresa", t.obtenerDatosNuevosyAntiguos(queryIn, antiguo));

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

        public object cambiarEmpresaDefecto(updateDefaultEmpresa request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                SqlTransaction transaccion = conn.BeginTransaction();
                string query = @"UPDATE web_empresa_user SET DEFECTO = 'SI' where USER_ID = @idUser and ID_EMPRESA = @idEmpresa;";

                cmd.Parameters.AddWithValue("@idEmpresa", request.empresa_id);
                cmd.Parameters.AddWithValue("@idUser", request.user_id);

                cmd.CommandText = query;
                cmd.Connection = conn;
                cmd.Transaction = transaccion;
                cmd.ExecuteNonQuery();

                transaccion.Commit();

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
        public object resetEmpresaDefecto(updateDefaultEmpresa request)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                SqlTransaction transaccion = conn.BeginTransaction();
                string query = @"UPDATE web_empresa_user SET DEFECTO = '' where USER_ID = @idUser;";

                cmd.Parameters.AddWithValue("@idUser", request.user_id);

                cmd.CommandText = query;
                cmd.Connection = conn;
                cmd.Transaction = transaccion;
                cmd.ExecuteNonQuery();

                transaccion.Commit();

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
    }
}