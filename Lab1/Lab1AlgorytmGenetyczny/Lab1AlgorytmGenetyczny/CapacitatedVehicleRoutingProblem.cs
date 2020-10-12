using Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1AlgorytmGenetyczny
{
    public class CapacitatedVehicleRoutingProblem : IProblem
    {
        public int Dimensions { get; set; }
        public int Capability { get; set; }
        public Market[] Markets;
        public CapacitatedVehicleRoutingProblem()
        {
        }
        public void GetDataFromSourceFile(string sourceFilePath)
        {
            List<string> allLinesText = File.ReadAllLines(sourceFilePath).ToList();
            string line = allLinesText[3];
            string substring = line.Substring(12);
            Dimensions = Int32.Parse(substring);

            line = allLinesText[5];
            substring = line.Substring(11);
            Capability = Int32.Parse(substring);
            Markets = new Market[Dimensions];
            for (int i=7; i< 7+ Dimensions; i++)
            {
                line = allLinesText[i];
                int marketId = i-7;
                Markets[marketId] = new Market();
                int indexTextStarts = line.IndexOf(" ", 1) + 1;
                substring = line.Substring(line.IndexOf(" ",1) + 1,line.IndexOf(" ", indexTextStarts)- indexTextStarts);
                Markets[marketId].CoordinateX = Int32.Parse(substring);
                substring = line.Substring(line.IndexOf(" ", indexTextStarts) + 1);
                Markets[marketId].CoordinateY = Int32.Parse(substring);
            }
            for (int i = 8 + Dimensions; i < 8 + (Dimensions*2); i++)
            {
                line = allLinesText[i];
                int marketId = i - 8- Dimensions;
                int indexTextStarts = line.IndexOf(" ", 1) + 1;
                substring = line.Substring(line.IndexOf(" ", 1) + 1, line.IndexOf(" ", indexTextStarts) - indexTextStarts);
                Markets[marketId].Demand = Int32.Parse(substring);
            }
        }

        public float CalculateFitness(int[] genotype)
        {
            float score = 0;
            for (int i = 0; i < genotype.Length; i++)
            {
                if (genotype[i] == i)
                    score++;
            }
            return score;
        }

        public class Market
        {
            public float CoordinateX {get; set;}
            public float CoordinateY { get; set;}
            public float Demand { get; set;}
            
        }
    }
}
