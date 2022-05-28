using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class documentoRequest
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
public class DatosTipo
{
    public string codigo { get; set; }
}
public class DatosDocumento
{
    public string keywords { get; set; }
    public string indice { get; set; }
    public string codigo { get; set; }
    public string nombre { get; set; }
    public string version { get; set; }
    public string tipo { get; set; }
    public string ult_revision { get; set; }
    public string caducidad { get; set; }
    public string area { get; set; }
    public string id_empresa { get; set; }
    public string nombre_empresa { get; set; }
    public string usuario { get; set; }
    public string documentos { get; set; }
    public string calendario_fin { get; set; }
    public string calendario_inicio { get; set; }
    public string ruta_archivo { get; set; }
    public string imprimir { get; set; }
    public string descargar { get; set; }
    public string estado { get; set; }
    public string exportar_Excel { get; set; }
    public string normas { get; set; }
    public string palabras_clave { get; set; }

}

public class selectorDocumento
{
    public string value { get; set; }
    public string label { get; set; }
}
public class datosDocumentoPublicar
{
    public string nombre { get; set; }
    public string idDocumento { get; set; }
    public string versionDocumento { get; set; }
    public string idEmpresaDocumento { get; set; }

}
public class registroActividad
{
    public string codigo { get; set; }

    public string tipo_actividad { get; set; }
}

public class trackingDocumento
{
    public string id_constante { get; set; }
    public string nombre { get; set; }
    public string valor { get; set; }
    public string descripcion { get; set; }
    public string tipoDato { get; set; }
}

public class selector_tipo_documento
{
    public string value { get; set; }
    public string label { get; set; }
}
public class filtro_tipo_documento
{
    public string id { get; set; }
    public string text { get; set; }
    public string codigo { get; set; }
    public Boolean value { get; set; }
}
public class tipoDocumentoFilter
{
    public string value { get; set; }
    public string label { get; set; }
}

public class filtro_norma
{
    public string id { get; set; }
    public string text { get; set; }
    public Boolean value { get; set; }
}

public class filtro_area
{
    public string id { get; set; }
    public string text { get; set; }
    public string codigo { get; set; }
    public Boolean value { get; set; }
}
public class areaFilter
{
    public string value { get; set; }
    public string label { get; set; }
}
public class selector_area
{
    public string value { get; set; }
    public string label { get; set; }
}
public class selector_editar
{
    public string id { get; set; }
    public string text { get; set; }
}
public class selector_palabra_clave
{
    public string value { get; set; }
    public string label { get; set; }
}

public class Datos_documento_crear
{
    public string codigo { get; set; }
    public string nombre { get; set; }
    public string tipo { get; set; }
    public string area { get; set; }
    public string imprimir { get; set; }
    public string descargar { get; set; }
    public string fecha_caducidad { get; set; }
    public string palabras_claves { get; set; }
    public string version { get; set; }
    public string observaciones { get; set; }
    public string archivo { get; set; }
    public string cambio_archivo { get; set; }
    public string ruta { get; set; }
    public string id_empresa { get; set; }

}

public class Datos_documento_editar
{
    public string codigo { get; set; }
    public string nombre { get; set; }
    public string tipo { get; set; }
    public object area { get; set; }
    public string imprimir { get; set; }
    public string descargar { get; set; }
    public string fecha_caducidad { get; set; }
    public object palabras_claves { get; set; }
    public string version { get; set; }
    public string observaciones { get; set; }
    public string archivo { get; set; }

    public string cambio_archivo { get; set; }
    public string ruta { get; set; }
}

public class DatosDocumento_historico
{
    public string revision { get; set; }
    public string codigo { get; set; }
    public string nombre { get; set; }
    public string version { get; set; }
    public string observacion { get; set; }
    public string usuario_creador { get; set; }
    public string usuario_publicador { get; set; }
    public string estado { get; set; }
    public string nombre_archivo { get; set; }
    public string ruta { get; set; }
    public string id_empresa { get; set; }
}

public class InMemoryMultipartFormDataStreamProvider_documento : MultipartStreamProvider
{
    private NameValueCollection _formData = new NameValueCollection();
    private List<HttpContent> _fileContents = new List<HttpContent>();

    // Set of indexes of which HttpContents we designate as form data  
    private Collection<bool> _isFormData = new Collection<bool>();

    /// <summary>  
    /// Gets a <see cref="NameValueCollection"/> of form data passed as part of the multipart form data.  
    /// </summary>  
    public NameValueCollection FormData
    {
        get { return _formData; }
    }

    /// <summary>  
    /// Gets list of <see cref="HttpContent"/>s which contain uploaded files as in-memory representation.  
    /// </summary>  
    public List<HttpContent> Files
    {
        get { return _fileContents; }
    }

    public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
    {
        // For form data, Content-Disposition header is a requirement  
        ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
        if (contentDisposition != null)
        {
            // We will post process this as form data  
            _isFormData.Add(String.IsNullOrEmpty(contentDisposition.FileName));

            return new MemoryStream();
        }

        // If no Content-Disposition header was present.  
        throw new InvalidOperationException(string.Format("Did not find required '{0}' header field in MIME multipart body part..", "Content-Disposition"));
    }

    /// <summary>  
    /// Read the non-file contents as form data.  
    /// </summary>  
    /// <returns></returns>  
    public override async Task ExecutePostProcessingAsync()
    {
        // Find instances of non-file HttpContents and read them asynchronously  
        // to get the string content and then add that as form data  
        for (int index = 0; index < Contents.Count; index++)
        {
            if (_isFormData[index])
            {
                HttpContent formContent = Contents[index];
                // Extract name from Content-Disposition header. We know from earlier that the header is present.  
                ContentDispositionHeaderValue contentDisposition = formContent.Headers.ContentDisposition;
                string formFieldName = UnquoteToken(contentDisposition.Name) ?? String.Empty;

                // Read the contents as string data and add to form data  
                string formFieldValue = await formContent.ReadAsStringAsync();
                FormData.Add(formFieldName, formFieldValue);
            }
            else
            {
                _fileContents.Add(Contents[index]);
            }
        }
    }

    /// <summary>  
    /// Remove bounding quotes on a token if present  
    /// </summary>  
    /// <param name="token">Token to unquote.</param>  
    /// <returns>Unquoted token.</returns>  
    private static string UnquoteToken(string token)
    {
        if (String.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
        {
            return token.Substring(1, token.Length - 2);
        }

        return token;
    }

}
