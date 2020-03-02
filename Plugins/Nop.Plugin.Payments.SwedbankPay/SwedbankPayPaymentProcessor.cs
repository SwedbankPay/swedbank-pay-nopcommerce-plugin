using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using SwedbankPay.Sdk.Payments;

namespace Nop.Plugin.Payments.SwedbankPay
{
    public class SwedbankPayPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly SwedbankPayPaymentSettings _swedbankPayPaymentSettings;
        private readonly ISettingService _settingService;



        #endregion


        #region Ctor

        public SwedbankPayPaymentProcessor(IPaymentService paymentService, IWebHelper webHelper, SwedbankPayPaymentSettings swedbankPayPaymentSettings, ISettingService settingService)
        {
            _paymentService = paymentService;
            _webHelper = webHelper;
            _swedbankPayPaymentSettings = swedbankPayPaymentSettings;
            _settingService = settingService;

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
            return $"{_webHelper.GetStoreLocation()}Admin/SwedbankPay/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new SwedbankPayPaymentSettings
            {
                UseSandbox = true
            });
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
        public string PaymentMethodDescription => "Payment Method Description";


        #endregion
    }
}
