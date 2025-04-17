using DotNext.Threading;
using System.Collections.Concurrent;
using System.Text.Json;


namespace provaweb
{

    public static class RelaySwitchServiceExtension
    {
        public static IServiceCollection AddRelaySwitch(this IServiceCollection services)
        {
            services.AddSingleton<ProgrmmaModificaStatoRelay>();
            services.AddSingleton<IRelaySwitchService>(s => s.GetRequiredService<ProgrmmaModificaStatoRelay>());
            services.AddHostedService(s => s.GetRequiredService<ProgrmmaModificaStatoRelay>());
            services.AddSingleton<MemoriaStato>();
            services.AddHttpClient("ESPClient");
            services.AddSingleton<ProgrammaSettimanale>();
            services.AddSingleton<GetStateRelay>();
            services.AddHostedService(s => s.GetRequiredService<GetStateRelay>());
            return services;
        }
    }

    public interface IRelaySwitchService
    {
        bool StateRelay { get; set; }
        string mac { get; set; }
    }
    public record ProgrammaGiornaliero(DayOfWeek Day, TimeOnly OraInizio, TimeOnly OraFine)
    {
        public static readonly ProgrammaGiornaliero Empty = new(DayOfWeek.Sunday, TimeOnly.MinValue, TimeOnly.MinValue);
    }
    public class ProgrammaSettimanale
    {
        private readonly AsyncLazy<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>> m_Programma;
        private readonly static string s_percorso = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "provaweb/ProgrammaSettimanale.txt");
        private readonly object _lock = new AsyncExclusiveLock();
        private static RegistroEsp? m_programmaDizionarioEsp8266;

