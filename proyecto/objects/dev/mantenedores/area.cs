public class areaRequest
{
    //parametros para el listado
    public string NOMBRE { get; set; }
    public string CODAREA { get; set; }
    public string RESPONSABLE { get; set; }
    public string ESTADO { get; set; }
    public string ID_EMPRESA { get; set; }
    public int ID_AREA { get; set; }


}
public class DatosArea
{
    public int ID_AREA { get; set; }
    public string NOMBRE { get; set; }
    public string CODAREA { get; set; }
    public string RESPONSABLE { get; set; }
    public string ESTADO { get; set; }
    public string nombreUsuario { get; set; }
}
public class trackingArea
{
    public string id_constante { get; set; }
    public string nombre { get; set; }
    public string valor { get; set; }
    public string descripcion { get; set; }
    public string tipoDato { get; set; }
}
public class selectorArea
{
    public string id { get; set; }
    public string text { get; set; }
}

public class habilitarDeshabilitarArea
{
    public string ID { get; set; }
    public string ESTADO { get; set; }
}
public class excelArea
{
    public string nombre { get; set; }
   
}