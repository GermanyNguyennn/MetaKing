using MetaKing.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace MetaKing.BackendServer.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSenderService(IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new RestClient(new RestClientOptions
            {
                BaseUrl = new Uri(_emailSettings.ApiBaseUrl),
                Authenticator = new HttpBasicAuthenticator("api", _emailSettings.ApiKey)
            });

            var request = new RestRequest();
            request.AddParameter("domain", _emailSettings.Domain, ParameterType.UrlSegment);
            request.AddParameter("from", _emailSettings.From);
            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("html", htmlMessage);
            request.Method = RestSharp.Method.Post;

            var response = await client.ExecuteAsync(request);
            // Xử lý response nếu cần
        }
    }
}
