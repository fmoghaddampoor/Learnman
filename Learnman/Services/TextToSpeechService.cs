namespace Learnman.Services;

public interface ITextToSpeechService
{
    Task<string> SynthesizeAsync(string text, string voiceId);
}

public class MockTextToSpeechService : ITextToSpeechService
{
    public async Task<string> SynthesizeAsync(string text, string voiceId)
    {
        await Task.Delay(500);
        // In a real app, this returns a Blob URL or Base64 audio.
        // For now, we returns empty to signify success, frontend can just show a playing indicator.
        return ""; 
    }
}
