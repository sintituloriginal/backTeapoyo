using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
public class datosInformeAuditoria { 
    //parametros para el listado
    public string norma { get; set; }
    public string tipo { get; set; }
    public string auditoria { get; set; }
    public string auditor { get; set; }
    public string fechaInicio { get; set; }
    public string fechaFin { get; set; }
    public string id_empresa { get; set; }

}

public class respuestaInformeAuditoria
{
    //parametros para el listado
    public string ID_AUDITORIA { get; set; }
    public string NOMBRE_NORMA { get; set; }
    public string FECHA { get; set; }
    public string FULLNAME { get; set; }
    public string EVALUACION { get; set; }
    public string OBSERVACION { get; set; }
    public string ID_PUNTO_NORMATIVO { get; set; }
    public string DESC_PUNTO_NORMATIVO { get; set; }
    public string TIPO { get; set; }
    public string COD_NORMA { get; set; }
    public string VERSION { get; set; }
    public string PERTENECE { get; set; }
}

public class selectores
{
    //parametros para el listado
    public string id { get; set; }
    public string text { get; set; }
    
}

public class datosAuditoriaExportar
{
    //parametros para el listado
    public string ID_AUDITORIA { get; set; }
    public string NOMBRE_NORMA { get; set; }
    public string AUDITOR { get; set; }
    public string VERSION { get; set; }
    public string FECHA { get; set; }
    public string EVALUACION { get; set; }
    public string OBSERVACION { get; set; }
    public string ID_PUNTO_NORMATIVO { get; set; }
    public string DESC_PUNTO_NORMATIVO { get; set; }
    public string FECHA_REVISION { get; set; }
    public string TIPO { get; set; }
    public string COD_NORMA { get; set; }
    public string PERTENECE { get; set; }
    public string EXPORTAR_EXCEL { get; set; }
    public string DESCRIPCION { get; set; }

}
public class filtrosReporte
{
    //parametros para el listado
    public string id { get; set; }
    public string text { get; set; }
    public Boolean value { get; set; }
}

