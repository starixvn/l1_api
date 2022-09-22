namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        int TokenExpiredSeconds { get; set; }

        MongoDBSettings MongoDBSettings { get; set; }

        public int HighStake { get; set; }

        PartnerIds PartnerIds { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public int TokenExpiredSeconds { get; set; }

        public MongoDBSettings MongoDBSettings { get; set; }

        public int HighStake { get; set; }

        public PartnerIds PartnerIds { get; set; }
    }
}