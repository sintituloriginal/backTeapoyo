public class constantesRequest
{
    //parametros para el listado
    public string inicio { get; set; }
    public string final { get; set; }
    public string id_constante { get; set; }

    //para el autocomplete
    public string term { get; set; }

    //para crear o editar un registro
    public string nombre { get; set; }
    public string valor { get; set; }
    public string descripcion { get; set; }
    public string tipo_dato { get; set; }
    public string estado { get; set; }
    public string id_constantes { get; set; }
    public string constantes { get; set; }
}
public class DatosConstantes
{
    public string nombre { get; set; }
    public string id_constante { get; set; }
    public string valor { get; set; }
    public string descripcion { get; set; }
    public string tipoDato { get; set; }
    public string name { get; set; }
    public string code { get; set; }
    public string value { get; set; }
    public string estado { get; set; }
}

public class autocompleteConstantes
{
    public string id { get; set; }
    public string text { get; set; }
}

public class trackingConstantes
{
    public string id_constante { get; set; }
    public string nombre { get; set; }
    public string valor { get; set; }
    public string descripcion { get; set; }
    public string tipoDato { get; set; }
}