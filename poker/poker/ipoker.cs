
// file name: ipoker.cs

using DataContractAttribute = System.Runtime.Serialization.DataContractAttribute;
using DataMemberAttribute = System.Runtime.Serialization.DataMemberAttribute;
using OperationContractAttribute = System.ServiceModel.OperationContractAttribute;
using ServiceContractAttribute = System.ServiceModel.ServiceContractAttribute;

namespace Poker
{
    [ServiceContract]
    public interface IPoker
    {
        [OperationContract]
        string OminousPokerFunction();

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
