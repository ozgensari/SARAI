using System;
using System.Threading.Tasks;

namespace SARAI.Modules
{
    public enum PaymentProvider { None, PayGo, ProPay }
    public enum PaymentResult { Approved, Declined, Error }

    public sealed class PosIntegration
    {
        public PaymentProvider Provider { get; }
        public PosIntegration(PaymentProvider provider) => Provider = provider;

        public async Task<(PaymentResult Result, string Reference)> ChargeAsync(decimal amount, string currency = "TRY")
        {
            await Task.Delay(600);
            if (Provider == PaymentProvider.None) return (PaymentResult.Error, "POS pasif");
            if (amount <= 0) return (PaymentResult.Declined, "Tutar geçersiz");

            if (new Random().NextDouble() < 0.95)
                return (PaymentResult.Approved, $"{Provider}-REF-{DateTime.Now:HHmmss}");
            return (PaymentResult.Declined, $"{Provider}-DECLINE");
        }
    }
}
