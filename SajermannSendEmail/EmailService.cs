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
    public static async Task Send(ModelSajermannSendEmail model)
    {

      #region GetAppSettings
      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())  //location of the exe file
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

      IConfigurationRoot configuration = builder.Build();
      #endregion


      #region ConfigFluentEmail
      var sender = new SmtpSender(() => new SmtpClient(configuration.GetSection("SajermannSendEmailConfigs")["Host"])
      {
        UseDefaultCredentials = Convert.ToBoolean(configuration.GetSection("SajermannSendEmailConfigs")["UseDefaultCredentials"]),
        EnableSsl = Convert.ToBoolean(configuration.GetSection("SajermannSendEmailConfigs")["EnableSsl"]),
        DeliveryMethod = SmtpDeliveryMethod.Network,
        Port = int.Parse(configuration.GetSection("SajermannSendEmailConfigs")["Port"]),
        Credentials = new NetworkCredential(configuration.GetSection("SajermannSendEmailConfigs")["Username"], configuration.GetSection("SajermannSendEmailConfigs")["Password"])
      });
      Email.DefaultSender = sender;
      #endregion

      try
      {
        string emailMounted = BuildHtml($"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}/TemplateSajermannSendEmail/{model.TemplateName}.html", model.MessageForHandlerbars);
        var email = Email
            .From(model.EmailFrom.EmailAddress, model.EmailFrom.Name)
            .To(model.EmailsTo.ToAddressFluent())
            .CC(model.EmailsCc.ToAddressFluent())
            .BCC(model.EmailsBcc.ToAddressFluent())
            .Subject(model.Subject)
            .Attach(model.Attachments.ToAttachmentFluent())
            .UsingTemplate(emailMounted, model.MessageForHandlerbars);

        var t = await email.SendAsync();
      }
      catch (Exception e)
      {
        var erro = e.Message;
      }
    }

    private static List<Address> ToAddressFluent(this List<ModelSajermannSendEmail.Address> model)
    {
      var result = new List<Address>();
      if (model == null || model.Count == 0) return result;
      foreach(var item in model)
      {
        var itemComplete = new Address(item.EmailAddress, item.Name);
        result.Add(itemComplete);
      }
      return result;
    }

    private static List<FluentEmail.Core.Models.Attachment> ToAttachmentFluent(this List<ModelSajermannSendEmail.Attachment> model)
    {
      var result = new List<FluentEmail.Core.Models.Attachment>();
      if (model == null || model.Count == 0) return result;
      foreach (var item in model)
      {
        var itemComplete = new FluentEmail.Core.Models.Attachment();
        itemComplete.IsInline = item.IsInline;
        itemComplete.Filename = item.Filename;
        itemComplete.Data = item.Data;
        itemComplete.ContentType = item.ContentType;
        itemComplete.ContentId = item.ContentId;
        result.Add(itemComplete);
      }
      return result;
    }

    private static string BuildHtml(string fileTemplate, object data)
    {
      string source = File.ReadAllText(fileTemplate);

      var template = Handlebars.Compile(source);

      return template(data);
    }
  }
}
