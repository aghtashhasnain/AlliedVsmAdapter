using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AlfalahAdapter.Repositories
{
    public class SOAServicesRepository
    {
        #region Service response classes
        public class CustomerEnquiryService
        {
			[XmlRoot(ElementName = "Customer")]
			public class Customer
			{
				[XmlElement(ElementName = "CustomerId")]
				public string CustomerId { get; set; }
				[XmlElement(ElementName = "CustomerName")]
				public string CustomerName { get; set; }
				[XmlElement(ElementName = "Nationality")]
				public string Nationality { get; set; }
				[XmlElement(ElementName = "CellNumber")]
				public string CellNumber { get; set; }
				[XmlElement(ElementName = "Email")]
				public string Email { get; set; }
				[XmlElement(ElementName = "IdType")]
				public string IdType { get; set; }
				[XmlElement(ElementName = "IdNumber")]
				public string IdNumber { get; set; }
				[XmlElement(ElementName = "Address")]
				public string Address { get; set; }
			}

			[XmlRoot(ElementName = "Data")]
			public class Data
			{
				[XmlElement(ElementName = "Customer")]
				public Customer Customer { get; set; }
			}

			[XmlRoot(ElementName = "GetDetailsByCustomerIdResultOld")]
			public class GetDetailsByCustomerIdResultOld
			{
				[XmlElement(ElementName = "ResponseCode")]
				public string ResponseCode { get; set; }
				[XmlElement(ElementName = "ResponseDescription")]
				public string ResponseDescription { get; set; }
				[XmlElement(ElementName = "Data")]
				public Data Data { get; set; }
			}

            [XmlRoot(ElementName = "GetDetailsByCustomerIdResultOldForAD")]
            public class GetDetailsByCustomerIdResultOldForAD
            {
                [XmlElement(ElementName = "ValidateLDAPResult")]
                public bool ValidateLDAPResult { get; set; }
            }

            [XmlRoot(ElementName = "GetDetailsByCustomerIdResult")]
			public class GetDetailsByCustomerIdResult
			{
				[XmlElement(ElementName = "ResponseCode")]
				public string ResponseCode { get; set; }
				[XmlElement(ElementName = "ResponseDescription")]
				public string ResponseDescription { get; set; }
			}
		}

		public class RapidAccountOpeningService
        {
			[XmlRoot(ElementName = "CustomerOpeningResponse", Namespace = "http://AccountOpening")]
			public class CustomerOpeningResponse
			{
				[XmlElement(ElementName = "ResponseCode")]
				public string ResponseCode { get; set; }
				[XmlElement(ElementName = "ResponseDesc")]
				public string ResponseDesc { get; set; }
				[XmlElement(ElementName = "CustomerID")]
				public string CustomerID { get; set; }
				[XmlAttribute(AttributeName = "NS1", Namespace = "http://www.w3.org/2000/xmlns/")]
				public string NS1 { get; set; }
			}
		}
        #endregion

        public T DeserializeSOAPResponse<T>(string xNamespace, string xmlResponse, string targetNode, int targetNodeLevel)
        {
			var xDocument = XDocument.Parse(xmlResponse);
			var rootNode = xDocument.Descendants((XNamespace)xNamespace + "Body").First().FirstNode;

			XmlSerializer oXmlSerializer = new XmlSerializer(typeof(T));

			if (targetNodeLevel > 0)
            {
				var firstNode = XDocument.Parse(rootNode.ToString());
				var secondNode = firstNode.Descendants(targetNode).First();

				return (T)oXmlSerializer.Deserialize(secondNode.CreateReader());
			}

			return (T)oXmlSerializer.Deserialize(rootNode.CreateReader());
		}
    }
}
