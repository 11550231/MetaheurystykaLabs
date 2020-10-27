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
        public float BestPosibleSolution { get; set; } = 0;
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
            Dimensions--;



            List<string> allLinesTextSolution = File.ReadAllLines(sourceFilePath.Replace(".vrp",".sol")).ToList();
            BestPosibleSolution = Int32.Parse(allLinesTextSolution.Last().Substring(5));
        }

        public float CalculateSortingFitness(int[] genotype)
        {
            float score = 0;
            for (int i = 0; i < genotype.Length; i++)
            {
                if (genotype[i] == i)
                    score++;
            }
            return (score/genotype.Length)*100;
        }
        public float CalculateFitness(int[] genotype)
        {
            float score = 0;
            var warehouse = Markets[0];
            score += warehouse.GetDistanceBetweenMarkets(Markets[genotype[0] + 1]);
            float weight = Markets[genotype[0] + 1].Demand;
            for (int i = 0; i < genotype.Length - 1; i++)
            {
                var market1 = Markets[genotype[i]+1];
                var market2 = Markets[genotype[i + 1]+1];
               
                weight += market2.Demand;
                if (weight>Capability)
                {
                    weight = market2.Demand;
                    score += market1.GetDistanceBetweenMarkets(warehouse);
                    score += warehouse.GetDistanceBetweenMarkets(market2);
                }
                else
                    score += market1.GetDistanceBetweenMarkets(market2);
            }
            score += Markets[genotype[genotype.Length - 1] + 1].GetDistanceBetweenMarkets(warehouse);
            return (-score);
        }
        public class Market
        {
            public float CoordinateX {get; set;}
            public float CoordinateY { get; set;}
            public float Demand { get; set;}
            public float GetDistanceBetweenMarkets(Market secondMarker)
            {
                var market1 = this;
                var market2 = secondMarker;
                var distance = Math.Sqrt(Math.Pow(market1.CoordinateX - market2.CoordinateX, 2) + Math.Pow(market1.CoordinateY - market2.CoordinateY,2));
                return (float)distance;
            }
        }
    }
}
