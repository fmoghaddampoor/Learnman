using Microsoft.AspNetCore.Mvc;

namespace Learnman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        [HttpPost]
        public IActionResult SetTheme([FromBody] ThemeRequest request)
        {
            if (string.IsNullOrEmpty(request.Theme))
                return BadRequest();

            // Save to root directory so Tray App can find it
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme.lock");
            
            try 
            {
                System.IO.File.WriteAllText(path, request.Theme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetTheme()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme.lock");
            try
            {
                if (System.IO.File.Exists(path))
                {
                    var theme = System.IO.File.ReadAllText(path).Trim();
                    return Ok(new { theme });
                }
                return Ok(new { theme = "light" }); // Default
            }
            catch
            {
                return Ok(new { theme = "light" });
            }
        }
    }

    public class ThemeRequest
    {
        public string Theme { get; set; } = "";
    }
}
