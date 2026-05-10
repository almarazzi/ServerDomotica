using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


namespace provaweb
{
    public class relaySwitchHub : Hub
    {
        private readonly MemoriaStato m_memoriaStati;
        private readonly ProgrammaSettimanale m_programmaSettimanale;
        private readonly ContorolloEspOnline m_stateRelayGet;
        private readonly ProgrmmaModificaStatoRelay m_progrmmaModificaStatoRelay;
        private readonly RegistroEsp m_RegistroEsp;

        public relaySwitchHub(RegistroEsp registroEsp, MemoriaStato memoriaStato, ProgrammaSettimanale programmaSettimanale, ContorolloEspOnline stateRelayGet, ProgrmmaModificaStatoRelay progrmmaModificaStatoRelay)
        {
            m_RegistroEsp = registroEsp;
            m_memoriaStati = memoriaStato;
            m_programmaSettimanale = programmaSettimanale;
            m_stateRelayGet = stateRelayGet;
            m_progrmmaModificaStatoRelay = progrmmaModificaStatoRelay;

        }
        public record Oggreturn(bool state, string macricever);

        [Authorize]
        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();

            var y = await m_memoriaStati.DammiStati();
            var s = y.Select(x => new Oggreturn(x.Value.StateRelay, x.Key));
            await Clients.Caller.SendAsync("CambioStatoRealy", s);

            var f = await m_RegistroEsp.dammiListaEsp();
            var g = m_stateRelayGet.Offline;
            var l = f.OrderBy(x => g.Contains(x.Key)).ToList();
            await Clients.Caller.SendAsync("ListaEsp", l);

            await Clients.Caller.SendAsync("GetProgmmaManu", y.Select(x => new Oggreturn(x.Value.StateProgrammManu, x.Key)));

            await Clients.Caller.SendAsync("GetProgmmaAuto", y.Select(x => new Oggreturn(x.Value.StateProgrammAuto, x.Key)));


    
            var y3 = await m_programmaSettimanale.DammiProgrammaSettimanale();
            await Clients.Caller.SendAsync("ProgrammaSettimanale",y3.ToList());
  


        }

    }

}
