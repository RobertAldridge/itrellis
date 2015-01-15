
// file name: ipoker.cs

using DataContractAttribute = System.Runtime.Serialization.DataContractAttribute;
using DataMemberAttribute = System.Runtime.Serialization.DataMemberAttribute;
//using ListInt = System.Collections.Generic.List<int>;
using OperationContractAttribute = System.ServiceModel.OperationContractAttribute;
using ServiceContractAttribute = System.ServiceModel.ServiceContractAttribute;
using WebGetAttribute = System.ServiceModel.Web.WebGetAttribute;
using WebInvokeAttribute = System.ServiceModel.Web.WebInvokeAttribute;
using WebMessageBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle;
using WebMessageFormat = System.ServiceModel.Web.WebMessageFormat;
using XElement = System.Xml.Linq.XElement;

namespace Poker
{
    [ServiceContract]
    public interface IPoker
    {
        [OperationContract]
        XElement OminousPokerFunction(XElement input);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }

    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get
            {
                return boolValue;
            }
            set
            {
                boolValue = value;
            }
        }

        [DataMember]
        public string StringValue
        {
            get
            {
                return stringValue;
            }
            set
            {
                stringValue = value;
            }
        }
    }
}
