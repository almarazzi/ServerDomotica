using System.Collections.Concurrent;

namespace provaweb
{
    public class ActiveUsersService : BackgroundService
    {
        private readonly ConcurrentDictionary<string, long> m_lastUserVisit = new();
        private readonly TimeProvider m_timeProvider;
        private readonly ILogger m_logger;
        public ActiveUsersService(ILogger<ActiveUsersService> logger, TimeProvider timeProvider)
        {
            m_logger = logger;
            m_timeProvider = timeProvider;
        }

        public void SetLastVisit(string userName)
            => m_lastUserVisit[userName] = m_timeProvider.GetTimestamp();

        public bool IsActive(string userName)
        {
            if (m_lastUserVisit.TryGetValue(userName, out long dt))
            {
                return m_timeProvider.GetElapsedTime(dt).TotalSeconds < 5;
            }
            return false;
        }

        internal bool IsRegistered(string userName)
            => m_lastUserVisit.ContainsKey(userName);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                m_logger.LogInformation("Eviction cycle...");
                var itemsToRemove = m_lastUserVisit.Where(x => !IsActive(x.Key)).Select(x => x.Key).ToList();
                foreach (var item in itemsToRemove)
                {
                    m_lastUserVisit.Remove(item, out long _);
                    m_logger.LogInformation("Removed user {UserName}", item);
                }

                await m_timeProvider.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }



    }
}
