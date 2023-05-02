using Core.Interfaces;
using Core.Models;
using FluentEmail.Core;
using MailKit.Net.Smtp;
using MailKit.Security;
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
    public class MailData
    {
        // Receiver
        public List<string> To { get; }
        public List<string> Bcc { get; }

        public List<string> Cc { get; }

        // Sender
        public string? From { get; }

        public string? DisplayName { get; }

        public string? ReplyTo { get; }

        public string? ReplyToName { get; }

        // Content
        public string Subject { get; }

        public string? Body { get; }

        public MailData(List<string> to, string subject, string? body = null, string? from = null, string? displayName = null, string? replyTo = null, string? replyToName = null, List<string>? bcc = null, List<string>? cc = null)
        {
            // Receiver
            To = to;
            Bcc = bcc ?? new List<string>();
            Cc = cc ?? new List<string>();

            // Sender
            From = from;
            DisplayName = displayName;
            ReplyTo = replyTo;
            ReplyToName = replyToName;

            // Content
            Subject = subject;
            Body = body;
        }
    }
    public class WelcomeMail
    {
        public string? Name { get; set; }
        public string? Email { get; set; }

        public WelcomeMail(string? name, string? email)
        {
            Name = name;
            Email = email;
        }
    }



    public class EmailService : IEmailService
    {
        public async Task<ServiceResponse<bool>> sendEmail(string bodyparam)
        {
            var data = new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = "Cart item does not exist."
            };

            string mailText = BuildTemplate("StaticFiles/", "index.cshtml");
            mailText = mailText.Replace("[fromEmail]", "tonystars@gmail.com").Replace("[fromName]", "TONY").Replace("[contactMessage]", bodyparam);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("porter.hand77@ethereal.email"));
            email.To.Add(MailboxAddress.Parse("porter.hand77@ethereal.email"));

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
                GetEmailTemplate("templateRazor", employeeList ));

            // Add Content to Mime Message
            var body = new BodyBuilder();
            //mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            email.Body = body.ToMessageBody();

            //cshtml

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("porter.hand77@ethereal.email", "uHzDd8qhyY8r411Uvx");
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


        public string GetEmailTemplate<T>(string emailTemplate, List<T> emailTemplateModel)
        {
            string mailTemplate = LoadTemplate(emailTemplate);

            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate modifiedMailTemplate = razorEngine.Compile(mailTemplate);

            return modifiedMailTemplate.Run(emailTemplateModel);
        }

        public string LoadTemplate(string emailTemplate)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string templateDir = Path.Combine(baseDir, "StaticFiles/");
            string templatePath = Path.Combine(templateDir, $"{emailTemplate}.cshtml");

            using FileStream fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);

            string mailTemplate = streamReader.ReadToEnd();
            streamReader.Close();

            return mailTemplate;
        }

        public async Task<bool> SendAsync(MailData mailData, CancellationToken ct = default)
        {
            try
            {
               

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }



    }
}
