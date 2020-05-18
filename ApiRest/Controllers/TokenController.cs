using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ApiRest.GeneradorTokens;
using ApiRest.Models;
using Entidades.Modelos;

namespace ApiRest.Controllers
{
    [Authorize]
    public class TokenController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("Token")]
        [ActionName("Token/usuario/{usuario:string}/contrasena/{contrasena:string}")]
        public IHttpActionResult Authenticate(string usuario, string contrasena)
        {
            if (usuario == null || contrasena == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            //Aqui se puede validar cualquier cosa desde usuario de sesion aplicacion otra cosa.
            bool isCredentialValid = (contrasena == "123456");
            if (isCredentialValid)
            {
                var stringtoken = GeneradorToken.GeneradorTokenJWT(usuario);
                var token = new Token();
                token.auth_token = stringtoken;
                token.expire_token = new DateTimeOffset().AddHours(1).Hour;
                token.user_token = usuario;
                token.role = "Cualquiera";
                return Ok(token);
            }
            else
            {
                var respueta = new HttpError(
                    string.Format($"Petición no auturizada codigo {(int)HttpStatusCode.Unauthorized}", 
                    CultureInfo.CurrentCulture));
                return Content(HttpStatusCode.Unauthorized, respueta);
            }
        }
    }
}
