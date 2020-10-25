using ValueCards.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ValueCards.Services
{
  public interface IRichEmailSender
  {
    Task SendEmailAsync(string recipient, string senderName, string subject, string htmlBody, params object[] attachments);
  }

  public class EmailSender : IEmailSender, IRichEmailSender
  {
    private readonly EmailSettings _emailSettings;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        IOptions<EmailSettings> emailSettings,
        IWebHostEnvironment env,
        ILogger<EmailSender> logger)
    {
      _emailSettings = emailSettings.Value;
      _env = env;
      _logger = logger;
    }

    public async Task SendEmailAsync(string recipient, string subject, string message)
    {
      try
      {
        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));

        mimeMessage.To.Add(new MailboxAddress(recipient));

        mimeMessage.Subject = subject;

        mimeMessage.Body = new TextPart("html")
        {
          Text = message
        };

        using (var client = new SmtpClient())
        {
          // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
          client.ServerCertificateValidationCallback = (s, c, h, e) => true;

          if (_env.IsDevelopment())
          {
            // The third parameter is useSSL (true if the client should make an SSL-wrapped
            // connection to the server; otherwise, false).
            await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, false);
          }
          else
          {
            await client.ConnectAsync(_emailSettings.MailServer);
          }

          // Note: only needed if the SMTP server requires authentication
          await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

          await client.SendAsync(mimeMessage);

          await client.DisconnectAsync(true);
        }

      }
      catch (Exception ex)
      {
        // TODO: handle exception
        throw new InvalidOperationException(ex.Message);
      }
    }

    public async Task SendEmailAsync(string recipient, string senderName, string subject, string htmlBody, params object[] attachments)
    {
      try
      {
        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(new MailboxAddress(senderName, _emailSettings.Sender));

        mimeMessage.To.Add(new MailboxAddress(recipient));

        mimeMessage.Subject = subject;

        var builder = new BodyBuilder();

        //var arguments = new List<string>();

        int index = 0;
        foreach (var attachment in attachments)
        {
          if (attachment is Bitmap bitmap)
          {
            using(var stream = new MemoryStream())
            {
              bitmap.Save(stream, ImageFormat.Jpeg);
              var image = builder.LinkedResources.Add($"image_{index + 1}.jpg", stream.ToArray());
              image.ContentId = MimeUtils.GenerateMessageId();
              //arguments.Add(image.ContentId);

              htmlBody = htmlBody.Replace($"cid:{{{index}}}", $"cid:{image.ContentId}");
            }
          }
          index++;
        }

        builder.HtmlBody = htmlBody;

        _logger.LogDebug(builder.HtmlBody);

        mimeMessage.Body = builder.ToMessageBody();

        using (var client = new SmtpClient())
        {
          // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
          client.ServerCertificateValidationCallback = (s, c, h, e) => true;

          if (_env.IsDevelopment())
          {
            // The third parameter is useSSL (true if the client should make an SSL-wrapped
            // connection to the server; otherwise, false).
            await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, false);
          }
          else
          {
            await client.ConnectAsync(_emailSettings.MailServer);
          }

          // Note: only needed if the SMTP server requires authentication
          await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);

          await client.SendAsync(mimeMessage);

          await client.DisconnectAsync(true);
        }

      }
      catch (Exception ex)
      {
        // TODO: handle exception
        throw new InvalidOperationException(ex.Message);
      }
    }


  }

}
