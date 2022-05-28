using proyecto.Models.dev.reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using OfficeOpenXml;
using System.Net.Http.Headers;



namespace proyecto.Controllers.dev.reportes.reporteVisualizacionDocumento
{

    public class reporteVisualizacionDocumentoController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/Usuario
        public object Post(ReporteVisualizacionDocumento request)
        {
            try
            {
                reporteVisualizacionDocumentoModel modelo = new reporteVisualizacionDocumentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosTablaRVD", modelo.cargarDatos(request));
                resultado.Add("totalVisualizados", modelo.totalVisualizados(request));
                resultado.Add("totalDescargados", modelo.totalDescargados(request));
                resultado.Add("totalImpresos", modelo.totalImpresos(request));
                resultado.Add("filtroUsuarios", modelo.listaUsuarios());
                resultado.Add("filtroDocumentos", modelo.listaDocumentos());

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        } 
    }

        
        public class registro_visualizar_exportarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        
        // GET api/<controller>
        public HttpResponseMessage Post(ReporteVisualizacionDocumento request)
        {
            try
            {
                string nombreExportar = "" ;
                switch(request.tipoActividad)
                {
                    case "1":
                    nombreExportar = "Reporte de Visualización de Documentos";
                            break;
                    case "2":
                    nombreExportar = "Reporte de Impresión de Documentos";
                            break;
                    case "3":
                    nombreExportar = "Reporte de Descarga de Documentos";
                            break;
                }
                
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add(nombreExportar);

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.
                

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = nombreExportar;
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "Fecha revisión", "Código ", "Nombre documento", "Versión", "Tipo actividad", "Usuario" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[4, c].Value = header[i];
                    c += 1;
                }
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["B4:I4"].AutoFitColumns();

                //escribimos los usuarios de la bd
                int f = 5;

                
                reporteVisualizacionDocumentoModel modelo = new reporteVisualizacionDocumentoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                List<ReporteVisualizacionDocumento> listadoRegistro = (List<ReporteVisualizacionDocumento>) modelo.cargarDatos(request);

                foreach (ReporteVisualizacionDocumento obj in listadoRegistro)
                {
                    sheet1.Cells[f, 2].Value = obj.fechaRevision;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.codigoDocumento;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.nombreDocumento;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.versionDocumento;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 6].Value = obj.tipoActividad;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 7].Value = obj.fullNameUsuario;
                    sheet1.Cells[f, 7].AutoFitColumns();
                    f++;
                }
                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = nombreExportar+".xlsx";

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
}