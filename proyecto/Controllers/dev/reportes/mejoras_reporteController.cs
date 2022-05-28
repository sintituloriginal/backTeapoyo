using proyecto.Models.dev.reportes;
using proyecto.objects.dev.reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace proyecto.Controllers.dev.reportes
{
    public class mejoras_reporteController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/Usuario
        public object Get()
        {
            try
            {
                mejoras_reportesModel modelo = new mejoras_reportesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();               
                resultado.Add("datosGraficos", modelo.cargarGraficos());
                resultado.Add("datosTabla", modelo.cargarDatos());

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Put(data_mejoras_reporte datos)
        {
            try
            {
                mejoras_reportesModel modelo = new mejoras_reportesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosGraficos", modelo.cargarGraficos());
                resultado.Add("datosTabla", modelo.filtrar(datos));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}