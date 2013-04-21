using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryElectron.Domain
{
    public abstract class ChemicalGroup : IChemical, IEnumerator<IChemical>, IEnumerable<IChemical>
    {
        protected List<IChemical> contents = new List<IChemical>();

        protected Dictionary<string, Element> dictionaryOfElements;

        public ChemicalGroup()
        {
            initializeDictionaryOfElements();
        }

        public virtual void Add(IChemical chemical)
        {
            contents.Add(chemical);
        }

        public List<Element> ListOfElements
        {
            get
            {
                List<Element> elementList = new List<Element>();
                foreach (IChemical chemical in contents)
                    if (chemical is Element && !elementList.Contains(chemical))
                        elementList.Add((Element)chemical);
                    else if (chemical is ChemicalGroup)
                    {
                        ChemicalGroup chemicalGroup = (ChemicalGroup)chemical;
                        elementList = elementList.Union(chemicalGroup.ListOfElements).ToList();
                    }
                return elementList;
            }
        }

        public List<IChemical> ListOfContents
        {
            get
            {
                List<IChemical> listOfContents = new List<IChemical>();
                foreach (IChemical chemical in contents)
                {
                    if (!listOfContents.Contains(chemical))
                        listOfContents.Add(chemical);
                }
                return listOfContents;
            }
        }

        private void initializeDictionaryOfElements()
        {
            TableOfElements tableOfElements = new TableOfElements();
            dictionaryOfElements = new Dictionary<string, Element>();
            foreach (Element element in tableOfElements)
                dictionaryOfElements.Add(element.Symbol, element);
        }

        public int GetDeepElementCount(Element elementToCheck)
        {
            int count = 0;
            foreach (IChemical chemical in contents)
            {
                if (chemical == elementToCheck)
                    count++;
                else if (chemical is ChemicalGroup)
                {
                    ChemicalGroup chemicalGroup = (ChemicalGroup)chemical;
                    count += chemicalGroup.GetDeepElementCount(elementToCheck);
                }
            }
            return count;
        }

        public int GetShallowChemicalCount(IChemical chemicalToCheck)
        {
            int count = 0;
            foreach (IChemical chemical in contents)
                if (chemicalToCheck == chemical)
                    count++;
            return count;
        }

        public virtual double Mass
        {
            get
            {
                double mass = 0;
                foreach (IChemical chemical in contents)
                    mass += chemical.Mass;
                return mass;
            }
        }

        public abstract string ToHTML();

        int position = -1;

        public IChemical Current
        {
            get { return contents[position]; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return contents[position]; }
        }

        public bool MoveNext()
        {
            position++;
            return (position < contents.Count);
        }

        public void Reset()
        {
            position = 0;
        }

        public IEnumerator<IChemical> GetEnumerator()
        {
            return (IEnumerator<IChemical>)this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (IEnumerator<IChemical>)this;
        }

        public void Dispose()
        {
        }

        public int Count
        {
            get
            {
                return contents.Count;
            }
        }

        public IChemical this[int i]
        {
            get { return contents[i]; }
            set { contents[i] = value; }
        }
    }
}