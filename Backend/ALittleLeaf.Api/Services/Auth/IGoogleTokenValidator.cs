using Google.Apis.Auth;

namespace ALittleLeaf.Api.Services.Auth
{
    public interface IGoogleTokenValidator
    {
        Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken, string clientId);
    }
}
