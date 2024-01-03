using BLL.DTOs.Paypal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Paypal
{
    public class PaypalPurchaseUnitAmount : PaypalBaseAmount
    {
        public PaypalBreakdown Breakdown { get; set; }
    }
}
