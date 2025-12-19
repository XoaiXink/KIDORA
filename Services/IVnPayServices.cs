using KIDORA.ViewModels;

namespace KIDORA.Models.Services
{
    public interface IVnPayServices
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
