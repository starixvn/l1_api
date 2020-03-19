
namespace OW.SimulatorApi.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using L1.Features.OWCommunicators;
    using Microsoft.Extensions.DependencyInjection;

    public static class SimulatorExtensions
    {
        public static void AddOW(this IServiceCollection services)
        {
            var baseResult = new OWResult();
            services.AddSingleton<OWResult>(baseResult);
        }
    }
}
