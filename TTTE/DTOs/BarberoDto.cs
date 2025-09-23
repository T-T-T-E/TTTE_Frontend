using System.Text.Json.Serialization;

namespace TTTE.DTOs
{
    public class BarberoDto
    {
        public int Id {get; set; }

        [JsonPropertyName("nombre_completo")]
        public string Nombre { get; set; } = string.Empty;
    }
}
