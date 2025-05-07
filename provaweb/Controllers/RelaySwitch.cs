using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace provaweb
{
    [Route("api/[controller]")]
    [ApiController]

    public class RelaySwitch : ControllerBase
    {
        private readonly ILogger<RelaySwitch> m_logger;
        private readonly MemoriaStato m_memoriaStati;
        private readonly ProgrammaSettimanale m_programmaSettimanale;
        private readonly ContorolloEspOnline m_stateRelayGet;
        private readonly IRelaySwitchService m_relaySwitchService;
        public RelaySwitch(ILogger<RelaySwitch> logger, MemoriaStato memoriaStato, ProgrammaSettimanale programmaSettimanale, IRelaySwitchService relaySwitchService, ContorolloEspOnline stateRelayGet)
        {
            m_logger = logger;
            m_memoriaStati = memoriaStato;
            m_programmaSettimanale = programmaSettimanale;
            m_relaySwitchService = relaySwitchService;
            m_stateRelayGet = stateRelayGet;
        }
        public record StateProgrammManu(bool stateProgrammManu, string macricever);
        public record StateProgrammAuto(bool stateProgrammAuto, string macricever);
        public record SetState1(bool state, string macricever);
        public record Oggreturn(bool state, string macricever);


        public record setData(string inizio, string fine, DayOfWeek day, string mac);

        [HttpPut("SetState")]
        [Authorize]
        public async Task<IActionResult> SetState(SetState1 setState)
        {

            var y = await m_memoriaStati.DammiStati();
            var f = y.Where(x => x.Key == setState.macricever).Select(x => x.Value.StateProgrammManu).First();
            if (f == true && !m_stateRelayGet.Offline.Contains(setState.macricever))
            {
                m_relaySwitchService.StateRelay = setState.state;
                m_relaySwitchService.mac = setState.macricever;
            }
            return Ok();
        }
        [HttpGet("GetState")]
        [Authorize]

        public async Task<IActionResult> GetState()
        {
            var y = await m_memoriaStati.DammiStati();
            return Ok(y.Select(x => new Oggreturn(x.Value.StateRelay, x.Key)));

        }

        [HttpPut("stateProgrammManu")]
        [Authorize]

        public async Task<IActionResult> stateProgrammManu(StateProgrammManu StateProgrammManu)
        {
            var y = await m_memoriaStati.DammiStati();
            await m_memoriaStati.Modifica(y[StateProgrammManu.macricever] with { StateProgrammManu = StateProgrammManu.stateProgrammManu }, StateProgrammManu.macricever);
            return Ok();
        }
        [HttpGet("GetProgrammManu")]
        [Authorize]

        public async Task<IActionResult> GetProgrammManu()
        {
            var y = await m_memoriaStati.DammiStati();
            return Ok(y.Select(x => new Oggreturn(x.Value.StateProgrammManu, x.Key)));
        }

        [HttpPut("stateProgrammAuto")]
        [Authorize]

        public async Task<IActionResult> stateProgrammauto(StateProgrammAuto StateProgrammAuto)
        {
            var y = await m_memoriaStati.DammiStati();
            await m_memoriaStati.Modifica(y[StateProgrammAuto.macricever] with { StateProgrammAuto = StateProgrammAuto.stateProgrammAuto }, StateProgrammAuto.macricever);
            return Ok();
        }
        [HttpGet("GetProgrammAuto")]
        [Authorize]

        public async Task<IActionResult> GetProgrammAuto()
        {
            var y = await m_memoriaStati.DammiStati();
            return Ok(y.Select(x => new Oggreturn(x.Value.StateProgrammAuto, x.Key)));
        }

        [HttpPut("SetData")]
        [Authorize]

        public async Task<IActionResult> SetData(setData setData)
        {
            DayOfWeek day = setData.day;
            TimeOnly inizio = TimeOnly.MinValue;
            TimeOnly fine = TimeOnly.MinValue;
            if (setData.inizio != "")
            {
                inizio = TimeOnly.Parse(setData.inizio);
            }

            if (setData.fine != "")
            {
                fine = TimeOnly.Parse(setData.fine);
            }
            await m_programmaSettimanale.SetProgrammaGiornaliero(ProgrammaGiornaliero.Empty with { Day = day, OraInizio = inizio, OraFine = fine }, setData.mac);
            return Ok(await m_programmaSettimanale.DammiProgrammaSettimanale());
        }


        [HttpGet("GetWeekProgram")]
        [Authorize]

        public async Task<IActionResult> GetWeekProgram()
        {
            var y = await m_programmaSettimanale.DammiProgrammaSettimanale();
            return Ok(y.ToList());
        }
    }

}

