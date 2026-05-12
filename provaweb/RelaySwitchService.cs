using DotNext.Threading;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;


namespace provaweb
{

    public static class RelaySwitchServiceExtension
    {
        public static IServiceCollection AddRelaySwitch(this IServiceCollection services)
        {
            services.AddSingleton<ProgrmmaModificaStatoRelay>();
            // services.AddSingleton<IRelaySwitchService>(s => s.GetRequiredService<ProgrmmaModificaStatoRelay>());
            services.AddHostedService(s => s.GetRequiredService<ProgrmmaModificaStatoRelay>());
            services.AddSingleton<MemoriaStato>();
            services.AddHttpClient("ESPClient");
            services.AddSingleton<ProgrammaSettimanale>();
            services.AddSingleton<ContorolloEspOnline>();
            services.AddHostedService(s => s.GetRequiredService<ContorolloEspOnline>());
            return services;
        }
    }

    public record ProgrammaGiornaliero(DayOfWeek Day, TimeOnly OraInizio, TimeOnly OraFine)
    {
        public static readonly ProgrammaGiornaliero Empty = new(DayOfWeek.Sunday, TimeOnly.MinValue, TimeOnly.MinValue);
    }
    public class ProgrammaSettimanale
    {
        private readonly AsyncLazy<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>> m_Programma;
        private readonly static string s_percorso = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "provaweb/ProgrammaSettimanale.txt");
        private readonly AsyncLock _lock = new();
        private static RegistroEsp? m_programmaDizionarioEsp8266;
        public event Func<Task>? Evento;

        public void SetNotifice(Notifice notifice)
        {
            Evento += notifice.GetWeekProgram;
        }

