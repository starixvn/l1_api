namespace L1.Shared.Configurations
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string ConnectionString { get; set; }
    }

    public interface IMongoDBSettings
    {
        string ConnectionString { get; set; }
    }
}