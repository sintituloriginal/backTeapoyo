using OfficeOpenXml;
using proyecto.Models.web.mantenedores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web;

using System.Web.Script.Serialization;

namespace proyecto.Controllers.web.mantenedores
{
    [Authorize]
    public class EmpresaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/empresa
        public object Get()
        {
            try
            {
                EmpresaModel modelo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosTabla", modelo.cargarDatos());

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/crear empresa
        public object Post(empresaFullData request)
        {
            try
            {
                EmpresaModel modelo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                bool result = false;
                if (modelo.existeEmpresa(request))
                {
                    resultado.Add("resultado", "Registro ya existe");
                }
                else
                {
                    result = modelo.guardarEmpresa(request);
                    resultado.Add("resultado", result);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //Eliminar empresa
        public object Delete()
        {
            try
            {
                string id = HttpContext.Current.Request.QueryString.Get("ID");
                string codigo = HttpContext.Current.Request.QueryString.Get("CODIGO");
                EmpresaModel modelo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminar(id, codigo));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //carga editar
        public object Get(string ID)
        {
            try
            {
                EmpresaModel modelo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarEditar(ID));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //Editar empresa
        public object Patch(empresaFullData request)
        {
            try
            {
                EmpresaModel modelo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                bool result = false;
                if (modelo.existeEmpresaEditar(request))
                {
                    resultado.Add("resultado", "Registro ya existe");
                }
                else
                {
                    result = modelo.editar(request);
                    resultado.Add("resultado", result);
                }

                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //habilitar_deshabilitar
        public object Put(habilitarDeshabiltiar_empresa request)
        {
            try
            {
                EmpresaModel modelo = new EmpresaModel();
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



    public class empr_selectoresController : ApiController
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
                EmpresaModel modeloPlazo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("empresaSelector", modeloPlazo.SelectCargarEmpresaSelector());
                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }

    public class Empresa_selectoresController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        //selector empresas
        public object Get(string empresa)
        {
            try
            {
                EmpresaModel modelo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("empresa", modelo.cargar_empresa(empresa));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //filtrar
        public object Put(empresaFullData datos)
        {
            try
            {
                EmpresaModel modelo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosTabla", modelo.filtrar(datos));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class empresaDefectoController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        public object Post(updateDefaultEmpresa request)
        {
            try
            {
                EmpresaModel modelo = new EmpresaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultadoReset", modelo.resetEmpresaDefecto(request));
                resultado.Add("resultadoUpdate", modelo.cambiarEmpresaDefecto(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }

    public class empresaExcelController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public HttpResponseMessage Post(DatosEmpresa request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<empresaFullData> listadoEmpresas = (List<empresaFullData>)js.Deserialize(request.empresaExcel, typeof(List<empresaFullData>));
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Empresas");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = "Empresas";
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "Código", "Empresa", "Descripción", "Estado" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[4, c].Value = header[i];
                    c += 1;
                }
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["B4:I4"].AutoFitColumns();


                int f = 5;


                foreach (empresaFullData obj in listadoEmpresas)
                {
                    sheet1.Cells[f, 2].Value = obj.codigo;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.empresa;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.descripcion;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.estado;
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
                response.Content.Headers.ContentDisposition.FileName = "Empresas.xlsx";

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