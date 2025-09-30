using System.Text.Json;
using System.Text;
using TTTE.DTOs;

namespace TTTE.Services
{
    public class CitaService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;
        private readonly string _baseUrl = "https://ttte-devs.onrender.com/citas";

        public CitaService(HttpClient httpClient, AutheService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task<bool> ConfigurarAutenticacionAsync()
        {
            var token = await _authService.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        public async Task<List<CitaDto>> ObtenerCitasAsync()
        {
            try
            {
                // Configurar token antes de la solicitud
                if (!await ConfigurarAutenticacionAsync())
                {
                    Console.WriteLine("Token no disponible o inválido");
                    return new List<CitaDto>();
                }

                var response = await _httpClient.GetAsync($"{_baseUrl}/citas");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error HTTP: {response.StatusCode}");
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("Token no autorizado o expirado");
                    }
                    return new List<CitaDto>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"JSON recibido: {json}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                };

                var citas = JsonSerializer.Deserialize<List<CitaDto>>(json, options);
                return citas ?? new List<CitaDto>();
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Error de deserialización JSON: {jsonEx.Message}");
                return new List<CitaDto>();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Error de solicitud HTTP: {httpEx.Message}");
                return new List<CitaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return new List<CitaDto>();
            }
        }

        public async Task<bool> CrearCitaAsync(CrearCitaDto cita)
        {
            try
            {
                if (!await ConfigurarAutenticacionAsync())
                {
                    Console.WriteLine("Token no disponible para crear cita");
                    return false;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                };

                var json = JsonSerializer.Serialize(cita, options);
                Console.WriteLine($"JSON enviado para crear: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/cita", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al crear cita: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear cita: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarCitaAsync(int id, CrearCitaDto cita)
        {
            try
            {
                if (!await ConfigurarAutenticacionAsync())
                {
                    Console.WriteLine("Token no disponible para actualizar cita");
                    return false;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                };

                var json = JsonSerializer.Serialize(cita, options);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al actualizar cita: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar cita: {ex.Message}");
                return false;
            }
        }

        public async Task<List<CitaDto>> ObtenerCitasPorBarberoAsync(int idBarbero)
        {
            if (!await ConfigurarAutenticacionAsync())
                return new List<CitaDto>();

            var response = await _httpClient.GetAsync($"{_baseUrl}/barbero/{idBarbero}");
            if (!response.IsSuccessStatusCode)
                return new List<CitaDto>();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<CitaDto>>(json, options) ?? new List<CitaDto>();
        }

        public async Task<List<DatosPersonal>> ObtenerBarberosAsync()
        {
            try
            {
                if (!await ConfigurarAutenticacionAsync())
                    return new List<DatosPersonal>();

                var response = await _httpClient.GetAsync("https://ttte-devs.onrender.com/api/users");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al obtener usuarios: {response.StatusCode}");
                    return new List<DatosPersonal>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"JSON de usuarios recibido: {json}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var todosLosUsuarios = JsonSerializer.Deserialize<List<DatosPersonal>>(json, options) ?? new List<DatosPersonal>();

                // dejo esto pendiente por si acaso es necesario
                var barberos = todosLosUsuarios.Where(u => u.rol == 2).ToList();
                Console.WriteLine($"Barberos encontrados: {barberos.Count}");

                return barberos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener barberos: {ex.Message}");
                return new List<DatosPersonal>();
            }
        }
        public async Task<bool> EliminarCitaAsync(int id)
        {
            try
            {
                if (!await ConfigurarAutenticacionAsync())
                {
                    Console.WriteLine("Token no disponible para eliminar cita");
                    return false;
                }

                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al eliminar cita: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar cita: {ex.Message}");
                return false;
            }
        }

        public async Task<List<CitaDto>> ObtenerCitasPorClienteAsync(int idCliente)
        {
            try
            {
                if (!await ConfigurarAutenticacionAsync())
                    return new List<CitaDto>();

                var response = await _httpClient.GetAsync($"{_baseUrl}/cliente/{idCliente}");
                if (!response.IsSuccessStatusCode)
                    return new List<CitaDto>();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<CitaDto>>(json, options) ?? new List<CitaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener citas del cliente: {ex.Message}");
                return new List<CitaDto>();
            }
        }
    }
}