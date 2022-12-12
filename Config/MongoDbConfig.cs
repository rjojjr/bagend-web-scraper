namespace bagend_web_scraper.Config
{
    public class MongoDbConfig
    {

        public string DatabaseName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Host { get; set; } = null!;

        public int Port { get; set; } = 27017;
    }
}