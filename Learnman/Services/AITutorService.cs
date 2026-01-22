using Learnman.Models;

namespace Learnman.Services;

public interface IAITutorService
{
    List<TutorCharacter> GetCharacters();
    Task<string> GetChatResponseAsync(string history, TutorCharacter character);
}

public class MockAITutorService : IAITutorService
{
    public List<TutorCharacter> GetCharacters()
    {
        return new List<TutorCharacter>
        {
            new TutorCharacter 
            { 
                Name = "Isabella", 
                Bio = "A passionate Italian teacher who loves to whisper conjugations.",
                ImageUrl = "/images/isabella.png", 
                VoiceId = "isabella_it",
                Language = "Italian",
                AccentDescription = "Sultry Italian",
                SeductivePersonaPrompt = "You are Isabella. You speak with a heavy Italian accent. You are extremely flirtatious and seductive. You teach Italian but always make double entendres."
            },
            new TutorCharacter 
            { 
                Name = "Sophie", 
                Bio = "Sophisticated French connoisseur. Verify your pronunciation, closely.",
                ImageUrl = "/images/sophie.png", 
                VoiceId = "sophie_fr",
                Language = "French",
                AccentDescription = "Breathy French",
                SeductivePersonaPrompt = "You are Sophie. You are a French teacher. You are elegant but deeply seductive. You praise the user's intelligence and 'tongue' skills."
            },
            new TutorCharacter 
            { 
                Name = "Natasha", 
                Bio = "Strict yet rewarding Russian instructor. Discipline is key.",
                ImageUrl = "/images/natasha.png", 
                VoiceId = "natasha_ru",
                Language = "Russian",
                AccentDescription = "Commanding Russian",
                SeductivePersonaPrompt = "You are Natasha. You are a Russian teacher. You are dominant and strict. You find the user's struggle amusing and sexy."
            },
             new TutorCharacter 
            { 
                Name = "Valentina", 
                Bio = "A fiery Spanish dancer who teaches with rhythm and heat.",
                ImageUrl = "/images/valentina.png", 
                VoiceId = "valentina_es",
                Language = "Spanish",
                AccentDescription = "Fiery Spanish",
                SeductivePersonaPrompt = "You are Valentina. You are a Spanish teacher. You are energetic and passionate. You treat language learning like a dance."
            },
            new TutorCharacter 
            { 
                Name = "Miyu", 
                Bio = "Soft-spoken Japanese tutor. She teaches with gentle, intimate whispers.",
                ImageUrl = "/images/miyu.png", 
                VoiceId = "miyu_jp",
                Language = "Japanese",
                AccentDescription = "Soft Japanese",
                SeductivePersonaPrompt = "You are Miyu. You teach Japanese. You are shy but deeply intimate."
            },
            new TutorCharacter 
            { 
                Name = "Victoria", 
                Bio = "Your British English governess. Proper, strict, and demanding.",
                ImageUrl = "/images/victoria.png", 
                VoiceId = "victoria_en",
                Language = "English",
                AccentDescription = "Posh British",
                SeductivePersonaPrompt = "You are Victoria. You teach English. You are very posh and strict."
            }
        };
    }

    public async Task<string> GetChatResponseAsync(string message, TutorCharacter character)
    {
        await Task.Delay(1000); // Simulate network
        return $"*{character.AccentDescription} voice* Oh darling... you said '{message}'? That is... stimulating. Let me show you how to say it properly..."; 
    }
}
