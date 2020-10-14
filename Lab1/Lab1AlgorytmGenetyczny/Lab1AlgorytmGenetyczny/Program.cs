using Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

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
        static int CalculationNumber = 1;
        static async Task Main(string[] args)
        {
            //var geenotype = ("3;48;34;50;23;31;58;52;56;48;30;4;21;0;22;31;56;6;0;10;51;43;29;16;22;10;37;55;51;14;44;40;12;42;19;49;38;48;22;33;41;25;52;34;42;24;8;47;7;37;21;4;41;30;29;22;19;18;28;12;55;2;0;"
            //).Split(';').Where(y=>y!="").Select(x=>Int32.Parse(x)).ToList();
            //CheckIfGenotypeGood(geenotype);
           
            string baseDirectory = System.IO.Directory.GetCurrentDirectory();
            _outputFolderPath = baseDirectory+"\\dataOutput\\";
            _inputFolderPath = baseDirectory + "\\dataInput\\A\\";
            Console.WriteLine("Hello World! 1");
            //RunGeneticAlgorytm(0.7f, 0.2f, 1000, 200, 3, "A-n33-k5.vrp");
            var tasks = new List<Task<bool>>();
            //tasks.Add(RunGeneticAlgorytmTask(0.7f, 0.2f, 50000, 100, 5, "A-n54-k7.vrp"));
            // tasks.Add(RunGeneticAlgorytmTask(0.47f, 0.13f, 50000, 50, 5, "A-n33-k5.vrp"));
            //tasks.Add(RunGeneticAlgorytmTask(0.44f, 0.13f, 50000, 100, 5, "A-n32-k5.vrp"));
            //tasks.Add(RunGeneticAlgorytmTask(0.44f, 0.13f, 50000, 100, 5, "A-n54-k7.vrp"));
            //tasks.Add(RunGeneticAlgorytmTask(0.44f, 0.13f, 50000, 100, 5, "A-n61-k9.vrp"));
            //tasks.Add(RunGeneticAlgorytmTask(0.44f, 0.13f, 50000, 100, 5, "A-n60-k9.vrp"));
            //tasks.Add(RunGeneticAlgorytmTask(0.44f, 0.13f, 50000, 100, 5, "A-n65-k9.vrp"));
            //tasks.Add(RunGeneticAlgorytmTask(0.44f, 0.13f, 50000, 100, 5, "A-n80-k10.vrp"));
            RunGreedyAlgorytm("A-n54-k7.vrp");
            RunRandomAlgorytm("A-n54-k7.vrp");
            foreach (var task in tasks)
            {
                var result = await task;
                //do more processing on whatever item is done now
            }     
        }
        static Task<bool> RunGeneticAlgorytmTask(float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, string sourceFileName)
        {
            var task = Task.Run(() => RunGeneticAlgorytm(crossProbability, mutationProbability, amountOfGenerations, generationSize, tournamentSize, sourceFileName));
            return task;
        }
        static bool RunRandomAlgorytm(string sourceFileName)
        {
            DateTime date = DateTime.Now;
            int calculationNumber = CalculationNumber++;
            CapacitatedVehicleRoutingProblem problem = new CapacitatedVehicleRoutingProblem();
            problem.GetDataFromSourceFile(_inputFolderPath + sourceFileName);
            RandomAlgorythm algorythm = new RandomAlgorythm(problem);
            var result = algorythm.Calculate();
            SaveRandomResultToFile(result, _outputFolderPath + sourceFileName + date.Month + "_" + date.Day + "_" + date.Hour + "_" + date.Minute + "_" + date.Second+"_random");
            Console.WriteLine("Hello World! 2");
            return true;
        }
        static bool RunGreedyAlgorytm(string sourceFileName)
        {
            DateTime date = DateTime.Now;
            int calculationNumber = CalculationNumber++;
            CapacitatedVehicleRoutingProblem problem = new CapacitatedVehicleRoutingProblem();
            problem.GetDataFromSourceFile(_inputFolderPath + sourceFileName);
            GreedyAlgorythm algorythm = new GreedyAlgorythm(problem);
            var result = algorythm.Calculate();
            SaveGreedyResultToFile(result, _outputFolderPath + sourceFileName + date.Month+"_"+date.Day + "_"+date.Hour + "_" + date.Minute+ "_" + date.Second + "_greedy");
            Console.WriteLine("Hello World! 2");
            return true;
        }
        static bool RunGeneticAlgorytm(float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, string sourceFileName)
        {
            DateTime date = DateTime.Now;
            int calculationNumber = CalculationNumber++;
            Thread.CurrentThread.Name = GetOutputFileName(date, crossProbability, mutationProbability, amountOfGenerations, generationSize, tournamentSize, sourceFileName, calculationNumber);
            CapacitatedVehicleRoutingProblem problem = new CapacitatedVehicleRoutingProblem();
            problem.GetDataFromSourceFile(_inputFolderPath + sourceFileName);
            GeneticAlgorythm algorythm = new GeneticAlgorythm(problem, crossProbability, mutationProbability, amountOfGenerations, generationSize, tournamentSize);
            var result = algorythm.Calculate();
            SaveResultToFile(result, _outputFolderPath+ GetOutputFileName(date,crossProbability, mutationProbability, amountOfGenerations, generationSize, tournamentSize, sourceFileName, calculationNumber), problem);
            Console.WriteLine("Hello World! 2");
            return true;
        }
        static void SaveRandomResultToFile(RandomAlgorythm.Result result, string filePath)
        {
            // Create a file to write to.
            List<string> lines = new List<string>();
            lines.Add($"\"BestValue: {((result != null) ? result.Value.ToString() : "null")}\",");
            var answer = "";
            foreach (int gen in result.Answer)
                answer+= (gen + 1).ToString()+';';
            lines.Add($"answer: {answer}");
            File.AppendAllLines(filePath+".csv", lines);
        }
        static void SaveGreedyResultToFile(GreedyAlgorythm.Result result, string filePath)
        {
            // Create a file to write to.
            List<string> lines = new List<string>();
            lines.Add($"\"BestValue: {((result != null) ? result.Value.ToString() : "null")}\",");
            var answer = "";
            foreach (int gen in result.Answer)
                answer += (gen+1).ToString() + ';';
            lines.Add($"answer: {answer}");
            File.AppendAllLines(filePath + ".csv", lines);
        }
        static string GetOutputFileName(DateTime date, float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, string sourceFileName, int calculationNumber)
        {
            return $"{date.Month}_{date.Day}__{date.Hour}_{date.Minute}_{date.Second}_{calculationNumber} In{sourceFileName}q c {crossProbability}; m {mutationProbability} q g {amountOfGenerations} q gSize {generationSize}q" + ".csv";
        }
            static void SaveResultToFile(Result[] results, string filePath, CapacitatedVehicleRoutingProblem problem)
        {
            // Create a file to write to.
            List<string> lines = new List<string>();
            var lastResult = results.Last();
            lines.Add($"\"Generation\",\"Average\",\"Max\",\"Min\",\"BestGen\",\"amountOfTours\",\"repeatedGens\",\"BestValue: {((lastResult!=null)? lastResult.Max.ToString(): "null")}\"");
            foreach(Result r in results)
                if(r!=null)
                    lines.Add($"{r.GenerationNumber},{r.Average},{r.Max},{r.Min},{IndividualToString(r.BestIndividual, problem)}");
            File.AppendAllLines(filePath, lines);
        }
        static string IndividualToString(Individual ind, CapacitatedVehicleRoutingProblem capacitatedVehicleRoutingProblem)
        {

            float currentSum = 0;
            string value = ";";
            int amountOfTours = 1;
            string repeatedNums = "";
            for (int i=0;i<ind.Genotype.Length;i++)
            {
                var marketid = (ind.Genotype[i]);
                var market = capacitatedVehicleRoutingProblem.Markets[marketid];
                currentSum += market.Demand;
                if (currentSum  > capacitatedVehicleRoutingProblem.Capability)
                {
                    currentSum = market.Demand;
                    value += '*';
                    amountOfTours++;
                }
                else
                    value += ';';
                value += (marketid+1).ToString();
            }
            for (int i = 0; i < ind.Genotype.Length; i++)
            {
                for (int j = i+1; j < ind.Genotype.Length; j++)
                    if(ind.Genotype[i]== ind.Genotype[j])
                    {
                        repeatedNums += (";" +( ind.Genotype[j]+1).ToString());
                    }
            }
                return "\""+value+ "\",\"" + amountOfTours + "\",\"" + repeatedNums + "\"";
        }
    }
}
