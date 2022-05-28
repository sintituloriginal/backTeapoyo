
using Newtonsoft.Json;
using OfficeOpenXml;
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

namespace proyecto.Controllers.web.mantenedores
{
    [Authorize]
    public class logEventoController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }


        public object Put(logEventoRequest request)
        {
            try
            {
                logEventoModel modelo = new logEventoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("logEvento", modelo.tablaLogDeEvento(request));

                resultado.Add("today", DateTime.Today.ToString("dd-MM-yyyy"));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Get()
        {
            try
            {
                logEventoModel modelo = new logEventoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("modulo", modelo.selectorModulo());
                resultado.Add("perfil", modelo.selectorPerfil());
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Post(perfilRequestPorUsuario request)
        {
            try
            {
                logEventoModel modelo = new logEventoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("usuario", modelo.usuariosPorPerfil(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }

    [Authorize]
    public class Log_Eventos_Cargar_SelectsController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Post(obj_fecha_filtro request)
        {
            try
            {
                logEventoModel modelo = new logEventoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosSelectPerfil", modelo.getDatosSelectPerfil(request));
                resultado.Add("datosSelectUsuario", modelo.getDatosSelectUsuario(request));
                resultado.Add("datosSelectModulo", modelo.getDatosSelectModulo(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }

    //[Authorize]
    public class logEventoExcelController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public HttpResponseMessage Post(excelRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = 500000000;
                List<data> listadoLog = (List<data>)js.Deserialize(request.log, typeof(List<data>));


                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Log de Eventos");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = "Log de Eventos";
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "Fecha", "Módulo", "Acción", "Perfil", "Usuario" };
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
                foreach (data obj in listadoLog)
                {
                    sheet1.Cells[f, 2].Value = obj.fecha;
                    sheet1.Cells[f, 2].AutoFitColumns();

                    sheet1.Cells[f, 3].Value = obj.modulo;
                    sheet1.Cells[f, 3].AutoFitColumns();

                    sheet1.Cells[f, 4].Value = obj.action;
                    sheet1.Cells[f, 4].AutoFitColumns();

                    sheet1.Cells[f, 5].Value = obj.perfil;
                    sheet1.Cells[f, 5].AutoFitColumns();

                    sheet1.Cells[f, 6].Value = obj.usuario;
                    sheet1.Cells[f, 6].AutoFitColumns();

                    int filaDetalle = f + 1;
                   
                    var list = JsonConvert.DeserializeObject<ListofListsRootClass>(obj.json);
                    // escribimos el encabezado de la tabla
                    
                    foreach (var item in list)
                    {
                        bool flag = false;
                        int co = 2;
                        int cDetalle = 2;
                       
                        foreach (var o in item)
                        {


                            sheet1.Cells[filaDetalle, co].Value = o.Key;
                            sheet1.Cells[filaDetalle, co].AutoFitColumns();
                            sheet1.Row(filaDetalle).OutlineLevel = 2;
                            sheet1.Row(filaDetalle).Collapsed = true;
                            co += 1;
                        }
                         
                        filaDetalle++;

                        foreach (var o in item)
                        {
                            if (flag) { filaDetalle--; }

                            sheet1.Cells[filaDetalle, cDetalle].Value = o.Value.Antiguo;
                            sheet1.Cells[filaDetalle, cDetalle].AutoFitColumns();

                            sheet1.Row(filaDetalle).OutlineLevel = 2;
                            sheet1.Row(filaDetalle).Collapsed = true;

                            filaDetalle++;

                            sheet1.Cells[filaDetalle, cDetalle].Value = o.Value.Nuevo;
                            sheet1.Cells[filaDetalle, cDetalle].AutoFitColumns();

                            sheet1.Row(filaDetalle).OutlineLevel = 2;
                            sheet1.Row(filaDetalle).Collapsed = true;
                            flag = true;
                            cDetalle++;
                        }
                        filaDetalle++;
                        f = filaDetalle;
                    }
                }
                sheet1.Cells[sheet1.Dimension.Address].AutoFitColumns(); //AJUSTO EL ANCHO DE LAS COLUMNAS DE ACUERDO AL CONTENIDO DE TODA LA WORKSHEET

                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Log de Eventos.xlsx";

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