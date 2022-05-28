using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class DatosEmpresa
{
    public int id { get; set; }
    public string codigo { get; set; }
    public string empresa { get; set; }
    public string descripcion { get; set; }
    public string fecha_create { get; set; }
    public string fecha_update { get; set; }
    public string estado { get; set; }
    public string empresaExcel { get; set; }
}

public class SelectorEMPRESA
{
    public string label { get; set; }
    public string value { get; set; }

}

public class empresaFullData{
    public int id_empresa { get; set; }
    public string codigo { get; set; }
    public string empresa { get; set; }
    public string descripcion { get; set; }
    public string estado { get; set; }
    
}
public class habilitarDeshabiltiar_empresa
{
    public string ID { get; set; }
    public string estado { get; set; }
}

public class selector_empresa
{
    public string empresa { get; set; }
}
public class menuListaEmpresas
{
    public string value { get; set; }
    public string label { get; set; }
    public string activa { get; set; }
}
public class updateDefaultEmpresa
{
    public string user_id { get; set; }
    public string empresa_id { get; set; }
}