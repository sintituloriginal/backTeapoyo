using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace proyecto.objects.dev.gestores
{
    public class norma_data
    {
        public string ruta { get; set; }
        public string nombre_archivo { get; set; }
        public string cod_norma { get; set; }
    }

    public class erroresExcel
    {
        public string motivo { get; set; }
        public string filas { get; set; }
    }

    public class norma_excel
    {
        public string numero { get; set; }
        public string requisito { get; set; }
        public string relevancia { get; set; }
        public Boolean es_evaluable { get; set; }
        public string v_model { get; set; }
        public string respuesta { get; set; }
        public Boolean titulo { get; set; }
        public string comentario { get; set; }
        public string responsable { get; set; }
        public string resumen { get; set; }
        public string auditor { get; set; }
        public string fecha { get; set; }
        public string id_auditoria { get; set; }
        public string ultima_observacion { get; set; }
        public string no_conformidades { get; set; }
        public string id_nc { get; set; }
    }


    public class InMemoryMultipartFormDataStreamProvider : MultipartStreamProvider
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

    public class doc_asociado
    {
        public string codigo { get; set; }
        public string version { get; set; }
        public string tipo { get; set; }
        public string nombre { get; set; }
    }

    public class auditoria
    {

        public string COD_NORMA { get; set; }
        public string TIPO_AUDITORIA { get; set; }
        public string RUT_EMPRESA { get; set; }
        public string FECHA { get; set; }
        public string OBSERVACION { get; set; }
        public string USER_ID { get; set; }
        public string ESTADO { get; set; }

    }
    public class evaluacion
    {

        public string ID_AUDITORIA { get; set; }
        public string ID_PUNTO_NORMATIVO { get; set; }
        public string EVALUACION { get; set; }
        public string FECHA { get; set; }
        public string USER_ID { get; set; }
        public string TIPO { get; set; }
        public string OBSERVACION { get; set; }
        public string COD_NORMA { get; set; }
        public string USER_RESP { get; set; }
        public string NOMBRE_ARCHIVO { get; set; }

    }
    public class datosNorma
    {

        public string COD_NORMA { get; set; }
        public string VERSION { get; set; }
        public string NOMBRE_NORMA { get; set; }
        public string DESCRIPCION_NORMA { get; set; }
        public string FECHA_ACTUALIZACION { get; set; }
        public string ESTADO { get; set; }


    }
    public class datos_perfil
    {
        public string ADMINISTRADOR { get; set; }
        public string AUDITOR { get; set; }

    }
    public class responsables
    {
        public string value { get; set; }
        public string label { get; set; }

    }
    public class guardarDatos
    {
        public string datos { get; set; }
        public string version { get; set; }
        public string id_empresa { get; set; }
        public string norma { get; set; }
        public string puntoSeleccionado { get; set; }
        public string nombre_archivo { get; set; }

    }
    public class datosAuditoria
    {
        public string ID_AUDITORIA { get; set; }
        public string COD_NORMA { get; set; }
        public string TIPO_AUDITORIA { get; set; }
        public string RUT_EMPRESA { get; set; }
        public string NOMBRE_EMPRESA { get; set; }
        public string FECHA { get; set; }
        public string OBSERVACION { get; set; }
        public string USER_ID { get; set; }
        public string ESTADO { get; set; }
        public string USER { get; set; }
        public string ID_AUDITORIA_ORI { get; set; }

    }
    public class datosDocumentoPuntoNormativo
    {
        public string ID_PUNTO_NORMATIVO { get; set; }
        public string estado { get; set; }
        public string indice { get; set; }
        public string ID_EMPRESA { get; set; }
        public string COD_NORMA { get; set; }
        public string codigo_documento { get; set; }
        public string version { get; set; }
        public string tipo { get; set; }
        public string nombre { get; set; }
        public string ruta_archivo { get; set; }


    }




    public class datosNormaMantenedor
    {
        public string COD_NORMA { get; set; }
        public string VERSION { get; set; }
        public string NOMBRE_NORMA { get; set; }
        public string DESCRIPCION_NORMA { get; set; }
        public string FECHA_ACTUALIZACION { get; set; }
        public string ESTADO { get; set; }
        public string COLOR_NORMA { get; set; }
        public string COLOR_CUMPLIMIENTO { get; set; }
        public string ID_EMPRESA { get; set; }
        public string NOMBRE_EMPRESA { get; set; }
        public string EXPORTAR_EXCEL { get; set; }
    }

    public class habilitarDeshabilitarNorma
    {
        public string id { get; set; }
        public string estado { get; set; }
    }

    public class datosRequestNoConformidades
    {
        public string ID_PUNTO_NORMATIVO { get; set; }
        public string ID_AUDITORIA { get; set; }
        public string COD_NORMA { get; set; }
    }
    public class normaInt
    {
        public int COD_NORMA { get; set; }
    }
    public class idnormaEmpresa
    {
        public int id_norma { get; set; }
        public string id_empresa { get; set; }
    }

}