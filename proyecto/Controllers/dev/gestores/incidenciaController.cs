

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

namespace proyecto.Controllers.dev.gestores
{
    [Authorize]
    public class IncidenciaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public object Get()
        {
            try
            {
                string ID_EMPRESA = HttpContext.Current.Request.QueryString.Get("ID_EMPRESA");

                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                resultado.Add("datos", modelo.cargarDatos(ID_EMPRESA));
                resultado.Add("areas", modelo.getAreas(ID_EMPRESA));
                resultado.Add("analistas", modelo.getAnalistas(ID_EMPRESA));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object post(filtrarPor request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.filtrarDatos(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class IncidenciaExportarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
    
        public object Post(filtrarPor request)
        {
            try
            {
                
                 string nombreExportar = "Reporte de Incidencias" ;
             // Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                // HOJA 1
                // Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add(nombreExportar);
    
                // Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.
                
    
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = nombreExportar;
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
    
                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "NºRegistro","Fecha Hora Evento", "Area","Criterio Afectado ", "Tipo Evento"
                    ,"Reportado Por","Responsable Análisis" , "Estado", "Descripción", "Observación Análisis", "Observación Cierre", "Actualización Documento", "Revisión Riesgos" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[4, c].Value = header[i];
                    c += 1;
                }
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["B4:I4"].AutoFitColumns();
    
                //escribimos los usuarios de la bd
                int f = 5;
    
                
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                List<incidenciaExport> listadoRegistro = (List<incidenciaExport>) modelo.cargarDatosExport(request);
    
                foreach (incidenciaExport obj in listadoRegistro)
                {
                    sheet1.Cells[f, 2].Value = obj.ID_INCIDENCIA;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.FECHA_HORA_EVENTO;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.AREA;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.CRITERIO_AFECTADO;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 6].Value = obj.TIPO_EVENTO;
                    sheet1.Cells[f, 6].AutoFitColumns();
                    sheet1.Cells[f, 7].Value = obj.REPORTADO_POR;
                    sheet1.Cells[f, 7].AutoFitColumns();
                    sheet1.Cells[f, 8].Value = obj.RESPONS_ANALISIS;
                    sheet1.Cells[f, 8].AutoFitColumns();
                    sheet1.Cells[f, 9].Value = obj.ESTADO;
                    sheet1.Cells[f, 9].AutoFitColumns();
                    sheet1.Cells[f, 10].Value = obj.DESCRIPCION;
                    sheet1.Cells[f, 10].AutoFitColumns();
                    sheet1.Cells[f, 11].Value = obj.OBSERVACION_ANALISIS;
                    sheet1.Cells[f, 11].AutoFitColumns();
                    sheet1.Cells[f, 12].Value = obj.OBSERVACION_CIERRE;
                    sheet1.Cells[f, 12].AutoFitColumns();
                    sheet1.Cells[f, 13].Value = obj.ACTUALIZAR_DOCUMENTO=="Si"? obj.NOMBRE_DOCUMENTO: obj.ACTUALIZAR_DOCUMENTO;
                    sheet1.Cells[f, 13].AutoFitColumns();
                    sheet1.Cells[f, 14].Value = obj.REVISION_RIESGOS=="Si"? obj.DETALLE_RIESGOS: obj.REVISION_RIESGOS;
                    sheet1.Cells[f, 14].AutoFitColumns();
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

    public class CrearIncidenciaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public object Get()
        {
            try
            {
                string ID_EMPRESA = HttpContext.Current.Request.QueryString.Get("ID_EMPRESA");
                string ID_INCIDENCIA = HttpContext.Current.Request.QueryString.Get("ID_INCIDENCIA");

                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                resultado.Add("datos", modelo.cargarDatos(ID_EMPRESA));
                resultado.Add("areas", modelo.getAreas(ID_EMPRESA));
                resultado.Add("analistas", modelo.getAnalistas(ID_EMPRESA));
                resultado.Add("documentos_reg", modelo.getDocumentos_reg(ID_INCIDENCIA));
                resultado.Add("documentos_analisis", modelo.getDocumentos_analisis(ID_INCIDENCIA));
                resultado.Add("documentos_cierre", modelo.getDocumentos_cierre(ID_INCIDENCIA));
                resultado.Add("data_incidencia", modelo.getDataIncidencia(ID_INCIDENCIA));
                resultado.Add("data_analisis", modelo.getDataAnalisis(ID_INCIDENCIA));
                resultado.Add("data_cierre", modelo.getDataCierre(ID_INCIDENCIA));


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Post(crearIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.crearIncidencia(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Put(crearIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.actualizarIncidencia(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Patch(crearIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.publicarIncidencia(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
    public class AnalizarIncidenciaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public object Get()
        {
            //disponible
            try
            {
                string ID_EMPRESA = HttpContext.Current.Request.QueryString.Get("ID_EMPRESA");
                string ID_INCIDENCIA = HttpContext.Current.Request.QueryString.Get("ID_INCIDENCIA");

                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();

                resultado.Add("datos", modelo.cargarDatos(ID_EMPRESA));
                resultado.Add("areas", modelo.getAreas(ID_EMPRESA));
                resultado.Add("analistas", modelo.getAnalistas(ID_EMPRESA));
                resultado.Add("documentos_reg", modelo.getDocumentos_reg(ID_INCIDENCIA));
                resultado.Add("documentos_analisis", modelo.getDocumentos_analisis(ID_INCIDENCIA));
                resultado.Add("documentos_cierre", modelo.getDocumentos_cierre(ID_INCIDENCIA));
                resultado.Add("data_incidencia", modelo.getDataIncidencia(ID_INCIDENCIA));


                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Post(crearIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.analizarIncidencia(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Put(crearIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.actualizarAnalisis(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Patch(crearIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.publicarAnalisis(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class CerrarIncidenciaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
       
        public object Post(cerrarIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cerrarIncidencia(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Put(cerrarIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.actualizarCierre(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Patch(cerrarIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.publicarCierre(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
    public class RechazarAnalisisIncidenciaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
       
        
        public object Put(cerrarIncidenciaReq request)
        {
            try
            {
                incidenciaModel modelo = new incidenciaModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.rechazarAnalisis(request));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }


}
