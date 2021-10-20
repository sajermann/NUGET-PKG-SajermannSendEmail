using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using HandlebarsDotNet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace SajermannSendEmail
{
  public static class EmailService
  {
    public static async Task Send(List<AddressSajermann> emailsTo, string message, List<string> emailsCc = null, List<string> emailsBcc = null)
    {

      #region GetAppSettings
      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())  //location of the exe file
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

      IConfigurationRoot configuration = builder.Build();
      #endregion


      #region ConfigFluentEmail
      var sender = new SmtpSender(() => new SmtpClient(configuration.GetSection("EmailConfigs")["Host"])
      {
        UseDefaultCredentials = Convert.ToBoolean(configuration.GetSection("EmailConfigs")["UseDefaultCredentials"]),
        EnableSsl = Convert.ToBoolean(configuration.GetSection("EmailConfigs")["EnableSsl"]),
        DeliveryMethod = SmtpDeliveryMethod.Network,
        Port = int.Parse(configuration.GetSection("EmailConfigs")["Port"]),
        Credentials = new NetworkCredential(configuration.GetSection("EmailConfigs")["Username"], configuration.GetSection("EmailConfigs")["Password"])
      });
      Email.DefaultSender = sender;
      #endregion


      var listEmailsTo = new List<Address>();
      foreach(var item in emailsTo)
      {
        var newItem = new Address(item.EmailAddress, item.Name);
        listEmailsTo.Add(newItem);
      }

      var batata = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      
      try
      {
        var order = new { name = message };
        string emailMounted = BuildHtml(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Mytemplate.html", order);

        var email = Email
            .From("systemsajermann@gmail.com", "Systemas Sajermann")
            .To(listEmailsTo) // Aceita List
                                                  //.CC("Email do Destinatário em Cópia") // Aceita List
                                                  //.BCC("Email do Destinatário em Cópia Oculta") // Aceita List
            .Subject("Teste")
            .UsingTemplate(emailMounted, order);

        var t = await email.SendAsync();


      }
      catch (Exception e)
      {
        var ttttt = e.Message;
      }
    }

    private static string BuildHtml(string fileTemplate, object data)
    {
      string source = File.ReadAllText(fileTemplate);

      var template = Handlebars.Compile(source);

      return template(data);
    }
  }
}
