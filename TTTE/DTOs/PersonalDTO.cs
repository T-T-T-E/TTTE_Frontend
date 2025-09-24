using System.Text.Json.Serialization;

namespace TTTE.DTOs
{
    public class DatosPersonal
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre_completo")]
        public string? nombreCompleto { get; set; }

        [JsonPropertyName("email")]
        public string? email { get; set; }

        [JsonPropertyName("telefono")]
        public string? telefono { get; set; }

        [JsonPropertyName("password")]
        public string? password { get; set; }

        [JsonPropertyName("rol_id")]
        public int? rol { get; set; }

    }

    public class CrearPersonal
    {
        [JsonPropertyName("nombre_completo")]
        public string? nombreCompleto { get; set; }

        [JsonPropertyName("telefono")]
        public string? telefono { get; set; }

        [JsonPropertyName("email")]
        public string? email { get; set; }

        [JsonPropertyName("password")]
        public string? password { get; set; }

        [JsonPropertyName("rol")]
        public string? rol { get; set; }
    }

    public class ActualizarPersonal
    {
        [JsonPropertyName("nombre_completo")]
        public string? nombreCompleto { get; set; }

        [JsonPropertyName("telefono")]
        public string? telefono { get; set; }

        [JsonPropertyName("email")]
        public string? email { get; set; }

        [JsonPropertyName("password")]
        public string password { get; set; } = "";
    }

}
