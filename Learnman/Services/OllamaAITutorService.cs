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

    public async Task<ChatResponse> GetChatResponseAsync(IEnumerable<ChatMessage> history, TutorCharacter character)
    {
        var url = "http://localhost:11434/api/chat";
        
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
        foreach (var msg in history.TakeLast(10))
        {
            apiMessages.Add(new { role = msg.Role, content = msg.Content });
        }

        var requestBody = new
        {
            model = "llama3.2", 
            messages = apiMessages,
            stream = false,
            format = "json" 
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try 
        {
            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                return new ChatResponse { Message = $"*frowns* I cannot reach my local brain (Ollama)... Is it running? (Error: {response.StatusCode})" };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);
            
            if (doc.RootElement.TryGetProperty("message", out var msgElem) && 
                msgElem.TryGetProperty("content", out var contentElem))
            {
                var contentStr = contentElem.GetString();
                if (!string.IsNullOrEmpty(contentStr))
                {
                    try 
                    {
                        var chatResp = JsonSerializer.Deserialize<ChatResponse>(contentStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (chatResp != null) return chatResp;
                    }
                    catch 
                    {
                        return new ChatResponse { Message = contentStr };
                    }
                }
            }

            return new ChatResponse { Message = "*confused* Ollama sent a blank stare." };
        }
        catch (Exception ex)
        {
            return new ChatResponse { Message = $"*sighs* Is Ollama installed and running? I can't connect to localhost:11434. ({ex.Message})" };
        }
    }
}
