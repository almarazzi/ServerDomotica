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
        public record Spazio(string Nome, double SpazioLibero, double SpazioTotale)
        {
            public static readonly Spazio Empty = new("",0.0,0.0);
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
            
            var g = Spazio.Empty with { Nome = m_driveInfo.Name, SpazioLibero =Math.Round((double)(m_driveInfo.TotalFreeSpace)/(1024*1024*1024),2), SpazioTotale = Math.Round((double)(m_driveInfo.TotalSize)/(1024*1024*1024),2)};
            return Ok(g);
        }
    }
}
