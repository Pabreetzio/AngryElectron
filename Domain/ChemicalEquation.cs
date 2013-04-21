using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryElectron.Domain
{
    public class ChemicalEquation
    {
        public EquationSide Reactants = new EquationSide();
        public EquationSide Products = new EquationSide();

        public int MoleculeCount { get { return Reactants.Count + Products.Count; } }

        public List<Element> ListOfElements
        {
            get
            {
                List<Element> listOfElements = Reactants.ListOfElements.Union(Products.ListOfElements).ToList();
                return listOfElements;
            }
        }

        public void AddToEquation(IChemical chemical, Side side)
        {
            if (side == Side.LeftSide)
                Reactants.Add(chemical);
            if (side == Side.RightSide)
                Products.Add(chemical);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Reactants.ToString());
            sb.Append(" -> ");
            sb.Append(Products.ToString());
            return sb.ToString();
        }

        public string ToHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Reactants.ToHTML());
            sb.Append(" -> ");
            sb.Append(Products.ToHTML());
            return sb.ToString();
        }

        public int GetDeepElementCount(Element element)
        {
            return Products.GetDeepElementCount(element) + Reactants.GetDeepElementCount(element);
        }

        public bool IsBalanced
        {
            get
            {
                if (Reactants.Mass == Products.Mass)
                    return true;
                else
                    return false;
            }
        }
    }
}