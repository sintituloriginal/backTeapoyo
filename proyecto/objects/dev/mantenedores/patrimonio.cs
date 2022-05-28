public class patrimonioRequest
{
    public int idPatrimonio { get; set; }
    public string nombreItem { get; set; }
    public int idActivo { get; set; }
    public string nombreActivo { get; set; }
    public string nobmreDeuda { get; set; }
    public int idDeuda { get; set; }
    public string montoActivo { get; set; }
    public int idValorizacion { get; set; }
    public int idBanco { get; set; }
    public string nombreBanco { get; set; }
    public string nombreValorizacion { get; set; }
    public string total { get; set; }
    public string indice { get; set; }
    public string patrimonioExcel { get; set; }
    public string totalGeneral { get; set; }
    public string fechaCreacion { get; set; }
    public string fullname { get; set; }
    public string username { get; set; }
    public string fechaNacimiento { get; set; }
    public string sexo { get; set; }
}

public class SelectorACTIVO
{
    public string label { get; set; }
    public string value { get; set; }
}

public class SelectorVALORIZACION
{
    public string label { get; set; }
    public string value { get; set; }
}

public class SelectorDEUDAP
{
    public string label { get; set; }
    public string value { get; set; }
}

public class SelectorBANCOP
{
   public string label { get; set; }
    public string value { get; set; }
}

