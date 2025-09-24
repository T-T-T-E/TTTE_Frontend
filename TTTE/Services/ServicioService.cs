using System.Net.Http;
using System.Text;
using System.Text.Json;
using TTTE.DTOs;

namespace TTTE.Services
{
    public class ServicioService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;
        private readonly string _baseUrl = "https://ttte-devs.onrender.com/servicios/services";

        public ServicioService(HttpClient httpClient, AutheService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task AgregarTokenAsync()
        {
            var token = await _authService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<List<ServicioDto>> ObtenerServiciosAsync()
        {
            try
            {
                await AgregarTokenAsync();
                var response = await _httpClient.GetAsync(_baseUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error HTTP: {response.StatusCode}");
                    return new List<ServicioDto>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"JSON recibido: {json}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };

                var servicios = JsonSerializer.Deserialize<List<ServicioDto>>(json, options);
                return servicios ?? new List<ServicioDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener servicios: {ex.Message}");
                return new List<ServicioDto>();
            }
        }

        public async Task<bool> CrearServicioAsync(CrearServicioDto servicio)
        {
            try
            {
                await AgregarTokenAsync();

                using var formData = new MultipartFormDataContent();

                formData.Add(new StringContent(servicio.NombreServicio), "nombre_servicio");
                formData.Add(new StringContent(servicio.Precio.ToString()), "precio");

                if (!string.IsNullOrEmpty(servicio.FotoServicio))
                {
                    formData.Add(new StringContent(servicio.FotoServicio), "foto_servicio");
                }

                var response = await _httpClient.PostAsync(_baseUrl, formData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear servicio: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarServicioAsync(int id, CrearServicioDto servicio)
        {
            try
            {
                await AgregarTokenAsync();

                using var formData = new MultipartFormDataContent();

                formData.Add(new StringContent(servicio.NombreServicio), "nombre_servicio");
                formData.Add(new StringContent(servicio.Precio.ToString()), "precio");

                if (!string.IsNullOrEmpty(servicio.FotoServicio))
                {
                    formData.Add(new StringContent(servicio.FotoServicio), "foto_servicio");
                }

                var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", formData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar servicio: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarServicioAsync(int id)
        {
            try
            {
                await AgregarTokenAsync();
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar servicio: {ex.Message}");
                return false;
            }
        }
    }
}