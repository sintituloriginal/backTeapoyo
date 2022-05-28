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

namespace proyecto.Controllers.web.mantenedores.tipoDeudas
{


    [Authorize]
    public class plazoSelectorController : ApiController
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
                tipoDeudasModel modelo = new tipoDeudasModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("plazoSelector", modelo.SelectCargarPlazoSelector());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        //public object Get()
        //{
        //    try
        //    {
        //        tipoDeudasModel modeloPlazo = new tipoDeudasModel();
        //        Dictionary<string, object> resultado = new Dictionary<string, object>();
        //        resultado.Add("PlazoSelector", modeloPlazo.SelectCargarPlazoSelector());
        //        return Ok(resultado);
        //    }
        //    catch (Exception ex)
        //    {

        //        return InternalServerError(ex);
        //    }
        //}
    }


    [Authorize]
    public class tipoDeudasController : ApiController
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
                tipoDeudasModel modelo = new tipoDeudasModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("data", modelo.SelectCargarDatos());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Post(tipoDeudasRequest request)
        {
            try
            {
                tipoDeudasModel modelo = new tipoDeudasModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.comprobarCreacionNombre(request))
                {
                    resultado.Add("error", "Código o nombre de deuda ya existe, reintente nuevamente");
                    return Ok(resultado);
                }

                resultado.Add("resultado", modelo.InsertDEUDA(request));
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
                string idDeuda = HttpContext.Current.Request.QueryString.Get("idDeuda");
                tipoDeudasModel modelo = new tipoDeudasModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminar(idDeuda));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Put(tipoDeudasRequest request)
        {

            try
            {
                tipoDeudasModel modelo = new tipoDeudasModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.comprobarCreacionNombre(request))
                {
                    resultado.Add("error", "Código o nombre de deuda ya existe, reintente nuevamente");
                    return Ok(resultado);
                }
                resultado.Add("resultado", modelo.UpdateDEUDA(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }


    [Authorize]
    public class deuda_datosDEUDAController : ApiController
    {
        //POST api/Region_datosREGION
        public object Post(tipoDeudasRequest request)
        {
            try
            {
                tipoDeudasModel modelo = new tipoDeudasModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                tipoDeudasRequest deuda = (tipoDeudasRequest)modelo.SelectCargarDEUDAS(request);
                resultado.Add("datosDEUDA", deuda);
                resultado.Add("plazoSelector", modelo.SelectCargarPlazoSelector());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    public class Deuda_ExcelController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(tipoDeudasRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<tipoDeudasRequest> listado = (List<tipoDeudasRequest>)js.Deserialize(request.deudaExcel, typeof(List<tipoDeudasRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("DEUDA");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = { "ID DEUDA", "NOMBRE DEUDA", "NOMBRE PLAZO" };

                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    sheet1.Cells[1, c].Style.Font.Bold = true;
                    sheet1.Cells[1, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet1.Cells[1, c].Style.WrapText = true;
                    c += 1;
                }
                sheet1.Cells["A1:C1"].AutoFitColumns();

                int f = 2;
                foreach (tipoDeudasRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.idDeuda;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.nombreDeuda.Trim();
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 3].Value = obj.nombrePlazo.Trim();
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
                response.Content.Headers.ContentDisposition.FileName = "Deudas_" + fechaHoy + ".xlsx";

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
