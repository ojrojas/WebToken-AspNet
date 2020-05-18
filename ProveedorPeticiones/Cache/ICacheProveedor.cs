using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Modelos;

namespace ProveedorPeticiones.Cache
{
    public interface ICacheProveedor
    {
        void SetearTokenCache(Token token);
        object ObtenerTokenCache();
    }
}
