using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using SwedbankPay.Sdk;
using SwedbankPay.Sdk.PaymentOrders;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.SwedbankPay
{
    class SwedbankPayPaymentProcessor : BasePlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly SwedbankPayPaymentSettings _swedbankPayPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ISwedbankPayClient _swedbankPayClient;
        private readonly IHttpContextAccessor _httpContextAccessor;



        #endregion


        #region Ctor

        public SwedbankPayPaymentProcessor(IPaymentService paymentService, ILocalizationService localizationService, IWebHelper webHelper, SwedbankPayPaymentSettings swedbankPayPaymentSettings, ISettingService settingService, ISwedbankPayClient swedbankPayClient, IHttpContextAccessor httpContextAccessor)
        {
            _paymentService = paymentService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _swedbankPayPaymentSettings = swedbankPayPaymentSettings;
            _settingService = settingService;
            _swedbankPayClient = swedbankPayClient;
            _httpContextAccessor = httpContextAccessor;


        }

        public SwedbankPayPaymentProcessor()
        {
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

        public static Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        //TODO: Check this
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var lugn = System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\plugins\\Payments.SwedbankPay\\plugin.json");
            var tests = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(lugn);
            var version = tests["Version"];
            //var payeeId = settingsService.LoadSetting<SwedbankPayPaymentSettings>(storeContext.ActiveStoreScopeConfiguration)




            var operation = Operation.Purchase;
            var currency = new CurrencyCode(postProcessPaymentRequest.Order.CustomerCurrencyCode);
            var amount = Amount.FromDecimal(postProcessPaymentRequest.Order.OrderTotal);
            var vatAmount = Amount.FromDecimal(postProcessPaymentRequest.Order.OrderTax);
            string description = postProcessPaymentRequest.Order.CheckoutAttributeDescription;
            string userAgent = $"{System.Reflection.Assembly.GetExecutingAssembly().FullName}/{version}";
            var language = new System.Globalization.CultureInfo("nb-no");
            //var language = new System.Globalization.CultureInfo(postProcessPaymentRequest.Order.CustomerLanguageId);
            bool generateRecurrenceToken = false;
            var hostUrls = new List<Uri>();
            hostUrls.Add(new Uri(_webHelper.GetStoreLocation()));
            var urls = new Urls(hostUrls, new Uri(GetCompleteUrl()), new Uri(GetTermsOfServiceUrl()), new Uri(GetCancelPaymentUrl()));
            var payeeInfo = new PayeeInfo(_swedbankPayPaymentSettings.MerchantId, postProcessPaymentRequest.Order.CustomOrderNumber);
            //var payeeId = _swedbankPayPaymentSettings.MerchantId;




            var paymentOrder = new PaymentOrderRequest(operation, currency, amount, vatAmount, description, userAgent, language, generateRecurrenceToken, urls, payeeInfo);
            var paymentOrderResponse = _swedbankPayClient.PaymentOrders.Create(paymentOrder).Result;



            _httpContextAccessor.HttpContext.Response.Redirect(paymentOrderResponse.Operations.View.Href.AbsoluteUri);
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

        public string GetCompleteUrl()
        {
            return $"{_webHelper.GetStoreLocation()}/PaymentSwedbankPay/PaymentComplete";
        }

        public string GetTermsOfServiceUrl()
        {

            return $"{_webHelper.GetStoreLocation(true)}/PaymentSwedbankPay/TermsOfService";
        }

        public string GetCancelPaymentUrl()
        {
            return $"{_webHelper.GetStoreLocation()}/PaymentSwedbankPay/CancelPage";
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
        public string PaymentMethodDescription => "SwedbankPay";
        //_localizationService.GetResource("Plugins.Payments.SwedbankPay.Fields.PaymentMethodDescription.Hint");


        #endregion
    }
}
