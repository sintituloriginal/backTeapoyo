using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using proyecto.objects;
namespace proyecto.Controllers.web
{
    [Authorize]
    public class AccionesController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET: api/Acciones
        public object Get()
        {
            return 0;
        }

        // GET: api/Acciones/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Acciones
        public object Post(rutaRequest request)
        {
            try
            {
                accionesModel modelo = new accionesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("acciones", modelo.acciones(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/Acciones/5
        public object Put(validacionesAccesoRequest request)
        {
            try
            {
                accionesModel model = new accionesModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                var permiso = model.validarPermisosAcceso(request);
                resultado.Add("permiso", permiso);
                return  Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/Acciones/5
        public void Delete(int id)
        {
        }
    }
}
