namespace Common.Models.Paypal
{
    public class PaypalCapture
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public PaypalBaseAmount Amount { get; set; }
    }
}