        public ProgrammaSettimanale(RegistroEsp registroEsp)
        {
            m_Programma = new AsyncLazy<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>>(async (_) => await Create(s_percorso));
            m_programmaDizionarioEsp8266 = registroEsp;
        }
        private static async Task<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>> Create(string percorso)
        {

            if (File.Exists(percorso) && new FileInfo(percorso).Length != 0)
            {
                using (new FileStream(percorso, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                {
                    var leggi = await File.ReadAllTextAsync(percorso);
                    var programmaModifica = JsonSerializer.Deserialize<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>>(leggi)!;
                    return programmaModifica;
                }
            }
            else
            {
                var f = new ConcurrentDictionary<string, List<ProgrammaGiornaliero>>();
                var g = await m_programmaDizionarioEsp8266!.dammiListaEsp();
                do
                {
                    g = await m_programmaDizionarioEsp8266.dammiListaEsp();
                } while (g.Count == 0);

                foreach (var item in g.Select(x => x.Key))
                {
                    var l = new List<ProgrammaGiornaliero>(7);
                    for (var i = DayOfWeek.Sunday; i <= DayOfWeek.Saturday; i++)
                    {
                        l.Add(ProgrammaGiornaliero.Empty with { Day = i });

                    }
                    f[item] = l;
                }

                return f;
            }
        }

        private async Task Salva()
        {

            var stati = await m_Programma.WithCancellation(CancellationToken.None);
            using (var fs = new FileStream(s_percorso, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.SetLength(0);
                await JsonSerializer.SerializeAsync(fs, stati);
            }
            if (Evento is not null)
                await Evento.Invoke();
        }


        public async Task<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>> DammiProgrammaSettimanale()
        {
            var stati = await m_Programma.WithCancellation(CancellationToken.None);
            return stati;
        }

        public async Task SetProgrammaGiornaliero(ProgrammaGiornaliero pg, string mac)
        {
            int day = (int)pg.Day;
            using (await _lock.AcquireAsync(CancellationToken.None))
            {
                if (m_Programma == null)
                {
                    return;
                }
                else
                {
                    var programma = await m_Programma.WithCancellation(CancellationToken.None);
                    programma[mac][day] = pg;
                    await Salva();

                }
            }

        }

    }

    public record Stati(bool StateRelay, bool StateProgrammAuto, bool StateProgrammManu)
    {
        public static readonly Stati Empty = new(false, false, false);
    }
    public class MemoriaStato
    {

        private readonly AsyncLazy<ConcurrentDictionary<string, Stati>> m_Stati;
        private readonly static string s_percorso = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "provaweb/SetSati.txt");
        private readonly AsyncLock _lock = new();
        private static RegistroEsp? m_dizionarioEsp8266;
        public event Func<Task>? Evento;

        public void SetNotifice(Notifice notifice)
        {
            Evento += notifice.GetState;
        }
        public MemoriaStato(RegistroEsp registroEsp)
        {
            m_Stati = new AsyncLazy<ConcurrentDictionary<string, Stati>>(async (_) => await Create(s_percorso));
            m_dizionarioEsp8266 = registroEsp;
        }
        private static async Task<ConcurrentDictionary<string, Stati>> Create(string percorso)
        {
            if (File.Exists(percorso) && new FileInfo(percorso).Length != 0)
            {
                using (new FileStream(percorso, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                {
                    var leggi = await File.ReadAllTextAsync(percorso);
                    var leggiStato = JsonSerializer.Deserialize<ConcurrentDictionary<string, Stati>>(leggi)!;
                    return leggiStato;
                }

            }
            else
            {
                var r = new ConcurrentDictionary<string, Stati>();
                var g = await m_dizionarioEsp8266!.dammiListaEsp();
                do
                {
                    g = await m_dizionarioEsp8266.dammiListaEsp();
                } while (g.Count == 0);
                foreach (var item in g.Select(x => x.Key))
                {
                    r[item] = Stati.Empty;
                }
                return r;
            }

        }

        public async Task Salva()
        {

            using (await _lock.AcquireAsync(CancellationToken.None))
            {
                var stati = await m_Stati.WithCancellation(CancellationToken.None);
                using (var fs = new FileStream(s_percorso, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.SetLength(0);
                    await JsonSerializer.SerializeAsync(fs, stati);
                  
                }
            }
            if (Evento is not null)
                await Evento.Invoke();

        }
        public async Task<ConcurrentDictionary<string, Stati>> DammiStati()
        {

            var stati = await m_Stati.WithCancellation(CancellationToken.None);
            return stati;
        }


        public async Task Modifica(Stati y, string mac)
        {

            using (await _lock.AcquireAsync(CancellationToken.None))
            {
                if (m_Stati == null)
                {
                    return;
                }
                else
                {
                    var stati = await m_Stati.WithCancellation(CancellationToken.None);
                    stati[mac] = y;
                    await Salva();
                }
            }

        }
    }

    public class ProgrmmaModificaStatoRelay : BackgroundService
    {
        private readonly object _lock = new();
        private bool M_StateRelay = false;
        private string M_mac = string.Empty;
        public bool StateRelay
        {
            get { lock (_lock) { return M_StateRelay; } }
            set { lock (_lock) { M_StateRelay = value; } }
        }
        public string mac
        {
            get { lock (_lock) { return M_mac; } }
            set { lock (_lock) { M_mac = value; } }
        }
        private readonly MemoriaStato m_memoriaStati;
        private readonly ProgrammaSettimanale m_programmaSettimanale;
        private readonly RegistroEsp m_programmaDizionarioEsp8266;
        private readonly TimeProvider m_timeProvider;
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly ILogger<ProgrmmaModificaStatoRelay> m_logger;
        private readonly ContorolloEspOnline m_stateRelayGet;

        private record State(bool stateRlay);
        public ProgrmmaModificaStatoRelay(TimeProvider t, IHttpClientFactory clientFactory, ILogger<ProgrmmaModificaStatoRelay> logger, RegistroEsp registro, MemoriaStato memoriaStato, ProgrammaSettimanale programmaSettimanale, ContorolloEspOnline stateRelay)
        {
            m_timeProvider = t;
            HttpClientFactory = clientFactory;
            m_logger = logger;
            m_programmaDizionarioEsp8266 = registro;
            m_memoriaStati = memoriaStato;
            m_programmaSettimanale = programmaSettimanale;
            m_stateRelayGet = stateRelay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                if (string.IsNullOrEmpty(M_mac))
                {
                    await m_timeProvider.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }
                var Ms = await m_memoriaStati.DammiStati();
                var Re = await m_programmaSettimanale.DammiProgrammaSettimanale();
                if (Ms.Count == 0 || Re.Count == 0)
                {
                    await m_timeProvider.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }
                var d = m_timeProvider.GetLocalNow().DayOfWeek;
                ProgrammaGiornaliero pg;


                var ip = await m_programmaDizionarioEsp8266.dammiListaEsp();

                pg = Re[M_mac][(int)d];
                using var http = HttpClientFactory.CreateClient("ESPClient");
                http.BaseAddress = new Uri("http://" + ip[M_mac].ipEsp);
                if (ip[M_mac].abilitazione)
                {

                    if (Ms[M_mac].StateProgrammAuto == true && Ms[M_mac].StateProgrammManu == false)
                    {
                        var t = TimeOnly.FromDateTime(m_timeProvider.GetLocalNow().LocalDateTime);
                        M_StateRelay = t.IsBetween(pg.OraInizio, pg.OraFine);

                    }
                    else if (Ms[M_mac].StateProgrammAuto == false && Ms[M_mac].StateProgrammManu == true)
                    {

                    }
                    else
                    {
                        M_StateRelay = false;
                    }
                }
                else
                {
                    M_StateRelay = false;
                }
                if (m_stateRelayGet.Offline.Contains(M_mac))
                {
                    await m_memoriaStati.Modifica(Ms[M_mac] with { StateRelay = false }, M_mac);
                    continue;
                }

                if (M_StateRelay != Ms[M_mac].StateRelay)
                {
                    await m_memoriaStati.Modifica(Ms[M_mac] with { StateRelay = M_StateRelay }, M_mac);
                    var jsonContent = new StringContent(JsonSerializer.Serialize(new State(M_StateRelay)));
                    try
                    {
                        using var invio = await http.PutAsync("/api/RelaySwitch/StateRelay", jsonContent, stoppingToken);
                    }
                    catch (HttpRequestException ex)
                    {
                        m_logger.LogWarning(ex.Message);
                    }
                }
                // await m_timeProvider.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    public class ContorolloEspOnline(IHttpClientFactory httpClient, TimeProvider timeProvider, ILogger<ContorolloEspOnline> logger, RegistroEsp registroEsp) : BackgroundService
    {
        private readonly Dictionary<string, long> m_EspOffline = new();
        public IReadOnlyList<string> Offline = Array.Empty<string>();
        public event Func<Task>? Evento;

        public void SetNotifice(Notifice notifice)
        {
            Evento += notifice.StatorelyOff;
        }


        private async Task<(string key, bool result)> TestItem(string key, string ipEsp, CancellationToken stoppingToken)
        {
            using var http = httpClient.CreateClient("ESPClient");
            http.BaseAddress = new Uri("http://" + ipEsp);
            http.Timeout = TimeSpan.FromSeconds(2);
            try
            {
                using var p = await http.GetAsync("/api/RelaySwitch/ping", stoppingToken);
                return (key, true);

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);

            }
            return (key, false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var resultTasks = new List<Task<(string key, bool result)>>();
                foreach (var item in await registroEsp.dammiListaEsp())
                {
                    if (m_EspOffline.TryGetValue(item.Key, out long value))
                    {
                        if (timeProvider.GetElapsedTime(value).TotalSeconds < 5)
                        {
                            continue;
                        }
                    }
                    resultTasks.Add(TestItem(item.Key, item.Value.ipEsp, stoppingToken));
                }

                var tNow = timeProvider.GetTimestamp();
                await Task.WhenAll(resultTasks);

                foreach (var r in resultTasks)
                {
                    if (r.Result.result)
                        m_EspOffline.Remove(r.Result.key, out long _);
                    else
                        m_EspOffline[r.Result.key] = tNow;
                }

                Offline = m_EspOffline.Keys.ToImmutableArray();
                if (Evento is not null)
                    await Evento.Invoke();
                await timeProvider.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

    }

}