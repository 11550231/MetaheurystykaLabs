using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace
{
    public class RandomAlgorythm : IAlgorythm<RandomAlgorythm.Result>
    {
        public IProblem Problem { get; }
        private Random Random{ get; set; }
        private int AmountOfRandoms { get;}
        private Individual[] Generation { get;}
        public RandomAlgorythm(IProblem problem, int amountOfRandoms)
        {
            Problem = problem;
            AmountOfRandoms = amountOfRandoms;
            Generation = new Individual[AmountOfRandoms];
            Random = new Random();
        }
        public Result Calculate()
        {
            var result = new Result();
            float suma = 0;
            for(int i=0;i< AmountOfRandoms; i++)
            {
                Generation[i] = new Individual();
                Generation[i].Geotype = GenerateRandomIndividual();
                Generation[i].Fitness = - Problem.CalculateFitness(Generation[i].Geotype);
                suma += Generation[i].Fitness;
                if (result.Best> Generation[i].Fitness)
                {
                    result.Best = Generation[i].Fitness;
                    result.Answer = Generation[i].Geotype;
                }
                if (result.Wrost < Generation[i].Fitness)
                    result.Wrost = Generation[i].Fitness;
            }
            //obliczenie średniej i odchylenia standardowego
            result.Average = suma/ AmountOfRandoms;
            float averageDiviationsSum = 0;
            for (int i = 0; i < AmountOfRandoms; i++)
                averageDiviationsSum+= (float) Math.Pow( Generation[i].Fitness - result.Average,2);
            result.StandardDeviation = (float) Math.Sqrt(averageDiviationsSum / AmountOfRandoms);
            //obliczenie średniej i odchylenia standardowego
            return result;
        }
        public int[] GenerateRandomIndividual()
        {
            int[] geotype = new int[Problem.Dimensions];
            for (int i = 0; i < Problem.Dimensions; i++)
            {
                geotype[i] = Random.Next(0, Problem.Dimensions - 1);
            }
            return geotype;
        }
        public class Individual
        {
            public int[] Geotype { get; set; }
            public float Fitness { get; set; }
        }
            public class Result
        {
            public Result()
            {
                Wrost = float.MinValue;
                Best = float.MaxValue;
            }
            public float Average { get; set; }
            public float StandardDeviation { get; set; }
            public float Best { get; set; }
            public float Wrost { get; set; }
            public int[] Answer { get; set; }
        }
    }
}
