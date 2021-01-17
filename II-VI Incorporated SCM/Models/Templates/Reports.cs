using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace II_VI_Incorporated_SCM.Models.Templates
{
    #region 4 Panel
    [XmlRoot(ElementName = "Month")]
    public class Month
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "RecQty")]
    public class RecQty
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "RejQty")]
    public class RejQty
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "Target")]
    public class Target
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "PPMYTD")]
    public class PPMYTD
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "PPM")]
    public class PPM
    {
        [XmlElement(ElementName = "Month")]
        public Month Month { get; set; }
        [XmlElement(ElementName = "RecQty")]
        public RecQty RecQty { get; set; }
        [XmlElement(ElementName = "RejQty")]
        public RejQty RejQty { get; set; }
        [XmlElement(ElementName = "Target")]
        public Target Target { get; set; }
        [XmlElement(ElementName = "PPMYTD")]
        public PPMYTD PPMYTD { get; set; }
        [XmlAttribute(AttributeName = "begin")]
        public int Begin { get; set; }
    }

    [XmlRoot(ElementName = "Early")]
    public class Early
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "Late")]
    public class Late
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "OnTime")]
    public class OnTime
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "Total")]
    public class Total
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "Actual")]
    public class Actual
    {
        [XmlAttribute(AttributeName = "begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "OTD")]
    public class OTD
    {
        [XmlElement(ElementName = "Month")]
        public Month Month { get; set; }
        [XmlElement(ElementName = "Early")]
        public Early Early { get; set; }
        [XmlElement(ElementName = "Late")]
        public Late Late { get; set; }
        [XmlElement(ElementName = "OnTime")]
        public OnTime OnTime { get; set; }
        [XmlElement(ElementName = "Total")]
        public Total Total { get; set; }
        [XmlElement(ElementName = "Target")]
        public Target Target { get; set; }
        [XmlElement(ElementName = "Actual")]
        public Actual Actual { get; set; }
        [XmlAttribute(AttributeName = "begin")]
        public int Begin { get; set; }
    }

    [XmlRoot(ElementName = "Ref")]
    public class Ref
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "Desc")]
    public class Desc
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "JUL")]
    public class JUL
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "AUG")]
    public class AUG
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "SEP")]
    public class SEP
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "OCT")]
    public class OCT
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "NOV")]
    public class NOV
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "DEC")]
    public class DEC
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "JAN")]
    public class JAN
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "FEB")]
    public class FEB
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "MAR")]
    public class MAR
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "APR")]
    public class APR
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "MAY")]
    public class MAY
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "JUN")]
    public class JUN
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "YTD")]
    public class YTD
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "ImprovementTracking")]
    public class ImprovementTracking
    {
        [XmlElement(ElementName = "Ref")]
        public Ref Ref { get; set; }
        [XmlElement(ElementName = "Desc")]
        public Desc Desc { get; set; }
        [XmlElement(ElementName = "JUL")]
        public JUL JUL { get; set; }
        [XmlElement(ElementName = "AUG")]
        public AUG AUG { get; set; }
        [XmlElement(ElementName = "SEP")]
        public SEP SEP { get; set; }
        [XmlElement(ElementName = "OCT")]
        public OCT OCT { get; set; }
        [XmlElement(ElementName = "NOV")]
        public NOV NOV { get; set; }
        [XmlElement(ElementName = "DEC")]
        public DEC DEC { get; set; }
        [XmlElement(ElementName = "JAN")]
        public JAN JAN { get; set; }
        [XmlElement(ElementName = "FEB")]
        public FEB FEB { get; set; }
        [XmlElement(ElementName = "MAR")]
        public MAR MAR { get; set; }
        [XmlElement(ElementName = "APR")]
        public APR APR { get; set; }
        [XmlElement(ElementName = "MAY")]
        public MAY MAY { get; set; }
        [XmlElement(ElementName = "JUN")]
        public JUN JUN { get; set; }
        [XmlElement(ElementName = "YTD")]
        public YTD YTD { get; set; }
        [XmlAttribute(AttributeName = "Begin")]
        public int Begin { get; set; }
    }

    [XmlRoot(ElementName = "CAD")]
    public class CAD
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "TargetDate")]
    public class TargetDate
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "ActualDate")]
    public class ActualDate
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "Status")]
    public class Status
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "Owner")]
    public class Owner
    {
        [XmlAttribute(AttributeName = "Begin")]
        public string Begin { get; set; }
    }

    [XmlRoot(ElementName = "SCARProblem")]
    public class SCARProblem
    {
        [XmlElement(ElementName = "Ref")]
        public Ref Ref { get; set; }
        [XmlElement(ElementName = "CAD")]
        public CAD CAD { get; set; }
        [XmlElement(ElementName = "TargetDate")]
        public TargetDate TargetDate { get; set; }
        [XmlElement(ElementName = "ActualDate")]
        public ActualDate ActualDate { get; set; }
        [XmlElement(ElementName = "Status")]
        public Status Status { get; set; }
        [XmlElement(ElementName = "Owner")]
        public Owner Owner { get; set; }
        [XmlAttribute(AttributeName = "Begin")]
        public int Begin { get; set; }
    }

    [XmlRoot(ElementName = "FourPanel")]
    public class FourPanel
    {
        [XmlElement(ElementName = "PPM")]
        public PPM PPM { get; set; }
        [XmlElement(ElementName = "OTD")]
        public OTD OTD { get; set; }
        [XmlElement(ElementName = "ImprovementTracking")]
        public ImprovementTracking ImprovementTracking { get; set; }
        [XmlElement(ElementName = "SCARProblem")]
        public SCARProblem SCARProblem { get; set; }
        [XmlAttribute(AttributeName = "FullNameIndex")]
        public int FullNameIndex { get; set; }
        [XmlAttribute(AttributeName = "FullNameCol")]
        public string FullNameCol { get; set; }
        [XmlAttribute(AttributeName = "VendorIndex")]
        public int VendorIndex { get; set; }
        [XmlAttribute(AttributeName = "VendorCol")]
        public string VendorCol { get; set; }
        [XmlAttribute(AttributeName = "VendorText")]
        public string VendorText { get; set; }

        public FourPanel() { }
        public FourPanel(string path)
        {
            var tmp = GetEMailTemplate(path);
            this.FullNameCol = tmp.FullNameCol;
            this.FullNameIndex = tmp.FullNameIndex;
            this.ImprovementTracking = tmp.ImprovementTracking;
            this.OTD = tmp.OTD;
            this.PPM = tmp.PPM;
            this.SCARProblem = tmp.SCARProblem;
            this.VendorCol = tmp.VendorCol;
            this.VendorIndex = tmp.VendorIndex;
            this.VendorText = tmp.VendorText;
        }

        public FourPanel GetEMailTemplate(string path)
        {
            FourPanel fourPanel = new FourPanel();

            XmlSerializer serializer = new XmlSerializer(typeof(FourPanel));

            StreamReader reader = new StreamReader(path);
            fourPanel = (FourPanel)serializer.Deserialize(reader);
            reader.Close();

            return fourPanel;
        }
    }
    #endregion
}