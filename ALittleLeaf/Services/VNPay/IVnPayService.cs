using ALittleLeaf.ViewModels;

namespace ALittleLeaf.Services.VNPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, string txnRef);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
