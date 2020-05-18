using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedorPeticiones.Exception
{
    public class ServiceAuthenticationException : System.Exception
    {
        private string Contenido;
        public ServiceAuthenticationException(string contenido)
        {
            Contenido = contenido;
        }

        public ServiceAuthenticationException() { }
    }
}
