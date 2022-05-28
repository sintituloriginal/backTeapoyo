

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class NoConformidadController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
       /* public object Get()
        {
            try
            {
                NoConformidadModel modelo = new NoConformidadModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarDatos());


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }*/
        public object Post(Guardar datos)
        {
            try
            {
                NoConformidadModel modelo = new NoConformidadModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.guardar(datos));


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Put(inicial data)
        {
            try
            {
                NoConformidadModel modelo = new NoConformidadModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarInicio(data));
                resultado.Add("historial", modelo.cargarHistorial(data));


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Patch(Guardar data)
        {
            try
            {
                NoConformidadModel modelo = new NoConformidadModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.guardarRespuesta(data));


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        
    }



    [Authorize]
    public class NoConformidadFiltroController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public object Post(Cargar filtros)
       {
            try
            {
             
                NoConformidadModel modelo = new NoConformidadModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarDatos(filtros));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Put(Eliminar request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = 500000000;
                List<noConformidadRequest> datos = (List<noConformidadRequest>)js.Deserialize(request.Datos, typeof(List<noConformidadRequest>));
                NoConformidadModel modelo = new NoConformidadModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.eliminar(datos));


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }


    public class NoConformidadExportarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Post(Cargar request)
        {
            try
            {
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Listado de no conformidades");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.
                

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["A1:K1"].Style.Font.Bold = true;
                sheet1.Cells["A1:K1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 1;
                string[] header = { "Norma", "Pto.Normativo", "Descripción", "NC", "Estado", "Fecha", "Compromiso", "Responsable",  "Auditor",  "Tipo Auditoría", "Fecha Auditoría" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    c += 1;
                }
                int f = 2;
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["A1:K1"].AutoFitColumns();


                //escribimos los usuarios de la bd

                NoConformidadModel modelo = new NoConformidadModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                List<noConformidadRequest> listadoRegistro = (List<noConformidadRequest>) modelo.cargarDatos(request);

                foreach (noConformidadRequest obj in listadoRegistro)
                {
                    sheet1.Cells[f, 1].Value = obj.NOMBRE_NORMA;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 2].Value = obj.ID_PUNTO_NORMATIVO;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.DESC_PUNTO_NORMATIVO;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.NO_CONFORMIDAD;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.ESTADO;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 6].Value = obj.FECHA;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 7].Value = obj.FECHA_COMPROMISO;
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 8].Value = obj.RESPONSABLE;
                    sheet1.Cells[f, 8].AutoFitColumns();
                    sheet1.Cells[f, 9].Value = obj.TIPO_AUDITORIA;
                    sheet1.Cells[f, 9].AutoFitColumns();
                    sheet1.Cells[f, 10].Value = obj.AUDITOR;
                    sheet1.Cells[f, 10].AutoFitColumns();
                    sheet1.Cells[f, 11].Value = obj.FECHA_AUDITORIA;
                    sheet1.Cells[f, 11].AutoFitColumns();
                    f++;
                }

                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Listado de no conformidades.xlsx";

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