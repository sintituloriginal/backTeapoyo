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
    public class financieraController : ApiController
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
                financieraModel modelo = new financieraModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("data", modelo.SelectCargarDatos());
                resultado.Add("total", modelo.SelectCargarTotal());            
                resultado.Add("totalActualCortoPlazo", modelo.SelectTotalActualCortoPlazo());
                resultado.Add("totalConsumoFuturo", modelo.SelectCargarTotalConsumoFuturo());
                resultado.Add("totalActualLargoPlazo", modelo.SelectCargarTotalActualLargoPlazo());
                resultado.Add("totalHipotecarioFuturo", modelo.SelectCargarTotalHiptecarioFuturo());
                resultado.Add("pagoMensualCortoPlazo", modelo.SelectCargarPagoMensualCortoPLazo());
                resultado.Add("pagoMensualLargoPlazo", modelo.SelectCargarPagoMensualLargoPlazo());
                resultado.Add("dataAdmin", modelo.SelectCargarDatosAdmin());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Post(financieraRequest request)
        {
            try
            {
                financieraModel modelo = new financieraModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                resultado.Add("resultado", modelo.InsertFINANCIERA(request));
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
                string idFinanciera = HttpContext.Current.Request.QueryString.Get("idFinanciera");
                financieraModel modelo = new financieraModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminar(idFinanciera));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        public object Put(financieraRequest request)
        {

            try
            {
                financieraModel modelo = new financieraModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.UpdateFINANCIERA(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    [Authorize]
    public class Financiera_totalesController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        //GET api/Provincia_selectores
        public object Post(financieraRequest request)
        {
            try
            {
                financieraModel modelo = new financieraModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("totalAdmin", modelo.SelectCargarTotalAdmin(request));
                resultado.Add("totalActualCortoPlazoAdmin", modelo.SelectTotalActualCortoPlazoAdmin(request));
                resultado.Add("totalConsumoFuturoAdmin", modelo.SelectCargarTotalConsumoFuturoAdmin(request));
                resultado.Add("totalActualLargoPlazoAdmin", modelo.SelectCargarTotalActualLargoPlazoAdmin(request));
                resultado.Add("totalHipotecarioFuturoAdmin", modelo.SelectCargarTotalHiptecarioFuturoAdmin(request));
                resultado.Add("pagoMnesualCortoPlazoAdmin", modelo.SelectCargarPagoMensualCortoPLazoAdmin(request));
                resultado.Add("pagoMensualLargoPlazoAdmin", modelo.SelectCargarPagoMensualLargoPlazoAdmin(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }


    [Authorize]
    public class Financiera_selectoresController : ApiController
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
                financieraModel modeloPlazo = new financieraModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("plazoSelector", modeloPlazo.SelectCargarPlazoSelector());
                resultado.Add("deudaSelector", modeloPlazo.SelectCargarDeudaSelector());
                resultado.Add("bancoSelector", modeloPlazo.SelectCargarBancoSelector());
                resultado.Add("UsuarioSelector", modeloPlazo.SelectCargarUsuarioSelector());
                resultado.Add("UsuarioNombreSelector", modeloPlazo.SelectCargarUserSelector());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    [Authorize]
    public class Filtrar_FinancieraFINANCIERAController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        //POST api/Region_datosREGION
        public object Post(financieraRequest request)
        {
            try
            {
                financieraModel modelo = new financieraModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("FiltroFinanciera", modelo.FiltrarTabla(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }


    [Authorize]
    public class financiera_datosFINANCIERAController : ApiController
    {
        //POST api/Region_datosREGION
        public object Post(financieraRequest request)
        {
            try
            {
                financieraModel modelo = new financieraModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                financieraRequest financiera = (financieraRequest)modelo.SelectCargarFINANCIERA(request);
                resultado.Add("datosFINANCIERA", financiera);
                resultado.Add("dedudaSelector", modelo.SelectCargarDeudaSelector());
                resultado.Add("plazoSelector", modelo.SelectCargarPlazoSelector());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }



    public class financiera_ExcelAdminController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(financieraRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<financieraRequest> listado = (List<financieraRequest>)js.Deserialize(request.financieraExcel, typeof(List<financieraRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("FINANCIERA");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = {"USUARIO", "NOMBRE DE USUARIO", "ID CARGA FINANCIERA", "NOMBRE DEUDA", "TIPO DEUDA", "TIPO PLAZO", "SALDO", "CUPO", "VALOR CUOTA", "PAGO MENSUAL" };

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
                foreach (financieraRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.username;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.fullname;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 3].Value = obj.idFinanciera;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 4].Value = obj.nombreDeuda.Trim();
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 5].Value = obj.TipoDeduda.Trim();
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 6].Value = obj.nombrePlazo.Trim();
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 7].Value = obj.saldoDeuda;
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 8].Value = obj.cupoTotal;
                    sheet1.Cells[f, 8].AutoFitColumns();
                    sheet1.Cells[f, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 9].Value = obj.valorCuota;
                    sheet1.Cells[f, 9].AutoFitColumns();
                    sheet1.Cells[f, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 10].Value = obj.pagoMensual;
                    sheet1.Cells[f, 10].AutoFitColumns();
                    sheet1.Cells[f, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
                response.Content.Headers.ContentDisposition.FileName = "Financiera_" + fechaHoy + ".xlsx";

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


    public class  reporte_ExcelController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(financieraRequest request, ingresoRequest request2)
        {
            try
            {

                JavaScriptSerializer js = new JavaScriptSerializer();
                List<financieraRequest> listado = (List<financieraRequest>)js.Deserialize(request.financieraExcel, typeof(List<financieraRequest>));
                List<ingresoRequest> listado2 = (List<ingresoRequest>)js.Deserialize(request2.ingresoExcel, typeof(List<ingresoRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("FINANCIERA");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = { "ID CARGA FINANCIERA", "NOMBRE DEUDA", "TIPO DEUDA", "TIPO PLAZO", "SALDO", "CUPO", "VALOR CUOTA", "PAGO MENSUAL" };

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
                foreach (financieraRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.idFinanciera;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.nombreDeuda.Trim();
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 3].Value = obj.TipoDeduda.Trim();
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 4].Value = obj.nombrePlazo.Trim();
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 5].Value = obj.saldoDeuda;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 6].Value = obj.cupoTotal;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 7].Value = obj.valorCuota;
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 8].Value = obj.pagoMensual;
                    sheet1.Cells[f, 8].AutoFitColumns();
                    sheet1.Cells[f, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    f++;
                }

                excel.Workbook.Worksheets.Add("Gestión de Ingresos");
                ExcelWorksheet sheet2 = excel.Workbook.Worksheets[2];
                int d = 1;
                string[] header2 = { "USUARIO", "NOMBRE DE USUARIO", "ID INGRESO", "DESCRIPCIÓN", "TIPO DE INGRESO", "MONTO" };

                for (int i = 0; i <= header2.Length - 1; i++)
                {
                    sheet2.Cells[1, d].Value = header2[i];
                    sheet2.Cells[1, d].Style.Font.Bold = true;
                    sheet2.Cells[1, d].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet2.Cells[1, d].Style.WrapText = true;
                    d += 1;
                }
                sheet2.Cells["A1:E1"].AutoFitColumns();

                int l = 2;
                foreach (ingresoRequest obj in listado2)
                {
                    sheet2.Cells[l, 1].Value = obj.username;
                    sheet2.Cells[l, 1].AutoFitColumns();
                    sheet2.Cells[l, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet2.Cells[l, 1].Value = obj.fullname;
                    sheet2.Cells[l, 1].AutoFitColumns();
                    sheet2.Cells[l, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet2.Cells[l, 1].Value = obj.idIngreso;
                    sheet2.Cells[l, 1].AutoFitColumns();
                    sheet2.Cells[l, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet2.Cells[l, 2].Value = obj.descripcion.Trim();
                    sheet2.Cells[l, 2].AutoFitColumns();
                    sheet2.Cells[l, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet2.Cells[l, 3].Value = obj.nombreTipoIngreso.Trim();
                    sheet2.Cells[l, 3].AutoFitColumns();
                    sheet2.Cells[l, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet2.Cells[l, 4].Value = obj.montoCarga.Trim();
                    sheet2.Cells[l, 4].AutoFitColumns();
                    sheet2.Cells[l, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    l++;
                }


                excel.Workbook.Worksheets.Add("Gestión de Patrimonios");
                ExcelWorksheet sheet3 = excel.Workbook.Worksheets[3];
                int e = 1;
                string[] header3 = { "ID PATRIMONIO", "NOMBRE ACTIVO", "TIPO ACTIVO", "TIPO DEUDA", "MONTO", "VALORIZACION", "TOTAL" };

                for (int i = 0; i <= header3.Length - 1; i++)
                {
                    sheet3.Cells[1, e].Value = header3[i];
                    sheet3.Cells[1, e].Style.Font.Bold = true;
                    sheet3.Cells[1, e].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    sheet3.Cells[1, e].Style.WrapText = true;
                    e += 1;
                }
                sheet3.Cells["A1:E1"].AutoFitColumns();


                excel.Workbook.Worksheets.Add("Indicadores");
                ExcelWorksheet sheet4 = excel.Workbook.Worksheets[3];

                //string[] header3 = { "ID PATRIMONIO", "NOMBRE ACTIVO", "TIPO ACTIVO", "TIPO DEUDA", "MONTO", "VALORIZACION", "TOTAL" };

                //for (int i = 0; i <= header3.Length - 1; i++)
                //{
                //    sheet3.Cells[1, e].Value = header3[i];
                //    sheet3.Cells[1, e].Style.Font.Bold = true;
                //    sheet3.Cells[1, e].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //    sheet3.Cells[1, e].Style.WrapText = true;
                //    e += 1;
                //}
                //sheet3.Cells["A1:E1"].AutoFitColumns();

                byte[] content = excel.GetAsByteArray();

                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                string fechaHoy = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");
                response.Content.Headers.ContentDisposition.FileName = "Financiera_" + fechaHoy + ".xlsx";

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

    public class financiera_ExcelController : ApiController
    {

        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //GET api/<controller>
        public HttpResponseMessage Post(financieraRequest request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<financieraRequest> listado = (List<financieraRequest>)js.Deserialize(request.financieraExcel, typeof(List<financieraRequest>));
                ExcelPackage excel = new ExcelPackage();
                excel.Workbook.Worksheets.Add("FINANCIERA");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                int c = 1;
                string[] header = { "ID CARGA FINANCIERA", "NOMBRE DEUDA", "TIPO DEUDA", "TIPO PLAZO", "SALDO", "CUPO", "VALOR CUOTA", "PAGO MENSUAL"};

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
                foreach (financieraRequest obj in listado)
                {
                    sheet1.Cells[f, 1].Value = obj.idFinanciera;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 2].Value = obj.nombreDeuda.Trim();
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 3].Value = obj.TipoDeduda.Trim();
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 4].Value = obj.nombrePlazo.Trim();
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 5].Value = obj.saldoDeuda;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 6].Value = obj.cupoTotal;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 7].Value = obj.valorCuota;
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    sheet1.Cells[f, 8].Value = obj.pagoMensual;
                    sheet1.Cells[f, 8].AutoFitColumns();
                    sheet1.Cells[f, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
                response.Content.Headers.ContentDisposition.FileName = "Financiera_" + fechaHoy + ".xlsx";

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
