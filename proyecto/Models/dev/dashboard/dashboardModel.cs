using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


//agregar para crear lo basico
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Security.Claims;
using System.Threading;
using System.Web.Configuration;
using System.IO;
using System.Globalization;

public class dashboardModel : Conexion
{
    public object cargarDatos(string id_empresa)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;

        try
        {
            string query =  @" select dn.VERSION,dn.COD_NORMA  , dn.NOMBRE_NORMA , dn.DESCRIPCION_NORMA , dn.COLORNORMA , dn.COLORCUMPLIMIENTO, count(dpn.ID_PUNTO_NORMATIVO) TOTAL , 
                    (select count(ID_PUNTO_NORMATIVO) from dev_punto_normativo where COD_NORMA = dn.COD_NORMA and EVALUABLE = 'true' and ULTIMA_EVALUACION = 'SI') as POSITIVO,  
                    (select count(ID_PUNTO_NORMATIVO) from dev_punto_normativo where COD_NORMA = dn.COD_NORMA and EVALUABLE = 'true' and ULTIMA_EVALUACION = 'NO')as NEGATIVO  
                    from dev_normas dn  
                    left  
                    join dev_punto_normativo dpn on dn.COD_NORMA = dpn.COD_NORMA and dpn.EVALUABLE = 'true'  
                    join dev_norma_empresa dne on  dn.COD_NORMA = dne.ID_NORMA and  dne.ID_EMPRESA = "+ id_empresa + @"
                    where dn.ESTADO = 'Habilitado'  
                    GROUP BY dn.COD_NORMA  , dn.NOMBRE_NORMA , dn.DESCRIPCION_NORMA , dn.COLORNORMA ,dn.COLORCUMPLIMIENTO, dn.VERSION "; 
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            SqlDataReader reader = cmd.ExecuteReader();

            List<datosNormas> lista = new List<datosNormas>();

            while (reader.Read())
            {
                datosNormas obj = new datosNormas();
                obj.VERSION = reader["VERSION"].ToString();
                obj.COD_NORMA = reader["COD_NORMA"].ToString();
                obj.NOMBRE_NORMA = reader["NOMBRE_NORMA"].ToString();
                obj.DESCRIPCION_NORMA = reader["DESCRIPCION_NORMA"].ToString();
                obj.COLORNORMA = reader["COLORNORMA"].ToString();
                obj.COLORCUMPLIMIENTO = reader["COLORCUMPLIMIENTO"].ToString();
                obj.TOTAL = reader["TOTAL"].ToString();
                obj.POSITIVO = reader["POSITIVO"].ToString();
                obj.NEGATIVO = reader["NEGATIVO"].ToString();


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
    public object auditorias(string id_empresa)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;

        try
        {
            string query = "select top 10  convert(nvarchar(10),da.FECHA, 105 ) FECHA  , wu.FULLNAME , CONCAT(dn.NOMBRE_NORMA,' ver. ',dn.VERSION) as NOMBRE_NORMA , da.COD_NORMA , da.ID_AUDITORIA from dev_auditoria da " +
                           " inner join dev_normas dn on dn.COD_NORMA = da.COD_NORMA "+
                           " inner join dev_norma_empresa dne on dne.ID_NORMA = dn.COD_NORMA and dne.ID_EMPRESA = "+id_empresa+" "+
                           " inner join web_users wu on wu.USER_ID = da.USER_ID where ( da.TIPO_AUDITORIA = 'Externa' or da.TIPO_AUDITORIA = 'Interna') AND da.ESTADO<>'Anulada' ORDER BY da.FECHA DESC";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            SqlDataReader reader = cmd.ExecuteReader();

            List<Auditoria> lista = new List<Auditoria>();

            while (reader.Read())
            {
                Auditoria obj = new Auditoria();
                obj.FECHA = reader["FECHA"].ToString();
                obj.NOMBRE_NORMA = reader["NOMBRE_NORMA"].ToString();
                obj.FULLNAME = reader["FULLNAME"].ToString();
                obj.ID_AUDITORIA = reader["ID_AUDITORIA"].ToString();
                obj.COD_NORMA = reader["COD_NORMA"].ToString();
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
    public object noConformidad(string id_empresa)
    {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;

        try
        {
            string query = " SELECT top(10) CONCAT(dn.NOMBRE_NORMA,' ver. ',dn.VERSION) as NOMBRE_NORMA  , dnc.ID_PUNTO_NORMATIVO , convert(nvarchar(10) ,dnc.FECHA, 105) FECHA , convert(nvarchar(70) , dnc.NO_CONFORMIDAD) NO_CONFORMIDAD " +
                           " from dev_no_conformidad dnc "+
                           " inner join dev_auditoria da on da.ID_AUDITORIA = dnc.ID_AUDITORIA "+
                           " inner join dev_normas dn on dn.COD_NORMA = da.COD_NORMA "+
                           " inner join dev_norma_empresa dne on dne.ID_NORMA = dn.COD_NORMA and dne.ID_EMPRESA = "+id_empresa+" "+
                           " where dnc.ESTADO != 'Aprobado' ";
             
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            SqlDataReader reader = cmd.ExecuteReader();

            List<noConformidad> lista = new List<noConformidad>();

            while (reader.Read())
            {
                noConformidad obj = new noConformidad();
                obj.FECHA = reader["FECHA"].ToString();
                obj.NOMBRE_NORMA = reader["NOMBRE_NORMA"].ToString();
                obj.ID_PUNTO_NORMATIVO = reader["ID_PUNTO_NORMATIVO"].ToString();
                obj.NO_CONFORMIDAD = reader["NO_CONFORMIDAD"].ToString();

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

    public object documentos(string id_empresa)
        {
        SqlConnection conn = dbproyecto.openConnection();
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;

        try
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var group_id = identity.Claims.Where(c => c.Type == "group_id")
                               .Select(c => c.Value).SingleOrDefault();
            var user_id = identity.Claims.Where(c => c.Type == "user_id")
                               .Select(c => c.Value).SingleOrDefault();

            string query = "";
            query = "select  ADMINISTRADOR , AUDITOR from web_groups where GROUP_ID =  " + group_id + " ";

            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            SqlDataReader reader2 = cmd.ExecuteReader();
            var ADMINISTRADOR = "";
            var AUDITOR = "";
            while (reader2.Read())
            {
                ADMINISTRADOR = reader2["ADMINISTRADOR"].ToString();
                AUDITOR = reader2["AUDITOR"].ToString();

            }
            reader2.Close();

            
            if (ADMINISTRADOR == "false")
            {
                query = " select top(10) dd.codigo_documento , dd.version , dt.tipo , dd.ult_revision, dd.nombre from dev_documento dd " +
                    " inner join dev_tipo_documento dt on dt.id = dd.tipo " +
                    " inner join dev_user_area dua on dua.user_id = dd.user_id " +
                    " inner join dev_doc_area dda on dda.id_area = dua.area and dda.cod_documento = dd.codigo_documento " +
                    " inner join dev_archivo_documento dad on dad.cod_documento =dd.codigo_documento and dad.estado = 'Publicado' and dad.version=dd.version " +
                    " union " +
                    " select dd.codigo_documento , dd.version , dt.tipo , dd.ult_revision, dd.nombre from dev_documento dd " +
                    " inner join dev_archivo_documento dad on dad.cod_documento =dd.codigo_documento and dad.estado = 'Publicado' and case when dd.caducidad = '' then CONVERT(char(10), GetDate(),126) else  dd.caducidad end  >= CONVERT(char(10), GetDate(),126) and dad.version=dd.version   " +
                    " inner join dev_tipo_documento dt on dt.id = dd.tipo " +
                    " where dd.id_empresa = "+id_empresa+" ";
            }else
            {
                query = " select top(10) dd.codigo_documento , dd.ult_revision, dd.version , dt.tipo , dd.nombre from dev_documento dd " +
                        " inner join dev_tipo_documento dt on dt.id = dd.tipo " +
                        " inner join dev_archivo_documento dad on dad.cod_documento =dd.codigo_documento and dad.estado = 'Publicado' and case when dd.caducidad = '' then CONVERT(char(10), GetDate(),126) else  dd.caducidad end  >= CONVERT(char(10), GetDate(),126) and dad.version=dd.version  "+
                        " where dd.id_empresa = "+id_empresa+" ";
            }
            query += "order by dd.ult_revision desc";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();


            SqlDataReader reader = cmd.ExecuteReader();

            List<documentos> lista = new List<documentos>();

            while (reader.Read())
            {
                documentos obj = new documentos();
                obj.codigo_documento = reader["codigo_documento"].ToString();
                obj.version = reader["version"].ToString();
                obj.tipo = reader["tipo"].ToString();
                obj.nombre = reader["nombre"].ToString();

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