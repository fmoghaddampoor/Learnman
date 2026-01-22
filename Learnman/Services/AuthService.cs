using Learnman.Data;
using Learnman.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Learnman.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        if (VerifyPassword(password, user.PasswordHash))
        {
            CurrentUser = user;
            return user;
        }
        return null;
    }

    public async Task<User> RegisterAsync(string firstName, string lastName, string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            throw new Exception("Email already exists");
        }

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        CurrentUser = user;
        return user;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }

    // Simple Session State for this demo
    public User? CurrentUser { get; private set; }

    public async Task UpdateUserPreferencesAsync(string native, string location, string target, string? apiKey = null)
    {
        if (CurrentUser == null) return;

        var user = await _context.Users.FindAsync(CurrentUser.Id);
        if (user != null)
        {
            user.NativeLanguage = native;
            user.Location = location;
            user.TargetLanguage = target;
            if (apiKey != null) user.GeminiApiKey = apiKey;
            
            CurrentUser = user; // Update local state
            await _context.SaveChangesAsync();
        }
    }
}

