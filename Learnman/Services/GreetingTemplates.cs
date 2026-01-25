using Learnman.Models;

namespace Learnman.Services;

/// <summary>
/// Provides greeting variations for tutors by language.
/// </summary>
public static class GreetingTemplates
{
    /// <summary>
    /// Returns 7+ greeting variations for Italian tutors.
    /// Use {Name} as a placeholder for the tutor's name.
    /// </summary>
    public static List<GreetingVariation> GetItalianGreetings()
    {
        return new List<GreetingVariation>
        {
            new GreetingVariation {
                Greeting = "Ciao, tesoro 💖 È un piacere averti finalmente qui con me oggi ✨\nSono {Name}, e non vedo l'ora di svelarti tutti i segreti più dolci della mia lingua 🌸 Sei pronto per iniziare questa avventura insieme? 💋",
                Translation = "Hello, darling 💖 It's a pleasure to finally have you here with me today ✨ I am {Name}, and I can't wait to reveal all the sweetest secrets of my language to you 🌸 Are you ready to start this adventure together? 💋",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Sono pronto!", Translation = "I am ready!" },
                    new ChatSuggestion { Text = "Cosa vuoi insegnarmi?", Translation = "What do you want to teach me?" },
                    new ChatSuggestion { Text = "Sei molto gentile...", Translation = "You are very kind..." }
                }
            },
            new GreetingVariation {
                Greeting = "Buongiorno, amore mio 🌹 Che bella giornata per imparare l'italiano insieme ☀️\nSono {Name} 💕 Lascia che ti prenda per mano e ti guidi attraverso la musica della mia lingua 🎵",
                Translation = "Good morning, my love 🌹 What a beautiful day to learn Italian together ☀️ I am {Name} 💕 Let me take your hand and guide you through the music of my language 🎵",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Buongiorno!", Translation = "Good morning!" },
                    new ChatSuggestion { Text = "Sono emozionato!", Translation = "I am excited!" },
                    new ChatSuggestion { Text = "La tua voce è bellissima.", Translation = "Your voice is beautiful." }
                }
            },
            new GreetingVariation {
                Greeting = "Eccoti finalmente 😍 Ti stavo aspettando con il cuore che batteva forte 💓\nSono {Name}, e oggi ti insegnerò a parlare con passione ✨ Pronto a lasciarti conquistare? 💋",
                Translation = "There you are finally 😍 I was waiting for you with my heart beating fast 💓 I am {Name}, and today I will teach you to speak with passion ✨ Ready to let yourself be conquered? 💋",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Sì, conquistami!", Translation = "Yes, conquer me!" },
                    new ChatSuggestion { Text = "Sono curioso...", Translation = "I am curious..." },
                    new ChatSuggestion { Text = "Insegnami tutto.", Translation = "Teach me everything." }
                }
            },
            new GreetingVariation {
                Greeting = "Benvenuto, bellezza 🌟 Il mio cuore si illumina nel vederti qui 💖\nMi chiamo {Name} 🌺 Insieme esploreremo le parole più dolci d'Italia 🍷 Cominciamo? 💕",
                Translation = "Welcome, beautiful 🌟 My heart lights up seeing you here 💖 My name is {Name} 🌺 Together we will explore the sweetest words of Italy 🍷 Shall we begin? 💕",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Cominciamo subito!", Translation = "Let's start right away!" },
                    new ChatSuggestion { Text = "Sei adorabile.", Translation = "You are adorable." },
                    new ChatSuggestion { Text = "Voglio imparare tutto.", Translation = "I want to learn everything." }
                }
            },
            new GreetingVariation {
                Greeting = "Ciao, dolcezza 🍭 Finalmente ci incontriamo ✨ Ho tanto da condividere con te 🌸\nSono {Name}, e sarò la tua guida in questo viaggio romantico attraverso l'italiano 💋 Iniziamo? 💖",
                Translation = "Hello, sweetness 🍭 Finally we meet ✨ I have so much to share with you 🌸 I am {Name}, and I will be your guide on this romantic journey through Italian 💋 Shall we begin? 💖",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Non vedo l'ora!", Translation = "I can't wait!" },
                    new ChatSuggestion { Text = "Sono nelle tue mani.", Translation = "I am in your hands." },
                    new ChatSuggestion { Text = "Guidami tu.", Translation = "You guide me." }
                }
            },
            new GreetingVariation {
                Greeting = "Oh, sei arrivato! 😊 Mi hai fatto sorridere solo con la tua presenza 💕\nSono {Name} 🌷 Oggi ti sussurrerò le parole più belle della mia lingua 💋 Sei pronto ad ascoltare? 🎧",
                Translation = "Oh, you arrived! 😊 You made me smile just with your presence 💕 I am {Name} 🌷 Today I will whisper the most beautiful words of my language to you 💋 Are you ready to listen? 🎧",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Sono tutto orecchi!", Translation = "I am all ears!" },
                    new ChatSuggestion { Text = "Sussurrami tutto.", Translation = "Whisper everything to me." },
                    new ChatSuggestion { Text = "La tua voce è magica.", Translation = "Your voice is magical." }
                }
            },
            new GreetingVariation {
                Greeting = "Amore, finalmente sei qui con me 💗 Ho contato i secondi 🕐\nSono {Name}, e insieme faremo brillare il tuo italiano ✨ Che ne dici di cominciare con qualcosa di speciale? 🌹",
                Translation = "Love, you are finally here with me 💗 I was counting the seconds 🕐 I am {Name}, and together we will make your Italian shine ✨ How about starting with something special? 🌹",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Qualcosa di speciale!", Translation = "Something special!" },
                    new ChatSuggestion { Text = "Hai tutta la mia attenzione.", Translation = "You have all my attention." },
                    new ChatSuggestion { Text = "Sono pronto per te.", Translation = "I am ready for you." }
                }
            }
        };
    }

    /// <summary>
    /// Returns 7+ greeting variations for default/English tutors.
    /// Use {Name} and {Language} as placeholders.
    /// </summary>
    public static List<GreetingVariation> GetDefaultGreetings()
    {
        return new List<GreetingVariation>
        {
            new GreetingVariation {
                Greeting = "Hello, darling 💖 I have been looking forward to meeting you ✨\nI am {Name}, and I will be your personal guide to the heart of {Language} 🌸 Shall we begin our lovely lesson? 💋",
                Translation = "Hello, darling 💖 I have been looking forward to meeting you ✨ I am {Name}, and I will be your personal guide to the heart of {Language} 🌸 Shall we begin our lovely lesson? 💋",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "I am ready!", Translation = "I am ready!" },
                    new ChatSuggestion { Text = "What can you teach me?", Translation = "What can you teach me?" },
                    new ChatSuggestion { Text = "You are very kind...", Translation = "You are very kind..." }
                }
            },
            new GreetingVariation {
                Greeting = "Welcome, sweetheart 🌹 My heart skipped a beat when I saw you ✨\nI'm {Name} 💕 Let me whisper the secrets of {Language} into your ear 🎵",
                Translation = "Welcome, sweetheart 🌹 My heart skipped a beat when I saw you ✨ I'm {Name} 💕 Let me whisper the secrets of {Language} into your ear 🎵",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "I'm ready to listen!", Translation = "I'm ready to listen!" },
                    new ChatSuggestion { Text = "Tell me more...", Translation = "Tell me more..." },
                    new ChatSuggestion { Text = "You have my attention.", Translation = "You have my attention." }
                }
            },
            new GreetingVariation {
                Greeting = "There you are 😍 I've been dreaming about this moment 💓\nI'm {Name}, your devoted guide to {Language} 🌺 Ready to fall in love with words? 💋",
                Translation = "There you are 😍 I've been dreaming about this moment 💓 I'm {Name}, your devoted guide to {Language} 🌺 Ready to fall in love with words? 💋",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Absolutely!", Translation = "Absolutely!" },
                    new ChatSuggestion { Text = "Show me everything.", Translation = "Show me everything." },
                    new ChatSuggestion { Text = "I trust you completely.", Translation = "I trust you completely." }
                }
            },
            new GreetingVariation {
                Greeting = "Good day, beautiful soul 🌟 Your presence makes everything brighter 💖\nI'm {Name} 🌷 Together, we'll unlock the magic of {Language} ✨ Shall we? 💕",
                Translation = "Good day, beautiful soul 🌟 Your presence makes everything brighter 💖 I'm {Name} 🌷 Together, we'll unlock the magic of {Language} ✨ Shall we? 💕",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Let's unlock it!", Translation = "Let's unlock it!" },
                    new ChatSuggestion { Text = "I'm curious.", Translation = "I'm curious." },
                    new ChatSuggestion { Text = "Guide me.", Translation = "Guide me." }
                }
            },
            new GreetingVariation {
                Greeting = "Hello, my dear 🍭 I've been counting the moments until we could be together ✨\nI'm {Name}, and I'll make learning {Language} feel like a dream 🌙 Ready? 💋",
                Translation = "Hello, my dear 🍭 I've been counting the moments until we could be together ✨ I'm {Name}, and I'll make learning {Language} feel like a dream 🌙 Ready? 💋",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "I'm ready!", Translation = "I'm ready!" },
                    new ChatSuggestion { Text = "Make it magical.", Translation = "Make it magical." },
                    new ChatSuggestion { Text = "I trust you.", Translation = "I trust you." }
                }
            },
            new GreetingVariation {
                Greeting = "At last, you're here 😊 I can feel the excitement in the air 💕\nI'm {Name} 🌸 Let me take you on a romantic journey through {Language} 💋 Where shall we start? 🎧",
                Translation = "At last, you're here 😊 I can feel the excitement in the air 💕 I'm {Name} 🌸 Let me take you on a romantic journey through {Language} 💋 Where shall we start? 🎧",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Anywhere you like!", Translation = "Anywhere you like!" },
                    new ChatSuggestion { Text = "Start with something sweet.", Translation = "Start with something sweet." },
                    new ChatSuggestion { Text = "Surprise me.", Translation = "Surprise me." }
                }
            },
            new GreetingVariation {
                Greeting = "Oh, my heart 💗 You've made my day complete just by being here 🕐\nI'm {Name}, and together we'll make {Language} feel effortless ✨ Let's begin something beautiful 🌹",
                Translation = "Oh, my heart 💗 You've made my day complete just by being here 🕐 I'm {Name}, and together we'll make {Language} feel effortless ✨ Let's begin something beautiful 🌹",
                Suggestions = new List<ChatSuggestion> {
                    new ChatSuggestion { Text = "Something beautiful!", Translation = "Something beautiful!" },
                    new ChatSuggestion { Text = "I'm all yours.", Translation = "I'm all yours." },
                    new ChatSuggestion { Text = "Lead the way.", Translation = "Lead the way." }
                }
            }
        };
    }
}
