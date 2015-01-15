
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using TripExpensesClient = ConsoleApplication1.ServiceReference1.TripExpensesClient;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //WebMessageEncoding isGreat = new WebMessageEncoding();

            TripExpensesClient blah = new TripExpensesClient("BasicHttpBinding_ITripExpenses", "http://localhost:1682/TripExpenses.svc");

            decimal[] expenseReport = new decimal[]{15.0m, 15.01m, 3.0m, 3.01m};
            decimal result = blah.GetData(expenseReport);
        }
    }
}
