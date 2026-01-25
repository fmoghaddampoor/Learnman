using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Learnman.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThemeController : ControllerBase
    {
        private readonly string _themeFile;

        public ThemeController()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string learnmanDir = Path.Combine(appData, "Learnman");
            
            // Ensure directory exists (Web App might run first)
            if (!System.IO.Directory.Exists(learnmanDir))
            {
                System.IO.Directory.CreateDirectory(learnmanDir);
            }

            _themeFile = Path.Combine(learnmanDir, "theme.lock");
        }

        [HttpGet]
        public IActionResult GetTheme()
        {
            try
            {
                if (System.IO.File.Exists(_themeFile))
                {
                    string theme = System.IO.File.ReadAllText(_themeFile).Trim();
                    return Ok(new { theme });
                }
                return Ok(new { theme = "light" }); // Default
            }
            catch
            {
                return Ok(new { theme = "light" });
            }
        }
        
        [HttpPost]
        public IActionResult SetTheme([FromBody] ThemeRequest request)
        {
             try
            {
                if (!string.IsNullOrEmpty(request.Theme))
                {
                     System.IO.File.WriteAllText(_themeFile, request.Theme);
                     return Ok();
                }
                return BadRequest();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }

    public class ThemeRequest
    {
        public string Theme { get; set; } = "";
    }
}
