using DotNext.Threading;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace provaweb
{

    public static class receiver_espServiceExtension
    {
        public static IServiceCollection Addreceiver_esp8266Service(this IServiceCollection services)
        {
            services.AddHostedService<receiver_espService_Brocast>();
            services.AddSingleton<RegistroEsp>();
            return services;
        }
    }

    public class receiver_espService_Brocast : BackgroundService
    {

        private readonly UdpClient m_UDPClient = new();
        private readonly int Port = 8888;
        private readonly TimeProvider m_timeProvider;
        private readonly RegistroEsp m_registroEsp;

        public receiver_espService_Brocast(TimeProvider time, RegistroEsp registro)
        {
            m_timeProvider = time;
            m_registroEsp = registro;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            m_UDPClient.Client.Bind(new IPEndPoint(IPAddress.Any, Port));
            while (!stoppingToken.IsCancellationRequested)
            {
                var receive = await m_UDPClient.ReceiveAsync(stoppingToken);
                var Mac = Encoding.ASCII.GetString(receive.Buffer);
                //m_UDPClient.Close();
                var f = await m_registroEsp.dammiListaEsp();
                var nome = f.Where(x => x.Key == Mac).Select(x => x.Value.NomeEspClient).FirstOrDefault();
                var abi = f.Where(x => x.Key == Mac).Select(x => x.Value.abilitazione).FirstOrDefault();
                await m_registroEsp.ModificareProgrammaEsp8266(Receiver_esp.Empty with { MAC = Mac, ipEsp = receive.RemoteEndPoint.Address.ToString(), NomeEspClient = (nome != null ? nome : "esp"), abilitazione = abi });

            }


        }
    }

    public record Value_Esp8266(string NomeEspClient, string ipEsp, bool abilitazione)
    {
        public static readonly Value_Esp8266 Empty = new("", "", false);
    }
    public record Receiver_esp(string MAC, string NomeEspClient, string ipEsp, bool abilitazione)
    {
        public static readonly Receiver_esp Empty = new("", "", "", false);
    }
    public record Ip(string ip, bool Abilitazione, string mac);

    public class RegistroEsp
    {

        private readonly AsyncLazy<ConcurrentDictionary<string, Value_Esp8266>> m_ProgrammaEsp8266;
        private readonly static string s_percorso = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "provaweb/ProgrammaDizionarioEsp8266.txt");
        private readonly object _lock = new AsyncExclusiveLock();

        public RegistroEsp()
        {
            m_ProgrammaEsp8266 = new AsyncLazy<ConcurrentDictionary<string, Value_Esp8266>>(async (_) => await LeggereProgrammaEsp8266(s_percorso));
        }

        private static async Task<ConcurrentDictionary<string, Value_Esp8266>> LeggereProgrammaEsp8266(string percorso)
        {

            if (File.Exists(percorso) && new FileInfo(percorso).Length != 0)
            {

                var leggi = await File.ReadAllTextAsync(percorso);
                var ProgrammaDizionarioEsp8266Deserializato = JsonSerializer.Deserialize<ConcurrentDictionary<string, Value_Esp8266>>(leggi)!;
                return ProgrammaDizionarioEsp8266Deserializato;

            }
            return new ConcurrentDictionary<string, Value_Esp8266>();
        }
        public async Task<ConcurrentDictionary<string, Value_Esp8266>> dammiListaEsp()
        {

            var f = await m_ProgrammaEsp8266.WithCancellation(CancellationToken.None);
            return f;

        }
        public async Task<List<Ip>> IP()
        {
            var d = await m_ProgrammaEsp8266.WithCancellation(CancellationToken.None);
            List<Ip> f = new List<Ip>();
            foreach (var item in d)
            {
                f.Add(new Ip(ip: "http://" + item.Value.ipEsp, Abilitazione: item.Value.abilitazione, mac: item.Key));
            }
            return f;
        }

        private async Task SalvareProgrammaEsp8266()
        {
            var registro = await m_ProgrammaEsp8266.WithCancellation(CancellationToken.None);
            var ProgrammaDizionarioEsp8266Serializato = JsonSerializer.Serialize(registro);
            if (ProgrammaDizionarioEsp8266Serializato != null)
            {
                await File.WriteAllTextAsync(s_percorso, ProgrammaDizionarioEsp8266Serializato);
            }
        }
        public async Task ModificareProgrammaEsp8266(Receiver_esp re)
        {
            using (await _lock.AcquireLockAsync(CancellationToken.None))
            {
                if (m_ProgrammaEsp8266 == null)
                {
                    return;
                }
                else
                {
                    var f = await m_ProgrammaEsp8266.WithCancellation(CancellationToken.None);
                    f[re.MAC] = new Value_Esp8266(NomeEspClient: re.NomeEspClient, ipEsp: re.ipEsp, abilitazione: re.abilitazione);
                    await SalvareProgrammaEsp8266();

                }
            }

        }

    }


}

