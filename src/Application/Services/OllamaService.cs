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

    public async Task<string> GenerateResponseAsync(string prompt)
    {
        try
        {
            var request = new
            {
                model = "llama3.2:1b",  // Usa el nombre exacto del modelo que mostraste
                prompt = prompt,
                stream = false
            };

            _logger.LogInformation($"Sending request to Ollama with prompt: {prompt}");

            var response = await _httpClient.PostAsJsonAsync("/api/generate", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Ollama API Error: {response.StatusCode}. Content: {errorContent}");
                throw new HttpRequestException($"Error: {response.StatusCode}. Content: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Received response from Ollama: {responseContent}");

            var result = JsonConvert.DeserializeObject<OllamaResponse>(responseContent);
            return result?.Response ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error connecting to Ollama: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GenerateImprovedDescriptionAsync(string title, string? currentDescription)
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

        // Limpiar la descripción de posibles prefijos o elementos no deseados
        return result?.Response?.Trim() ?? string.Empty;
    }

    private class OllamaResponse
    {
        public string Response { get; set; }
    }

}