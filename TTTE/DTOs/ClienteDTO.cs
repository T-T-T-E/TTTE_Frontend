using System.Text.Json.Serialization;

namespace TTTE.DTOs
{
    public class ClienteCreateDTO
    {
        [JsonPropertyName("nombre_completo")]
        public string? nombreCompleto { get; set; }

        [JsonPropertyName("email")]
        public string? email { get; set; }

        [JsonPropertyName("telefono")]
        public string? telefono { get; set; }

        [JsonPropertyName("password")]
        public string? password { get; set; }
    }
}
