
using System;

public class incidenciaRequest
{
    public string ID_INCIDENCIA { get; set; }
    public string FECHA_HORA_EVENTO { get; set; }
    public string TIPO_EVENTO { get; set; }
    public string CRITERIO_AFECTADO { get; set; }
    public string AREA { get; set; }
    public string REPORTADO_POR { get; set; }
    public string RESPONS_ANALISIS { get; set; }
    public string ESTADO { get; set; }
    public string DESCRIPCION { get; set; }

    public bool analista { get; set; }

}
public class incidenciaExport
{
    public string ID_INCIDENCIA { get; set; }
    public string FECHA_HORA_EVENTO { get; set; }
    public string TIPO_EVENTO { get; set; }
    public string CRITERIO_AFECTADO { get; set; }
    public string AREA { get; set; }
    public string REPORTADO_POR { get; set; }
    public string RESPONS_ANALISIS { get; set; }
    public string ESTADO { get; set; }
    public string DESCRIPCION { get; set; }
    public string OBSERVACION_ANALISIS { get; set; }
    public string OBSERVACION_CIERRE { get; set; }
    public string ACTUALIZAR_DOCUMENTO { get; set; }
    public string NOMBRE_DOCUMENTO { get; set; }
    public string REVISION_RIESGOS { get; set; }
    public string DETALLE_RIESGOS { get; set; }
    
    

}
public class filtro_incidencia
{
    public string id { get; set; }
    public string text { get; set; }
    public Boolean value { get; set; }
}
public class filtrarPor
{
    public string ID_EMPRESA { get; set; }
    public string ESTADO { get; set; }
    public string CRITERIO_AFECTADO { get; set; }
    public string TIPO_EVENTO { get; set; }
    public string AREA { get; set; }
    public string RESPONS_ANALISIS { get; set; }
    public string FECHA_INICIO { get; set; }
    public string FECHA_FIN { get; set; }
    public string USER_ID { get; set; }
}


public class crearIncidenciaReq
{
    public int id { get; set; }
    public string nombre_documento { get; set; }

    public string id_empresa { get; set; }
    public string id_area { get; set; }
    public string id_analista { get; set; }

    public string fecha_incidencia { get; set; }
    public string hora_incidencia { get; set; }

    public string criterio_afectado { get; set; }

    public string tipo_evento { get; set; }
    public string reportado_por { get; set; }
    public string responsable_analisis { get; set; }
    public string estado { get; set; }
    public string fecha_created { get; set; }

    public string fecha_updated { get; set; }
    public string descripcion { get; set; }





}
public class archivoIncidencia
{
    public string id_archivo{ get; set; }
    public string nombre { get; set; }
    public string ruta { get; set; }
    public string id_incidencia { get; set; }



}
public class cerrarIncidenciaReq
{
    public string id_cierre { get; set; }
    public string id_incidencia { get; set; }
    public string observacion { get; set; }
    public string fecha_created { get; set; }
    public string fecha_updated { get; set; }
    public string actualizar_documento { get; set; }
    public string id_documento { get; set; }
    public string revision_riesgos { get; set; }
    public string detalle_riesgos { get; set; }
    public string nombre_documento { get; set; }





}