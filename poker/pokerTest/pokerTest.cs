
// file name: pokerTest.cs

using Console = System.Console;
using Encoding = System.Text.Encoding;
using HttpWebRequest = System.Net.HttpWebRequest;
using HttpWebResponse = System.Net.HttpWebResponse;
using Message = System.ServiceModel.Channels.Message;
using PokerClient = ConsoleApplication1.ServiceReference1.PokerClient;
using ReaderOptions = System.Xml.Linq.ReaderOptions;
using Stream = System.IO.Stream;
using StreamReader = System.IO.StreamReader;
using WebRequest = System.Net.WebRequest;
using XElement = System.Xml.Linq.XElement;
using XmlDocument = System.Xml.XmlDocument;
using XmlReader = System.Xml.XmlReader;
using XmlReaderSettings = System.Xml.XmlReaderSettings;
using XmlWriter = System.Xml.XmlWriter;

namespace PokerTest
{
    class PokerTest
    {
        static void Main(string[] args)
        {
            PokerClient blah = new PokerClient("BasicHttpBinding_IPoker", "http://localhost:1682/Poker.svc");

            string fileContent = System.IO.File.ReadAllText("c:\\business\\pokerTest\\testcase\\poker01.xml");

            XElement xElementInput = XElement.Parse(fileContent);

            string simpleOutput = "";
            XElement xElementOutput = blah.OminousPokerFunction(xElementInput);

            XmlDocument xmlDocument = new System.Xml.XmlDocument();
            XmlReader xmlReader = XmlReader.Create(xElementOutput.CreateReader(ReaderOptions.OmitDuplicateNamespaces),
                new XmlReaderSettings(){IgnoreWhitespace = true} );

            xmlReader.MoveToContent();

            xmlReader.Read();

            while(!xmlReader.EOF)
            {
                if(xmlReader.Name == "" && xmlReader.Value != "")
                {
                    simpleOutput = xmlReader.Value;
                    break;
                }

                xmlReader.Read();

                if(!xmlReader.EOF)
                    xmlReader.MoveToContent();
            }

            xmlReader.Dispose();
        }
    }
}
