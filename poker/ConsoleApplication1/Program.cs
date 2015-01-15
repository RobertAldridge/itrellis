
using Console = System.Console;
using Encoding = System.Text.Encoding;
using HttpWebRequest = System.Net.HttpWebRequest;
using HttpWebResponse = System.Net.HttpWebResponse;
using Message = System.ServiceModel.Channels.Message;
using PokerClient = ConsoleApplication1.ServiceReference1.PokerClient;
using Stream = System.IO.Stream;
using StreamReader = System.IO.StreamReader;
using WebRequest = System.Net.WebRequest;
using XElement = System.Xml.Linq.XElement;
using XmlWriter = System.Xml.XmlWriter;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            PokerClient blah = new PokerClient("BasicHttpBinding_IPoker", "http://localhost:1682/Poker.svc");

            string xml =
"<?xml version=\"1.0\" encoding=\"utf-8\" ?>"+
"<hands>"+
  "<hand>"+
   "<name>Ted</name>"+
    "<card>2H</card>"+
    "<card>3D</card>"+
    "<card>5S</card>"+
    "<card>9C</card>"+
    "<card>KD</card>"+
  "</hand>"+
  "<hand>"+
    "<name>Louis</name>"+
    "<card>2C</card>"+
    "<card>3H</card>"+
    "<card>4S</card>"+
    "<card>8C</card>"+
    "<card>AH</card>"+
  "</hand>"+
"</hands>"
;
            XElement xElement = XElement.Parse(xml);

            string result = blah.OminousPokerFunction(xElement);
        }
    }
}
