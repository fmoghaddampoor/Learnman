using Learnman.Models;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Learnman.Services;

public class OpenRouterAITutorService : IAITutorService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public OpenRouterAITutorService(HttpClient httpClient, AuthService authService)
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
        var apiKey = ApiSecrets.OpenRouterKey;
        if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_OPENROUTER_KEY"))
        {
            return "*whispers* You must set your OpenRouter API Key in ApiSecrets.cs, darling...";
        }

        var url = "https://openrouter.ai/api/v1/chat/completions";
        
        // System Prompt
        var systemPrompt = $"{character.SeductivePersonaPrompt} Current Student Context: Native Language: {_authService.CurrentUser?.NativeLanguage}, Target Language: {character.Language}. Keep responses short, flirtatious, and educational. Correct mistakes gently but seductively.";

        var requestBody = new
        {
            model = "google/gemini-pro-1.5", // Or "openai/gpt-4o-mini", "meta-llama/llama-3-8b-instruct"
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = message }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        // OpenRouter optional headers
        if (!_httpClient.DefaultRequestHeaders.Contains("HTTP-Referer"))
        {
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:5000");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "Learnman AI Tutor");
        }

        try 
        {
            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                return $"*frowns* OpenRouter rejected us... ({response.StatusCode} - {errorDetail})";
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            
            // OpenAI Format: choices[0].message.content
            if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var contentElem = choices[0].GetProperty("message").GetProperty("content").GetString();
                return contentElem ?? "...";
            }

            return "*confused* OpenRouter sent silence.";
        }
        catch (Exception ex)
        {
            return $"*sighs* I cannot hear you... ({ex.Message})";
        }
    }
}
