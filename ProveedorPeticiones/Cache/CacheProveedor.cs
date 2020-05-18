using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Entidades.Modelos;

namespace ProveedorPeticiones.Cache
{
    /// <summary>
    /// Cache de token de aplicacion
    /// </summary>
    public class CacheProveedor : ICacheProveedor
    {
        /// <summary>
        /// Obejto cache referencia a la cache de aplicacion.
        /// </summary>
        ObjectCache cache = MemoryCache.Default;

        /// <summary>
        /// Politicas de aplicacion sobre el item cache.
        /// </summary>
        CacheItemPolicy policity = new CacheItemPolicy();

        /// <summary>
        /// Setear el objeto token de cache
        /// </summary>
        /// <param name="token"></param>
        public void SetearTokenCache(Token token)
        {
            DateTimeOffset tiempoExpiraciontoken = new DateTimeOffset(DateTime.Now.AddHours(1));
            policity.AbsoluteExpiration = tiempoExpiraciontoken;
            cache.Set("cacheToken", token, policity);
        }

        /// <summary>
        /// Obtener el token de la memoria cache.
        /// </summary>
        /// <returns>Retorna objeto cache.</returns>
        public object ObtenerTokenCache()
        {
            return cache.Get("cacheToken");
        }
    }
}
