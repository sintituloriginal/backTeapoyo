public class financieraRequest
{
    public int idFinanciera { get; set; }
    public string nombreDeuda { get; set; }
    public int idTipoDeuda { get; set; }
    public string TipoDeduda { get; set; }
    public int idPlazo { get; set; }
    public string nombrePlazo { get; set; }
    public string saldoDeuda { get; set; }
    public string valorCuota { get; set; }
    public string cupoTotal { get; set; }
    public int idBanco { get; set; }
    public string nombreBanco { get; set; }
    public string pagoMensual { get; set; }
    public string indice { get; set; }
    public string financieraExcel { get; set; }
    public string fechaCreacion { get; set; }
    public string username { get; set; }
    public string fullname { get; set; }
    public int total { get; set; }
    public string fechaNacimiento { get; set; }
    public string sexo { get; set; }

}

public class SelectorDEUDA
{
    public string label { get; set; }
    public string value { get; set; }
}

public class selectorPLAZO
{
    public string label { get; set; }
    public string value { get; set; }
}

public class SelectorBANCO
{
    public string label { get; set; }
    public string value { get; set; }
}

public class SelectorUSUARIO
{
    public string label { get; set; }
    public string value { get; set; }
}

public class SelectorUSUARIONOMBRE
{
    public string label { get; set; }
    public string value { get; set; }
}

