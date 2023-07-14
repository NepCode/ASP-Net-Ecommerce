using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailService
    {
        Task<ServiceResponse<bool>> sendEmail(string refreshToken);
        Task<ServiceResponse<bool>> SendAsync(MailData mailData, CancellationToken ct);
        Task<ServiceResponse<bool>> SendWithAttachmentsAsync(MailDataWithAttachments mailData, CancellationToken ct);
        string GetEmailTemplate<T>(string emailTemplate, T emailTemplateModel);
        string GetEmailTemplateList<T>(string emailTemplate, List<T> emailTemplateModel);
    }
}
