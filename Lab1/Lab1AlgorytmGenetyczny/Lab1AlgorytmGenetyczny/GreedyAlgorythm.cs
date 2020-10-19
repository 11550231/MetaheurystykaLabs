using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace
{
    public class GreedyAlgorythm : IAlgorythm<GreedyAlgorythm.Result>
    {
        public IProblem Problem { get; }
        private int[] Answer { get; set; }
        public Result Calculate()
        {
            var result = new Result();
            Answer = new int[Problem.Dimensions];
            for (int i = 0; i < Problem.Dimensions; i++)
                Answer[i] = -1;
            Answer[0] = 0;
            for (int i = 1; i < Problem.Dimensions; i++)
                Answer[i] = GetNotUsedClosestMarketIndex(Answer[i-1]);
            result.Answer = Answer;
            result.Value=Problem.CalculateFitness(result.Answer);
            return result;
        }
        private int GetNotUsedClosestMarketIndex(int currentMarkerIndex)
        {
            CapacitatedVehicleRoutingProblem problem = ((CapacitatedVehicleRoutingProblem)Problem);
            int nextMarket = -1;
            float bestScore = int.MaxValue;
            for (int i = 1; i < Problem.Dimensions; i++)
            {
                bool canUse = true;
                for (int j = 0; j < Answer.Length; j++)
                    if (Answer[j] == i)
                        canUse = false;
                if (canUse)
                {
                    float score = problem.Markets[currentMarkerIndex+1].GetDistanceBetweenMarkets(problem.Markets[i+1]);
                    if (bestScore > score)
                    {
                        bestScore = score;
                        nextMarket = i;
                    }
                }
            }
            return nextMarket;
        }
        public GreedyAlgorythm(IProblem problem)
        {
            Problem = problem;
        }

        public class Result
        {
            public float Value { get; set; }
            public int[] Answer { get; set; }
        }
    }
}
