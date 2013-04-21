using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryElectron.Domain
{
    public class EquationSide : ChemicalGroup
    {
        public Dictionary<IChemical, int> Coefficients = new Dictionary<IChemical, int>();

        public override void Add(IChemical chemical)
        {
            Coefficients.Add(chemical, 1);
            base.Add(chemical);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < contents.Count; i++)
            {
                if (Coefficients[contents[i]] > 1)
                    sb.Append(Coefficients[contents[i]].ToString());
                sb.Append(contents[i].ToString());
                if (i != contents.Count - 1)
                    sb.Append(" + ");
            }
            return sb.ToString();
        }

        public override string ToHTML()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < contents.Count; i++)
            {
                if (Coefficients[contents[i]] > 1)
                    sb.Append(Coefficients[contents[i]].ToString());
                sb.Append(contents[i].ToHTML());
                if (i != contents.Count - 1)
                    sb.Append(" + ");
            }
            return sb.ToString();
        }

        public override double Mass
        {
            get
            {
                double mass = 0;
                foreach (IChemical chemical in contents)
                    mass += (chemical.Mass * Coefficients[chemical]);
                return mass;
            }
        }
    }
}