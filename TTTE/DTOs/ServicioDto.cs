using System.Text.Json.Serialization;

namespace TTTE.DTOs
{
    public class ServicioDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre_servicio")]
        public string NombreServicio { get; set; } = string.Empty;

        [JsonPropertyName("precio")]
        public decimal Precio { get; set; } 

        [JsonPropertyName("foto_servicio")]
        public string? FotoServicio { get; set; }
    }

    public class CrearServicioDto
    {
        [JsonPropertyName("nombre_servicio")]
        public string NombreServicio { get; set; } = string.Empty;

        [JsonPropertyName("precio")]
        public decimal Precio { get; set; }

        [JsonPropertyName("foto_servicio")]
        public string? FotoServicio { get; set; }
    }
}