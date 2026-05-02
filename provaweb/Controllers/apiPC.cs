using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using provaweb.Controllers;

namespace provaweb
{
    [Route("[controller]")]
    [ApiController]
    public class apiPC : Controller
    {
        private readonly ILogger<Login> m_logger;
        private readonly DriveInfo m_driveInfo;
        public record Spazio(string Nome, long SpazioLibero, long SpazioTotale)
        {
            public static readonly Spazio Empty = new("",0,0);
        };

        public apiPC(ILogger<Login> logger,DriveInfo driveInfo)
        {
            m_logger = logger;
            m_driveInfo = driveInfo;
        }

        [HttpGet("SpazioDig")]
        [Authorize]
        public async Task<IActionResult> SpazioDig()
        {
            
            var g = Spazio.Empty with { Nome = m_driveInfo.Name, SpazioLibero = m_driveInfo.TotalFreeSpace/(1024*1024*1024), SpazioTotale = m_driveInfo.TotalSize/(1024*1024*1024) };
            return Ok(g);
        }
    }
}
