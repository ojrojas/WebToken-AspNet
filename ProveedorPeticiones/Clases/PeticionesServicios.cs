using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Entidades.Modelos;
using Newtonsoft.Json;
using Entidades.Constantes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ProveedorPeticiones.Exception;
using ProveedorPeticiones.Interfaces;
using ConstantesAplicacion = Entidades.Constantes.ConstantesAplicacion;
using System.Collections.Generic;
using ProveedorPeticiones.Cache;

namespace ProveedorPeticiones.Clases
{
    public class PeticionesServicios : IPeticionesServicios
    {
        #region Constantes

        /// <summary>
        /// Configuracion serializacion objetos
        /// </summary>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private readonly JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Uri base de peticiones dirección web de peticiones
        /// </summary>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private readonly Uri _uri;

        /// <summary>
        /// Uri de Token dirección web de token
        /// </summary>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private readonly Uri _uriToken = new Uri(ConstantesAplicacion.UriToken);

        private readonly ICacheProveedor cache = new CacheProveedor();

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor Peticiones servicios
        /// </summary>
        /// <param name="uri">Uri base de peticiones</param>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        public PeticionesServicios(Uri uri)
        {
            _uri = uri;
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializerSettings.Converters.Add(new StringEnumConverter());
        }

        #endregion

        #region Metodos publics

        /// <summary>
        /// Metodo de peticiones webapi
        /// </summary>
        /// <typeparam name="T">Tipo de parametro generico.</typeparam>
        /// <returns>Tipo de objeto generico</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        public async Task<T> GetAsync<T>()
        {
            try
            {
                var token = await ObtenerToken();
                HttpClient httpClient = CrearHttpCliente(token);
                HttpResponseMessage response = await httpClient.GetAsync(_uri.ToString());
                await HandleResponse(response);
                string serialized = await response.Content.ReadAsStringAsync();

                T result = await Task.Run(() =>
                    JsonConvert.DeserializeObject<T>(serialized, _serializerSettings));

                return result;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Metodo de peticiones webapi GET
        /// </summary>
        /// <typeparam name="T">Tipo de parametro generico.</typeparam>
        /// <param name="parametros">Parametro de petición</param>
        /// <returns>Tipo de objeto generico</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        public async Task<T> GetAsync<T>(Dictionary<string, string> parametros)
        {
            var token = await ObtenerToken();
            HttpClient httpClient = CrearHttpCliente(token);
            var uriConParametros = FomarUriConParametros(parametros);
            HttpResponseMessage response = await httpClient.GetAsync(uriConParametros);
            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            T result = await Task.Run(() =>
                JsonConvert.DeserializeObject<T>(serialized, _serializerSettings));

            return result;
        }

        /// <summary>
        /// Metodo de peticion webapi GET
        /// </summary>
        /// <typeparam name="T">Tipo de parametro generico</typeparam>
        /// <param name="parametros">Parametros peticion GET</param>
        /// <param name="header">Establecer header de petición si es necesario</param>
        /// <returns>Tipo de generico.</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        public async Task<T> GetAsync<T>(Dictionary<string, string> parametros,Dictionary<string,string> headers)
        {
            var token = await ObtenerToken();
            HttpClient httpClient = CrearHttpCliente(token);

            if (headers != null)
            {
                AddHeaderParameter(httpClient, headers);
            }

            var uriConParametros = FomarUriConParametros(parametros);
            HttpResponseMessage response = await httpClient.GetAsync(_uri.ToString());
            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            T result = await Task.Run(() =>
                JsonConvert.DeserializeObject<T>(serialized, _serializerSettings));

            return result;
        }

        /// <summary>
        /// Implemmentacion de peticiones Post
        /// </summary>
        /// <typeparam name="T">Objeto enviado</typeparam>
        /// <param name="Objeto">Objeto enviado como content de la peticion</param>
        /// <returns>Tipo generico</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        public async Task<T> PostAsync<T>(T Objeto,Dictionary<string,string> headers)
        {
            var token = await ObtenerToken();
            HttpClient httpClient = CrearHttpCliente(token);

            if (headers != null)
            {
                AddHeaderParameter(httpClient, headers);
            }

            var content = new StringContent(JsonConvert.SerializeObject(Objeto));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(_uri, content);

            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            T result = await Task.Run(() =>
                JsonConvert.DeserializeObject<T>(serialized, _serializerSettings));

            return result;
        }

        #endregion

        #region Metodos Privados

        /// <summary>
        /// Obtiene el token de cache.
        /// </summary>
        /// <returns>Token de aplicacion</returns>
        private async Task<Token> ObtenerToken()
        {
            Token token = null;
            if (cache.ObtenerTokenCache() != null)
                token = cache.ObtenerTokenCache() as Token;
            else
                 token = await ConsultarToken();
            return token;
        }

        /// <summary>
        /// Obtiene el token del webapi.
        /// </summary>
        /// <returns>Token</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private async Task<Token> ConsultarToken()
        {
            try
            {
                HttpClient client = CrearHttpClientToken();
                HttpResponseMessage response = await client.GetAsync(_uriToken);
                await HandleResponse(response);
                var resultado = await response.Content.ReadAsStringAsync();
                Token token = JsonConvert.DeserializeObject<Token>(resultado);
                if (token != null)
                    cache.SetearTokenCache(token);
                return token;
            }
            catch (HttpRequestExceptionEx ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Añade los headers a la peticion
        /// </summary>
        /// <param name="httpClient">Cliente de peticiones http</param>
        /// <param name="parameter">parametros headers</param>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private void AddHeaderParameter(HttpClient httpClient, Dictionary<string, string> parameters)
        {
            if (httpClient == null)
                return;

            if (parameters == null)
                return;

            foreach (var i in parameters)
                httpClient.DefaultRequestHeaders.Add(i.Key, i.Value);
        }

        /// <summary>
        /// Manejador de respuestas http.
        /// </summary>
        /// <param name="response">Respuesta del web api</param>
        /// <returns>Void</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException(content);
                }

                throw new HttpRequestExceptionEx(response.StatusCode, content);
            }
        }

        /// <summary>
        /// Crear el cliente de peticiones http para todas las consultas.
        /// </summary>
        /// <param name="token">Token de autorizacion webapi</param>
        /// <returns>Cliente de peticiones http.</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private HttpClient CrearHttpCliente(Token token = null)
        {
            HttpClient client = null;
            if (!string.IsNullOrEmpty(token?.auth_token))
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.auth_token);
            }

            return client;
        }

        /// <summary>
        /// Crea el cliente http de peticion token
        /// </summary>
        /// <returns>HttpClient de peticiones</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private HttpClient CrearHttpClientToken()
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", ConstantesAplicacion.ClaveSecreta);
            return client;
        }

        /// <summary>
        /// Formaliza un direccion uri de peticion get
        /// </summary>
        /// <param name="parametros">Parametros de peticion</param>
        /// <returns>Ruta de webapi con parametros.</returns>
        /// <author>Oscar Julian Rojas Garces</author>
        /// <date>03/06/2020.</date>
        private string FomarUriConParametros(Dictionary<string, string> parametros)
        {
            var cadena = string.Empty;
            if (parametros.Count > default(int))
                cadena = "?";
            foreach (var i in parametros)
                cadena += string.Join("&", $"{i.Key}={i.Value}");
            return _uri.ToString() + cadena;
        }

        #endregion
    }
}
