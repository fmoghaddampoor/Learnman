using Learnman.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Learnman.Services;

public class GeminiAITutorService : IAITutorService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public GeminiAITutorService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public List<TutorCharacter> GetCharacters()
    {
        // Reuse the mock list for now, or move to DB later. 
        // We'll just instantiate the same list as Mock/Static for consistency.
        return new MockAITutorService().GetCharacters(); 
    }

    public async Task<string> GetChatResponseAsync(string message, TutorCharacter character)
    {
        var apiKey = _authService.CurrentUser?.GeminiApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            return "*whispers* I need your API Key to speak, darling... (Please enter it in Settings)";
        }

        // Construct system prompt
        var systemPrompt = $"{character.SeductivePersonaPrompt} Current Student Context: Native Language: {_authService.CurrentUser?.NativeLanguage}, Target Language: {character.Language}. Keep responses short, flirtatious, and educational. Correct mistakes gently but seductively.";

        var modelId = "gemini-1.5-flash"; // Default
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelId}:generateContent?key={apiKey}";

        // Helper to create content
        StringContent CreateRequest(string m) 
        {
             var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = systemPrompt + "\n\nUser: " + message } }
                    }
                }
            };
            return new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        }

        try 
        {
            var response = await _httpClient.PostAsync(url, CreateRequest(modelId));
            
            // If 404, the model doesn't exist. Let's ask the API what models ARE available.
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var listUrl = $"https://generativelanguage.googleapis.com/v1beta/models?key={apiKey}";
                var listResponse = await _httpClient.GetAsync(listUrl);
                
                if (listResponse.IsSuccessStatusCode)
                {
                    var listJson = await listResponse.Content.ReadAsStringAsync();
                    using var listDoc = JsonDocument.Parse(listJson);
                    
                    // Find a suitable model
                    string? fallbackModel = null;
                    if (listDoc.RootElement.TryGetProperty("models", out var modelsElement))
                    {
                         foreach (var m in modelsElement.EnumerateArray())
                         {
                             var name = m.GetProperty("name").GetString(); // e.g., "models/gemini-1.5-flash"
                             var methods = m.GetProperty("supportedGenerationMethods");
                             bool supportsGenerate = false;
                             foreach(var method in methods.EnumerateArray()) 
                             {
                                 if (method.GetString() == "generateContent") supportsGenerate = true;
                             }

                             if (supportsGenerate && name != null && name.Contains("gemini"))
                             {
                                 fallbackModel = name; // format: "models/model-name"
                                 if (name.Contains("flash")) break; // Prefer flash
                             }
                         }
                    }

                    if (fallbackModel != null)
                    {
                        // Retry with fallback
                        // fallbackModel already contains "models/", so just append :generateContent
                        // wait, the name is "models/foo", but URL expects "models/foo:generateContent"
                        // v1beta/models/gemini-1.5-flash:generateContent
                        // if fallback is "models/gemini-1.0-pro", we want https://.../v1beta/models/gemini-1.0-pro:generateContent
                        
                        var fallbackUrl = $"https://generativelanguage.googleapis.com/v1beta/{fallbackModel}:generateContent?key={apiKey}";
                        response = await _httpClient.PostAsync(fallbackUrl, CreateRequest(message));
                    }
                    else
                    {
                         return $"*frowns* I looked at the stars (ListModels) but found no Gemini models I can use... ({listJson})";
                    }
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                return $"*frowns* My connection to the stars is broken... (Status: {response.StatusCode} - {errorDetail})";
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            
            // Handle potentially empty candidates (safety blocks)
            if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                 var candidate = candidates[0];
                 if (candidate.TryGetProperty("content", out var contentElem) && 
                     contentElem.TryGetProperty("parts", out var parts) && 
                     parts.GetArrayLength() > 0)
                 {
                     return parts[0].GetProperty("text").GetString() ?? "...";
                 }
                 else
                 {
                     return "*blushes* I... I cannot say what I'm thinking (Safety Filter Triggered).";
                 }
            }
            return "*confused* The stars are silent today.";
        }
        catch (Exception ex)
        {
            return $"*sighs* I cannot hear you... ({ex.Message})";
        }
    }
}
