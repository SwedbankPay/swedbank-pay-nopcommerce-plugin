using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.SwedbankPay.Components
{
    public class PaymentSwedbankPayViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.SwedbankPay/Views/PaymentInfo.cshtml");
        }
    }
}
