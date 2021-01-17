using II_VI_Incorporated_SCM.Models.NCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace II_VI_Incorporated_SCM.Models.MeetingNote
{
    public class MeetingNoteViewmodel
    {
        public string MINUTES_NUM { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string SUBJECT { get; set; }
        public string MINUTES_CONTENT { get; set; }
        public System.DateTime MEETING_DATE { get; set; }
        public string STATUS { get; set; }
        public string ATT_PATH { get; set; }
        public List<string> FILE_NAME { get; set; }
        public List<string> ATTENDANT { get; set; }
        public string ATTEND_NAME { get; set; }
        public string EMAIL { get; set; }
        public List<EvidenceView> ModelEvidence { get; set; }
        public List<MEETING_ATT> OldEvidence { get; set; }

    }
}