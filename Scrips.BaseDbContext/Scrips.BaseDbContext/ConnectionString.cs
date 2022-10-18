using Microsoft.Extensions.Configuration;

namespace Scrips.BaseDbContext
{
    public sealed class ConnectionString
    {
        public ConnectionString(IConfiguration configuration)
        {
            DefaultConnection = configuration.GetConnectionString("SchedulingDb");
        }

        public string DefaultConnection { get; }
    }
}