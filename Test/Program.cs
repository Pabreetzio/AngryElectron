using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngryElectron.Domain;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //writeBalancedEquation("Na2CO3 + HCl -> NaCl + H2O + CO2");
            //writeBalancedEquation("FeS2 + O2 -> Fe2O3 + SO2");
            //writeBalancedEquation("SnO2 + H2 -> Sn + H2O");
            //writeBalancedEquation("H2 + O2 -> H2O");
            writeBalancedEquation("Fe2(SO4)3 + KOH -> K2SO4 + Fe(OH)3");
            //writeBalancedEquation("KMnO4 + HCl = KCl + MnCl2 + H2O + Cl2");

            //writeParsableSymbols("2Co2 + H2O -> C6H12O6 + O2");

            writeBalancedEquation("HCl + Na -> NaCl + H2");

            Balancer myBalancer = new Balancer();

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Press any key to end debugging...");
            Console.ReadKey();
        }

        private static void writeBalancedEquation(string myString)
        {
            Parser myParser = new Parser();
            Balancer myBalancer = new Balancer();
            ChemicalEquation myEquation;
            try
            {
                myEquation = myParser.Parse(myString);
                myEquation = myBalancer.Balance(myEquation);
                Console.WriteLine(myEquation.ToString());
                Console.WriteLine(myEquation.ToHTML());
                Console.Write(Environment.NewLine);
            }
            catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
        }
    }
}
