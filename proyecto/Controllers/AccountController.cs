using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using proyecto.Models;
using proyecto.Providers;
using proyecto.Results;
using System.Net;
using proyecto.Models.web.mantenedores;
using proyecto.objects.dev.mantenedores.Usuario;


using proyecto.Models.menu;
using System.Web.Routing;
using System.Configuration;

namespace proyecto.Controllers
{
    [Authorize]
    //[RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        // TODO se propone eliminar de la funcion por desuso
       // private object resultado;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
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

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account
        public IHttpActionResult Post()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

       


        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        
        // POST api/Account/ChangePassword
        // [Route("ChangePassword")]
        [AllowAnonymous]
        public Task<IHttpActionResult> Put(ChangePasswordBindingModel model)
        {
            usuarioModel modelo = new usuarioModel();
            if (!string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                UserManager.RemovePassword(model.ID);
                UserManager.AddPassword(model.ID, model.ConfirmPassword);

                modelo.resetear(model);      
            }

            UsuarioFullData obj = new UsuarioFullData();
            obj.ID = model.ID;
            obj.nombre = model.FULLNAME;
            obj.contraseña = model.ConfirmPassword;
            obj.email = model.Email;
            obj.estado = model.ESTADO;
            obj.nombreUsuario = model.USERNAME;
            obj.perfil = model.GROUP_ID;
            obj.empresa = model.EMPRESA;
            obj.avatar = model.AVATAR;
            obj.empresa_defecto = model.EMPRESA_DEFECTO;
            obj.areas = model.areas;
            obj.nombreUsuario = model.USERNAME;

            Dictionary<string, object> resultado = new Dictionary<string, object>();
            resultado.Add("resultado", modelo.editar(obj));
            return Task.FromResult<IHttpActionResult>(Ok(resultado));
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

    

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("Error de inicio de sesión externo.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("El inicio de sesión externo ya está asociado a una cuenta.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                
                 ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }


        // GET api/Account/ForgotPassword?ID=
        [AllowAnonymous]
        [Route("ForgotPassword")]

        public async Task<IHttpActionResult> ForgotPassword(string ID)
        {
            string Password = "12345";
            string code = await UserManager.GeneratePasswordResetTokenAsync(ID);
            IdentityResult result = await UserManager.ResetPasswordAsync(ID, code, Password);

            return Ok();
        }


        // POST api/Account/Register
        [AllowAnonymous]
        //[Route("Register")]
        public async Task<IHttpActionResult> Patch(RegisterBindingModel model)
        {
            Dictionary<string, object> resultado = new Dictionary<string, object>();
            
           
            if (!ModelState.IsValid)
            {
                resultado.Add("resultado", false);
                return Ok(resultado);
            }

            var user = new ApplicationUser()
            {
                UserName = model.USERNAME,
                Email = model.Email,
                RUT = model.RUT,
                SEXO = model.SEXO,
                TELEFONO = model.TELEFONO,
                CELULAR = model.CELULAR,
                FECHA_NACIMIENTO = model.FECHA_NACIMIENTO,
                ESTADO = model.ESTADO,
                CORPORATIVO = model.CORPORATIVO,
                GROUP_ID = model.GROUP_ID,
                INTENTOS = model.INTENTOS,
                FULLNAME = model.FULLNAME,
                AVATAR = model.AVATAR,
                SEGUNDO_NOMBRE = model.SEGUNDO_NOMBRE,
                APELLIDO_MATERNO = model.APELLIDO_MATERNO,
                APELLIDO_PATERNO = model.APELLIDO_PATERNO,
                DIRECCION = model.DIRECCION,
                CARGO = model.CARGO,
                FECHA_CADUCIDAD = DateTime.Now
            };


            IdentityResult result = await UserManager.CreateAsync(user, model.ConfirmPassword);



            if (!result.Succeeded)
            {
                resultado.Add("resultado", false);
                return Ok(resultado);
            }
            
            
            
            string email = model.Email;
            string pass = model.Password;
            usuarioModel modelo = new usuarioModel();
            resultado.Add("resultado",modelo.guardarUser(user,model.EMPRESA,model.EMPRESA_DEFECTO, email, pass));
            //añador aqui el metodo de contraseña nueva
            string code = UserManager.GeneratePasswordResetToken(user.Id);
            string caso = "crear";
            resultado.Add("data", modelo.correoRecuperaPass(user.Email, code, caso,user.UserName));
            //TRACKING
            Tracking t = new Tracking();
            string queryIn = " SELECT u.username," +
                          " u.fullname," +
                          " u.email," +
                          " u.avatar," +
                          " u.rut," +
                          " u.sexo," +
                          " u.telefono," +
                          " u.celular," +
                          " u.direccion," +
                          " u.fecha_nacimiento," +
                          " wp.nombre AS empresa," +
                          " u.estado" +
                        " FROM web_users u" +
                        " LEFT JOIN web_groups g" +
                        " ON g.group_id = u.group_id" +
                        " LEFT OUTER JOIN WEB_EMPRESA_USER WPU" +
                        " ON WPU.USER_ID = U.USER_ID" +
                        " LEFT OUTER JOIN WEB_EMPRESA WP" +
                        " ON WP.ID_EMPRESA = WPU.ID_EMPRESA" +
                        " WHERE u.user_id = '" + user.Id + "' ";
            //GUARDAMOS EL TRACKING
            t.guardarTracking("Crear", "Usuario", t.obtenerDatosNuevosyAntiguos(queryIn));
            return Ok(resultado);
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
        public object Post(string userid)
        {
            try
            {
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                menuModel modelo = new menuModel();
                resultado.Add("datosUsuario", modelo.datosUsuario(userid));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        #region Aplicaciones auxiliares

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No hay disponibles errores ModelState para enviar, por lo que simplemente devuelva un BadRequest vacío.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        public void Execute(RequestContext requestContext)
        {
            throw new NotImplementedException();
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits debe ser uniformemente divisible por 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
