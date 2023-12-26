using L1.Features.OWCommunicators;
using L1.Features.PFFCommunicators;
using L1.Features.SabaPlayCommunicators;
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

            if (appSettings.PartnerIds.OW_VG != null)
            {
                services.Add(appSettings.PartnerIds.OW_VG, typeof(OWCustomerService));
            }

            if (appSettings.PartnerIds.PFF != null)
            {
                services.Add(appSettings.PartnerIds.PFF, typeof(PFFCustomerService));
            }

            if (appSettings.PartnerIds.ONEworksSeal888 != null)
            {
                services.Add(appSettings.PartnerIds.ONEworksSeal888, typeof(OWCustomerService));
            }

            if (appSettings.PartnerIds.SabaPlay != null)
            {
                services.Add(appSettings.PartnerIds.SabaPlay, typeof(SabaPlayCustomerService));
            }
        }

        public IPartnerCustomerServices GetCustomerService(string partnerId)
        {
            var service = !string.IsNullOrWhiteSpace(partnerId) && services.ContainsKey(partnerId)
                ? (IPartnerCustomerServices)serviceProvider.GetService(services[partnerId])
                : default;

            if (service != default)
            {
                service.OperatorId = partnerId;
            }

            return service;
        }
    }
}