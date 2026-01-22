using Learnman.Models;
using System.Text;
using System.Text.Json;

namespace Learnman.Services;

public class OllamaAITutorService : IAITutorService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public OllamaAITutorService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public List<TutorCharacter> GetCharacters()
    {
        return new MockAITutorService().GetCharacters(); 
    }

    public async Task<string> GetChatResponseAsync(string message, TutorCharacter character)
    {
        var url = "http://localhost:11434/api/chat";
        
        // System Prompt
        var systemPrompt = $"{character.SeductivePersonaPrompt} Current Student Context: Native Language: {_authService.CurrentUser?.NativeLanguage}, Target Language: {character.Language}. Keep responses short, flirtatious, and educational. Correct mistakes gently but seductively.";

        var requestBody = new
        {
            model = "llama3.2", 
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = message }
            },
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try 
        {
            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                return $"*frowns* I cannot reach my local brain (Ollama)... Is it running? (Error: {response.StatusCode})";
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            
            // Ollama Format: { "message": { "content": "..." } }
            if (doc.RootElement.TryGetProperty("message", out var msgElem) && 
                msgElem.TryGetProperty("content", out var contentElem))
            {
                return contentElem.GetString() ?? "...";
            }

            return "*confused* Ollama sent a blank stare.";
        }
        catch (Exception ex)
        {
            return $"*sighs* Is Ollama installed and running? I can't connect to localhost:11434. ({ex.Message})";
        }
    }
}
