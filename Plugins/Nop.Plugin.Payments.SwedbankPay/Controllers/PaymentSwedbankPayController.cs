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


            if (storeScope <= 0)
                return View("~/Plugins/Payments.SwedbankPay/Views/Configure.cshtml", swedbankPayPaymentSettings);

            model.UseSandbox_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.UseSandbox, storeScope);
            model.BusinessEmail_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.BusinessEmail, storeScope);
            model.PdtToken_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.PdtToken, storeScope);
            model.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);
            model.AdditionalFee_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.AdditionalFee, storeScope);
            model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(swedbankPayPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            return View("~/Plugins/Payments.SwedbankPay/Views/Configure.cshtml", swedbankPayPaymentSettings);
        }
    }
}
