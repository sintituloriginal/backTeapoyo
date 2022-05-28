using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Threading;

namespace proyecto.Controllers.web.mantenedores.ingreso
{
    [Authorize]
    public class ingresoController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        //GET api/Region
        public object Get()
        {
            try
            {
                ingresoModel modelo = new ingresoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("data", modelo.SelectCargarDatos());
                resultado.Add("total", modelo.SelectCargarTotal());
                resultado.Add("dataAdmin", modelo.SelectCargarDatosAdmin());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Post(ingresoRequest request)
        {
            try
            {
                ingresoModel modelo = new ingresoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                resultado.Add("resultado", modelo.InsertINGRESO(request));
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
                string idIngreso = HttpContext.Current.Request.QueryString.Get("idIngreso");
                ingresoModel modelo = new ingresoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminar(idIngreso));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Put(ingresoRequest request)
        {

            try
            {
                ingresoModel modelo = new ingresoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.UpdateINGRESO(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    [Authorize]
    public class total_IngresoController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        //GET api/Provincia_selectores
        public object Post(ingresoRequest request)
        {
            try
            {
                ingresoModel modeloPlazo = new ingresoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("totalAdmin", modeloPlazo.SelectCargarTotalAdmin(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    [Authorize]
    public class Filtro_IngresoController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        //GET api/Provincia_selectores
        public object Post(ingresoRequest request)
        {
            try
            {
                ingresoModel modeloPlazo = new ingresoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("FiltroIngreso", modeloPlazo.FiltrarTabla(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }



    [Authorize]
    public class Ingreso_selectoresController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        //GET api/Provincia_selectores
        public object Get()
        {
            try
            {
                ingresoModel modeloPlazo = new ingresoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("IngresoSelector", modeloPlazo.SelectCargarIngresoSelector());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }


    [Authorize]
    public class ingreso_datosINGRESOController : ApiController
    {
        //POST api/Region_datosREGION
        public object Post(ingresoRequest request)
        {
            try
            {
                ingresoModel modelo = new ingresoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                ingresoRequest ingreso = (ingresoRequest)modelo.SelectCargarINGRESO(request);
                resultado.Add("datosINGRESO", ingreso);
                resultado.Add("ingresoSelector", modelo.SelectCargarIngresoSelector());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    public class Ingreso_gestion_ExcelAdminController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(ingresoRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ingresoRequest> listado = (List<ingresoRequest>)js.Deserialize(request.ingresoExcel, typeof(List<ingresoRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("INGRESO");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = {"USUARIO", "NOMBRE DE USUARIO", "ID INGRESO", "DESCRIPCIÓN", "TIPO DE INGRESO", "MONTO" };

                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    sheet1.Cells[1, c].Style.Font.Bold = true;
                    sheet1.Cells[1, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet1.Cells[1, c].Style.WrapText = true;
                    c += 1;
                }
                sheet1.Cells["A1:E1"].AutoFitColumns();

                int f = 2;
                foreach (ingresoRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.username;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 1].Value = obj.fullname;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 1].Value = obj.idIngreso;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.descripcion.Trim();
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 3].Value = obj.nombreTipoIngreso.Trim();
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 4].Value = obj.montoCarga.Trim();
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    f++;
                }

                byte[] content = excel.GetAsByteArray();

                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                string fechaHoy = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                response.Content.Headers.ContentDisposition.FileName = "Ingreso_" + fechaHoy + ".xlsx";

                return response;

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);

            }
        }
    }

    public class Ingreso_gestion_ExcelController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(ingresoRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ingresoRequest> listado = (List<ingresoRequest>)js.Deserialize(request.ingresoExcel, typeof(List<ingresoRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("INGRESO");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = { "ID INGRESO", "DESCRIPCIÓN", "TIPO DE INGRESO", "MONTO" };

                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    sheet1.Cells[1, c].Style.Font.Bold = true;
                    sheet1.Cells[1, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet1.Cells[1, c].Style.WrapText = true;
                    c += 1;
                }
                sheet1.Cells["A1:E1"].AutoFitColumns();

                int f = 2;
                foreach (ingresoRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.idIngreso;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.descripcion.Trim();
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 3].Value = obj.nombreTipoIngreso.Trim();
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 4].Value = obj.monto;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    f++;
                }

                byte[] content = excel.GetAsByteArray();

                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                string fechaHoy = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                response.Content.Headers.ContentDisposition.FileName = "Ingreso_" + fechaHoy + ".xlsx";

                return response;

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);

            }
        }
    }
}
