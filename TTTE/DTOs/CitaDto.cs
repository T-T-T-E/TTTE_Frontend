using System.Text.Json.Serialization;

namespace TTTE.DTOs
{
    public class CitaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nombre_cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [JsonPropertyName("id_servicio")]
        public int IdServicio { get; set; }

        [JsonPropertyName("id_barbero")]
        public int IdBarbero { get; set; }

        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }

        [JsonPropertyName("hora")]
        public string HoraString { get; set; } = string.Empty;

        // Propiedad calculada para mostrar la hora como TimeSpan
        [JsonIgnore]
        public TimeSpan Hora
        {
            get
            {
                if (TimeSpan.TryParse(HoraString, out var timeSpan))
                    return timeSpan;
                return TimeSpan.Zero;
            }
        }

       
    }

    public class CrearCitaDto
    {
        [JsonPropertyName("nombre_cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [JsonPropertyName("id_servicio")]
        public int IdServicio { get; set; }

        [JsonPropertyName("id_barbero")]
        public int IdBarbero { get; set; }

        [JsonPropertyName("fecha")]
        public string Fecha { get; set; } = string.Empty;

        [JsonPropertyName("hora")]
        public string Hora { get; set; } = string.Empty;
    }
}