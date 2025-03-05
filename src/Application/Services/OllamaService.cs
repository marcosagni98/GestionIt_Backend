using Application.Dtos.Ollama;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Json;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaService> _logger;

    public OllamaService(HttpClient httpClient, ILogger<OllamaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri("http://host.docker.internal:11434/");
        _httpClient.Timeout = TimeSpan.FromMinutes(2);
    }

    public async Task<ImproveDescriptionResponseDto> GenerateImprovedDescriptionAsync(string title, string? currentDescription)
    {
        // Prompt específico para generar descripciones de incidencias
        string prompt = $@"Estás ayudando a un sistema de gestión de incidencias de TI a crear una descripción más detallada y precisa de los problemas reportados por los usuarios.
        Título de la incidencia: '{title}'
        Descripción actual: '{currentDescription ?? "No hay descripción inicial"}'
       Genera una descripción mejorada que sea:
        - Clara, técnica y con un mínimo de 50 palabras.
        - Enfocada en proporcionar detalles útiles al equipo de soporte técnico.
        - Escrita desde la perspectiva del usuario, utilizando un lenguaje natural y describiendo el problema, de una manera profesional y tecnica.
        Devuelve SOLO la descripción mejorada, sin ningún texto adicional.";

        var request = new
        {
            model = "llama3.2:1b",
            prompt = prompt,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<OllamaResponse>(responseContent);

        // Limpiar la descripción
        string cleanedDescription = CleanDescription(result?.Response ?? string.Empty);

        return new ImproveDescriptionResponseDto
        {
            ImproveDescription = cleanedDescription
        };
    }

    // Método privado para limpiar la descripción
    private string CleanDescription(string rawDescription)
    {
        // Eliminar prefijos comunes
        string[] prefixesToRemove =
        [
            "Descripción:",
            "Descripción mejorada:",
            "Respuesta:",
            "Generando descripción:",
            "Informe técnico:"
        ];

        string cleanedText = rawDescription;

        // Eliminar prefijos
        foreach (var prefix in prefixesToRemove)
        {
            if (cleanedText.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                cleanedText = cleanedText.Substring(prefix.Length).Trim();
            }
        }

        // Eliminar comillas al inicio y al final si existen
        cleanedText = cleanedText.Trim('"', '\'');

        // Asegurar que tenga al menos 20 palabras
        if (cleanedText.Split(' ').Length < 20)
        {
            cleanedText += " [Descripción generada requiere revisión adicional]";
        }

        return cleanedText.Trim();
    }

    private class OllamaResponse
    {
        public string Response { get; set; }
    }
}