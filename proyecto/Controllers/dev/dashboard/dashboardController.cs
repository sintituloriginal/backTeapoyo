using proyecto.Models.dev.gestores;
using proyecto.objects.dev.gestores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace proyecto.Controllers.dev.dashboard
{
    public class dashboardController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/norma
        public object Get(string id_empresa)
        {
            try
            {
                dashboardModel modelo = new dashboardModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarDatos(id_empresa));
                resultado.Add("auditorias", modelo.auditorias(id_empresa));
                resultado.Add("noConformidad", modelo.noConformidad(id_empresa));
                resultado.Add("documentos", modelo.documentos(id_empresa));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
       
    }
    
}