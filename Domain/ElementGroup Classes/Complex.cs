using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryElectron.Domain
{
    class Complex : ChemicalGroup, IComplexContent, IMoleculeContent
    {
        public override void Add(IChemical chemical)
        {
            if (chemical is IComplexContent)
                base.Add(chemical);
            else
                throw new ArgumentException("Complexes may only contain other complexes and elements");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            foreach (IChemical chemical in ListOfContents)
            {
                int subScript = this.GetShallowChemicalCount(chemical);
                if (subScript == 1)
                    sb.Append(chemical.ToString());
                else
                    sb.Append(chemical.ToString() + subScript.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }

        public override string ToHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            foreach (IChemical chemical in ListOfContents)
            {
                int subScript = GetShallowChemicalCount(chemical);
                if (subScript == 1)
                    sb.Append(chemical.ToString());
                else
                    sb.Append(chemical.ToString() + "<sub>" + subScript.ToString() + "</sub>");
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}