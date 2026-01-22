namespace Learnman.Models;

public class TutorCharacter
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Bio { get; set; } = "";
    public string ImageUrl { get; set; } = ""; // Path to image
    public string VoiceId { get; set; } = "";
    public string Language { get; set; } = "English"; // Language the tutor teaches
    public string AccentDescription { get; set; } = "";
    public string SeductivePersonaPrompt { get; set; } = ""; // Internal prompt for the AI
}

public class ChatMessage
{
    public string Role { get; set; } = "user"; // "user" or "assistant"
    public string Content { get; set; } = "";
    public string? Translation { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class ChatResponse
{
    public string Message { get; set; } = "";
    public string MessageTranslation { get; set; } = "";
    public string UserMessageTranslation { get; set; } = "";
    public List<ChatSuggestion> Suggestions { get; set; } = new();
    public bool? IsCorrect { get; set; } // null if not an evaluation
    public string? CorrectionFeedback { get; set; }
}

public class ChatSuggestion
{
    public string Text { get; set; } = "";
    public string Translation { get; set; } = "";
}
