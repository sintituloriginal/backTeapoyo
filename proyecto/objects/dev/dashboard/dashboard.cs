public class datosNormas
{
    //parametros para el listado
    public string COD_NORMA { get; set; }
    public string VERSION { get; set; }
    public string NOMBRE_NORMA { get; set; }
    public string DESCRIPCION_NORMA { get; set; }
    public string FECHA_ACTUALIZACION { get; set; }
    public string ESTADO { get; set; }
    public string COLORNORMA { get; set; }
    public string COLORCUMPLIMIENTO { get; set; }
    public string TOTAL { get; set; }
    public string POSITIVO { get; set; }
    public string NEGATIVO { get; set; }
}

public class Auditoria
{
    //parametros para el listado
    public string FECHA { get; set; }
    public string FULLNAME { get; set; }
    public string NOMBRE_NORMA { get; set; }
    public string ID_AUDITORIA { get; set; }
    public string COD_NORMA { get; set; }


}

public class noConformidad
{
    //parametros para el listado
    public string NOMBRE_NORMA { get; set; }
    public string ID_PUNTO_NORMATIVO { get; set; }
    public string FECHA { get; set; }
    public string NO_CONFORMIDAD { get; set; }
    
}
public class documentos
{
    //parametros para el listado
    public string codigo_documento { get; set; }
    public string version { get; set; }
    public string tipo { get; set; }
    public string nombre { get; set; }

}