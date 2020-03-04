using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using SwedbankPay.Sdk;

namespace Nop.Plugin.Payments.SwedbankPay
{
    public class SwedbankPayPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly SwedbankPayPaymentSettings _swedbankPayPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ISwedbankPayClient _swedbankPayClient;


        #endregion


        #region Ctor

        public SwedbankPayPaymentProcessor(IPaymentService paymentService, ILocalizationService localizationService, IWebHelper webHelper, SwedbankPayPaymentSettings swedbankPayPaymentSettings, ISettingService settingService, ISwedbankPayClient swedbankPayClient)
        {
            _paymentService = paymentService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _swedbankPayPaymentSettings = swedbankPayPaymentSettings;
            _settingService = settingService;
            _swedbankPayClient = swedbankPayClient;

        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _paymentService.CalculateAdditionalFee(cart,
               _swedbankPayPaymentSettings.AdditionalFee, _swedbankPayPaymentSettings.AdditionalFeePercentage);
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public string GetPublicViewComponentName()
        {
            return "PaymentSwedbankPay";
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        //TODO: Check this
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult();
        }

        public bool CanRePostProcessPayment(Order order)
        {
            throw new NotImplementedException();
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult();
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult();
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult();
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentSwedbankPay/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new SwedbankPayPaymentSettings
            {
                UseDevelopmentMode = true
            });

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.AdditionalFee", "Additional fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.BusinessEmail", "Business Email");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.BusinessEmail.Hint", "Specify your Swedbank Pay business email.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.PassProductNamesAndTotals", "Pass product names and order totals to Swedbank Pay");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.PassProductNamesAndTotals.Hint", "Check if product names and order totals should be passed to Swedbank Pay.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.PDTToken", "PDT Identity Token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.PDTToken.Hint", "Specify PDT identity token");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.RedirectionTip", "You will be redirected to Swedbank Pay site to complete the order.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.UseSandbox", "Use Sandbox");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.PaymentMethodDescription.Hint", "Pay with Swedbank Pay");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.PaymentMethodDescription.Redirect", "");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.PaymentMethodDescription.SeamlessView", "");
            //_localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.SwedbankPay.Fields.PaymentMethodDescription.Direct", ""


            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<SwedbankPayPaymentSettings>();

            base.Uninstall();
        }

        #endregion

        #region Properties


        public bool SupportCapture => true;

        public bool SupportPartiallyRefund => true;

        public bool SupportRefund => true;

        public bool SupportVoid => true;

        //TODO: Check this
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Manual;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        //TODO: Check this
        public bool SkipPaymentInfo => false;

        //TODO: Check this
        public string PaymentMethodDescription => _localizationService.GetResource("Plugins.Payments.SwedbankPay.Fields.PaymentMethodDescription.Hint");


        #endregion
    }
}
