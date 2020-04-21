using System;
using System.Net.Http;
using Autofac;
using Autofac.Core;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Configuration;
using SwedbankPay.Sdk;

namespace Nop.Plugin.Payments.SwedbankPay
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 1;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.Register<ISwedbankPayClient>(ctx =>
            {
                var storeContext = ctx.Resolve<IStoreContext>();
                var settingsService = ctx.Resolve<ISettingService>();
                
                var configModel = settingsService.LoadSetting<SwedbankPayPaymentSettings>(storeContext.ActiveStoreScopeConfiguration);

                Uri baseUri;
                string merchantToken;
                string payeeId;

                if(configModel.UseDevelopmentMode)
                {
                    baseUri = new Uri($"https://api.{configModel.DevelopmentEnvironment}.payex.com");
                    merchantToken = configModel.DevelopmentMerchantToken ?? "MissingTokenxxxxx";
                    payeeId = configModel.MerchantId.ToString() ?? "Missing MerchantId";
                } else
                {
                    baseUri = new Uri("https://api.payex.com");
                    merchantToken = configModel.MerchantToken ?? "MissingTokenxxxxx";
                }

                var client = new HttpClient() { BaseAddress = baseUri };
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", merchantToken);

                return new SwedbankPayClient(client);
            }).InstancePerDependency();
        }
    }
}
