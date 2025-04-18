namespace L1.Shared.Constants
{
    public static class MongoDatabases
    {
        internal const string Admin = "admin";
        internal const string Member = "member";
        internal const string Log = "log";
        internal const string Minigame = "minigame";
        internal const string Summary = "summary";

        public static string FormatDatabaseName(string dbName, string prefix)
           => string.IsNullOrWhiteSpace(prefix) ? dbName : prefix + "_" + dbName;

        public static string GetMemberDatabase(string prefix)
            => FormatDatabaseName(Member, prefix);

        public static string GetLogDatabase(string prefix)
            => FormatDatabaseName(Log, prefix);

        public static string GetMinigameDatabase(string prefix)
            => FormatDatabaseName(Minigame, prefix);
    }
}