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
            //CheckIfGenotypeGood(geenotype);g
           
            string baseDirectory = System.IO.Directory.GetCurrentDirectory();
            _outputFolderPath = baseDirectory+"\\dataOutput\\";
            _inputFolderPath = baseDirectory + "\\dataInput\\A\\";
            Console.WriteLine("Hello World! 1");
            //RunGeneticAlgorytm(0.7f, 0.2f, 1000, 200, 3, "A-n33-k5.vrp");
            var tasks = new List<Task<bool>>();
            //tasks.Add(RunGeneticAlgorytmTask(0.7f, 0.2f, 50000, 100, 5, "A-n54-k7.vrp"));
            
            //tasks.Add(RunGeneticAlgorytmTask(0.75f, 0.0f, 5000, 100, 5, "A-n37-k6.vrp", false, false, true));
            //tasks.Add(RunGeneticAlgorytmTask(0.75f, 0.13f, 5000, 100, 5, "A-n37-k6.vrp", false, false, true));
            tasks.Add(RunGeneticAlgorytmTask(0.75f, 0.13f, 10000, 100, 5, "A-n60-k9.vrp", false, false, true));

            //RunRandomAlgorytm("A-n32-k5.vrp", 1000);
           // RunGreedyAlgorytm("A-n60-k9.vrp");
            foreach (var task in tasks)
            {
                var result = await task;
                //do more processing on whatever item is done now
            }     
        }
        static Task<bool> RunGeneticAlgorytmTask(float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, string sourceFileName, bool useRoulette, bool mutationBySwap , bool crossOX )
        {
            var task = Task.Run(() => RunGeneticAlgorytmWithStatistics(crossProbability, mutationProbability, amountOfGenerations, generationSize, tournamentSize, sourceFileName, useRoulette,mutationBySwap, crossOX));
            return task;
        }
        static bool RunRandomAlgorytm(string sourceFileName, int amountOfRandoms)
        {
            DateTime date = DateTime.Now;
            int calculationNumber = CalculationNumber++;
            CapacitatedVehicleRoutingProblem problem = new CapacitatedVehicleRoutingProblem();
            problem.GetDataFromSourceFile(_inputFolderPath + sourceFileName);
            RandomAlgorythm algorythm = new RandomAlgorythm(problem, amountOfRandoms);
            var result = algorythm.Calculate();
            SaveRandomResultToFile(result, _outputFolderPath + sourceFileName + date.Month + "_" + date.Day + "_" + date.Hour + "_" + date.Minute + "_" + date.Second+"_random");
            Console.WriteLine("Hello World! 2");
            return true;
        }
        static bool RunGreedyAlgorytm(string sourceFileName)
        {
            var tasks = new List<Task<float>>();
            DateTime date = DateTime.Now;
            List<string> lines = new List<string>();
            int calculationNumber = CalculationNumber++;
            CapacitatedVehicleRoutingProblem problem = new CapacitatedVehicleRoutingProblem();
            problem.GetDataFromSourceFile(_inputFolderPath + sourceFileName);
            float sum = 0;
            List<float> results = new List<float>();
            for (int i = 0; i < 59; i++)
            {
                var result = (new GreedyAlgorythm(problem, i)).Calculate();
                
                results.Add(result.Value);

            }
            sum = results.Sum();
            //obliczenie średniej i odchylenia standardowego
            float average = sum / results.Count;
            float max = results.Max();
            float min = results.Min();
            float averageDiviationsSum = 0;
            float standardDeviation = 0;
            for (int i = 0; i < results.Count; i++)
                averageDiviationsSum += (float)Math.Pow(results[i] - average, 2);
            standardDeviation = (float)Math.Sqrt(averageDiviationsSum / results.Count);
            //obliczenie średniej i odchylenia standardowego
            lines.Add("max: " + (int)max);
            lines.Add("min: " + (int)min);
            lines.Add("average: " + (int)average);
            lines.Add("standardDeviation: " + (int)standardDeviation);
            File.AppendAllLines(_outputFolderPath + "greedy1"+".txt", lines);

            return true;
        }
        static async Task<bool> RunGeneticAlgorytmWithStatistics(float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, 
            string sourceFileName, bool useRoulette, bool mutationBySwap , bool crossOX)
        {
            var tasks = new List<Task<float>>();
            DateTime date = DateTime.Now;
            List<string> lines = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => RunGeneticAlgorytmWithStatistics2(crossProbability, mutationProbability,
                    amountOfGenerations, generationSize, tournamentSize, sourceFileName, useRoulette, mutationBySwap, crossOX)));
            }
            float sum = 0;
            List<float> results = new List<float>();
            foreach (var task in tasks)
            {
                var result = await task;
                results.Add(result);
                lines.Add((int)result + ",");
                
            }
            sum = results.Sum();
            //obliczenie średniej i odchylenia standardowego
            float average = sum / results.Count ;
            float max = results.Max();
            float min = results.Min();
            float averageDiviationsSum = 0;
            float standardDeviation = 0;
            for (int i = 0; i < results.Count; i++)
                averageDiviationsSum += (float)Math.Pow(results[i] - average, 2);
            standardDeviation = (float)Math.Sqrt(averageDiviationsSum / results.Count);
            //obliczenie średniej i odchylenia standardowego
            lines.Add("max: " + (int)max );
            lines.Add("min: " + (int)min );
            lines.Add("average: " + (int)average );
            lines.Add("standardDeviation: " + (int)standardDeviation );
            File.AppendAllLines(_outputFolderPath + GetOutputFileName(date, crossProbability, mutationProbability, amountOfGenerations, generationSize, tournamentSize, useRoulette, sourceFileName, 99999999).Replace(".csv",".txt"), lines);

            return true;
        }
        static float RunGeneticAlgorytmWithStatistics2(float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, string sourceFileName, bool useRoulette, bool mutationBySwap , bool crossOX)
        {
            DateTime date = DateTime.Now;
            RunGeneticAlgorytm(crossProbability, mutationProbability,
                     amountOfGenerations, generationSize, tournamentSize, sourceFileName, useRoulette, out float bestScore, mutationBySwap, crossOX);

                return bestScore;
        }
        static bool RunGeneticAlgorytm(float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, string sourceFileName, bool useRoulette, out float bestScore, bool mutationBySwap , bool crossOX)
        {
            DateTime date = DateTime.Now;
            int calculationNumber = CalculationNumber++;
            Thread.CurrentThread.Name = GetOutputFileName(date, crossProbability, mutationProbability, amountOfGenerations, generationSize, tournamentSize,useRoulette, sourceFileName, calculationNumber);
            CapacitatedVehicleRoutingProblem problem = new CapacitatedVehicleRoutingProblem();
            problem.GetDataFromSourceFile(_inputFolderPath + sourceFileName);
            GeneticAlgorythm algorythm = new GeneticAlgorythm(problem, crossProbability, mutationProbability, 
                amountOfGenerations, generationSize, tournamentSize, useRoulette,  mutationBySwap , crossOX);
            var result = algorythm.Calculate();
            SaveResultToFile(result, _outputFolderPath+ GetOutputFileName(date,crossProbability, mutationProbability, 
                amountOfGenerations, generationSize, tournamentSize, useRoulette,sourceFileName, calculationNumber), problem);
            Console.WriteLine("Hello World! 2");
            bestScore = result[result.Length - 1].Max;
            return true;
        }
        static void SaveRandomResultToFile(RandomAlgorythm.Result result, string filePath)
        {
            // Create a file to write to.
            List<string> lines = new List<string>();
            lines.Add($"\"BestValue: {((result != null) ? result.Best.ToString() : "null")}\",");
            lines.Add($"\"Wrost: {((result != null) ? result.Wrost.ToString() : "null")}\",");
            lines.Add($"\"Average: {((result != null) ? result.Average.ToString() : "null")}\",");
            lines.Add($"\"StandardDeviation: {((result != null) ? result.StandardDeviation.ToString() : "null")}\",");


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
        static string GetOutputFileName(DateTime date, float crossProbability, float mutationProbability, int amountOfGenerations, int generationSize, int tournamentSize, bool useRoulette, string sourceFileName, int calculationNumber)
        {
            return $"{date.Month}_{date.Day}__{date.Hour}_{date.Minute}_{date.Second}_{calculationNumber} In{sourceFileName}q c {crossProbability}; m {mutationProbability} q g {amountOfGenerations} q gSize {generationSize}q"+ (useRoulette? "Roule" : "Tourname") + tournamentSize + ".csv";
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
