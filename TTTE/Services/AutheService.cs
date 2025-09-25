using TTTE.DTOs;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace TTTE.Services
{
    public class AutheService
    {
        private readonly ProtectedSessionStorage _localStore;
        private readonly HttpClient _httpClient;
        private string? _token;

        public AutheService(ProtectedSessionStorage localStore, HttpClient httpClient)
        {
            _localStore = localStore;
            _httpClient = httpClient;
        }

        public async Task<string> Login(UserSession userSesion)
        {
            var response = await _httpClient.PostAsJsonAsync("api/login", userSesion);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<string>();
                return result;
            }
            return null;

        }

        public async Task SetToken(string token)
        {
            _token = token;
            await _localStore.SetAsync("token", token);
        }

        public async Task<string?> GetToken()
        {
            try
            {
                var localStoreResult = await _localStore.GetAsync<string>("token");
                return localStoreResult.Success ? localStoreResult.Value : null;
            }
            catch (InvalidOperationException)
            {
                // Manejar el caso de prerendering
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await GetToken();

            return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);

        }

        public bool IsTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }

        public async Task Logout()
        {
            _token = null;
            await _localStore.DeleteAsync("token");

        }

        public async Task<string> GetUserRoleAsync()
        {
            try
            {
                var token = await GetToken();
                if (string.IsNullOrEmpty(token))
                    return "";

                // Decodifica el token (asumiendo que es JWT)
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                    return "";

                // Busca el claim del rol
                var rolClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "rol");
                if (rolClaim != null)
                {
                    // Devuelve "admin" o "cliente" según el valor del rol
                    return rolClaim.Value;
                }

                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}
