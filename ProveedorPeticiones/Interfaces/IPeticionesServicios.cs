using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedorPeticiones.Interfaces
{
    public interface IPeticionesServicios
    {
        Task<T> GetAsync<T>();
        Task<T> GetAsync<T>(Dictionary<string,string> parametros);
        Task<T> GetAsync<T>(Dictionary<string, string> parametros, Dictionary<string, string> headers);
        Task<T> PostAsync<T>(T objeto,Dictionary<string,string >headers);
    }
}
