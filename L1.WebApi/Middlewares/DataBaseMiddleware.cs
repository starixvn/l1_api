using System.Collections.Generic;
using System.Linq;
using Fanex.Data;
using Fanex.Data.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace L1.WebApi.Middlewares
{
    public static class DataBaseMiddleware
    {
        public static void UseDatabase(this IApplicationBuilder app, IConfiguration configuration)
        {
            Dictionary<string, ConnectionConfiguration> connections = configuration
                 .GetSection("AppSettings:MySqlConnectionStrings")
                 .GetChildren()
                 .ToDictionary(connection => connection.Key,
                               connection => new ConnectionConfiguration(connection.Key, connection.Value));

            DbSettingProviderManager
                .StartNewSession()
                .Use(connections)
                // It comes with a default connection string provider, which works well with MySql connections,
                // as well as a default DbSetting provider
                .WithMySql(resourcePath: configuration["AppSettings:AppDataPath"])
                .Run();
        }
    }
}