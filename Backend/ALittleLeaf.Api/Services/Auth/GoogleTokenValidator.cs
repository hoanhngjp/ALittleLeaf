using Google.Apis.Auth;

namespace ALittleLeaf.Api.Services.Auth
{
    public class GoogleTokenValidator : IGoogleTokenValidator
    {
        public Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken, string clientId)
            => GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });
    }
}
