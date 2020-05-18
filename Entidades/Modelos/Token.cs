using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Modelos
{
    public class Token
    {
        /// <summary>
        /// Token de authorizacion
        /// </summary>
        public string auth_token { get; set; }
        /// <summary>
        /// Tiempo expiracion token
        /// </summary>
        public int expire_token { get; set; }

        /// <summary>
        /// Usuario login del token
        /// </summary>
        public string user_token { get; set; }

        /// <summary>
        /// Rol del usuario token.
        /// </summary>
        public string role { get; set; }
    }
}
