using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

[XmlRoot("Root")]
public class Root
{
    [XmlElement("WebMethodResponse")]
    public WebMethodResponse WebMethodResponse { get; set; }

    [XmlElement("Output")]
    public Output Output { get; set; }
}

public class WebMethodResponse
{
    public string ResponseCode { get; set; }
    public string ResponseDescription { get; set; }
}

public class Output
{
    [XmlElement("CardInfo")]
    public List<CardInfo> Cards { get; set; }
}

public class CardInfo
{
    public string CARDNUMBER { get; set; }
    public string CARDSTATUS { get; set; }
    public string CARDEXPIRYDATE { get; set; }
    public string ACCOUNTID { get; set; }
    public string DEFAULTACCOUNT { get; set; }
    public string CARDNAME { get; set; }
    public string PRODUCTCODE { get; set; }
    public string PRODUCTDESCRIPTION { get; set; }
}
