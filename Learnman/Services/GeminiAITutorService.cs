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

    public async Task<ChatResponse> GetChatResponseAsync(IEnumerable<ChatMessage> history, TutorCharacter character)
    {
        var apiKey = _authService.CurrentUser?.GeminiApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            return new ChatResponse { Message = "*whispers* I need your API Key to speak, darling... (Please enter it in Settings)" };
        }

        // Construct system prompt
        var systemPrompt = $"{character.SeductivePersonaPrompt} Current Student Context: Native Language: {_authService.CurrentUser?.NativeLanguage}, Target Language: {character.Language}. " +
                           "Keep responses short, flirtatious, and educational. Correct mistakes gently but seductively. " +
                           "Use MANY emojis and express your emotions and physical reactions clearly (e.g., using *smiles*, *winks*, *giggles* or specific emojis like 😉, 🇮🇹, 🔥, ✨). " +
                           $"Your 'message' and the 'text' in 'suggestions' MUST BE ENTIRELY IN {character.Language.ToUpper()}. " +
                           "ALWAYS respond in strictly valid JSON format with exactly these keys: " +
                           "1. 'message': Your reply as the tutor in the target language. " +
                           "2. 'messageTranslation': The translation of your 'message' into the student's native language. " +
                           "3. 'userMessageTranslation': The translation of the STUDENT'S LAST message into their native language. " +
                           "4. 'isCorrect': (Boolean) Evaluate if the student's LAST message was grammatically and contextually correct for a student at their level. " +
                           "5. 'correctionFeedback': (Optional string) If 'isCorrect' is false, provide a very short, seductive correction. " +
                           "6. 'suggestions': A list of 3-4 objects, each with 'text' (a natural reply the STUDENT could say next, in the target language) and 'translation' (in the student's native language).";

        var modelId = "gemini-1.5-flash"; // Default
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelId}:generateContent?key={apiKey}";

        // Helper to create content with history
        StringContent CreateRequest(string model) 
        {
            var contents = new List<object>();
            
            // Add messages from history, mapping roles
            foreach (var msg in history.TakeLast(10))
            {
                contents.Add(new
                {
                    role = msg.Role == "user" ? "user" : "model",
                    parts = new[] { new { text = msg.Content } }
                });
            }

            var requestBody = new
            {
                system_instruction = new { parts = new[] { new { text = systemPrompt } } },
                contents = contents,
                generationConfig = new { response_mime_type = "application/json" }
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
                        var fallbackUrl = $"https://generativelanguage.googleapis.com/v1beta/{fallbackModel}:generateContent?key={apiKey}";
                        response = await _httpClient.PostAsync(fallbackUrl, CreateRequest(fallbackModel));
                    }
                    else
                    {
                         return new ChatResponse { Message = $"*frowns* I looked at the stars (ListModels) but found no Gemini models I can use... ({listJson})" };
                    }
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                return new ChatResponse { Message = $"*frowns* My connection to the stars is broken... (Status: {response.StatusCode} - {errorDetail})" };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            
            if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                 var candidate = candidates[0];
                 if (candidate.TryGetProperty("content", out var contentElem) && 
                     contentElem.TryGetProperty("parts", out var parts) && 
                     parts.GetArrayLength() > 0)
                 {
                     var text = parts[0].GetProperty("text").GetString();
                     if (!string.IsNullOrEmpty(text))
                     {
                        try 
                        {
                            var chatResp = JsonSerializer.Deserialize<ChatResponse>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            if (chatResp != null) return chatResp;
                        }
                        catch 
                        {
                            return new ChatResponse { Message = text };
                        }
                     }
                 }
                 else
                 {
                     return new ChatResponse { Message = "*blushes* I... I cannot say what I'm thinking (Safety Filter Triggered)." };
                 }
            }
            return new ChatResponse { Message = "*confused* The stars are silent today." };
        }
        catch (Exception ex)
        {
            return new ChatResponse { Message = $"*sighs* I cannot hear you... ({ex.Message})" };
        }
    }
}
