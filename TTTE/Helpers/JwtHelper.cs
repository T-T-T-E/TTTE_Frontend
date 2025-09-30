using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TTTE.Helpers
{
    public static class JwtHelper
    {
        //esta es una clase estatica que extra los roles del token con que se inicia sesion 
        //este metodo getuser recibe el token como string y devuelve un entero que seria el rol
        //usado para validar si quien inicio sesion es cliente,barbero o admin para asi crear mis propias restricciones xd
        public static int? GetUserRoleFromToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == "role" || c.Type == "rol_id" || c.Type == "rol" ||
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                if (roleClaim != null && int.TryParse(roleClaim.Value, out int role))
                {
                    return role;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // Método para obtener el ID del usuario desde el token
        public static int GetUserIdFromToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return 0;

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Buscar en diferentes claims que pueden contener el ID del usuario
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == "sub" || c.Type == "nameid" || c.Type == "userId" ||
                    c.Type == "id" || c.Type == "user_id");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}