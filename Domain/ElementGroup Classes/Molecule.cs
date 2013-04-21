using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryElectron.Domain
{
    class Molecule : ChemicalGroup
    {
        public override void Add(IChemical chemical)
        {
            if (chemical is IMoleculeContent)
                base.Add(chemical);
            else
                throw new ArgumentException("Error: Molecules may only contain complexes and elements");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IChemical chemical in ListOfContents)
            {
                int subScript = GetShallowChemicalCount(chemical);
                if (subScript == 1)
                    sb.Append(chemical.ToString());
                else
                    sb.Append(chemical.ToString() + subScript.ToString());
            }
            return sb.ToString();
        }

        public override string ToHTML()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IChemical chemical in ListOfContents)
            {
                int subScript = GetShallowChemicalCount(chemical);
                if (subScript == 1)
                    sb.Append(chemical.ToHTML());
                else
                    sb.Append(chemical.ToHTML() + "<sub>" + subScript.ToString() + "</sub>");
            }
            return sb.ToString();
        }
    }
}