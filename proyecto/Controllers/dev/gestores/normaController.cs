using OfficeOpenXml;
using proyecto.Models.dev.gestores;
using proyecto.objects.dev.gestores;
using System;
using System.Collections.Generic;
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
    public class normaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/norma
        public object Get()
        {
            try
            {
                int norma = Int32.Parse(HttpContext.Current.Request.QueryString.Get("norma"));
                string id_empresa = HttpContext.Current.Request.QueryString.Get("id_empresa");

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                //para el caso del perfil USUARIO LFE solo se cargaran los datos de la tabla definitiva
                if (modelo.esUsuarioLFE() == "true")
                {
                    resultado.Add("hayDatosDefinitivos", "true");
                    resultado.Add("puntoNormativo", modelo.cargaInicial(norma));
                }else
                {
                    //verifico si la norma tiene o no datos en la tabla definitiva
                    if (modelo.hayDatosDefinitivos(norma) == true)
                    {
                        //hay datos en la tabla definitiva
                        resultado.Add("hayDatosDefinitivos", "true");
                        resultado.Add("puntoNormativo", modelo.cargaInicial(norma));
                    }
                    else
                    {
                        //busco los datos desde la tabla temporal
                        resultado.Add("hayDatosDefinitivos", "false");
                        resultado.Add("puntoNormativo", modelo.cargaInicialTemporal(norma));
                    }
                }

                


                
                resultado.Add("norma", modelo.cargaNorma(norma));
                
                resultado.Add("usuarios", modelo.usuarios(id_empresa));
                resultado.Add("datosusuarios", modelo.datosusuarios());
                resultado.Add("auditoria", modelo.auditoria(norma));
                resultado.Add("tooltip", modelo.get_constante_tooltip());
                return Ok(resultado);

                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Post(norma_data request)
        {
            try
            {

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("excel", modelo.ReadExcelFile(request));
                resultado.Add("doc_asociados", modelo.doc_asociados(request));
                resultado.Add("responsables", modelo.responsables(request));               
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }
        public object Put(guardarDatos request)
        {
            try
            {

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.guardar(request));
               
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }
        public object Patch(datosDocumentoPuntoNormativo request)
        {
            try
            {

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("punoNormativo", modelo.puntoNormativo(request));
               
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }
    }

    public class normaDocumentosController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/norma
        public object Get()
        {
            try
            {
                string ID_EMPRESA = HttpContext.Current.Request.QueryString.Get("ID_EMPRESA");
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("documentos", modelo.documentos(ID_EMPRESA));
                return Ok(resultado);
                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Patch(guardarDatos request)
        {
            try
            {

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.guardarPuntoNormativo(request));

                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }
        public object Post(guardarDatos request)
        {
            try
            {

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminarPuntoNormativo(request));

                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }

        public object get(string tipo)
        {
            try
            {

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("empresas", modelo.empresas(tipo));

                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }
        public object Put(auditoria request)
        {
            try
            {

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.GuardarAuditoria(request));

                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }

    }
    public class normaEvaluacionController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/norma
        
        public object Put(evaluacion request)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.GuardarEvaluacion(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }
        public object Post(evaluacion request)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.cerrarAuditoria(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }

        public object Patch(idnormaEmpresa data)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.despublicarNorma(data.id_norma, data.id_empresa));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());

                throw new HttpResponseException(response);
            }
        }


    }


    public class normaValidarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Post(norma_data request)
        {
            try
            {

                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("errores", modelo.ValidarExcel(request));
                List<erroresExcel> listado = new List<erroresExcel>();
                listado = (List<erroresExcel>)resultado["errores"];                
                if (listado.Count() == 0)
                {
                    resultado.Add("excel", modelo.ReadExcelFile(request));
                }else
                {
                    resultado.Add("excel", new List<norma_excel>());
                }
                
                resultado.Add("doc_asociados", modelo.doc_asociados(request));
                resultado.Add("responsables", modelo.responsables(request));
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }



    }


    [Authorize]
    public class normaMantenedorController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Get()
        {
            try
            {
                string ID_EMPRESA = HttpContext.Current.Request.QueryString.Get("ID_EMPRESA");
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("normas", modelo.cargarNormas(ID_EMPRESA));
               // resultado.Add("areas", modelo.cargarAreas());
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }

        //Crear
        public object Post(datosNormaMantenedor nuevaNorma)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.crearNorma(nuevaNorma));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }


        //Actualizar
        public object Put(datosNormaMantenedor normaEditada)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.actualizarNorma(normaEditada));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }

        //Eliminar
        public object Patch(int ID)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminarNorma(ID));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }
    }

    public class normaMantenedorExportarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Post(datosNormaMantenedor request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                js.MaxJsonLength = 500000000;
                List<datosNormaMantenedor> listadoPanelControl = (List<datosNormaMantenedor>)js.Deserialize(request.EXPORTAR_EXCEL, typeof(List<datosNormaMantenedor>));

                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Listado de normas");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];

                sheet1.Cells["A1:J1"].Style.Font.Bold = true;
                sheet1.Cells["A1:J1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                //escribimos el encabezado de la tabla
                int c = 1;
                string[] header = { "Nombre", "Versión", "Descripción", "Estado", "Fecha Actualización", "Color Norma", "Color Cumplimiento", "Áreas" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[1, c].Value = header[i];
                    c += 1;
                }
                int f = 2;
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["A1:J1"].AutoFitColumns();

                foreach (datosNormaMantenedor obj in listadoPanelControl)
                {
                    sheet1.Cells[f, 1].Value = obj.NOMBRE_NORMA;
                    sheet1.Cells[f, 1].AutoFitColumns();
                    sheet1.Cells[f, 2].Value = obj.VERSION;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.DESCRIPCION_NORMA;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.ESTADO;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.FECHA_ACTUALIZACION;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 6].Value = obj.COLOR_CUMPLIMIENTO;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 7].Value = obj.COLOR_NORMA;
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 8].Value = obj.NOMBRE_EMPRESA;
                    sheet1.Cells[f, 8].AutoFitColumns();
                    f++;
                }

                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Mantenedor de Normas.xlsx";

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



    public class normaAreaMantenedorController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        public object Post(int ID)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("areas", modelo.cargarNormaAreas(ID));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }


        public object Put(habilitarDeshabilitarNorma normas)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.habilitarDeshabilitar(normas));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }

    }

    [Authorize]
    public class normaAuditoriaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
       
        public object Post(datosAuditoria request)
        {
            try
            {
                /*normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("excel", modelo.ReadExcelFile(request));
                resultado.Add("doc_asociados", modelo.doc_asociados(request));
                resultado.Add("responsables", modelo.responsables(request));
                return Ok(resultado);*/
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.editarAuditoria(request));
                resultado.Add("Respuesta", "Auditoría editada.");
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }
        public object Put(datosAuditoria request)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.anularAuditoria(request));
                resultado.Add("Respuesta", "Auditoría anulada.");
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }

        public object Patch(datosRequestNoConformidades request)
        {
            try
            {
                normaModel modelo = new normaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.obtenerNoConformidades(request));
                return Ok(resultado);

            }
            catch (Exception ex)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }

        }
    }

    public class puntos_normativos_exportarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        public HttpResponseMessage Post(normaInt request)
        {
            try
            {
                normaModel modelo = new normaModel();
            
                string nombreExportar = "";
                if (modelo.hayDatosDefinitivos(request.COD_NORMA) == true)
                    {
                        //hay datos en la tabla definitiva
                        nombreExportar = "Puntos Normativos";
                    }
                    else
                    {
                        //busco los datos desde la tabla temporal
                        nombreExportar = "Puntos Normativos Temporales";
                    }
                
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add(nombreExportar);

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.
                

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = nombreExportar;
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "N°", "Requisitos ", "cumple", "No conformidad","Responsable"};
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[4, c].Value = header[i];
                    c += 1;
                }
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["B4:I4"].AutoFitColumns();

                //escribimos los usuarios de la bd
                int f = 5;

                
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                List<norma_excel> listadoRegistro = new List<norma_excel>();
                if (modelo.hayDatosDefinitivos(request.COD_NORMA) == true)
                    {
                        //hay datos en la tabla definitiva
                        listadoRegistro = (List<norma_excel>)  modelo.cargaInicial(request.COD_NORMA);
                    }
                    else
                    {
                        //busco los datos desde la tabla temporal
                        listadoRegistro = (List<norma_excel>)  modelo.cargaInicialTemporal(request.COD_NORMA);
                    }

                foreach (norma_excel obj in listadoRegistro)
                {
                    sheet1.Cells[f, 2].Value = obj.numero;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.requisito;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.v_model;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.comentario;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 6].Value = obj.responsable;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    f++;
                }
                byte[] content = excel.GetAsByteArray();


                HttpResponseMessage response;
                response = Request.CreateResponse(HttpStatusCode.OK);
                MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = mediaType;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = nombreExportar+".xlsx";

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