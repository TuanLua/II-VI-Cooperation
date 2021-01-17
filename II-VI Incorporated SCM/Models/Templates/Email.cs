using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.Templates
{
    [Serializable()]
    public class Email
    {
        [System.Xml.Serialization.XmlElement("MailName")]
        public string MailName { get; set; }

        [System.Xml.Serialization.XmlElement("ProfileName")]
        public string ProfileName { get; set; }

        [System.Xml.Serialization.XmlElement("Body")]
        public string Body { get; set; }
        [System.Xml.Serialization.XmlElement("BodyFormat")]
        public string BodyFormat { get; set; }
        [System.Xml.Serialization.XmlElement("Subject")]
        public string Subject { get; set; }

        public string RecipientName { get; set; }
        public string MailAddress { get; set; }

        public static class Keys
        {
            public static string StringNCRNUM = "@@str_NCRNUMBER@@";
            public static string LinkNCRNUM = "@@link_NCRNUMBER";
            public static string StringRecipientName = "@@str_RecipientName@@";
            public static string StringComment = "@@str_Comment@@";
            public static string StringReason = "@@str_reason@@";
            public static string StringUser = "@@str_user@@";
            public static string StringForm = "@@str_form@@";
        }

    }
}