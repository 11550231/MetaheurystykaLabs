using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace
{
    public interface IAlgorythm<T>
    {
        T Calculate();
        IProblem Problem { get; }
    }
}
