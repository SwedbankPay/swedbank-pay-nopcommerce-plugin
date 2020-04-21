using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System;

namespace Nop.Plugin.Payments.SwedbankPay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.UseDevelopmentMode")]
        public bool UseDevelopmentMode { get; set; }
        public bool UseDevelopmentMode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.MerchantId")]
        public Guid MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.DevelopmentEnvironment")]
        public string DevelopmentEnvironment { get; set; }
        public bool DevelopmentEnvironment_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.DevelopmentMerchantToken")]
        public string DevelopmentMerchantToken { get; set; }
        public bool DevelopmentMerchantToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.MerchantToken")]
        public string MerchantToken { get; set; }
        public bool MerchantToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.BusinessEmail")]
        public string BusinessEmail { get; set; }
        public bool BusinessEmail_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotals_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SwedbankPay.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }
    }
}