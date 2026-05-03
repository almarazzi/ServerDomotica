using Microsoft.AspNetCore.SignalR;


namespace provaweb
{
    public class relaySwitchHub : Hub
    {


        private readonly ILogger<RelaySwitch> m_logger;
        private readonly MemoriaStato m_memoriaStati;
        private readonly ProgrammaSettimanale m_programmaSettimanale;
        private readonly ContorolloEspOnline m_stateRelayGet;
        private readonly ProgrmmaModificaStatoRelay m_progrmmaModificaStatoRelay;

        private readonly IHubContext<relaySwitchHub> _hub;

        public relaySwitchHub(ILogger<RelaySwitch> logger, MemoriaStato memoriaStato, ProgrammaSettimanale programmaSettimanale, ContorolloEspOnline stateRelayGet, ProgrmmaModificaStatoRelay progrmmaModificaStatoRelay, IHubContext<relaySwitchHub> hubContext)
        {
            m_logger = logger;
            m_memoriaStati = memoriaStato;
            m_programmaSettimanale = programmaSettimanale;
            m_stateRelayGet = stateRelayGet;
            m_progrmmaModificaStatoRelay = progrmmaModificaStatoRelay;
            _hub = hubContext;
            m_memoriaStati.Evento += async () => await GetState();

        }
        public record StateProgrammManu(bool stateProgrammManu, string macricever);
        public record StateProgrammAuto(bool stateProgrammAuto, string macricever);
        public record SetState1(bool state, string macricever);
        public record Oggreturn(bool state, string macricever);


        public record setData(string inizio, string fine, DayOfWeek day, string mac);

        public override async Task OnConnectedAsync()
        {
            // Esegui operazioni quando il client si connette
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Gestisci la disconnessione
            await base.OnDisconnectedAsync(exception);
        }
        public async Task GetState()
        {
            if (m_memoriaStati == null)
            {
                throw new InvalidOperationException("MemoriaStato non è stato iniettato correttamente.");
            }
            var y = await m_memoriaStati.DammiStati();
            var s = y.Select(x => new Oggreturn(x.Value.StateRelay, x.Key));
            if (Clients.All != null)
                await Clients.All.SendAsync("CambioStatoRealy", s);
        }






    }
}
