# Sending-Email-.Net-Api
This Repository build and show how to send emails with ASP.NET Core API 6 . Our API will send emails in plain text, support attachments (multiple attachments at once) and also Sending Branded Emails using HTML Templates like Welcome Emails you received after Login/Register .
# Sending Emails with ASP.NET Core API 6 using SMTP

This repository contains code for sending emails with attachments and HTML templates using an ASP.NET Core API 6 and SMTP (Simple Mail Transfer Protocol). The examples below demonstrate how to send plain text emails with attachments and welcome emails with an HTML template.

## Prerequisites

Before you start, make sure you have the following:

1. [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. A Gmail account (or any other email provider that supports SMTP)

## Setup SMTP Configuration

For sending emails using Gmail's SMTP server, you can use the following settings:

- Host: smtp.gmail.com
- Port: 587
- Enable SSL: false
- Enable TLS: true

## How to Use
1. Clone the repository to your local machine.

```bash
git clone https://github.com/your-username/your-repo.git
```

2. Navigate to the project folder.
```bash
cd your-repo
```

3. Open the appsettings.json file and update the email settings section with your Gmail account details.
```json
{
  "MailSettings": {
    "Mail": "your-gmail-account@gmail.com",
    "Password": "your-gmail-password",
    "Host": "smtp.gmail.com",
    "Port": 587
  }
}
```

**Note:** It's recommended to use environment variables or secret management tools to securely store sensitive information like passwords.

4. Install the required NuGet packages if they are not already installed.
```bash
dotnet restore
```

5. Start the ASP.NET Core API project.
```bash
dotnet run
```

## Examples
### 1. Sending a Plain Text Email with Attachments
```csharp
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class EmailService
{
    // ... other methods ...

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

    // ... other methods ...
}

```

### 2. Sending a Welcome Email with HTML Template
```csharp
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class EmailService
{
    // ... other methods ...

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

    // ... other methods ...
}
```

## Summery 
That's it! You now have an ASP.NET Core API project capable of sending plain text emails with attachments and welcome emails using an HTML template. Make sure to replace your-gmail-account@gmail.com and your-gmail-password with your actual Gmail account credentials.

Feel free to customize the code as per your requirements, and don't forget to explore other features provided by the MailKit library for more advanced email functionality. Happy coding!

## References 
 - [Code with Mukesh Article](https://codewithmukesh.com/blog/send-emails-with-aspnet-core/)
