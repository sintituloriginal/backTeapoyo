
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace proyecto.Controllers.web.mantenedores.perfiles
{
    [Authorize]
    public class PerfilesController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        //[Authorize]
        public object Get()
        {
            try
            {
                PerfilesModel modelo = new PerfilesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarDatos());
                resultado.Add("nombres", modelo.nombres());


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object get(string CargaNuevo)
        {
            try
            {
                PerfilesModel modelo = new PerfilesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("branch", modelo.branch());
                resultado.Add("sheet", modelo.sheet());
                resultado.Add("acciones", modelo.acciones());

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Put(perfilesRequest reques)
        {
            try
            {
                PerfilesModel modelo = new PerfilesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("guardar", modelo.guardar(reques));


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Get(int id_branch)
        {
            try
            {
                PerfilesModel modelo = new PerfilesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("branch", modelo.branch());
                resultado.Add("sheet", modelo.sheet());
                resultado.Add("acciones", modelo.acciones());
                resultado.Add("datos", modelo.obtenerDatosPerfil(id_branch));
                resultado.Add("accionesPerfil", modelo.obtenerAccionesScreenPerfil(id_branch));
                resultado.Add("datosScreen", modelo.obtenerScreen(id_branch));



                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object post(perfilesRequest reques)
        {
            try
            {
                PerfilesModel modelo = new PerfilesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("guardar", modelo.actualizar(reques));


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Patch(Datosperfiles id_perfil)
        {
            try
            {
                PerfilesModel modelo = new PerfilesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("branch", modelo.eliminar(id_perfil));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }

    public class perfilExcelController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public HttpResponseMessage Post(perfilesRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<Datosperfiles> listadoPerfiles = (List<Datosperfiles>)js.Deserialize(request.perfilExcel, typeof(List<Datosperfiles>));
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Perfiles");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = "Perfiles";
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "Nombre Perfil", "Descripción", "Fecha Creación", "Fecha Modificación" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[4, c].Value = header[i];
                    c += 1;
                }
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["B4:I4"].AutoFitColumns();


                int f = 5;


                foreach (Datosperfiles obj in listadoPerfiles)
                {
                    sheet1.Cells[f, 2].Value = obj.nombre;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.descripcion;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.fecha_created;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.fecha_updated;
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
                response.Content.Headers.ContentDisposition.FileName = "Perfiles.xlsx";

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