using System.Collections.Generic;

public class logEventoRequest
{
    public string modulo { get; set; }//
    public string perfil { get; set; }//
    public string id { get; set; }//
    public string user_id { get; set; }//
    public string fecha_inicio { get; set; }//
    public string fecha_fin { get; set; }//
}

public class data
{
    public string fecha { get; set; }
    public string modulo { get; set; }
    public string accion { get; set; }
    public string perfil { get; set; }
    public string id { get; set; }
    public string user_id { get; set; }
    public string fecha_inicio { get; set; }
    public string fecha_fin { get; set; }
    public string action { get; set; }
    public string usuario { get; set; }
    public string json { get; set; }
    public string json_old { get; set; }
}
public class perfilData
{
    public string codigo { get; set; }
    public string text { get; set; }
}

public class perfilRequestPorUsuario
{
    public string perfil { get; set; }
    public string term { get; set; }
}

public class perfilDataPorUsuario
{
    public string id { get; set; }
    public string text { get; set; }
}

public class excelRequest
{
    public string log { get; set; }
}

/*public class Value
{
    public string nuevo { get; set; }
    public string antiguo { get; set; }
}

public class RootObject
{
    public string Key { get; set; }
    public Value Value { get; set; }
}*/

public class ListofListsRootClass : List<ListRootClass>
{
    //public List<RootClass> RootClasses { get; set; }
}

public class ListRootClass : List<RootClass>
{
    //public List<RootClass> RootClasses { get; set; }
}

public class RootClass
{
    public string Key { get; set; }
    public ValueClass Value { get; set; }
}

public class ValueClass
{
    public string Nuevo { get; set; }
    public string Antiguo { get; set; }
}

public class obj_fecha_filtro
{
    public string fecha_inicio { get; set; }
    public string fecha_fin { get; set; }
}

public class usuarioSelect
{
    public string value { get; set; }
    public string label { get; set; }
}

public class perfilSelect
{
    public string value { get; set; }
    public string label { get; set; }
}

public class moduloSelect
{
    public string value { get; set; }
    public string label { get; set; }
}