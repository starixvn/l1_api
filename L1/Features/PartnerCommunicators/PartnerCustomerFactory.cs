using L1.Features.OWCommunicators;
using L1.Features.PFFCommunicators;
using L1.Features.SabaPlayCommunicators;
using L1.Features.SboCommunicators;
using L1.Shared.Configurations;
using Sentry;
using Sentry.Protocol;
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
        private readonly ISentryClient sentryClient;

        public PartnerCustomerFactory(IServiceProvider serviceProvider, IAppSettings appSettings, ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;
            this.serviceProvider = serviceProvider;
            services = new() { };

            AddService(appSettings.PartnerIds.ONEworksCash, typeof(OWCustomerService));
            AddService(appSettings.PartnerIds.OW, typeof(OWCustomerService));
            AddService(appSettings.PartnerIds.ONEworksCredit, typeof(OWCustomerService));
            AddService(appSettings.PartnerIds.SBO, typeof(SboCustomerService));
            AddService(appSettings.PartnerIds.OW_VG, typeof(OWCustomerService));
            AddService(appSettings.PartnerIds.PFF, typeof(PFFCustomerService));
            AddService(appSettings.PartnerIds.ONEworksSeal888, typeof(OWCustomerService));
            AddService(appSettings.PartnerIds.SabaPlay, typeof(SabaPlayCustomerService));
        }

        private void AddService(string key, Type serviceType)
        {
            if(key != null)
            {
                services.Add(key, serviceType);
            }
        }

        public IPartnerCustomerServices GetCustomerService(string partnerId)
        {
            var service = !string.IsNullOrWhiteSpace(partnerId) && services.TryGetValue(partnerId, out var serviceType)
                ? (IPartnerCustomerServices)serviceProvider.GetService(serviceType)
                : default;
            
            if (service != default)
            {
                service.OperatorId = partnerId;
            }

            return service;
        }
    }
}