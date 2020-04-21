using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.SwedbankPay.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.SwedbankPay.Controllers
{
    public class PaymentSwedbankPayController : BasePaymentController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentSwedbankPayController(IGenericAttributeService genericAttributeService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShoppingCartSettings shoppingCartSettings)
        {
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _settingService = settingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var swedbankPayPaymentSettings = _settingService.LoadSetting<SwedbankPayPaymentSettings>(storeScope);

            var swedbankPayPaymentSettingsModel = new ConfigurationModel
            {
                UseDevelopmentMode = swedbankPayPaymentSettings.UseDevelopmentMode,
                DevelopmentEnvironment = swedbankPayPaymentSettings.DevelopmentEnvironment,
                MerchantId = swedbankPayPaymentSettings.MerchantId,
                DevelopmentMerchantToken = swedbankPayPaymentSettings.DevelopmentMerchantToken,
                MerchantToken = swedbankPayPaymentSettings.MerchantToken,
                BusinessEmail = swedbankPayPaymentSettings.BusinessEmail,
                PassProductNamesAndTotals = swedbankPayPaymentSettings.PassProductNamesAndTotals,
                AdditionalFee = swedbankPayPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = swedbankPayPaymentSettings.AdditionalFeePercentage,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope <= 0)
                return View("~/Plugins/Payments.SwedbankPay/Views/Configure.cshtml", swedbankPayPaymentSettingsModel);

            swedbankPayPaymentSettingsModel.UseDevelopmentMode_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.UseDevelopmentMode, storeScope);
            swedbankPayPaymentSettingsModel.DevelopmentEnvironment_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.DevelopmentEnvironment, storeScope);
            swedbankPayPaymentSettingsModel.MerchantId_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.MerchantId, storeScope);
            swedbankPayPaymentSettingsModel.DevelopmentMerchantToken_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.DevelopmentMerchantToken, storeScope);
            swedbankPayPaymentSettingsModel.MerchantToken_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.MerchantToken, storeScope);
            swedbankPayPaymentSettingsModel.BusinessEmail_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.BusinessEmail, storeScope);
            swedbankPayPaymentSettingsModel.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);
            swedbankPayPaymentSettingsModel.AdditionalFee_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.AdditionalFee, storeScope);
            swedbankPayPaymentSettingsModel.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            return View("~/Plugins/Payments.SwedbankPay/Views/Configure.cshtml", swedbankPayPaymentSettings);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var swedbankPayPaymentSettings = _settingService.LoadSetting<SwedbankPayPaymentSettings>(storeScope);

            //save settings
            swedbankPayPaymentSettings.UseDevelopmentMode = model.UseDevelopmentMode;
            swedbankPayPaymentSettings.DevelopmentEnvironment = model.DevelopmentEnvironment;
            swedbankPayPaymentSettings.MerchantId = model.MerchantId;
            swedbankPayPaymentSettings.DevelopmentMerchantToken = model.DevelopmentMerchantToken;
            swedbankPayPaymentSettings.BusinessEmail = model.BusinessEmail;
            swedbankPayPaymentSettings.MerchantToken = model.MerchantToken;
            swedbankPayPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            swedbankPayPaymentSettings.AdditionalFee = model.AdditionalFee;
            swedbankPayPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.UseDevelopmentMode, model.UseDevelopmentMode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.DevelopmentEnvironment, model.DevelopmentEnvironment_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.DevelopmentMerchantToken, model.DevelopmentMerchantToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.BusinessEmail, model.BusinessEmail_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.MerchantToken, model.MerchantToken_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.PassProductNamesAndTotals, model.PassProductNamesAndTotals_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(swedbankPayPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
