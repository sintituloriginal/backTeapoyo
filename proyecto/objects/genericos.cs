using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proyecto.objects
{

    public class selector
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class acciones
    {
        public string action { get; set; }
        public string link { get; set; }
        public string cmd { get; set; }
        public string uiIcon { get; set; }

        public string vista_por_defecto { get; set; }
    }

    public class rutaRequest
    {
        public string ruta { get; set; }
        public string group_id { get; set; }
                public string busq_avanzada { get; set; }
    }
    public class validacionesAccesoRequest
    {
        public string id_perfil { get; set; }
        public string link { get; set; }
        public string accion { get; set; }
    }
    public class users
    {
        public string userid { get; set; }
    }

    public class data_filtros
    {
        public string id { get; set; }
        public string v_model { get; set; }
        public string link { get; set; }
        public string id_tipo_filtro { get; set; }
        public string label { get; set; }
        public string busqueda_avanzada { get; set; }
        public string estado { get; set; }
        public string query { get; set; }
        public string disabled { get; set; }
        public string dependiente { get; set; }
        public object items { get; set; }
    }
    public class item_filtros
    {
        public string id { get; set; }
        public string text { get; set; }

    }
}