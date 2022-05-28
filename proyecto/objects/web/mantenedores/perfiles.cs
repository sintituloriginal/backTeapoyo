public class perfilesRequest
{

    public string group_id { get; set; }
    public string nodos { get; set; }


    //para crear o editar un registro
    public string nombre { get; set; }

    public string descripcion { get; set; }
    public string acciones { get; set; }
    public string multiempresa { get; set; }

    public string perfilExcel { get; set; }

    public string vista_por_defecto { get; set; }
    public string administrador { get; set; }
    public string auditor { get; set; }

}
public class Datosperfiles
{
    public string nombre { get; set; }
    public string group_id { get; set; }

    public string descripcion { get; set; }

    public string name { get; set; }
    public string code { get; set; }
    public string fecha_created { get; set; }
    public string fecha_updated { get; set; }

}

public class Datosbranch
{
    public string nombre { get; set; }
    public string link { get; set; }
    public string source { get; set; }
    public string id_branch { get; set; }
    public string hijos { get; set; }

}

public class Datossheet
{
    public string nombre { get; set; }
    public string link { get; set; }
    public string namePadre { get; set; }
    public string id_branch { get; set; }
    public string id_sheet { get; set; }
    public string codigo { get; set; }
    public string estado { get; set; }

}

public class ActionResponse
{
    public string id_action { get; set; }
    public string name { get; set; }
    public string link { get; set; }
    public string estado { get; set; }
    public bool estadoPerfil { get; set; }
    public string cmd { get; set; }

}

public class ScreenResponse
{
    public string id_screen { get; set; }
    public string group_id { get; set; }
    public string link { get; set; }
    public string status { get; set; }
    public string fecha { get; set; }
    public string id_branch { get; set; }
    public string id_sheet { get; set; }


}
public class ActionScreenResponse
{
    public string id_action { get; set; }
    public string link { get; set; }


}

