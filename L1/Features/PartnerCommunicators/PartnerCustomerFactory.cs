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
            sentryClient.CaptureMessage($"GetCustomerService 1 {partnerId}", SentryLevel.Error);
            sentryClient.CaptureMessage($"GetCustomerService 2 {partnerId} {services.ContainsKey(partnerId)}", SentryLevel.Error);
            var service = !string.IsNullOrWhiteSpace(partnerId) && services.ContainsKey(partnerId)
                ? (IPartnerCustomerServices)serviceProvider.GetService(services[partnerId])
                : default;

            Console.WriteLine(string.Join(' ', services.Keys));

            if (service != default)
            {
                service.OperatorId = partnerId;
            }

            return service;
        }
    }
}