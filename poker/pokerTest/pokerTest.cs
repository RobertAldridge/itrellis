
// file name: pokerTest.cs

using Console = System.Console;
using Directory = System.IO.Directory;
using File = System.IO.File;
using PokerClient = ConsoleApplication1.ServiceReference1.PokerClient;
using StringComparison = System.StringComparison;
using StringList = System.Collections.Generic.List<string>;
using XElement = System.Xml.Linq.XElement;

namespace PokerTest
{
    class PokerTest
    {
        static string s_inputDataFolderName = System.IO.Directory.GetCurrentDirectory() + "\\testcase";
        static string s_outputVerifyDataFolderName = System.IO.Directory.GetCurrentDirectory() + "\\verification";

        static int Main(string[] args)
        {
            int resultCount = 0;

            PokerClient blah = new PokerClient("BasicHttpBinding_IPoker", "http://localhost:1682/Poker.svc");

            // read list of input file names to web service
            StringList inputDataFileNames = new StringList(Directory.GetFiles(PokerTest.s_inputDataFolderName, "*.xml"));
            inputDataFileNames.Sort();

            // read list of verification file names to match against output of web service
            StringList outputVerifyDataFileNames = new StringList(Directory.GetFiles(PokerTest.s_outputVerifyDataFolderName, "*.xml"));
            outputVerifyDataFileNames.Sort();

            if(inputDataFileNames.Count != outputVerifyDataFileNames.Count)
            {
                Console.WriteLine("input and verification file counts do not match");
                return -1;
            }

            for(int index = 0; index<inputDataFileNames.Count; index++)
            {
                // load input content and convert to XElement
                string inputContent = File.ReadAllText(inputDataFileNames[index] );
                XElement xElementInput = XElement.Parse(inputContent);

                // call web service api
                XElement xElementOutput = blah.OminousPokerFunction(xElementInput);

                // load verification content and convert to XElement
                string verifyContent = File.ReadAllText(outputVerifyDataFileNames[index]);
                XElement xElementVerify = XElement.Parse(verifyContent);

                // convert XElements back to strings for comparison
                string blahOutput = xElementOutput.ToString();
                while(blahOutput.Contains("  "))
                    blahOutput = blahOutput.Replace("  ", " ");
                blahOutput = blahOutput.Replace(" xmlns=\"\"", "");

                string blahVerify = xElementVerify.ToString();
                while(blahVerify.Contains("  "))
                    blahVerify = blahVerify.Replace("  ", " ");
                blahVerify = blahVerify.Replace(" xmlns=\"\"", "");

                // compare output of web api to verification data
                if(string.Equals(blahOutput, blahVerify, StringComparison.InvariantCultureIgnoreCase))
                    resultCount++;
            }

            int numFail = inputDataFileNames.Count - resultCount;
            if(resultCount==inputDataFileNames.Count)
            {
                Console.WriteLine("tests all pass");
                return 0;
            }
            
            Console.WriteLine(numFail.ToString() + "tests fail");
            return -1;
        }
    }
}
