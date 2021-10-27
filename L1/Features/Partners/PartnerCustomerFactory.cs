using System;
using System.Collections.Generic;
using L1.Features.OWCommunicators;
using L1.Shared.Constants;

namespace L1.Features.Partners
{
    public interface IPartnerCustomerFactory
    {
        IPartnerCustomerServices GetCustomerService(string partnerId);
    }

    public class PartnerCustomerFactory : IPartnerCustomerFactory
    {
        private readonly Dictionary<string, Type> services = new Dictionary<string, Type>
        {
            { PartnerIds.OW, typeof(OWCustomerService) },
        };

        private readonly IServiceProvider serviceProvider;

        public PartnerCustomerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IPartnerCustomerServices GetCustomerService(string partnerId)
        => !string.IsNullOrWhiteSpace(partnerId) && services.ContainsKey(partnerId)
            ? (IPartnerCustomerServices)serviceProvider.GetService(services[partnerId])
            : default;
    }
}