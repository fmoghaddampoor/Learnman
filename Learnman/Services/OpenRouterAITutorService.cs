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

    public async Task<ChatResponse> GetChatResponseAsync(IEnumerable<ChatMessage> history, TutorCharacter character)
    {
        var apiKey = ApiSecrets.OpenRouterKey;
        if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_OPENROUTER_KEY"))
        {
            return new ChatResponse { Message = "*whispers* You must set your OpenRouter API Key in ApiSecrets.cs, darling..." };
        }

        var url = "https://openrouter.ai/api/v1/chat/completions";
        
        // System Prompt
        var systemPrompt = $"{character.SeductivePersonaPrompt} Current Student Context: Native Language: {_authService.CurrentUser?.NativeLanguage}, Target Language: {character.Language}. " +
                           "Keep responses short, flirtatious, and educational. Correct mistakes gently but seductively. " +
                           $"Your 'message' and the 'text' in 'suggestions' MUST BE ENTIRELY IN {character.Language.ToUpper()}. " +
                           "ALWAYS respond in strictly valid JSON format with exactly these keys: " +
                           "1. 'message': Your reply as the tutor in the target language. " +
                           "2. 'messageTranslation': The translation of your 'message' into the student's native language. " +
                           "3. 'userMessageTranslation': The translation of the STUDENT'S LAST message into their native language. " +
                           "4. 'isCorrect': (Boolean) Evaluate if the student's LAST message was grammatically and contextually correct for a student at their level. " +
                           "5. 'correctionFeedback': (Optional string) If 'isCorrect' is false, provide a very short, seductive correction. " +
                           "6. 'suggestions': A list of 3-4 objects, each with 'text' (a natural reply the STUDENT could say next, in the target language) and 'translation' (in the student's native language).";

        var apiMessages = new List<object> { new { role = "system", content = systemPrompt } };
        foreach (var msg in history.TakeLast(10)) // Last 10 messages for context
        {
            apiMessages.Add(new { role = msg.Role, content = msg.Content });
        }

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
                messages = apiMessages,
                response_format = new { type = "json_object" } 
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try 
            {
                var response = await _httpClient.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                     var jsonResponse = await response.Content.ReadAsStringAsync();
                     using var doc = JsonDocument.Parse(jsonResponse);
                     if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                     {
                        var contentElem = choices[0].GetProperty("message").GetProperty("content").GetString();
                        if (!string.IsNullOrEmpty(contentElem))
                        {
                            try 
                            {
                                var chatResp = JsonSerializer.Deserialize<ChatResponse>(contentElem, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                if (chatResp != null) return chatResp;
                            }
                            catch 
                            {
                                // Fallback if AI didn't return perfect JSON
                                return new ChatResponse { Message = contentElem };
                            }
                        }
                     }
                }
                
                if ((int)response.StatusCode == 429)
                {
                    continue; 
                }
            }
            catch 
            {
                // Continue to next model on exception
            }
        }
        
        return new ChatResponse { Message = "*pouts* All the free stargates are busy right now! (Rate Limited on all free models). Please try again in a moment." };
    }
}
