using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace
{
    public class RandomAlgorythm : IAlgorythm<RandomAlgorythm.Result>
    {
        public IProblem Problem { get; }
        private int[] Answer { get; set; }
        public RandomAlgorythm(IProblem problem)
        {
            Problem = problem;
        }
        public Result Calculate()
        {
            var result = new Result();
            var random = new Random();
            Answer = new int[Problem.Dimensions];
            for (int i = 0; i < Problem.Dimensions; i++)
            {
                Answer[i] = random.Next(0, Problem.Dimensions-1);
            }

            result.Answer = Answer;
            result.Value=Problem.CalculateFitness(result.Answer);
            return result;
        }
     

        public class Result
        {
            public float Value { get; set; }
            public int[] Answer { get; set; }
        }
    }
}
