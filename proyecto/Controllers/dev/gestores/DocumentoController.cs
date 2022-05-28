

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace proyecto.Controllers.dev.gestores
{
    [Authorize]
    public class DocumentoController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public object Get(string ID_EMPRESA)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("tipo_documento", modelo.get_tipo_documento());
                resultado.Add("area", modelo.get_area(ID_EMPRESA));
                resultado.Add("fecha_minima", modelo.get_fecha_minima());
                resultado.Add("norma", modelo.get_norma(ID_EMPRESA));
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Put(DatosDocumento request)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.filtrar(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        public object Post(Datos_documento_crear request)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (!modelo.existe_documento(request))
                {
                    resultado.Add("resultado", modelo.guardar(request));
                }
                else
                {
                    resultado.Add("resultado", "Código ya existe");
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Get(string ID, int x)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos_editar", modelo.datos_editar(ID));
                resultado.Add("tipo_documento", modelo.get_tipo_documento());
                resultado.Add("area", modelo.get_area("002"));
                resultado.Add("palabra_clave", modelo.get_palabra_clave());
                resultado.Add("fecha_minima", modelo.get_fecha_minima());
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Patch(Datos_documento_crear request)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                //if (modelo.existe_documento(request))
                //{
                resultado.Add("resultado", modelo.actualizar(request));
                //}
                //else
                //{
                //    resultado.Add("resultado", "Código ya existe");
                //}
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Delete()
        {
            try
            {
                string indice = HttpContext.Current.Request.QueryString.Get("indice");
                string codigo = HttpContext.Current.Request.QueryString.Get("codigo");
                string id_empresa = HttpContext.Current.Request.QueryString.Get("id_empresa");
                string version = HttpContext.Current.Request.QueryString.Get("version");
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.es_publicadoD(codigo,id_empresa,version))
                {
                    resultado.Add("resultado", modelo.eliminar(codigo,id_empresa,version,indice));
                }
                else
                {
                    resultado.Add("resultado", "Los documentos publicados no pueden ser eliminados");
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class DocumentoPublicarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        public object Post(datosDocumentoPublicar request)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                bool publicado = modelo.estaPublicado(request);
                if (publicado)
                {
                    //resultado.Add("resultado", modelo.guardar(request));
                }
                else
                {
                    resultado.Add("resultado", modelo.publicarDocumento(request));
                    //resultado.Add("resultado", "Código ya existe");
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }

    public class Documento_dosController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Get()
        {
            try
            {
                string ID_EMPRESA = HttpContext.Current.Request.QueryString.Get("ID_EMPRESA");
                string CODIGO = HttpContext.Current.Request.QueryString.Get("CODIGO");

                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.es_administrador() == "true")
                {
                    resultado.Add("datos", modelo.cargarDatos(ID_EMPRESA,CODIGO));
                    resultado.Add("es_administrador", "si");
                }
                else{
                    resultado.Add("datos", modelo.cargarDatos_con_restricciones(ID_EMPRESA,CODIGO));
                    resultado.Add("es_administrador", "no");
                }
                resultado.Add("area", modelo.get_area(ID_EMPRESA));
                resultado.Add("es_jefe_area", modelo.es_jefe_area());
                resultado.Add("tipo", modelo.get_tipo_documento());
                resultado.Add("norma", modelo.get_norma(ID_EMPRESA));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        public object Get(string ID)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                if (modelo.es_publicado(ID))
                {
                    resultado.Add("resultado", modelo.publicar(ID));
                }
                else
                {
                    resultado.Add("resultado", "Documento ya se encuentra publicado");
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Put(DatosDocumento_historico request)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos_historial", modelo.cargarDatos_historial(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object post(DatosDocumento_historico request)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.despublicar(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }    //[Authorize]
    public class DocumentoExcelController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public HttpResponseMessage Post(documentoRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<DatosConstantes> listadoConstantes = (List<DatosConstantes>)js.Deserialize(request.constantes, typeof(List<DatosConstantes>));
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Constantes");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = "Constantes";
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "ID Constante", "Nombre", "Tipo Dato", "Valor", "Descripción" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[4, c].Value = header[i];
                    c += 1;
                }
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["B4:I4"].AutoFitColumns();

                //escribimos los usuarios de la bd
                int f = 5;

                //constantesModel modelo = new constantesModel();
                //var aux = modelo.generarExcel(request);
                foreach (DatosConstantes obj in listadoConstantes)
                {
                    sheet1.Cells[f, 2].Value = obj.id_constante;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.nombre;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.tipoDato;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.valor;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 6].Value = obj.descripcion;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    f++;
                }

                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "constantes.xlsx";

                return response;

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }

    }

    public class documento_ExportarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Post(DatosDocumento request)
        {
            try
            {
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Listado de Documentos");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.
                

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["A1:J1"].Style.Font.Bold = true;
                sheet1.Cells["A1:J1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 1;
                string[] header = { "Código", "Documento", "Versión", "Tipo", "Ult. revisión", "Caducidad", "Área", "Usuario","Empresa", "Estado" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    c += 1;
                }
                int f = 2;
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["A1:J1"].AutoFitColumns();

                //escribimos los usuarios de la bd

                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                List<DatosDocumento> listadoRegistro = (List<DatosDocumento>) modelo.filtrar(request);

                foreach (DatosDocumento obj in listadoRegistro)
                {
                    sheet1.Cells[f, 1].Value = obj.codigo;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 2].Value = obj.nombre;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.version;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.tipo;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.ult_revision;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 6].Value = obj.caducidad;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 7].Value = obj.area;
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 8].Value = obj.usuario;
                    sheet1.Cells[f, 8].AutoFitColumns();
                    sheet1.Cells[f, 9].Value = obj.nombre_empresa;
                    sheet1.Cells[f, 9].AutoFitColumns();
                    sheet1.Cells[f, 10].Value = obj.estado;
                    sheet1.Cells[f, 10].AutoFitColumns();
                    f++;
                }

                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Listado de Documentos.xlsx";

                return response;

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }
    }
    public class registrarActividadController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Post(registroActividad request)
        {
            try
            {
                documentoModel modelo = new documentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("registro_actividad", modelo.gurdarRegistroActividad(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
