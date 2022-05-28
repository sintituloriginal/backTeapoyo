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

namespace proyecto.Controllers.web.mantenedores.valorizacion
{
    [Authorize]
    public class valorizacionController : ApiController
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
               valorizacionModel modelo = new valorizacionModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("data", modelo.SelectCargarDatos());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Post(tipoValorizacionRequest request)
        {
            try
            {
                valorizacionModel modelo = new valorizacionModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.comprobarCreacionNombre(request))
                {
                    resultado.Add("error", "Código o nombre valorizacion ya existe, reintente nuevamente");
                    return Ok(resultado);
                }

                resultado.Add("resultado", modelo.InsertVALORIZACION(request));
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
                string idValorizacion = HttpContext.Current.Request.QueryString.Get("idValorizacion");
                valorizacionModel modelo = new valorizacionModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminar(idValorizacion));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Put(tipoValorizacionRequest request)
        {
            try
            {
                valorizacionModel modelo = new valorizacionModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.comprobarCreacionNombre(request))
                {
                    resultado.Add("error", "Código o nombre valorizacion ya existe, reintente nuevamente");
                    return Ok(resultado);
                }
                resultado.Add("resultado", modelo.UpdateVALORIZACION(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }


    public class Valorizacion_ExcelController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(tipoValorizacionRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<tipoValorizacionRequest> listado = (List<tipoValorizacionRequest>)js.Deserialize(request.valorizacionExcel, typeof(List<tipoValorizacionRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("Valorizacion");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = { "ID VALORIZACIÓN", "NOMBRE VALORIZACIÓN" };

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
                foreach (tipoValorizacionRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.idValorizacion;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.nombreValorizacion.Trim();
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

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
                response.Content.Headers.ContentDisposition.FileName = "Valorizacion_" + fechaHoy + ".xlsx";

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
