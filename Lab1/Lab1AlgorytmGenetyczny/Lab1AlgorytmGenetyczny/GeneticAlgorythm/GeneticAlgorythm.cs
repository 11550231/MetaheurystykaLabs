using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace
{

    public class GeneticAlgorythm: IAlgorythm<Result[]>
    {
        private Result[] AlgorythmResult { get;}
        public Individual[] Generation { get; set; }
        public Random RandomGenerator{ get; set; }

        public IProblem Problem { get; }

        public readonly float CROSS_PROBABILITY = 0;
        public readonly bool CROSS_OX = true;
        public readonly float MUTATION_PROBABILITY = 0;
        public readonly bool MUTATION_BY_SWAP = false;
        public readonly int GENERATION_SIZE = 0;
        public readonly int AMOUNT_OF_GENERATIONS = 0;
        public readonly int TOURNAMENT_SIZE = 0;
        public readonly bool USE_ROULETTE = false;
        public GeneticAlgorythm(IProblem problem, float crossProbability, float mutationProbability,int amountOfGenerations, 
            int generationSize, int tournamentSize, bool useRoulette, bool mutationBySwap , bool crossOX)
        {
            CROSS_PROBABILITY = crossProbability;
            MUTATION_BY_SWAP = mutationBySwap;
            CROSS_OX = crossOX;
            MUTATION_PROBABILITY = mutationProbability;
            GENERATION_SIZE = generationSize;
            TOURNAMENT_SIZE = tournamentSize;
            USE_ROULETTE = useRoulette;
            AMOUNT_OF_GENERATIONS = amountOfGenerations;
            AlgorythmResult = new Result[AMOUNT_OF_GENERATIONS];
            RandomGenerator = new Random();
            Problem = problem;
        }

        public Result[] Calculate()
        {
            GenerateFirstRandomGeneration();
            CalculateGenerationFitness();
            SaveGenetaionFitness(0);
            for (int geneationNumber=1; geneationNumber < AMOUNT_OF_GENERATIONS; geneationNumber++)
            {
                Individual[] newGeneration = CrossGeneration(AlgorythmResult[geneationNumber - 1]);
                Generation = newGeneration;
                MutateGeneration();
                Generation[0] = AlgorythmResult[geneationNumber - 1].BestIndividual;
                CalculateGenerationFitness();
                SaveGenetaionFitness(geneationNumber);
            }
            return AlgorythmResult;
        }
        private void MutateGeneration()
        {
            foreach (Individual individual in Generation)
                if (RandomGenerator.NextDouble() < MUTATION_PROBABILITY)
                    individual.Mutate();
        }
        private Individual[] CrossGeneration(Result previousGenerationResult)
        {
            Individual[] newGeneration = new Individual[GENERATION_SIZE];
            for (int i = 0; i < Generation.Length; i += 2)
            {
                //tournament
                Individual first = null, second = null;
                if (USE_ROULETTE)
                {
                    first = RunRoulette(previousGenerationResult);
                    second = RunRoulette(previousGenerationResult);
                }
                else
                {
                    first = RunTournament();
                    second = RunTournament();
                }

                //tournament
                if (RandomGenerator.NextDouble() < CROSS_PROBABILITY)
                {
                    var result = first.Cross(second);
                    first = result.First;
                    second = result.Second;
                }
                newGeneration[i] = first;
                newGeneration[i+1] = second;
            }
            return newGeneration;
        }
        private Individual RunTournament()
        {
            Individual[] tournamentParticipants = new Individual[TOURNAMENT_SIZE];
            for (int i = 0; i < TOURNAMENT_SIZE; i++)
            {
                int next = RandomGenerator.Next(0, GENERATION_SIZE-1);
                tournamentParticipants[i] = Generation[next];
            }
            Individual best = tournamentParticipants[0];
            foreach (var tourParticipant in tournamentParticipants)
                if (tourParticipant.Fitness > best.Fitness)
                    best = tourParticipant;
            return best;
        }
        private Individual RunRoulette(Result previousGenerationResult)
        {
            Individual[] tournamentParticipants = new Individual[TOURNAMENT_SIZE];
            float minFitness = float.MaxValue;
            for (int i = 0; i < TOURNAMENT_SIZE; i++)
            {
                int next = RandomGenerator.Next(0, GENERATION_SIZE - 1);
                tournamentParticipants[i] = Generation[next];
                if (minFitness > tournamentParticipants[i].Fitness)
                    minFitness = tournamentParticipants[i].Fitness;
            }
            var list = tournamentParticipants.ToList();
            list.Sort((x, y) => (int)(x.Fitness - y.Fitness));
            tournamentParticipants = list.ToArray();
            float[] maxValueOfThisParticipant = new float[TOURNAMENT_SIZE];
            for (int i = 0; i < TOURNAMENT_SIZE; i++)
            {
                if (i == 0)
                    maxValueOfThisParticipant[i] = 1000;
                else
                    maxValueOfThisParticipant[i] = 1000 / ((i+2) * (i + 2)) + maxValueOfThisParticipant[i - 1];

            }
            int chosenFitness = RandomGenerator.Next(0,(int) maxValueOfThisParticipant[TOURNAMENT_SIZE-1]);
            Individual chosen = null;
            for (int i = 0; i < TOURNAMENT_SIZE; i++)
            {
                if (chosenFitness < maxValueOfThisParticipant[i])
                    chosen = tournamentParticipants[i];
            }
            if(chosen==null)
                chosen = tournamentParticipants[TOURNAMENT_SIZE - 1];
            return chosen;
        }
        private void CalculateGenerationFitness()
        {
            for (int i = 0; i < Generation.Length; i++)
                Generation[i].CalculateFitness();
        }
        private void SaveGenetaionFitness(int generationNumber)
        {
            var result = new Result
            {
                GenerationNumber = generationNumber
            };
            float sum = 0;
            for (int i = 0; i < GENERATION_SIZE; i++)
            {
                Individual crentIndiv = Generation[i];
                float fitness=crentIndiv.Fitness;
                sum += fitness;
                if(fitness < result.Min)
                    result.Min = fitness;
                if (fitness > result.Max)
                {
                    result.Max = fitness;
                    result.BestIndividual = (Individual)crentIndiv.Clone();
                }
            }
            result.Average = sum / GENERATION_SIZE;
            result.Average = -result.Average;
            result.Max = -result.Max;
            result.Min = -result.Min;
            AlgorythmResult[generationNumber] = result;
            if(generationNumber%1000==0)
            Console.WriteLine($"Thread: {Thread.CurrentThread.Name} Generation: {generationNumber}; Average: {result.Average}; Max: {result.Max}; Min: {result.Min};");
        }
        private void GenerateFirstRandomGeneration()
        {
            Generation = new Individual[GENERATION_SIZE];
            for (int i = 0; i < GENERATION_SIZE; i++)
            {
                Generation[i] = new Individual(this);
                for (int j = 0; j < Problem.Dimensions; j++)
                    Generation[i].Genotype[j] = RandomGenerator.Next(0, Problem.Dimensions - 1);
                Generation[i].Repair();
            }
        }
     
    }

    public class Result
    {
        public int GenerationNumber { get; set; }
        public float Min { get; set; } = float.MaxValue;
        public float Average { get; set; } = 0;
        public float Max { get; set; } = float.MinValue;
        public Individual BestIndividual { get; set; }
    }
}
