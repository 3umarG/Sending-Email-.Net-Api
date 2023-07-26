using SendingEmailApi.Models;

namespace SendingEmailApi.Services
{
	public interface IMailService
	{
		public Task SendEmailAsync(MailRequest mailRequest);
	}
}
