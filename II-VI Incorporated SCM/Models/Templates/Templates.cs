using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace II_VI_Incorporated_SCM.Models.Templates
{
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("Templates")]
    public class Templates
    {
        [XmlArray("Emails")]
        [XmlArrayItem("Email", typeof(Email))]
        public Email[] Email { get; set; }

        public Email[] GetEMailTemplate(string path)
        {
            Templates emails = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Templates));

            StreamReader reader = new StreamReader(path);
            emails = (Templates)serializer.Deserialize(reader);
            reader.Close();

            return emails.Email;
        }

        public Email GetEmailByMailName(string MailName, string path)
        {
            var emails = GetEMailTemplate(path);
            return emails.Length > 0 ? emails.FirstOrDefault(x => x.MailName.Equals(MailName)) : null;
        }
    }
}