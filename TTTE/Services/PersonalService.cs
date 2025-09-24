using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TTTE.DTOs;

namespace TTTE.Services;

public class PersonalService
{
    private readonly HttpClient _httpClient;
    private readonly AutheService _authService;

    public PersonalService(HttpClient httpClient, AutheService authService)
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

    public async Task<List<DatosPersonal>> ObtenerPersonalAsync()
    {
        try
        {
            if (!await ConfigurarAutenticacionAsync())
            {
                Console.WriteLine("Token no disponible o inválido");
                return new List<DatosPersonal>();
            }

            var response = await _httpClient.GetAsync("/api/users");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error HTTP: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error content: {errorContent}");
                return new List<DatosPersonal>();
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"JSON recibido: {json}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var personal = JsonSerializer.Deserialize<List<DatosPersonal>>(json, options);
            return personal ?? new List<DatosPersonal>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener personal: {ex.Message}");
            return new List<DatosPersonal>();
        }
    }

    public async Task<bool> CrearPersonalAsync(CrearPersonal personal)
    {
        try
        {
            if (!await ConfigurarAutenticacionAsync())
            {
                Console.WriteLine("Token no disponible para crear personal");
                return false;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Asegurarse de que el rol sea "admin" o "barbero"
            if (personal.rol != "admin" && personal.rol != "barbero")
            {
                Console.WriteLine("Rol inválido. Debe ser 'admin' o 'barbero'");
                return false;
            }

            var json = JsonSerializer.Serialize(personal, options);
            Console.WriteLine($"JSON enviado para crear: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al crear personal: {response.StatusCode} - {errorContent}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear personal: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ActualizarPersonalAsync(int id, ActualizarPersonal personal)
    {
        try
        {
            if (!await ConfigurarAutenticacionAsync())
                return false;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

            var json = JsonSerializer.Serialize(personal, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Devuelve el error para mostrarlo en el alert
                throw new Exception($"API: {response.StatusCode} - {errorContent}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar personal: {ex.Message}");
            throw; // Lanza la excepción para que el componente la capture
        }
    }

    public async Task<bool> EliminarPersonalAsync(int id)
    {
        try
        {
            if (!await ConfigurarAutenticacionAsync())
                return false;

            var response = await _httpClient.DeleteAsync($"/api/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al eliminar personal: {response.StatusCode} - {errorContent}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar personal: {ex.Message}");
            return false;
        }
    }
}