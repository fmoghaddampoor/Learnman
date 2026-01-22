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

        var freeModels = new[] 
        { 
            "google/gemini-2.0-flash-exp:free", 
            "google/gemini-exp-1206:free",
            "meta-llama/llama-3-8b-instruct:free", 
            "microsoft/phi-3-mini-128k-instruct:free"
        };

        foreach (var model in freeModels)
        {
            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = message }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try 
            {
                var response = await _httpClient.PostAsync(url, content);
                
                // If success, return immediately
                if (response.IsSuccessStatusCode)
                {
                     var jsonResponse = await response.Content.ReadAsStringAsync();
                     using var doc = JsonDocument.Parse(jsonResponse);
                     if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                     {
                        var contentElem = choices[0].GetProperty("message").GetProperty("content").GetString();
                        return contentElem ?? "...";
                     }
                }
                
                // If 429 (Too Many Requests), loop to next model
                if ((int)response.StatusCode == 429)
                {
                    continue; // Try next model
                }
                
                // For other errors, maybe just return or continue? Let's just return the error if it's the last one.
                // Or better, only error if ALL fail.
            }
            catch 
            {
                // Continue to next model on exception
            }
        }
        
        return "*pouts* All the free stargates are busy right now! (Rate Limited on all free models). Please try again in a moment.";


    }
}
