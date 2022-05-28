using System;
using System.Collections.Generic;
using System.Web.Http;
using proyecto.Models.menu;
using System.Net.Http;
using System.Net;
using proyecto.objects;
using System.Web.Script.Serialization;

namespace proyecto.Controllers
{
    //[RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        public object Post(users userid)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                string[] usuarios = js.Deserialize<string[]>(userid.userid);
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                menuModel modelo = new menuModel();
                resultado.Add("datosUsuario", modelo.datosUsuarios(usuarios));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }


}
