using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using proyecto.objects.dev.reportes;
using System.Globalization;


namespace proyecto.Models.empresas
{
    public class empresasModel : Conexion
    {
        public object listaEmpresasUsuario(String userID)
        {
            SqlConnection conn = dbproyecto.openConnection();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string query = @"SELECT we.ID_EMPRESA as idEmpresa, we.NOMBRE as nombreEmpresa, weu.defecto as empresaDefecto
                                    FROM web_empresa_user weu
                                    JOIN web_empresa we
                                    ON we.ID_EMPRESA = weu.ID_EMPRESA
                                where weu.USER_ID = @userID ORDER BY idEmpresa ASC";

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
                    obj.activa = reader["empresaDefecto"].ToString();

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