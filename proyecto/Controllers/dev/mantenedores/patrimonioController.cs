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
    public class patrimonioController : ApiController
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
               patrimonioModel modelo = new patrimonioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("data", modelo.SelectCargarDatos());
                resultado.Add("total", modelo.SelectCargarTotal());
                resultado.Add("dataAdmin", modelo.SelectCargarDatosAdministrador());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Post(patrimonioRequest request)
        {
            try
            {
                patrimonioModel modelo = new patrimonioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                resultado.Add("resultado", modelo.InsertPATRIMONIO(request));
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
                string idPatrimonio = HttpContext.Current.Request.QueryString.Get("idPatrimonio");
                patrimonioModel modelo = new patrimonioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminar(idPatrimonio));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Put(patrimonioRequest request)
        {

            try
            {
                patrimonioModel modelo = new patrimonioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.UpdatePATRIMONIO(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }


    [Authorize]
    public class Patrimonio_selectoresController : ApiController
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
                patrimonioModel modeloPlazo = new patrimonioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("ActivoSelector", modeloPlazo.SelectCargarActivoSelector());
                resultado.Add("ValorizacionSelector", modeloPlazo.SelectCargarValorizacionSelector());
                resultado.Add("DeudaSelector", modeloPlazo.SelectCargarDeudaSelector());
                resultado.Add("BancoSelector", modeloPlazo.SelectCargarBancoSelector());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    [Authorize]
    public class Patrimonio_TotalController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        //GET api/Provincia_selectores
        public object Post(patrimonioRequest request)
        {
            try
            {
                patrimonioModel modeloPlazo = new patrimonioModel();
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
    public class Patrimonio_FiltroController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        //GET api/Provincia_selectores
        public object Post(patrimonioRequest request)
        {
            try
            {
                patrimonioModel modeloPlazo = new patrimonioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("filtro", modeloPlazo.filtrarNombreCarga(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }


    [Authorize]
    public class patrimonio_datosPATRIMONIOController : ApiController
    {
        //POST api/Region_datosREGION
        public object Post(patrimonioRequest request)
        {
            try
            {
                patrimonioModel modelo = new patrimonioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                patrimonioRequest patrimonio = (patrimonioRequest)modelo.SelectCargarPATRIMONIO(request);
                resultado.Add("datosPATRIMONIO", patrimonio);
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    public class Patrimonio_ExcelAdminController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(patrimonioRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<patrimonioRequest> listado = (List<patrimonioRequest>)js.Deserialize(request.patrimonioExcel, typeof(List<patrimonioRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("PATIRMONIO");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = { "USUARIO", "NOMBRE DE USUARIO", "ID PATRIMONIO", "NOMBRE ACTIVO", "TIPO ACTIVO", "TIPO DEUDA", "MONTO", "VALORIZACION", "TOTAL" };

                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    sheet1.Cells[1, c].Style.Font.Bold = true;
                    sheet1.Cells[1, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet1.Cells[1, c].Style.WrapText = true;
                    c += 1;
                }
                sheet1.Cells["A1:G1"].AutoFitColumns();

                int f = 2;
                foreach (patrimonioRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.username;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.fullname;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 3].Value = obj.idPatrimonio;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 4].Value = obj.nombreItem.Trim();
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 5].Value = obj.nombreActivo.Trim();
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 6].Value = obj.nobmreDeuda.Trim();
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 7].Value = Convert.ToInt32(obj.montoActivo);
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 8].Value = obj.nombreValorizacion.Trim();
                    sheet1.Cells[f, 8].AutoFitColumns();
                    sheet1.Cells[f, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 9].Value = Convert.ToInt32(obj.total);
                    sheet1.Cells[f, 9].AutoFitColumns();
                    sheet1.Cells[f, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
                response.Content.Headers.ContentDisposition.FileName = "Patrimonio_" + fechaHoy + ".xlsx";

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

    public class Patrimonio_ExcelController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(patrimonioRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<patrimonioRequest> listado = (List<patrimonioRequest>)js.Deserialize(request.patrimonioExcel, typeof(List<patrimonioRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("PATIRMONIO");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = { "ID PATRIMONIO", "NOMBRE ACTIVO", "TIPO ACTIVO", "TIPO DEUDA", "MONTO", "VALORIZACION", "TOTAL"};

                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    sheet1.Cells[1, c].Style.Font.Bold = true;
                    sheet1.Cells[1, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet1.Cells[1, c].Style.WrapText = true;
                    c += 1;
                }
                sheet1.Cells["A1:G1"].AutoFitColumns();

                int f = 2;
                foreach (patrimonioRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.idPatrimonio;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.nombreItem.Trim();
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 3].Value = obj.nombreActivo.Trim();
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 4].Value = obj.nobmreDeuda.Trim();
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 5].Value = obj.montoActivo.Trim();
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 6].Value = obj.nombreValorizacion.Trim();
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;



                    sheet1.Cells[f, 7].Value = obj.total.Trim();
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
                response.Content.Headers.ContentDisposition.FileName = "Patrimonio_" + fechaHoy + ".xlsx";

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
