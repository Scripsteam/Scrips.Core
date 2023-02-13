using Refit;
using Scrips.Core.Models.EmailSender;

namespace Scrips.Core.HttpApiClients;

public interface IEmailSenderApi
{
    [Post("/api/Email/Send/{emailKey}")]
    Task SendEmailAsync([Body] EmailData data, string emailKey);
}