
namespace SignalrAPI.Models
{
    public class AppSettings
    {
        public string SqlConnectionString { get; set; }
        public string OmnizantDevWebHost { get; set; }
        public string OmnizantPMAWebHost { get; set; }
        public string OmnizantProductionWebHost { get; set; }
        public string ZolaCryptoPassWord { get; set; }

        // NCache
        public bool IsNCacheEnabled { get; set; }
        public string NCacheName { get; set; }
        public string NCacheApplicationId { get; set; }
        public string NCacheUserId { get; set; }
        public string NCachePassword { get; set; }

        // Redis
        public bool IsRedisEnabled { get; set; }
        public string RedisConnectionString { get; set; }
      
    }
}
