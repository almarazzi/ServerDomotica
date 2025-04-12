using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using provaweb;
using SoloX.CodeQuality.Test.Helpers.Http;




namespace testActiveUsersService
{
    public class ActiveUsersService
    {
        readonly FakeTimeProvider m_fakeTimeProvider;
        readonly provaweb.ActiveUsersService m_activeUsersService;
        readonly provaweb.MemoriaStato m_memoria;
        readonly provaweb.RegistroEsp m_registroEsp;
        readonly provaweb.ProgrammaSettimanale m_programmaSettimanale;
        public ActiveUsersService()
        {
            m_fakeTimeProvider = new FakeTimeProvider();
            m_activeUsersService = new provaweb.ActiveUsersService(NullLogger<provaweb.ActiveUsersService>.Instance, m_fakeTimeProvider);
            m_memoria = new MemoriaStato();
            m_programmaSettimanale = new ProgrammaSettimanale();
            m_registroEsp =new RegistroEsp();


        }



        [Fact]
        public void IsActiveTimeoutAfter5min()
        {
            m_fakeTimeProvider.SetUtcNow(DateTime.UtcNow);
            m_activeUsersService.SetLastVisit("al");
            m_fakeTimeProvider.Advance(TimeSpan.FromSeconds(1));
            Assert.True(m_activeUsersService.IsActive("al"));
            m_fakeTimeProvider.Advance(TimeSpan.FromSeconds(4));
            Assert.False(m_activeUsersService.IsActive("al"));
        }

        [Fact]
        public async Task ProgrmmaModificaStatoRelay()
        {
            var CllientFactory = new Mock<IHttpClientFactory>();
            var http = new HttpClientMockBuilder().WithBaseAddress(new Uri("http://192.168.1.2/")).WithJsonContentRequest<bool>("api/RelaySwitch/StateRelay", HttpMethod.Put).RespondingJsonContent(x => x).Build();
            CllientFactory.Setup(f => f.CreateClient("ESPClient")).Returns(http);
            m_fakeTimeProvider.SetUtcNow(DateTime.UtcNow);
            var pp = new provaweb.ProgrmmaModificaStatoRelay(m_fakeTimeProvider, CllientFactory.Object, NullLogger<provaweb.ProgrmmaModificaStatoRelay>.Instance, m_registroEsp, m_memoria,m_programmaSettimanale);
            pp.m_StateRelay = true;
            await pp.StartAsync(CancellationToken.None);
            CllientFactory.SetReturnsDefault(pp);
            m_fakeTimeProvider.Advance(TimeSpan.FromMinutes(1));
            await pp.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task InactiveUsersEviction()
        {
            await m_activeUsersService.StartAsync(CancellationToken.None);

            Assert.False(m_activeUsersService.IsRegistered("al"));
            m_activeUsersService.SetLastVisit("al");
            Assert.True(m_activeUsersService.IsRegistered("al"));
            m_fakeTimeProvider.Advance(TimeSpan.FromSeconds(1));
            Assert.False(m_activeUsersService.IsRegistered("al"));

            await m_activeUsersService.StopAsync(CancellationToken.None);
        }
        
    }
}