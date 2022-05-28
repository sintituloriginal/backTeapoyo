using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data.OracleClient;

namespace proyecto.libraries
{
    public class Database
    {
        private String connectionString;

        public Database(string dataBaseName)
        {

            if (String.IsNullOrEmpty(WebConfigurationManager.ConnectionStrings[dataBaseName].ConnectionString))
            {
                throw new Exception("Se perdió la variable de conexión en el archivo web.config");
            }
            else
            {
                connectionString = WebConfigurationManager.ConnectionStrings[dataBaseName].ConnectionString;
            }
        }

        //obtenemos la conexion a la base de datos
        public SqlConnection openConnection()
        {
            var sqlConnection = new SqlConnection(connectionString);
            return sqlConnection;
        }
    }
}