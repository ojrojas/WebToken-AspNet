using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProveedorPeticiones.Clases;
using ProveedorPeticiones.Interfaces;

namespace AplicacionEjemplo
{
    class Program
    {

        static void Main(string[] args)
        {
            Thread.Sleep(15000);
            IPeticionesServicios peticiones = new PeticionesServicios(new Uri("http://localhost:55011/api/values"));
            var tarea =  Peticion(peticiones).GetAwaiter().GetResult();
            var parametro = new Dictionary<string, string>();
            parametro.Add("id", "5");
            var tarea2 = Peticion(parametro,peticiones).GetAwaiter().GetResult();

            Console.WriteLine("Tarea 1");
            foreach (var i in tarea)
                Console.WriteLine(i);

            Console.WriteLine("Tarea 2");
            Console.WriteLine(tarea2);

            Console.ReadLine();
        }

        //Get sin parametros
        static async Task<string[]> Peticion(IPeticionesServicios peticion)
        {
            try
            {
                return   await peticion.GetAsync<string[]>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get con parametros sin volver a obtener el token por que esta en cache
        static async Task<string> Peticion(Dictionary<string,string> parametros, IPeticionesServicios peticion)
        {
            try
            {
                return await peticion.GetAsync<string>(parametros);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static async Task<string[]> ObtenerDatos(Task<string[]> objeto)
        {
            return  await objeto;
        }
    }
}
