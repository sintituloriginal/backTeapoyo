

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

namespace proyecto.Controllers.dev.mantenedores
{
    [Authorize]
    public class AreaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public object Get(string id_empresa)
        {
            try
            {
                areaModel modelo = new areaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarDatos(id_empresa));
                

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
                areaModel modelo = new areaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarUsuarios());


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Put(areaRequest request)
        {
            try
            {
                areaModel modelo = new areaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                bool result = false;
                if (modelo.existe(request))
                {
                    resultado.Add("resultado", "Registro ya existe");
                }
                else
                {
                    result = modelo.crear(request);
                    resultado.Add("resultado", result);
                }


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Post(areaRequest request)
        {
            try
            {
                areaModel modelo = new areaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                
                resultado.Add("resultado", modelo.cargarDatosEditar(request));
                resultado.Add("datos", modelo.cargarUsuarios());

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Patch(areaRequest request)
        {
            try
            {
                areaModel modelo = new areaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                resultado.Add("resultado", modelo.actualizar(request));
                

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Delete(string id)
        {
            try
            {
                areaModel modelo = new areaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                bool result = false;
                if (modelo.areaEnUso(id))
                {
                    resultado.Add("resultado", "El área esta siendo Utilizada");
                }
                else
                {
                    result = modelo.eliminar(id);
                    resultado.Add("resultado", result);
                }

                
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }


    //[Authorize]
    public class AreaExcelController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public HttpResponseMessage Post(excelArea request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<DatosArea> listadoArea= (List<DatosArea>)js.Deserialize(request.nombre, typeof(List<DatosArea>));
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Área");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = "Área";
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "Área", "Abreviatura", "Responsable", "Estado" };
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
                foreach (DatosArea obj in listadoArea)
                {
                    sheet1.Cells[f, 2].Value = obj.NOMBRE;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.CODAREA;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.nombreUsuario;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.ESTADO;
                    sheet1.Cells[f, 5].AutoFitColumns();
                   
                    f++;
                }

                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Area.xlsx";

                return response;

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }

        public object Put(habilitarDeshabilitarArea request)
        {
            try
            {
                areaModel modelo = new areaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.habilitarDeshabilitar(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



    }
}