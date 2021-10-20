using System.Collections.Generic;
using System.IO;

namespace SajermannSendEmail
{
  public class ModelSajermannSendEmail
  {
    public Address EmailFrom { get; set; }
    public List<Address> EmailsTo { get; set; }
    public List<Attachment> Attachments { get; set; }
    public List<Address> EmailsCc { get; set; }
    public List<Address> EmailsBcc { get; set; }
    public object MessageForHandlerbars { get; set; }
    public string Subject { get; set; }
    public string TemplateName { get; set; }

    public class Address
    {
      public string EmailAddress { get; set; }
      public string Name { get; set; }
    }
  
    public class Attachment
    {
      public bool IsInline { get; set; }
      public string Filename { get; set; }
      public Stream Data { get; set; }
      public string ContentType { get; set; }
      public string ContentId { get; set; }
    }
  }
}
