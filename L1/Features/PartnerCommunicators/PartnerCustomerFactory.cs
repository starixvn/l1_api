using L1.Features.OWCommunicators;
using L1.Features.SboCommunicators;
using L1.Shared.Configurations;
using System;
using System.Collections.Generic;

namespace L1.Features.PartnerCommunicators
{
    public interface IPartnerCustomerFactory
    {
        IPartnerCustomerServices GetCustomerService(string partnerId);
    }

    public class PartnerCustomerFactory : IPartnerCustomerFactory
    {
        private readonly Dictionary<string, Type> services;
        private readonly IServiceProvider serviceProvider;

        public PartnerCustomerFactory(IServiceProvider serviceProvider, IAppSettings appSettings)
        {
            this.serviceProvider = serviceProvider;
            services = new()
            {
                { appSettings.PartnerIds.OW, typeof(OWCustomerService) },
                { appSettings.PartnerIds.SBO, typeof(SboCustomerService) },
            };
        }

        public IPartnerCustomerServices GetCustomerService(string partnerId)
        => !string.IsNullOrWhiteSpace(partnerId) && services.ContainsKey(partnerId)
            ? (IPartnerCustomerServices)serviceProvider.GetService(services[partnerId])
            : default;
    }
}