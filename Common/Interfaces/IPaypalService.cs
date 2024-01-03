using Common.Models.Paypal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IPaypalService
    {
        Task<PaypalOrderResponseDto?> CreatePayment(PaypalOrderDto order);
        Task<PaypalOrderCaptureDto?> CapturePayment(string orderId);
    }
}
