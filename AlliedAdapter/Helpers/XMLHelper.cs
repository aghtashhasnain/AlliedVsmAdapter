using System.IO;
using System.Linq;
using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using AlliedAdapter;
using System.Runtime.InteropServices;
using Microsoft.Build.Tasks;
using System.Xml;

public class XMLHelper
{
    public static string ExtractInnerXml(string xml)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNode rootNode = doc.SelectSingleNode("//Root");

        if (rootNode != null)
        {
            return rootNode.OuterXml;  // Extract entire `<Root>` content
        }

        throw new Exception("Invalid SOAP response: <Root> not found.");
    }

    public static string FixNestedCardInfo(string xml)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNode outerCardInfoNode = doc.SelectSingleNode("//Output/CardInfo");

        if (outerCardInfoNode != null)
        {
            XmlNode parentOutputNode = outerCardInfoNode.ParentNode;

            foreach (XmlNode innerCardInfo in outerCardInfoNode.ChildNodes)
            {
                parentOutputNode.AppendChild(innerCardInfo.CloneNode(true));
            }

            parentOutputNode.RemoveChild(outerCardInfoNode); // Remove extra `<CardInfo>` wrapper
        }

        return doc.OuterXml;
    }

    public static T DeserializeXml<T>(string xml)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (StringReader reader = new StringReader(xml))
        {
            return (T)serializer.Deserialize(reader);
        }
    }
}
