using Core.Interfaces;
using Core.Models;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.Generic;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }




        // GET: api/<EmailController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<EmailController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EmailController>
        [HttpPost("sendmail")]
        public async Task<ActionResult<ServiceResponse<bool>>> SendMailAsync(MailData mail)
        {
            //var result = await _emailService.sendEmail(mail.Body);
            var result = await _emailService.SendAsync(mail, new CancellationToken());

            if (result.Data)
            {
                return StatusCode(StatusCodes.Status200OK, "Mail has successfully been sent.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured. The Mail could not be sent.");
            }
        }

        [HttpPost("sendemailwithattachment")]
        public async Task<ActionResult<ServiceResponse<bool>>> SendMailWithAttachmentAsync([FromForm] MailDataWithAttachments mailData)
        {
            var result = await _emailService.SendWithAttachmentsAsync(mailData, new CancellationToken());

            if (result.Data)
            {
                return StatusCode(StatusCodes.Status200OK, "Mail with attachment has successfully been sent.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured. The Mail with attachment could not be sent.");
            }
        }


        [HttpPost("sendemailusingtemplate")]
        public async Task<ActionResult> SendEmailUsingTemplate(WelcomeMail welcomeMail)
        {
            // Create MailData object
            MailData mailData = new MailData(
                new List<string> { welcomeMail.Email },
                "Welcome to the MailKit Demo",
                _emailService.GetEmailTemplate("welcome", welcomeMail));


            var sendResult = await _emailService.SendAsync(mailData, new CancellationToken());

            if (sendResult.Success)
            {
                return StatusCode(StatusCodes.Status200OK, "Mail has successfully been sent using template.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured. The Mail could not be sent.");
            }
        }



        [HttpPost("sendemailusingtemplateWithAttachments")]
        public async Task<ActionResult> SendEmailUsingTemplate([FromForm] MailDataWithAttachments mailData)
        {
     
            mailData.Body = _emailService.GetEmailTemplate("test", mailData);

            var result = await _emailService.SendWithAttachmentsAsync(mailData, new CancellationToken());

            if (result.Success)
            {
                return StatusCode(StatusCodes.Status200OK, "Mail has successfully been sent using template.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured. The Mail could not be sent.");
            }
        }


        [HttpPost("testArray")]
        public async Task<ActionResult> SendEmailUsingArray(WelcomeMail mailDatadd)
        {
            MailData mailData = new MailData(
               new List<string> { mailDatadd.Email },
               "Welcome to the MailKit Demo",
               null);

            //Send Email to User
            await _emailService.SendEmailAsyncArray(mailData, new CancellationToken());
            return StatusCode(StatusCodes.Status200OK, "Mail has successfully been sent using template.");

        }
    }
}


