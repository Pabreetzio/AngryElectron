using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Solvers;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using System.Numerics;

namespace AngryElectron.Domain
{
    public class Balancer : IBalancer
    {
        public ChemicalEquation Balance(ChemicalEquation myEquation)
        {
            List<int> coefficients = solveMatrix(myEquation);
            addCoefficients(coefficients, myEquation);
            finalSanityCheck(myEquation);
            return myEquation;
        }

        private void finalSanityCheck(ChemicalEquation myEquation)
        {
            if (!myEquation.IsBalanced)
                throw new Exception("Error: Blancer failed to balance the equation!");
        }

        private void addCoefficients(List<int> answers, ChemicalEquation unbalancedEquation)
        {
            IChemical currentChemical;
            for (int i = 0; i < unbalancedEquation.MoleculeCount; i++)
            {
                if (i < unbalancedEquation.Reactants.Count)
                {
                    currentChemical = unbalancedEquation.Reactants[i];
                    unbalancedEquation.Reactants.Coefficients[currentChemical] = answers[i];
                }
                else
                {
                    currentChemical = unbalancedEquation.Products[i - unbalancedEquation.Reactants.Count];
                    unbalancedEquation.Products.Coefficients[currentChemical] = answers[i];
                }
            }
        }

        private List<int> solveMatrix(ChemicalEquation unbalancedEquation)
        {
            DenseMatrix unsolvedMatrix = buildMatrix(unbalancedEquation);
            DenseVector vector = buildVector(unbalancedEquation);
            List<double> answers = new List<double>();
            var matrixAnswers = unsolvedMatrix.QR().Solve(vector);
            foreach (double d in matrixAnswers)
                answers.Add(d);
            answers.Add(solveForZ(unsolvedMatrix, vector, matrixAnswers));
            List<int> coefficients = convertAnswersToIntegers(answers);
            return coefficients;
        }

        private List<int> convertAnswersToIntegers(List<double> answers)
        {
            List<int> coefficients = new List<int>();
            int numberToMultiplyBy = 1;
            while (numberToMultiplyBy < 10000)
            {
                List<double> checkList = multiplyAnswers(answers, numberToMultiplyBy);

                if (checkAllAnswersAreIntegers(checkList))
                {
                    foreach (double d in checkList)
                        coefficients.Add((int)d);
                    return coefficients;
                }
                else
                    numberToMultiplyBy++;
            }
            throw new ArgumentException("Error: Could not determine integer values of coefficients");
        }

        private static List<double> multiplyAnswers(List<double> answers, int numberToMultiplyBy)
        {
            List<double> checkList = new List<double>(answers);
            for (int i = 0; i < checkList.Count; i++)
            {
                checkList[i] *= numberToMultiplyBy;
                checkList[i] = Math.Round(checkList[i], 10);
            }
            return checkList;
        }

        private bool checkAllAnswersAreIntegers(List<double> checkList)
        {
            foreach (double d in checkList)
                if (d % 1 != 0)
                    return false;
            return true;
        }

        private static double solveForZ(DenseMatrix unsolvedMatrix, DenseVector vector, MathNet.Numerics.LinearAlgebra.Generic.Vector<double> matrixAnswers)
        {
            for (int i = 0; i < vector.Count; i++)
            {
                if (vector[i] != 0) //to solve for the vector, we need to find a row where it wasn't zero.
                {
                    double answer = 0;
                    for (int n = 0; n < unsolvedMatrix.ColumnCount; n++)
                    {
                        double matrixNumber = unsolvedMatrix[i, n];
                        double multipliedBy = matrixAnswers[n];
                        answer += matrixNumber * multipliedBy;
                    }
                    answer /= vector[i];
                    return answer;
                }
            }
            throw new ArgumentException("Balancer Error: Could not solve for z");
        }

        private DenseVector buildVector(ChemicalEquation unbalancedEquation)
        {
            DenseVector vector = new DenseVector(unbalancedEquation.ListOfElements.Count);
            for (int i = 0; i < unbalancedEquation.ListOfElements.Count; i++)
            {
                if (unbalancedEquation.Products[unbalancedEquation.Products.Count - 1] is Element)
                    vector[i] = 1;
                else
                {
                    ChemicalGroup lastChemical = (ChemicalGroup)unbalancedEquation.Products[unbalancedEquation.Products.Count - 1];
                    vector[i] = lastChemical.GetDeepElementCount(unbalancedEquation.ListOfElements[i]);
                }
            }
            return vector;
        }

        private DenseMatrix buildMatrix(ChemicalEquation unbalancedEquation)
        {
            List<Element> listOfElements = unbalancedEquation.ListOfElements;
            Side processingSide = Side.LeftSide;
            DenseMatrix myMatrix = new DenseMatrix(listOfElements.Count, unbalancedEquation.MoleculeCount - 1);
            for (int column = 0; column < unbalancedEquation.MoleculeCount - 1; column++)
            {
                for (int row = 0; row < listOfElements.Count; row++)
                {
                    if (column >= unbalancedEquation.Reactants.Count)
                        processingSide = Side.RightSide;
                    myMatrix[row, column] = getMatrixPoint(unbalancedEquation, processingSide, column, row, listOfElements);
                }
            }
            return myMatrix;
        }

        private double getMatrixPoint(ChemicalEquation unbalancedEquation, Side processingSide, int column, int row, List<Element> listOfElements)
        {
            EquationSide currentSide = setCurrentProcessingSide(unbalancedEquation, processingSide);
            if (processingSide == Side.RightSide)
                column -= unbalancedEquation.Reactants.Count;
            double matrixPoint = getElementCountOfChemical(column, row, listOfElements, currentSide);
            if (processingSide == Side.RightSide)
                matrixPoint *= -1.0;
            return matrixPoint;
        }

        private static double getElementCountOfChemical(int column, int row, List<Element> listOfElements, EquationSide currentSide)
        {
            double matrixPoint = 0;
            if (currentSide[column] == listOfElements[row]) //check to see if the current column is a single instance of the element we are searching for.
                matrixPoint = 1.0;
            else if (currentSide[column] is ChemicalGroup)
            {
                ChemicalGroup currentMolecule = (ChemicalGroup)currentSide[column];
                matrixPoint = currentMolecule.GetDeepElementCount(listOfElements[row]);
            }
            return matrixPoint;
        }

        private static EquationSide setCurrentProcessingSide(ChemicalEquation unbalancedEquation, Side processingSide)
        {
            EquationSide currentSide;
            if (processingSide == Side.LeftSide)
                currentSide = unbalancedEquation.Reactants;
            else
                currentSide = unbalancedEquation.Products;
            return currentSide;
        }
    }
}