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

namespace proyecto.Controllers.web.mantenedores.banco
{
    [Authorize]
    public class bancoController : ApiController
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
                bancoModel modelo = new bancoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("data", modelo.SelectCargarDatos());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Post(tipoBancoRequest request)
        {
            try
            {
                bancoModel modelo = new bancoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.comprobarCreacionNombre(request))
                {
                    resultado.Add("error", "Código o nombre banco ya existe, reintente nuevamente");
                    return Ok(resultado);
                }

                resultado.Add("resultado", modelo.InsertBANCO(request));
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
                string idBanco = HttpContext.Current.Request.QueryString.Get("idBanco");
                bancoModel modelo = new bancoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminar(idBanco));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Put(tipoBancoRequest request)
        {
            try
            {
                bancoModel modelo = new bancoModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.comprobarCreacionNombre(request))
                {
                    resultado.Add("error", "Código o nombre banco ya existe, reintente nuevamente");
                    return Ok(resultado);
                }
                resultado.Add("resultado", modelo.UpdateBANCO(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }


    public class Banco_ExcelController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(tipoBancoRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<tipoBancoRequest> listado = (List<tipoBancoRequest>)js.Deserialize(request.bancoExcel, typeof(List<tipoBancoRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("BANCO");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = { "ID BANCO", "NOMBRE BANCO" };

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
                foreach (tipoBancoRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.idBanco;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.nombreBanco.Trim();
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
                response.Content.Headers.ContentDisposition.FileName = "Banco_" + fechaHoy + ".xlsx";

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
