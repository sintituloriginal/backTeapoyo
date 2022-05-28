using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using proyecto.Models;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using proyecto.Models.menu;
using System.Web.Script.Serialization;
using System.Configuration;

namespace proyecto.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
      
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }
        //AQUI INICIA SESIÓN
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            constantesModel modelo = new constantesModel();
            Dictionary<string, object> resultado = new Dictionary<string, object>();
            resultado.Add("constantes", modelo.cargarDatos());


            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            var keybytes = Encoding.UTF8.GetBytes("7061737323313233");
            var iv = Encoding.UTF8.GetBytes("7061737323313233");

            //byte[] encrypted = Convert.FromBase64String(context.Password.ToString());

            

            var encrypted = Convert.FromBase64String(context.Password.ToString());
            var roundtrip = DecryptStringFromBytes(encrypted, keybytes, iv);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            var password = decriptedFromJavascript;

            ApplicationUser user = await userManager.FindAsync(context.UserName, password);

            if (user == null)
            {
                context.SetError("invalid_grant", "El nombre de usuario o la contraseña no son correctos.");
                return;
            }
            if (user.ESTADO == "Deshabilitado")
            {
                context.SetError("invalid_grant", "El usuario no esta disponible.");
                return;
            }
          

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            //AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationProperties properties = crearPropiedadesPersonalizadas(user.Id);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // La credenciales de la contraseña del propietario del recurso no proporcionan un id. de cliente.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
        public static AuthenticationProperties crearPropiedadesPersonalizadas(string user_id)
        {
            menuModel m = new menuModel();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string datosU = js.Serialize(m.datosUsuario(user_id));
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "datosU", datosU}
            };
            return new AuthenticationProperties(data);
        }
    }
}