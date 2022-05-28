using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proyecto.objects.dev.reportes
{
    public class data_mejoras_reporte
    {
        public string ID { get; set; }
        public string FULLNAME { get; set; }
        public string USERNAME { get; set; }
        public string PERFIL { get; set; }
        public string EMAIL { get; set; }
        public string ESTADO { get; set; }
    }

    

    public class graficos
    {
        public object grafico_dona { get; set; }
        public object grafico_columna { get; set; }
        public object grafico_torta { get; set; }
        public object grafico_linea { get; set; }
        
    }
    public class data_graficos
    {
        public int x{ get; set; }
        public int y { get; set; }
        public string name { get; set; }
        public string legendText { get; set; }
        public string label { get; set; }
        public string indexLabel { get; set; }
    }
}