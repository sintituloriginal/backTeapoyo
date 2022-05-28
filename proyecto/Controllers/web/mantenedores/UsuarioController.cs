using OfficeOpenXml;
using proyecto.Models.web.mantenedores;
using proyecto.objects.dev.mantenedores.Usuario;
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

namespace proyecto.Controllers.web.mantenedores
{

  
    [Authorize]
    public class UsuarioController : ApiController
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
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datosTabla", modelo.cargarDatos());

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/Usuario/5
        public object Get(string ID)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("datos", modelo.cargarEditar(ID));
                resultado.Add("perfil", modelo.perfiles());
                resultado.Add("empresas", modelo.empresas());
  
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Get(int cargarCrear)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("perfil", modelo.perfiles());
                resultado.Add("empresas", modelo.empresas());
                resultado.Add("nombres", modelo.nombresUsuarios());
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/Usuario
        public object Post(UsuarioFullData request)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                //resultado.Add("resultado", modelo.guardarUser(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Put(habilitarDeshabilitar request)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.habilitarDeshabilitar(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/Usuario/5
        public object Delete()
        {
            try
            {
                string id = HttpContext.Current.Request.QueryString.Get("ID");
                
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.eliminar(id));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/Usuario/5
        public object Patch(UsuarioFullData request)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("resultado", modelo.editar(request));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class cambiarPassController : ApiController
    {

        private ApplicationUserManager _userManager;

        public cambiarPassController()
        {

        }

        public cambiarPassController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        public async Task<IHttpActionResult> Put(UsuarioRecupera request)
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var userid = identity.Claims.Where(c => c.Type == "user_id")
                    .Select(c => c.Value).SingleOrDefault();
                usuarioModel modelo = new usuarioModel();

                string msj = "Contraseña incorrecta";
                
        
                IdentityResult result = await UserManager.ChangePasswordAsync(userid,request.currentPassword ,request.newPassword);
                
                if (!result.Succeeded == true)
                {
                    return BadRequest(msj);

                }
                // agregar password a historial tabla: dev_pass_history
                modelo.passHistory(userid, request.newPassword);
                return Ok(result);

            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }

          
        }

       







    }


    public class recuperaPassController : ApiController
    {
        private ApplicationUserManager _userManager;

        public recuperaPassController()
        {
            
        }

        public recuperaPassController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;       
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        public async Task<IHttpActionResult> Put(UsuarioRecupera request)
        {
            try
            {

                usuarioModel modelo = new usuarioModel();
                string user_id = (string)modelo.getByUser(request.username);
                string msj = "";
           
                IdentityResult result = await UserManager.ResetPasswordAsync(user_id, request.token, request.newPassword);
                if (result.Succeeded == true)
                {
                    return Ok();
                }
                else
                {
                    msj = ("El usuario no es válido o la recuperación ha caducado, comuníquese con el administrador");
                    return BadRequest(msj);
                }
               
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }


        public async Task<IHttpActionResult> Post(UsuarioRecupera request)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
                string ID = request.ID;
                string correo = request.email;
                string code = await UserManager.GeneratePasswordResetTokenAsync(ID);
                string caso = "recuperar";

                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("data", modelo.correoRecuperaPass(correo, code, caso,""));
                          
                return Ok();
                
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }

        }
    }
    
    public class recuperaController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        public object Post(Data data)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("email", modelo.buscarEmail(data.EMAIL));

                return Ok(resultado);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
    public class usuarioselectoresController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        public object Get(String nombre)
        {

            try
            {
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("nombres", modelo.cargarNombre(nombre));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Post(Data datos)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("perfil", modelo.cargarPerfil(datos.PERFIL));

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Put(Data datos)
        {
            try
            {
                usuarioModel modelo = new usuarioModel();
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

    public class usuarioExcelController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        // GET api/<controller>
        public HttpResponseMessage Post(UsuarioFullData request)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<Data> listadoUsuarios = (List<Data>)js.Deserialize(request.usuarioExcel, typeof(List<Data>));
                //Creamos el archivo
                ExcelPackage excel = new ExcelPackage();
                //HOJA 1
                //Le añadimos los 'worksheets' que necesitemos.
                excel.Workbook.Worksheets.Add("Usuarios");

                //Creamos un objecto tipo ExcelWorksheet para manejarlo facilmente.

                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[1];
                // Definir el valor de una celda de ambas maneras
                sheet1.Cells["B2"].Value = "Usuarios";
                sheet1.Cells["B2:I2"].Merge = true;
                sheet1.Cells["B2"].Style.Font.Bold = true;
                sheet1.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                //escribimos el encabezado de la tabla
                int c = 2;
                string[] header = { "Nombre", "Usuario","Perfil","Email","Estado" };
                for (int i = 0; i <= header.Length - 1; i++)
                {
                    sheet1.Cells[4, c].Value = header[i];
                    c += 1;
                }
                //ajustamos las columnas al ancho del contexto
                sheet1.Cells["B4:I4"].AutoFitColumns();


                int f = 5;


                foreach (Data obj in listadoUsuarios)
                {
                    sheet1.Cells[f, 2].Value = obj.FULLNAME;
                    sheet1.Cells[f, 2].AutoFitColumns();
                    sheet1.Cells[f, 3].Value = obj.USERNAME;
                    sheet1.Cells[f, 3].AutoFitColumns();
                    sheet1.Cells[f, 4].Value = obj.PERFIL;
                    sheet1.Cells[f, 4].AutoFitColumns();
                    sheet1.Cells[f, 5].Value = obj.EMAIL;
                    sheet1.Cells[f, 5].AutoFitColumns();
                    sheet1.Cells[f, 6].Value = obj.ESTADO;
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
                response.Content.Headers.ContentDisposition.FileName = "Usuarios.xlsx";

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
