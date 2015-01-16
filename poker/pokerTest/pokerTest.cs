
// file name: pokerTest.cs

using Console = System.Console;
using Debug = System.Diagnostics.Debug;
using Directory = System.IO.Directory;
using Encoding = System.Text.Encoding;
using File = System.IO.File;
using HttpWebRequest = System.Net.HttpWebRequest;
using HttpWebResponse = System.Net.HttpWebResponse;
using Message = System.ServiceModel.Channels.Message;
using PokerClient = ConsoleApplication1.ServiceReference1.PokerClient;
using ReaderOptions = System.Xml.Linq.ReaderOptions;
using Stream = System.IO.Stream;
using StreamReader = System.IO.StreamReader;
using WebRequest = System.Net.WebRequest;
using XElement = System.Xml.Linq.XElement;
//using XmlDocument = System.Xml.XmlDocument;
using XmlReader = System.Xml.XmlReader;
using XmlReaderSettings = System.Xml.XmlReaderSettings;
using XmlWriter = System.Xml.XmlWriter;

namespace PokerTest
{
    class PokerTest
    {
        static string s_inputDataFolderName = "c:\\business\\pokerTest\\testcase";
        static string s_outputVerifyDataFolderName = "c:\\business\\pokerTest\\verification";

        static int Main(string[] args)
        {
            int resultCount = 0;

            PokerClient blah = new PokerClient("BasicHttpBinding_IPoker", "http://localhost:1682/Poker.svc");

            string[] inputDataFileNames = Directory.GetFiles(PokerTest.s_inputDataFolderName, "*.xml");
            string[] outputVerifyDataFileNames = Directory.GetFiles(PokerTest.s_outputVerifyDataFolderName, "*.xml");

            Debug.Assert(inputDataFileNames.Length == outputVerifyDataFileNames.Length);

            for(int index = 0; index<PokerTest.s_inputDataFolderName.Length; index++)
            {
                string inputContent = File.ReadAllText(inputDataFileNames[index] );
                XElement xElementInput = XElement.Parse(inputContent);

                XElement xElementOutput = blah.OminousPokerFunction(xElementInput);

                string verifyContent = File.ReadAllText(outputVerifyDataFileNames[index]);
                XElement xElementVerify = XElement.Parse(verifyContent);

                if(XElement.DeepEquals(xElementInput, xElementOutput))
                    resultCount++;
            }

            if(resultCount==inputDataFileNames.Length)
            {
                return 0;
            }

            return -1;
        }
    }
}
