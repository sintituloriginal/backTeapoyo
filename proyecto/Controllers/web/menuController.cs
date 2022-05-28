using proyecto.Models.empresas;
using proyecto.Models.menu;
using proyecto.objects.web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace proyecto.Controllers.web
{

    [Authorize]
    public class MenuController : ApiController
    {

        [AllowAnonymous]
        [AcceptVerbs("OPTIONS")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }
        // GET api/<controller>
        public object Post(Users request)
        {
            try
            {
                empresasModel modelo = new empresasModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                resultado.Add("menuEmpresas", modelo.listaEmpresasUsuario(request.user_id));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public object Get()
        {
            try

            {
                menuModel modelo = new menuModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                Users usuario = new Users();

                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                // Get the claims values
                string group = identity.Claims.Where(c => c.Type == "group_id")
                                   .Select(c => c.Value).SingleOrDefault();

                usuario.group_id = Convert.ToInt32(group);
                List<Branch> branchPadres = modelo.branchPadre(usuario.group_id);
                List<Branch> branchPadresHijos = modelo.branchPadresHijos(usuario.group_id);
                List<Sheet> Sheet_hijos = modelo.sheet(usuario.group_id);
                List<object> menu = new List<object>();
                var json = new JavaScriptSerializer();
                line_divider divider = new line_divider();





                foreach (Branch branchPadres_ in branchPadres)
                {
                    bool isHeader = false;
                    bool firstHeader = true;
                    foreach (Branch branchPadresHijos_ in branchPadresHijos)
                    {
                        if (branchPadres_.title == branchPadresHijos_.source)
                        {
                            if (firstHeader)
                            {
                                divider = new line_divider();
                                divider.divider = true;
                                menu.Add(divider);

                                header_group header = new header_group();
                                header.header = branchPadres_.title;
                                menu.Add(header);
                            }
                            itemGroup itemGroup = new itemGroup();
                            itemGroup.title = branchPadresHijos_.title;
                            itemGroup.icon = branchPadresHijos_.icon;
                            itemGroup.group = branchPadresHijos_.title;
                            //AHORA TENGO QUE BUSCAR TODOS LOS HIJOS
                            List<item> Hijos = new List<item>();


                            foreach (Sheet Sheet_ in Sheet_hijos)
                            {
                                if (branchPadresHijos_.id_branch == Sheet_.id_branch)
                                {
                                    item obj = new item();
                                    obj.icon = Sheet_.icon;
                                    obj.href = Sheet_.href;
                                    obj.title = Sheet_.title;
                                    Hijos.Add(obj);
                                }

                            }
                            itemGroup.child = Hijos.ToArray();
                            menu.Add(itemGroup);
                            isHeader = true;
                            firstHeader = false;


                        }
                    }

                    //itemGroup itemGroupHijos = new itemGroup();
                    //foreach (Sheet Sheet_ in Sheet_hijos)
                    //{
                    //    if (branchPadres_.id_branch == Sheet_.id_branch)
                    //    {

                    //        //item obj = new item();
                    //        //obj.icon = Sheet_.icon;
                    //        //obj.href = Sheet_.href;
                    //        //obj.title = Sheet_.title;
                    //        //SoloHijos.Add(obj);
                    //        itemGroupHijos.name = Sheet_.title;
                    //        itemGroupHijos.icon = Sheet_.icon;
                    //        itemGroupHijos.group = Sheet_.title;
                    //        menu.Add(itemGroupHijos);
                    //    }
                    //}

                    //if (!firstHeader)
                    //{
                    //    divider = new line_divider();
                    //    divider.divider = true;
                    //    menu.Add(divider);
                    //}
                    if (!isHeader)
                    {

                        //si no es header por tabla branch entonces tengo que buscar si tiene hijos en la tabla sheet

                        itemGroup itemGroup = new itemGroup();
                        itemGroup.title = branchPadres_.title;
                        itemGroup.icon = branchPadres_.icon;
                        itemGroup.group = branchPadres_.title;
                        //AHORA TENGO QUE BUSCAR TODOS LOS HIJOS
                        List<item> Hijos = new List<item>();
                        bool have_sheet = false;
                        foreach (Sheet Sheet_ in Sheet_hijos)
                        {
                            if (branchPadres_.id_branch == Sheet_.id_branch)
                            {
                                item obj = new item();
                                obj.icon = Sheet_.icon;
                                obj.href = Sheet_.href;
                                obj.title = Sheet_.title;
                                Hijos.Add(obj);
                                have_sheet = true;
                            }
                        }
                        if (have_sheet)
                        {
                            itemGroup.child = Hijos.ToArray();
                            menu.Add(itemGroup);
                        }
                        else
                        {

                            item item = new item();
                            item.icon = branchPadres_.icon;
                            item.href = branchPadres_.href;
                            item.title = branchPadres_.title;
                            menu.Add(item);
                        }
                    }
                    else
                    {

                        foreach (Sheet Sheet_ in Sheet_hijos)
                        {
                            if (branchPadres_.id_branch == Sheet_.id_branch)
                            {
                                item itemGroupHijos = new item();
                                //item obj = new item();
                                //obj.icon = Sheet_.icon;
                                //obj.href = Sheet_.href;
                                //obj.title = Sheet_.title;
                                //SoloHijos.Add(obj);
                                itemGroupHijos.title = Sheet_.title;
                                itemGroupHijos.icon = Sheet_.icon;
                                itemGroupHijos.href = Sheet_.href;
                                menu.Add(itemGroupHijos);
                            }
                        }
                    }


                }
                //PARA OBTENER LOS DATOS DEL USUARIO, SE DEFINEN EN IDENTITY MODEL COMO CLAIMS, SETEAR EL TIPO PARA OBTENER EL QUE QUIERAS
                //Get the current claims principal

                // Get the claims values
                var userid = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();

                resultado.Add("menu", menu);
                resultado.Add("datosUsuario", modelo.datosUsuario(userid));
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public object Put(notificaciones datos)

        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                // Get the claims values
                string user_id = identity.Claims.Where(c => c.Type == "user_id")
                                   .Select(c => c.Value).SingleOrDefault();

                menuModel modelo = new menuModel();
                Dictionary<string, object> resultado = new Dictionary<string, object>();
                if (modelo.notificacion_visto(datos.id_notificacion, user_id, datos.id_empresa))
                {
                    resultado.Add("notificaciones", modelo.get_notificaciones(user_id));
                }

                return Ok(resultado);


            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
