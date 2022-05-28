using OfficeOpenXml;
using OfficeOpenXml.Style;
using proyecto.Models.dev.reportes;
using proyecto.objects.dev.reportes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace proyecto.Controllers.dev.reportes
{
    public class informeAuditoriaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/Usuario
        public object post(datosInformeAuditoria datos)
        {
            try
            {
                informeAuditoriaModel modelo = new informeAuditoriaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosTabla", modelo.cargarDatos(datos));
                resultado.Add("titulos", modelo.titulos(datos));
                resultado.Add("auditorias", modelo.auditorias(datos));
                resultado.Add("normas", modelo.normas(datos));
                resultado.Add("auditores", modelo.auditor(datos));

                //DateTime FirstDay = new DateTime(DateTime.Now.Year, 1, 1);
                //DateTime LastDay = new DateTime(DateTime.Now.Year, 12, 31);
                //resultado.Add("fecha_inicio", FirstDay.ToString("dd-MM-yyyy"));
                //resultado.Add("fecha_termino", LastDay.ToString("dd-MM-yyyy"));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Put(datosInformeAuditoria datos)
        {
            try
            {
                informeAuditoriaModel modelo = new informeAuditoriaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("auditoriasFiltro", modelo.auditoriasFiltro(datos));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }

    public class informeAuditoriaFiltroController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/Usuario


        public object post(datosInformeAuditoria datos)
        {
            try
            {
                informeAuditoriaModel modelo = new informeAuditoriaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosTabla", modelo.cargarDatos(datos));
                resultado.Add("titulos", modelo.titulos(datos));
                resultado.Add("auditorias", modelo.auditorias(datos));
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }


    public class auditoria_ExportarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Post(datosAuditoriaExportar request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = 500000000;
                List<datosAuditoriaExportar> listadoPanelControl = (List<datosAuditoriaExportar>)js.Deserialize(request.EXPORTAR_EXCEL, typeof(List<datosAuditoriaExportar>));

                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Listado de Auditorías");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                /*sheet1.Cells["B2"].Value = "Listado de Planificaciones";
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                */

                //escribimos el encabezado de la tabla
                int c = 1;
                string[] header = { "Norma", "Versión", "Fecha Auditoría", "Auditor", "N°", "Requisito", "Cumple", "Detalle", "Fecha Revisión", "Observación Auditoría" };

                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    sheet1.Cells["A1:J1"].Style.Font.Bold = true;
                    sheet1.Cells["A1:J1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    c += 1;
                }
                int f = 2;
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["A1:J1"].AutoFitColumns();
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#EFEFEF");

                foreach (datosAuditoriaExportar obj in listadoPanelControl)
                {
                    if (obj.VERSION != null && obj.VERSION != "")
                    {
                        sheet1.Cells[f, 1].Value = obj.NOMBRE_NORMA;
                        sheet1.Cells[f, 1].AutoFitColumns();
                        sheet1.Cells[f, 2].Value = obj.VERSION;
                        sheet1.Cells[f, 2].AutoFitColumns();
                        sheet1.Cells[f, 3].Value = obj.FECHA;
                        sheet1.Cells[f, 3].AutoFitColumns();
                        sheet1.Column(3).Width = 15;
                        sheet1.Cells[f, 4].Value = obj.AUDITOR;
                        sheet1.Cells[f, 4].AutoFitColumns();
                        sheet1.Column(3).Width = 15;
                        sheet1.Cells[f, 5].Value = obj.ID_PUNTO_NORMATIVO;
                        sheet1.Cells[f, 5].AutoFitColumns();
                        sheet1.Cells[f, 6].Value = obj.DESC_PUNTO_NORMATIVO;
                        sheet1.Cells[f, 6].AutoFitColumns();
                        sheet1.Column(6).Width = 100;
                        sheet1.Cells[f, 7].Value = obj.EVALUACION;
                        sheet1.Cells[f, 7].AutoFitColumns();
                        sheet1.Cells[f, 8].Value = obj.OBSERVACION;
                        sheet1.Cells[f, 8].AutoFitColumns();
                        sheet1.Column(8).Width = 16;
                        sheet1.Cells[f, 9].Value = obj.FECHA_REVISION;
                        sheet1.Cells[f, 9].AutoFitColumns();
                        sheet1.Column(9).Width = 15;
                        sheet1.Cells[f, 10].Value = obj.DESCRIPCION;
                        sheet1.Cells[f, 10].AutoFitColumns();
                        sheet1.Column(10).Width = 20;
                    }
                    else
                    {
                        //sheet1.Cells["A"+f+":I"+f].Style.Font.Bold = true;
                        sheet1.Cells["A" + f + ":J" + f].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet1.Cells["A" + f + ":J" + f].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        sheet1.Cells[f, 1].Value = obj.NOMBRE_NORMA;
                        sheet1.Cells[f, 1].AutoFitColumns();
                        sheet1.Cells[f, 2].Value = obj.VERSION;
                        sheet1.Cells[f, 2].AutoFitColumns();
                        sheet1.Cells[f, 3].Value = obj.FECHA;
                        sheet1.Cells[f, 3].AutoFitColumns();
                        sheet1.Cells[f, 4].Value = obj.AUDITOR;
                        sheet1.Cells[f, 4].AutoFitColumns();
                        sheet1.Cells[f, 5].Value = obj.ID_PUNTO_NORMATIVO;
                        sheet1.Cells[f, 5].AutoFitColumns();
                        sheet1.Cells[f, 6].Value = obj.DESC_PUNTO_NORMATIVO.ToUpper();
                        sheet1.Cells[f, 6].AutoFitColumns();
                        sheet1.Cells[f, 7].Value = obj.EVALUACION;
                        sheet1.Cells[f, 7].AutoFitColumns();
                        sheet1.Cells[f, 8].Value = obj.OBSERVACION;
                        sheet1.Cells[f, 8].AutoFitColumns();
                        sheet1.Cells[f, 9].Value = obj.FECHA_REVISION;
                        sheet1.Cells[f, 9].AutoFitColumns();
                        sheet1.Cells[f, 10].Value = obj.DESCRIPCION;
                        sheet1.Cells[f, 10].AutoFitColumns();
                    }



                    f++;
                }

                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Listado de Auditorías.xlsx";

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