        public ProgrammaSettimanale(RegistroEsp registroEsp)
        {
            m_Programma = new AsyncLazy<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>>(async (_) => await Create(s_percorso));
            m_programmaDizionarioEsp8266 = registroEsp;
        }
        private static async Task<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>> Create(string percorso)
        {

            if (File.Exists(percorso) && new FileInfo(percorso).Length != 0)
            {
                var leggi = await File.ReadAllTextAsync(percorso);
                var programmaModifica = JsonSerializer.Deserialize<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>>(leggi)!;
                return programmaModifica;
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
            var programma = await m_Programma.WithCancellation(CancellationToken.None);
            var json = JsonSerializer.Serialize(programma);
            if (json != null)
            {
                await File.WriteAllTextAsync(s_percorso, json);
            }
        }

        public async Task<ConcurrentDictionary<string, List<ProgrammaGiornaliero>>> DammiProgrammaSettimanale()
        {
            var stati = await m_Programma.WithCancellation(CancellationToken.None);
            return stati;
        }

        public async Task SetProgrammaGiornaliero(ProgrammaGiornaliero pg, string mac)
        {
            int day = (int)pg.Day;
            using (await _lock.AcquireLockAsync(CancellationToken.None))
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
        private readonly object _lock = new AsyncExclusiveLock();
        private static RegistroEsp? m_dizionarioEsp8266;

        public MemoriaStato(RegistroEsp registroEsp)
        {
            m_Stati = new AsyncLazy<ConcurrentDictionary<string, Stati>>(async (_) => await Create(s_percorso));
            m_dizionarioEsp8266 = registroEsp;
        }
        private static async Task<ConcurrentDictionary<string, Stati>> Create(string percorso)
        {
            if (File.Exists(percorso) && new FileInfo(percorso).Length != 0)
            {
                var leggi = await File.ReadAllTextAsync(percorso);
                var leggiStato = JsonSerializer.Deserialize<ConcurrentDictionary<string, Stati>>(leggi)!;
                return leggiStato;
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
            var stati = await m_Stati.WithCancellation(CancellationToken.None);
            var json = JsonSerializer.Serialize(stati);
            if (json != null)
            {
                await File.WriteAllTextAsync(s_percorso, json);
            }

        }
        public async Task<ConcurrentDictionary<string, Stati>> DammiStati()
        {
            var stati = await m_Stati.WithCancellation(CancellationToken.None);
            return stati;
        }


        public async Task Modifica(Stati y, string mac)
        {

            using (await _lock.AcquireLockAsync(CancellationToken.None))
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

    public class ProgrmmaModificaStatoRelay : BackgroundService, IRelaySwitchService
    {
        private bool m_StateRelay { get; set; }
        public bool StateRelay { get; set; }
        public string mac { get; set; } = " ";
        private readonly MemoriaStato m_memoriaStati;
        private readonly ProgrammaSettimanale m_programmaSettimanale;
        private readonly RegistroEsp m_programmaDizionarioEsp8266;
        private readonly TimeProvider m_timeProvider;
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly ILogger<ProgrmmaModificaStatoRelay> m_logger;
        private record State(bool stateRlay);
        public ProgrmmaModificaStatoRelay(TimeProvider t, IHttpClientFactory clientFactory, ILogger<ProgrmmaModificaStatoRelay> logger, RegistroEsp registro, MemoriaStato memoriaStato, ProgrammaSettimanale programmaSettimanale)
        {
            m_timeProvider = t;
            HttpClientFactory = clientFactory;
            m_logger = logger;
            m_programmaDizionarioEsp8266 = registro;
            m_memoriaStati = memoriaStato;
            m_programmaSettimanale = programmaSettimanale;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                var Ms = await m_memoriaStati.DammiStati();
                var Re = await m_programmaSettimanale.DammiProgrammaSettimanale();
                if (Ms.Count == 0 || Re.Count == 0)
                {
                    await Task.Delay(100, stoppingToken);
                    break;
                }
                var d = m_timeProvider.GetLocalNow().DayOfWeek;
                ProgrammaGiornaliero pg;


                var ip = await m_programmaDizionarioEsp8266.dammiListaEsp();


                foreach (var item in ip)
                {
                    pg = Re[item.Key][(int)d];
                    var http = HttpClientFactory.CreateClient("ESPClient");
                    if (item.Value.abilitazione)
                    {
                        if (Ms[item.Key].StateProgrammAuto == true && Ms[item.Key].StateProgrammManu == false)
                        {
                            var t = TimeOnly.FromDateTime(m_timeProvider.GetLocalNow().LocalDateTime);
                            m_StateRelay = t.IsBetween(pg.OraInizio, pg.OraFine);

                        }
                        else if (Ms[item.Key].StateProgrammAuto == false && Ms[item.Key].StateProgrammManu == true)
                        {
                            if (mac == item.Key)
                            {
                                m_StateRelay = StateRelay;
                            }

                        }
                        else
                        {
                            m_StateRelay = false;
                        }
                    }
                    else
                    {
                        m_StateRelay = false;
                    }

                    http.BaseAddress = new Uri("http://" + item.Value.ipEsp);
                    if (m_StateRelay != Ms[item.Key].StateRelay)
                    {
                        await m_memoriaStati.Modifica(Ms[item.Key] with { StateRelay = m_StateRelay }, item.Key);
                        var jsonContent = new StringContent(JsonSerializer.Serialize(new State(m_StateRelay)));
                        try
                        {
                            using var invio = await http.PutAsync("/api/RelaySwitch/StateRelay", jsonContent, stoppingToken);
                            invio.EnsureSuccessStatusCode();

                        }
                        catch (HttpRequestException ex)
                        {
                            m_logger.LogWarning(ex.Message);
                        }
                    }

                }
            }
        }
    }
    public record State(bool stateRlay);
    public class GetStateRelay(IHttpClientFactory httpClient, TimeProvider timeProvider, ILogger<GetStateRelay> logger, RegistroEsp registroEsp, MemoriaStato memoriaStato) : BackgroundService
    {
        private readonly IHttpClientFactory m_httpClientFactory = httpClient;
        private readonly TimeProvider m_timeProvider = timeProvider;
        private readonly ILogger<GetStateRelay> m_logger = logger;
        private readonly RegistroEsp m_registroEsp = registroEsp;
        private readonly MemoriaStato m_memoriaStati = memoriaStato;
        public bool Isonline;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var http = m_httpClientFactory.CreateClient("ESPClient");
                var f = await m_memoriaStati.DammiStati();
                foreach (var item in await m_registroEsp.dammiListaEsp())
                {
                    http.BaseAddress = new Uri("http://" + item.Value.ipEsp);
                    try
                    {

                        using var t = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        t.CancelAfter(TimeSpan.FromSeconds(5));
                        using var p = await http.GetAsync("/api/RelaySwitch/GetStateRelay", t.Token);
                        Isonline = p.IsSuccessStatusCode;
                        if (Isonline)
                        {
                            p.EnsureSuccessStatusCode();
                            var leggi = await p.Content.ReadAsStringAsync();
                            var g = JsonSerializer.Deserialize<State>(leggi)!.stateRlay;
                            await m_memoriaStati.Modifica(f[item.Key] with { StateRelay = g }, item.Key);
                        }
                        else
                        {

                        }



                    }
                    catch (HttpRequestException ex)
                    {
                        m_logger.LogWarning(ex.Message);

                    }
                    catch (TaskCanceledException ex)
                    {
                        m_logger.LogWarning(ex.Message);
                    }
                }
                await m_timeProvider.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

    }
}

