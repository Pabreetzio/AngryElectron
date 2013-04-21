using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AngryElectron.Domain;
using System.Collections.Generic;

namespace AngryElectron.Tests.Domain
{
    [TestClass]
    public class BalancerTest
    {
        Balancer myBalancer;
        ChemicalEquation myEquation;
        Parser myParser;

        [TestInitialize]
        public void Inititalize()
        {
            myBalancer = new Balancer();
            myParser = new Parser();
        }

        [TestMethod]
        public void BasicBalancerTest()
        {
            myEquation = myParser.Parse("FeS2 + O2 -> Fe2O3 + SO2");
            myEquation = myBalancer.Balance(myEquation);
            Assert.IsTrue(myEquation.ToString() == "4FeS2 + 11O2 -> 2Fe2O3 + 8SO2");
        }

        [TestMethod]
        public void BalancerHTMLTest()
        {
            myEquation = myParser.Parse("FeS2 + O2 -> Fe2O3 + SO2");
            myEquation = myBalancer.Balance(myEquation);
            Assert.IsTrue(myEquation.ToHTML() == "4FeS<sub>2</sub> + 11O<sub>2</sub> -> 2Fe<sub>2</sub>O<sub>3</sub> + 8SO<sub>2</sub>");
        }

        [TestMethod]
        public void BalancerWithComplexTest()
        {
            myEquation = myParser.Parse("CaCl2 + Ag(NO3) -> Ca(NO3)2 + AgCl");
            myEquation = myBalancer.Balance(myEquation);
            Assert.IsTrue(myEquation.ToString() == "CaCl2 + 2Ag(NO3) -> Ca(NO3)2 + 2AgCl");
        }

        [TestMethod]
        public void BalancerHTMLWithComplexTest()
        {
            myEquation = myParser.Parse("CaCl2 + Ag(NO3) -> Ca(NO3)2 + AgCl");
            myEquation = myBalancer.Balance(myEquation);
            Assert.IsTrue(myEquation.ToHTML() == "CaCl<sub>2</sub> + 2Ag(NO<sub>3</sub>) -> Ca(NO<sub>3</sub>)<sub>2</sub> + 2AgCl");
        }

        [TestMethod]
        public void BalancerWithLoneElementTest()
        {
            myEquation = myParser.Parse("HCl + Na = NaCl + H2");
            myEquation = myBalancer.Balance(myEquation);
            Assert.IsTrue(myEquation.ToString() == "2HCl + 2Na -> 2NaCl + H2");
        }

        [TestMethod]
        public void BalancerHTMLWithLoneElementTest()
        {
            myEquation = myParser.Parse("HCl + Na = NaCl + H2");
            myEquation = myBalancer.Balance(myEquation);
            Assert.IsTrue(myEquation.ToHTML() == "2HCl + 2Na -> 2NaCl + H<sub>2</sub>");
        }

        [TestMethod]
        public void UnusualInputTest()
        {
            myEquation = myParser.Parse("CaCl2 + Ag[NO3] ==> Ca[NO3]2 + AgCl");
            myEquation = myBalancer.Balance(myEquation);
            Assert.IsTrue(myEquation.ToString() == "CaCl2 + 2Ag(NO3) -> Ca(NO3)2 + 2AgCl");
        }

        [TestMethod]
        public void IsBalancedTest()
        {
            myEquation = myParser.Parse("FeS2 + O2 -> Fe2O3 + SO2");
            myEquation = myBalancer.Balance(myEquation);
            Assert.IsTrue(myEquation.IsBalanced);
        }
    }
}
