using proyecto.Models.web.mantenedores;
using proyecto.objects.dev.mantenedores.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Specialized;
using System.IO;
using System.Web.Configuration;
using System.Security.Claims;
using System.Threading;

namespace proyecto.Controllers.web.mantenedores
{
    [Authorize]
    public class UsuarioAvatarController : ApiController
    {
        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        /* public IHttpActionResult Post()
         {
             try
             {
                 foreach (var file in files)
                 {
                     string filePath = Guid.NewGuid() + System.IO.Path.GetExtension(file.FileName);
                     file.SaveAs(System.IO.Path.Combine("asdas", filePath));

                 }


                 return Ok();
             }
             catch (Exception ex)
             {
                 return InternalServerError(ex);
             }
         }*/
        //upload Avatar
        [AllowAnonymous]
        [AcceptVerbs("POST")]
        public async Task<object> Post()
        {
            try
            {
                // Check if the request contains multipart/form-data.  
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = await Request.Content.ReadAsMultipartAsync<InMemoryMultipartFormDataStreamProvider>(new InMemoryMultipartFormDataStreamProvider());
                //access form data  
                NameValueCollection formData = provider.FormData;
                //access files  
                IList<HttpContent> files = provider.Files;

                HttpContent file1 = files[0];
                //var thisFileName = file1.Headers.ContentDisposition.FileName.Trim('\"');

                ////-------------------------------------For testing----------------------------------  
                //to append any text in filename.  

                //PARA OBTENER LOS DATOS DEL USUARIO, SE DEFINEN EN IDENTITY MODEL COMO CLAIMS, SETEAR EL TIPO PARA OBTENER EL QUE QUIERAS
                //Get the current claims principal
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                // Get the claims values
                var userid = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();

                var thisFileName = file1.Headers.ContentDisposition.FileName.Trim('\"'); //ToDo: Uncomment this after UAT as per Jeeevan  

                List<string> tempFileName = thisFileName.Split('.').ToList();
                int counter = 0;
                foreach (var f in tempFileName)
                {
                    if (counter == 0)
                        thisFileName = f;

                    if (counter > 0)
                    {
                        thisFileName = thisFileName + "." + f;
                    }
                    counter++;
                }

                string archivoAux = thisFileName;
                ////-------------------------------------For testing----------------------------------  

                string filename = String.Empty;
                Stream input = await file1.ReadAsStreamAsync();
                string directoryName = String.Empty;
                string URL = String.Empty;
                string tempDocUrl = WebConfigurationManager.AppSettings["DocsUrl"];

                /*if (formData["ClientDocs"] == "ClientDocs")
                {*/
                var path = HttpRuntime.AppDomainAppPath;
                directoryName = System.IO.Path.Combine(path, "avatar");
                filename = System.IO.Path.Combine(directoryName, thisFileName);

                //Deletion exists file  
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                string DocsPath = tempDocUrl + "/" + "avatar" + "/";
                URL = DocsPath + thisFileName;

                //}


                Directory.CreateDirectory(@directoryName);
                using (Stream file = File.OpenWrite(filename))
                {
                    input.CopyTo(file);
                    //close file  
                    file.Close();
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("DocsUrl", URL);
                return Ok(archivoAux);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }






    }
}