using Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Lab1AlgorytmGenetyczny
{
    class Program
    {
          public static string _inputFolderPath { get; set; }
        public static string _outputFolderPath { get; set; }
        static void CheckIfGenotypeGood(List<int> genotype)
        {
            for(int i=0;i<genotype.Count;i++)
                for(int j=i+1;j<genotype.Count;j++)
                {
                    if (genotype[i] == genotype[j])
                        Console.WriteLine(genotype[j]);
                }    
        }
        static void Main(string[] args)
        {
            //var geenotype = ("3;48;34;50;23;31;58;52;56;48;30;4;21;0;22;31;56;6;0;10;51;43;29;16;22;10;37;55;51;14;44;40;12;42;19;49;38;48;22;33;41;25;52;34;42;24;8;47;7;37;21;4;41;30;29;22;19;18;28;12;55;2;0;"
            //).Split(';').Where(y=>y!="").Select(x=>Int32.Parse(x)).ToList();
            //CheckIfGenotypeGood(geenotype);
           
            string baseDirectory = System.IO.Directory.GetCurrentDirectory();
            _outputFolderPath = baseDirectory+"\\dataOutput\\";
            _inputFolderPath = baseDirectory + "\\dataInput\\A\\";
            Console.WriteLine("Hello World! 1");
            //RunGeneticAlgorytm(0.7f, 0.1f, 100, 100, "A-n32-k5.vrp");
            RunGeneticAlgorytm(0.7f, 0.1f, 100, 100, 1,"A-n63-k10.vrp");
         
        }
        static void RunGeneticAlgorytm(float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, string sourceFileName)
        {
            DateTime date = DateTime.Now;
            CapacitatedVehicleRoutingProblem problem = new CapacitatedVehicleRoutingProblem();
            problem.GetDataFromSourceFile(_inputFolderPath + sourceFileName);
            GeneticAlgorythm algorythm = new GeneticAlgorythm(problem, crossProbability, mutationProbability, amountOfGenerations, generationSize, tournamentSize);
            var result = algorythm.Calculate();
            SaveResultToFile(result, _outputFolderPath+$"{date.Month}_{date.Day}_{date.Hour}v{date.Minute}v{date.Second}vInput {sourceFileName}q cross {crossProbability}; mut {mutationProbability} q generations {amountOfGenerations} q genSize {generationSize}q"+".csv",problem);
            Console.WriteLine("Hello World! 2");
        }
        static void SaveResultToFile(Result[] results, string filePath, CapacitatedVehicleRoutingProblem problem)
        {
            // Create a file to write to.
            List<string> lines = new List<string>();
            lines.Add("Generation,Average,Max,Min,BestGen,ride1,ride2,ride3,ride4,ride5");
            foreach(Result r in results)
                lines.Add($"{r.GenerationNumber},{r.Average},{r.Max},{r.Min},,{IndividualToString(r.BestIndividual, problem)}");
            File.AppendAllLines(filePath, lines);
        }
        static string IndividualToString(Individual ind, CapacitatedVehicleRoutingProblem capacitatedVehicleRoutingProblem)
        {

            float currentSum = 0;
            string value = "";
            for(int i=0;i<ind.Genotype.Length;i++)
            {
                var marketid = (ind.Genotype[i]);
                var market = capacitatedVehicleRoutingProblem.Markets[marketid];
                currentSum += market.Demand;
                if (currentSum  > capacitatedVehicleRoutingProblem.Capability)
                {
                    currentSum = market.Demand;
                    value += ',';
                }
                else
                    value += ';';
                value += (marketid+1).ToString();
            }
            
            return value;
        }
    }
}
