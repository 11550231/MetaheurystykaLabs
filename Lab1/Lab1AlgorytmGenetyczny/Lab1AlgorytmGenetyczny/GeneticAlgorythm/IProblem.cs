using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace
{
    public interface IProblem
    {
        float CalculateFitness(int[] genotype);
        int Dimensions { get; }
    }
}
