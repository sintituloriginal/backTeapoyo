using proyecto.Models.web;
using proyecto.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace proyecto.Controllers.web
{
    public class filtrosController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/Usuario
        public object Post(rutaRequest request)
        {
            try
            {
                filtrosModel modelo = new filtrosModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosFiltros", modelo.cargarFiltros(request));                

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}