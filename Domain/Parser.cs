using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryElectron.Domain
{
    public class Parser : IParser
    {
        ChemicalGroupBuilder myBuilder = new ChemicalGroupBuilder();

        public ChemicalEquation Parse(string inputString)
        {
            string[] symbolArray = createSymbolArray(inputString);
            ChemicalEquation myChemicalEquation = convertArrayToEquation(symbolArray);
            checkForValidEquation(myChemicalEquation);
            return myChemicalEquation;  
        }

        private ChemicalEquation convertArrayToEquation(string[] symbolArray)
        {
            ChemicalEquation myChemicalEquation = new ChemicalEquation();
            List<string> moleculeString = new List<string>();
            Side parsingSide = Side.LeftSide;
            for (int i = 0; i < symbolArray.Length; i++)
            {
                if (symbolArray[i] == "+" || symbolArray[i] == ">" || symbolArray[i] == "|")  //Finding one of these operators tells us we're at the end of a molecule.
                {
                    myChemicalEquation.AddToEquation(myBuilder.buildChemicalGroup(moleculeString, false), parsingSide);
                    moleculeString = new List<string>(); //reset for the next loop
                    if (symbolArray[i] == ">")
                        parsingSide = Side.RightSide;
                }
                else
                    moleculeString.Add(symbolArray[i]); //If we didn't find an "end of molecule" operator, the symbol is added to moleculeString to be processed when we do find one. 
            }
            return myChemicalEquation;
        }

        private static string[] createSymbolArray(string inputString)
        {
            inputString = normalizeCharacters(inputString);
            StringBuilder commaSeperatedSymbols = generateCommaSeperatedSymbols(inputString);
            removeUnwantedCharacters(commaSeperatedSymbols);
            string[] symbolArray = commaSeperatedSymbols.ToString().Split(',');
            return symbolArray;
        }

        private static void removeUnwantedCharacters(StringBuilder commaSeperatedSymbols)
        {
            commaSeperatedSymbols.Replace(" ", "");
            commaSeperatedSymbols.Replace("-", "");
            removeExtraArrows(commaSeperatedSymbols);
        }

        private static void removeExtraArrows(StringBuilder commaSeperatedSymbols)
        {
            bool foundFirstArrow = false;
            for (int i = commaSeperatedSymbols.Length - 1; i > 0; i--)
            {
                if (commaSeperatedSymbols[commaSeperatedSymbols.Length - i] == '>')
                {
                    if (foundFirstArrow == false)
                        foundFirstArrow = true;
                    else
                        commaSeperatedSymbols.Remove(commaSeperatedSymbols.Length - i, 2);
                }
            }
        }

        private static StringBuilder generateCommaSeperatedSymbols(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inputString.Length; i++)
            {
                if ((Char.IsUpper(inputString[i]) || inputString[i] == '+' || inputString[i] == '>' || inputString[i] == ')' || inputString[i] == '(') && sb.Length > 0)
                    sb.Append(",");
                if (char.IsDigit(inputString[i]))
                {
                    if (i == 0)
                        throw new ArgumentException("Parser encountered an error: Equations may not begin with a number");
                    else
                        if (!char.IsDigit(inputString[i - 1])) //Make sure this is the last digit in case of a two digit number.
                            sb.Append(",");
                }
                sb.Append(inputString[i]);
            }
            sb.Append(",|"); //Pipe is used as an "end of equation" character for the parser.
            return sb;
        }

        private static string normalizeCharacters(string inputString)
        {
            inputString = inputString.Replace("[", "(");
            inputString = inputString.Replace("]", ")");
            inputString = inputString.Replace("=", ">");
            return inputString;
        }

        private void checkForValidEquation(ChemicalEquation unbalancedEquation)
        {
            foreach (Element e in unbalancedEquation.ListOfElements)
            {
                if (!unbalancedEquation.Products.ListOfElements.Contains(e) || !unbalancedEquation.Reactants.ListOfElements.Contains(e))
                    throw new ArgumentException("Error: the element or complex " + e.ToString() + " could not be found on both sides of the equation");
            }
        }
    }
}
