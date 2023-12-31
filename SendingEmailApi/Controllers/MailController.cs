﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SendingEmailApi.Models;
using SendingEmailApi.Services;

namespace SendingEmailApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MailController : ControllerBase
	{
		private readonly IMailService _mailService;

		public MailController(IMailService mailService)
		{
			_mailService = mailService;
		}

		[HttpPost("send-email")]
		public async Task<IActionResult> SendEmailAsync([FromForm] MailRequest request)
		{
			try
			{
				await _mailService.SendEmailAsync(request);
				return Ok("Email Send Successfully ");
			}catch(Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("welcome-email")]
		public async Task<IActionResult> SendWelcomeEmailAsync([FromBody] WelcomeMailRequest request)
		{
			try
			{
				await _mailService.SendWelcomeEmailAsync(request);
				return Ok("Send Welcome Email Successfully !!");
			}catch(Exception e)
			{
				return BadRequest(e.Message);
			}
		}
	}
}
