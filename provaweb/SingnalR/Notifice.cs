using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using provaDatabase;
using static provaweb.Controllers.Login;

namespace provaweb
{
    public class Notifice
    {

        private readonly MemoriaStato m_memoriaStati;
        private readonly ProgrammaSettimanale m_programmaSettimanale;
        private readonly ContorolloEspOnline m_StatoRelayOff;
        private readonly ProgrmmaModificaStatoRelay m_progrmmaModificaStatoRelay;
        private readonly RegistroEsp m_RegistroEsp;
        private readonly ActiveUsersService _activeUser;
        private readonly IServiceProvider _provaider;

        private readonly IHubContext<relaySwitchHub> _hub;

        public Notifice(IServiceProvider provider, RegistroEsp registroEsp, MemoriaStato memoriaStato, ProgrammaSettimanale programmaSettimanale, ContorolloEspOnline SatoRelayOff, ProgrmmaModificaStatoRelay progrmmaModificaStatoRelay, IHubContext<relaySwitchHub> hubContext, ActiveUsersService activeUsersService)
        {
            m_memoriaStati = memoriaStato;
            m_RegistroEsp = registroEsp;
            m_programmaSettimanale = programmaSettimanale;
            m_StatoRelayOff = SatoRelayOff;
            m_progrmmaModificaStatoRelay = progrmmaModificaStatoRelay;
            _hub = hubContext;
            _activeUser = activeUsersService;
            _provaider = provider;
        }
        public record Oggreturn(bool state, string macricever);


        public record setData(string inizio, string fine, DayOfWeek day, string mac);
        [Authorize]
        public async Task GetState()
        {

            var y = await m_memoriaStati.DammiStati();
            var s = y.Select(x => new Oggreturn(x.Value.StateRelay, x.Key));
            await _hub.Clients.All.SendAsync("CambioStatoRealy", s);
            await _hub.Clients.All.SendAsync("GetProgmmaAuto", y.Select(x => new Oggreturn(x.Value.StateProgrammAuto, x.Key)));
            await _hub.Clients.All.SendAsync("GetProgmmaManu", y.Select(x => new Oggreturn(x.Value.StateProgrammManu, x.Key)));
        }
        [Authorize]
        public async Task StatorelyOff()
        {
            var f = m_StatoRelayOff.Offline;
            await _hub.Clients.All.SendAsync("StatoRelayOff", f);
        }
        [Authorize]
        public async Task ListaEsp()
        {
            var f = await m_RegistroEsp.dammiListaEsp();
            var g = m_StatoRelayOff.Offline;
            var l = f.OrderBy(x => g.Contains(x.Key)).ToList();
            await _hub.Clients.All.SendAsync("ListaEsp", l);
        }
        [Authorize(Roles = "Admin")]
        public async Task Getlistuser()
        {
            using var scope = _provaider.CreateScope();
            var dblogic = scope.ServiceProvider.GetRequiredService<Databaselogic>();
            var u = await dblogic.UtentiDatabase1();
            var hh = u.Select(x => _activeUser.IsActive(x.UserName));
            var l = u.Select(x => new UserDatabase(x.UserName, x.Ruolo, _activeUser.IsActive(x.UserName), x.StatoAccount));
            await _hub.Clients.All.SendAsync("Getlistuser", l);
        }
        
        
        [Authorize]
        public async Task GetWeekProgram()
        {
            var y = await m_programmaSettimanale.DammiProgrammaSettimanale();
            await _hub.Clients.All.SendAsync("ProgrammaSettimanale",y.ToList());
        }
    }
}
