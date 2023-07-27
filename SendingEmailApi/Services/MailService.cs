using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SendingEmailApi.Models;

namespace SendingEmailApi.Services
{
	public class MailService : IMailService
	{
		private readonly MailSettings _mailSettings;

		public MailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}

		public async Task SendEmailAsync(MailRequest mailRequest)
		{
			var email = new MimeMessage();

			email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
			email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
			email.Subject = mailRequest.Subject;

			var builder = new BodyBuilder();

			if (mailRequest.Attachments is not null)
			{
				byte[] fileBytes;

				foreach (var file in mailRequest.Attachments)
				{
					if (file.Length > 0)
					{
						// convert file to byte[]
						using var memoryStream = new MemoryStream();
						file.CopyTo(memoryStream);
						fileBytes = memoryStream.ToArray();

						builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
					}
				}
			}

			builder.HtmlBody = mailRequest.Body;
			email.Body = builder.ToMessageBody();
			using var smtp = new SmtpClient();
			smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
			await smtp.SendAsync(email);
			smtp.Disconnect(true);
		}

		public async Task SendWelcomeEmailAsync(WelcomeMailRequest request)
		{
			string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\WelcomeTemplate.html";
			using StreamReader str = new(FilePath);
			string MailText = str.ReadToEnd();
			MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
			var email = new MimeMessage
			{
				Sender = MailboxAddress.Parse(_mailSettings.Mail)
			};
			email.To.Add(MailboxAddress.Parse(request.ToEmail));
			email.Subject = $"Welcome {request.UserName}";
			var builder = new BodyBuilder
			{
				HtmlBody = MailText
			};
			email.Body = builder.ToMessageBody();
			using var smtp = new SmtpClient();
			smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
			smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
			await smtp.SendAsync(email);
			smtp.Disconnect(true);
		}
	}
}
