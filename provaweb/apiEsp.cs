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

        public apiEsp(ILogger<apiEsp> logger, RegistroEsp registro)
        {
            m_logger = logger;
            m_registroEsp = registro;

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
            var lista = f.Where(x => x.Key == esp.mac).Select(x => x.Value).ToList();
            await m_registroEsp.ModificareProgrammaEsp8266(Receiver_esp.Empty with { ipEsp = lista.Select(x => x.ipEsp).First(), MAC = esp.mac, NomeEspClient = esp.nomeEsp, abilitazione = lista.Select(x => x.abilitazione).First() });
            return Ok();
        }
        [HttpPut("abilitazione")]
        [Authorize]
        public async Task<IActionResult> abilitazione(Abilitazione abilitazione)
        {

            var f = await m_registroEsp.dammiListaEsp();
            var lista = f.Where(x => x.Key == abilitazione.mac).Select(x => x.Value).ToList();
            await m_registroEsp.ModificareProgrammaEsp8266(Receiver_esp.Empty with { abilitazione = abilitazione.abilitazione, MAC = abilitazione.mac, NomeEspClient = lista.Select(x => x.NomeEspClient).First(), ipEsp = lista.Select(x => x.ipEsp).First() });
            return Ok();
        }
    }
}
