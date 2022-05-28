public class noConformidadRequest
{
    //parametros para el listado
    public string COD_NORMA { get; set; }
    public string NOMBRE_NORMA { get; set; }
    public string ID_PUNTO_NORMATIVO { get; set; }
    public string NO_CONFORMIDAD { get; set; }
    public string AUDITOR { get; set; }
    public string FECHA { get; set; }
    public string FECHA_COMPROMISO { get; set; }
    public string TIPO_AUDITORIA { get; set; }
    public string RESPONSABLE { get; set; }
    public string ESTADO { get; set; }
    public string DESC_PUNTO_NORMATIVO { get; set; }
    public string ID_AUDITORIA { get; set; }
    public string CreadorNoConformidad { get; set; }
    public string FECHAEVALUACION { get; set; }

    public string AREA_RESPONSABLE { get; set; }
    public string ESTADO_RESPONSABLE { get; set; }
    public string FECHA_AUDITORIA { get; set; }
    public string EXPORTAR_EXCEL { get; set; }
    public string ID_NC { get; set; }
}
public class Guardar
{
    //parametros para el listado
    public string ID_WORKFLOW { get; set; }
    public string ID_AUDITORIA { get; set; }
    public string ID_PUNTO_NORMATIVO { get; set; }
    public string TIPO { get; set; }
    public string FECHA { get; set; }
    public string RESPONSABLE { get; set; }
    public string OBSERVACION { get; set; }
    public string FECHA_COMPROMISO { get; set; }
    public string ADJUNTO { get; set; }
    public string ESTADO { get; set; }
    public string datos { get; set; }
    public string COD_NORMA { get; set; }
    public string ID_AUDITORIA_ORIG { get; set; }
    public string ID_NC { get; set; }

}

public class inicial
{
    public string ID_AUDITORIA { get; set; }
    public string ID_PUNTO_NORMATIVO { get; set; }
    public string GESTOR { get; set; }
    public string COD_NORMA { get; set; }
    public string ID_AUDITORIA_ORIG { get; set; }
    public string ID_NC { get; set; }

}
public class inicioModal
{
    public string date  { get; set; }
    public string responsable { get; set; }
    public string observacion { get; set; }
    public string datecompromiso { get; set; }
    public string archivo { get; set; }
    public string tipo { get; set; }
    public string ruta { get; set; }
    public string ID_WORKFLOW { get; set; }
    
}

public class Cargar
{
    public string COD_NORMA { get; set; }
    public string NOMBRE_NORMA { get; set; }
    public string ID_PUNTO_NORMATIVO { get; set; }
    public string NO_CONFORMIDAD { get; set; }
    public string AUDITOR { get; set; }
    public string FECHA { get; set; }
    public string FECHA_COMPROMISO { get; set; }
    public string TIPO_AUDITORIA { get; set; }
    public string RESPONSABLE { get; set; }
    public string ESTADO { get; set; }
    public string FECHA_FILTRO_DESDE { get; set; }
    public string FECHA_FILTRO_HASTA { get; set; }
    public string DESC_PUNTO_NORMATIVO { get; set; }
    public string ID_AUDITORIA { get; set; }
    public string CreadorNoConformidad { get; set; }
    public string FECHAEVALUACION { get; set; }

    public string AREA_RESPONSABLE { get; set; }
    public string ESTADO_RESPONSABLE { get; set; }
    public string EXPORTAR_EXCEL { get; set; }
    public string FECHA_AUDITORIA { get; set; }
    public string ID_EMPRESA { get; set; }
    public string GROUP_ID { get; set; }
    public string USER_ID { get; set; }

    

}


public class Eliminar
{
    public string Datos { get; set; }
    

}

public class datosNoConformidadMultiple
{
    public string ID_AUDITORIA { get; set; }
    public string ID_PUNTO_NORMATIVO { get; set; }
    public string OBSERVACION { get; set; }
    public string FECHA { get; set; }
    public string FECHA_AUDITORIA { get; set; }
    public string ESTADO { get; set; }
    public string RESPONSABLE { get; set; }
    public string ID_NC { get; set; }
    public string RESPUESTA { get; set; }
}