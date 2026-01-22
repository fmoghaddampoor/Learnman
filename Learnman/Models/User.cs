using System.ComponentModel.DataAnnotations;

namespace Learnman.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string FirstName { get; set; } = "";
    
    [Required]
    public string LastName { get; set; } = "";
    
    [Required]
    public string Email { get; set; } = "";
    
    [Required]
    public string PasswordHash { get; set; } = "";
    
    // Preferences
    public string NativeLanguage { get; set; } = "English";
    public string TargetLanguage { get; set; } = "Spanish";
    public string Location { get; set; } = "USA";
    
    public string? GeminiApiKey { get; set; } // User's API Key for BYOK
    public int TotalPoints { get; set; } = 0;
}
