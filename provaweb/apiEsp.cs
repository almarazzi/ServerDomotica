using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace provaweb
{
    [Route("[controller]")]
    [ApiController]
    public class apiEsp : ControllerBase
    {
        private readonly ILogger<apiEsp> m_logger;
        private readonly RegistroEsp m_registroEsp;
        private readonly ContorolloEspOnline m_stateRelay;

        public apiEsp(ILogger<apiEsp> logger, RegistroEsp registro, ContorolloEspOnline stateRelay)
        {
            m_logger = logger;
            m_registroEsp = registro;
            m_stateRelay = stateRelay;
        }

        public record ESP(string nomeEsp, string mac);
        public record Abilitazione(bool abilitazione, string mac);
        [HttpGet("ListaEsp")]
        [Authorize]
        public async Task<IActionResult> ListaEsp()
        {
            var f = await m_registroEsp.dammiListaEsp();
            return Ok(f.ToList());
        }
        [HttpPut("NomeEsp")]
        [Authorize]
        public async Task<IActionResult> NomeEsp(ESP esp)
        {

            var f = await m_registroEsp.dammiListaEsp();
            await m_registroEsp.ModificareProgrammaEsp8266(f[esp.mac] with { NomeEspClient = esp.nomeEsp}, esp.mac);
            return Ok();
        }
        [HttpPut("abilitazione")]
        [Authorize]
        public async Task<IActionResult> abilitazione(Abilitazione abilitazione)
        {

            var f = await m_registroEsp.dammiListaEsp();
            await m_registroEsp.ModificareProgrammaEsp8266(f[abilitazione.mac] with { abilitazione = abilitazione.abilitazione}, abilitazione.mac);
            return Ok();
        }
        [HttpGet("StatoRelay")]
        [Authorize]
        public IActionResult StatoEsp()
        {

            var f =  m_stateRelay.Offline;
            return Ok(f);
        }
    }
}
