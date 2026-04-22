using ALittleLeaf.Api.Models.VnPay;

namespace ALittleLeaf.Api.Services.VNPay
{
    public interface IVnPayService
    {
        /// <summary>Builds the redirect URL for the VNPay payment gateway.</summary>
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, string txnRef);

        /// <summary>Validates the VNPay callback query string and returns a result model.</summary>
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
