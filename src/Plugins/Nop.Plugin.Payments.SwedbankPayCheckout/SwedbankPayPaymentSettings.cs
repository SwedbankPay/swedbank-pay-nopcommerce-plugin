using System;
using NuGet;

namespace Nop.Plugin.Payments.SwedbankPay
{
    /// <summary>
    /// Represents settings of the Swedbank Pay plugin
    /// </summary>
    public class SwedbankPayPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseDevelopmentMode { get; set; }

        /// <summary>
        /// Set to "dev", "stage", "internaltest", "externalintegration"
        /// </summary>
        public string DevelopmentEnvironment { get; set; }

        public Guid MerchantId { get; set; }

        public string DevelopmentMerchantToken { get; set; }

        /// <summary>
        /// Your API token gotten from the Admin pages of SwedbankPay
        /// </summary>
        public string MerchantToken { get; set; }

        /// <summary>
        /// Gets or sets a business email
        /// </summary>
        public string BusinessEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass info about purchased items to Swedbank Pay
        /// </summary>
        public bool PassProductNamesAndTotals { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
    }
}
