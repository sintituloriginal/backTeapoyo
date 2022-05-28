using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proyecto.objects.web
{
    public class Users
    {
        public string username { get; set; }
        public string fullname { get; set; }
        public string apellidoPat { get; set; }
        public string apellidoMat { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int age { get; set; }
        public string birth { get; set; }
        public int group_id { get; set; }
        public string group_name { get; set; }
        public string user_id { get; set; }
        public string avatar { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }
        public int id_area { get; set; }
        public string id_empresa { get; set; }
        public string empresa { get; set; }
        public string area { get; set; }
        public string cargo { get; set; }
        public string corporativo { get; set; }
        public string intentos { get; set; }
    }
    public class Branch
    {
        public int id_branch { get; set; }
        public string title { get; set; }
        public string href { get; set; }
        public string icon { get; set; }
        public string source { get; set; }
        public int bandera { get; set; }
    }
    public class Sheet
    {
        public int id_sheet { get; set; }
        public string title { get; set; }
        public string href { get; set; }
        public string icon { get; set; }
        public int id_branch { get; set; }
    }


    public class item
    {
        public string href { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
    }
    public class line_divider
    {
        public bool divider { get; set; }
    }
    public class header_group
    {
        public string header { get; set; }
    }
    public class itemGroup
    {
        public string group { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public Array child { get; set; }

    }
    public class datosUsuario
    {
        public string user_id { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string group_id { get; set; }
        public string fullname { get; set; }
        public string avatar { get; set; }
        public string id_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public string defecto { get; set; }
        public string administrador { get; set; }
        public string auditor { get; set; }
        public object areas { get; set; }

    }

    public class areas
    {
        public string id { get; set; }
    }

    public class notificaciones
    {
        public string id_notificacion { get; set; }
        public string fecha { get; set; }
        public string notificacion { get; set; }
        public string id_empresa { get; set; }
    }

}