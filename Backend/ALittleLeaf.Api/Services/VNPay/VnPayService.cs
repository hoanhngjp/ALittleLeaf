using ALittleLeaf.Api.Models.VnPay;
using ALittleLeaf.Api.Utils;

namespace ALittleLeaf.Api.Services.VNPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, string txnRef)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(
                _configuration["TimeZoneId"] ?? "SE Asia Standard Time");

            var timeNow     = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var pay         = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];

            pay.AddRequestData("vnp_Version",    _configuration["Vnpay:Version"]!);
            pay.AddRequestData("vnp_Command",    _configuration["Vnpay:Command"]!);
            pay.AddRequestData("vnp_TmnCode",    _configuration["VNPAY_TMNCODE"]!);
            pay.AddRequestData("vnp_Amount",     ((long)(model.Amount * 100)).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode",   _configuration["Vnpay:CurrCode"]!);
            pay.AddRequestData("vnp_IpAddr",     pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale",     _configuration["Vnpay:Locale"]!);
            pay.AddRequestData("vnp_OrderInfo",  $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType",  model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl",  urlCallBack!);
            pay.AddRequestData("vnp_TxnRef",     txnRef);

            return pay.CreateRequestUrl(
                _configuration["VNPAY_URL"]!,
                _configuration["VNPAY_HASHSECRET"]!);
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            return pay.GetFullResponseData(collections, _configuration["VNPAY_HASHSECRET"]!);
        }
    }
}
