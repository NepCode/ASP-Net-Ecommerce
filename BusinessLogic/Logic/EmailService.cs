using Core.Configuration;
using Core.Interfaces;
using Core.Models;
using FluentEmail.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
  


    public class EmailService : IEmailService
    {

        private readonly MailSettings _settings;

        public EmailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }


        public async Task<ServiceResponse<bool>> sendEmail(string bodyparam)
        {
            var data = new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cart item does not exist."
            };

            //string mailText = BuildTemplate("StaticFiles/", "index.cshtml");
            //mailText = mailText.Replace("[fromEmail]", "tonystars@gmail.com").Replace("[fromName]", "TONY").Replace("[contactMessage]", bodyparam);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("antonetta.harber51@ethereal.email"));
            email.To.Add(MailboxAddress.Parse("antonetta.harber51@ethereal.email"));

            email.Subject = "sent it from asp net 6 api";


            //email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
            //email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = mailText };


            //cshtml
            List<WelcomeMail> employeeList = new List<WelcomeMail>();
            // populate the list with objects of 'employee'
            employeeList.Add(new WelcomeMail("John", "Developer"));
            employeeList.Add(new WelcomeMail("Mike", "Engineer"));

            //List<WelcomeMail> AuthorList = new List<WelcomeMail>(
            //    new WelcomeMail { Name = "ohgiasd", Email = "mi nombre" }
            //    );
            //AuthorList.Add(new WelcomeMail("Mahesh Chand", 35, "A Prorammer's Guide to ADO.NET", true, new DateTime(2003, 7, 10)));


            //var welcomeMail = new WelcomeMail { Email = "ohgiasd", Name = "mi nombre" };
            MailData mailData = new MailData(
               new List<string> { "welcome email" },
               "Welcome to the MailKit Demo",
                GetEmailTemplateList("templateRazor", employeeList ));

            // Add Content to Mime Message
            var body = new BodyBuilder();
            //mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            email.Body = body.ToMessageBody();

            //cshtml

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("antonetta.harber51@ethereal.email", "9AyMnaeUpKCRr4tmB2");
            smtp.Send(email);
            smtp.Disconnect(true);

            await Task.Delay(1000);
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cart item does not exist."
            };
        }

        public static string BuildTemplate(string path, string template)

        {

            StreamReader str = new StreamReader(Path.Combine(path, template));

            string mailText = str.ReadToEnd();

            str.Close();


            return mailText;

        }


        public async Task<ServiceResponse<bool>> SendAsync(MailData mailData, CancellationToken ct = default)
        {

            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
                mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

                // Receiver
                foreach (string mailAddress in mailData.To)
                    mail.To.Add(MailboxAddress.Parse(mailAddress));

                // Set Reply to if specified in mail data
                if (!string.IsNullOrEmpty(mailData.ReplyTo))
                    mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

                // BCC
                // Check if a BCC was supplied in the request
                if (mailData.Bcc != null)
                {
                    // Get only addresses where value is not null or with whitespace. x = value of address
                    foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }

                // CC
                // Check if a CC address was supplied in the request
                if (mailData.Cc != null)
                {
                    foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
                #endregion

                #region Content

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = mailData.Subject;
                body.HtmlBody = mailData.Body;
                mail.Body = body.ToMessageBody();

                #endregion

                #region Send Mail

                using var smtp = new SmtpClient();

                if (_settings.UseSSL)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                }
                else if (_settings.UseStartTls)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
                }

                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                var data = new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Cart item does not exist."
                };
                return data;
                #endregion

            }
            catch (Exception e)
            {
                var data = new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist."
                };
                return data;
            }
        }

        public async Task<ServiceResponse<bool>> SendWithAttachmentsAsync(MailDataWithAttachments mailData, CancellationToken ct = default)
        {
         

            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
                mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

                // Receiver
                foreach (string mailAddress in mailData.To)
                    mail.To.Add(MailboxAddress.Parse(mailAddress));

                // Set Reply to if specified in mail data
                if (!string.IsNullOrEmpty(mailData.ReplyTo))
                    mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

                // BCC
                // Check if a BCC was supplied in the request
                if (mailData.Bcc != null)
                {
                    // Get only addresses where value is not null or with whitespace. x = value of address
                    foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }

                // CC
                // Check if a CC address was supplied in the request
                if (mailData.Cc != null)
                {
                    foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
                #endregion

                #region Content

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = mailData.Subject;

                // Check if we got any attachments and add the to the builder for our message
                if (mailData.Attachments != null)
                {
                    byte[] attachmentFileByteArray;
                    foreach (IFormFile attachment in mailData.Attachments)
                    {
                        if (attachment.Length > 0)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                attachment.CopyTo(memoryStream);
                                attachmentFileByteArray = memoryStream.ToArray();
                            }
                            body.Attachments.Add(attachment.FileName, attachmentFileByteArray, ContentType.Parse(attachment.ContentType));
                        }
                    }
                }
                body.HtmlBody = mailData.Body;
                mail.Body = body.ToMessageBody();

                #endregion

                #region Send Mail

                using var smtp = new SmtpClient();

                if (_settings.UseSSL)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                }
                else if (_settings.UseStartTls)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
                }

                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                var data = new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Cart item does not exist."
                };
                return data;
                #endregion

            }
            catch (Exception e)
            {
                var data = new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist."
                };
                return data;
            }
        }

        public string GetEmailTemplate<T>(string emailTemplate, T emailTemplateModel)
        {
            string mailTemplate = LoadTemplate(emailTemplate);

            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate modifiedMailTemplate = razorEngine.Compile(mailTemplate);

            return modifiedMailTemplate.Run(emailTemplateModel);
        }

        public string GetEmailTemplateList<T>(string emailTemplate, List<T> emailTemplateModel)
        {
            //is responsible for modifying the mail template returned from 

            string mailTemplate = LoadTemplate(emailTemplate);

            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate modifiedMailTemplate = razorEngine.Compile(mailTemplate);

            return modifiedMailTemplate.Run(emailTemplateModel);
        }

        public string LoadTemplate(string emailTemplate)
        {
            /*
            is responsible for returning the email template file, which we would like to modify with our model data. 
            I generates a path for the template, then used a file- and stream reader to open the file, read it to the 
            end and return it to the requesting method.
            */
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templateDir = Path.Combine(baseDir, "StaticFiles/");
            string templatePath = Path.Combine(templateDir, $"{emailTemplate}.cshtml");

            using FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);

            string mailTemplate = streamReader.ReadToEnd();
            streamReader.Close();

            return mailTemplate;
        }

        public async Task<ServiceResponse<bool>> SendEmailAsyncArray(MailData mailData, CancellationToken ct = default)
        {

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templateDir = Path.Combine(baseDir, "StaticFiles/");
            string templatePath = Path.Combine(templateDir, "htmlpage.html");

            using FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);

            string mailTemplate = streamReader.ReadToEnd();
            streamReader.Close();

            // Initialize a new instance of the MimeKit.MimeMessage class
            var mail = new MimeMessage();

            #region Sender / Receiver
            // Sender
            mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
            mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

            // Receiver
            foreach (string mailAddress in mailData.To)
                mail.To.Add(MailboxAddress.Parse(mailAddress));

            // Set Reply to if specified in mail data
            if (!string.IsNullOrEmpty(mailData.ReplyTo))
                mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

            // BCC
            // Check if a BCC was supplied in the request
            if (mailData.Bcc != null)
            {
                // Get only addresses where value is not null or with whitespace. x = value of address
                foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
            }

            // CC
            // Check if a CC address was supplied in the request
            if (mailData.Cc != null)
            {
                foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
            }
            #endregion

            #region Content
            var body = new BodyBuilder();
            body.HtmlBody = mailTemplate;
            //using (StreamReader SourceReader = System.IO.File.OpenText(mailTemplate))
            //{
            //    body.HtmlBody = SourceReader.ReadToEnd();
            //}

            string messageBody = string.Format(body.HtmlBody,
                        "subject",
                        String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                        mailData.To,
                        mailData.To,
                        "passwword",
                        "message",
                        "callbackurl"
                        );

            // Add Content to Mime Message
            //var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();

            #endregion


            #region Send Mail

            using var smtp = new SmtpClient();

            if (_settings.UseSSL)
            {
                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
            }
            else if (_settings.UseStartTls)
            {
                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
            }

            await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
            await smtp.SendAsync(mail, ct);
            await smtp.DisconnectAsync(true, ct);

            var data = new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Cart item does not exist."
            };
            return data;
            #endregion





        }
    }
}
