using Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1AlgorytmGenetyczny
{
    public class Individual
    {
        public GeneticAlgorythm Algorythm { get; }
        public float Fitness { get; set; } = 0;
        public Individual(GeneticAlgorythm algorythm)
        {
            Algorythm = algorythm;
            Genotype = new int[algorythm.Problem.Dimensions];
        }
        public Individual(GeneticAlgorythm algorythm, int[] genotype)
        {
            Algorythm = algorythm;
            Genotype = genotype;
        }

        public int[] Genotype { get; set; }
        public void Mutate()
        {
                //exchange 2 gens
                int mutatedGen1Id = Algorythm.RandomGenerator.Next(0, Algorythm.Problem.Dimensions);
                int mutatedGen2Id = Algorythm.RandomGenerator.Next(0, Algorythm.Problem.Dimensions);
                int exchange = Genotype[mutatedGen1Id];
                Genotype[mutatedGen1Id] = Genotype[mutatedGen2Id];
                Genotype[mutatedGen2Id] = exchange;
        }
        public void Repair()
        {
            for (int i = 0; i < Genotype.Length; i++)
                for (int j = i+1; j < Genotype.Length; j++)
                {
                    //if gen repeats
                    if (Genotype[i]== Genotype[j])
                    {
                        //insert free number into gen
                        bool canInsert = true;
                        for (int k = 0; k < Genotype.Length; k++)
                        {
                            //insert free number into gen
                            for (int l = 0; l < Genotype.Length; l++)
                            {
                                if (Genotype[l] == k)
                                {
                                    canInsert = false;
                                    break;
                                }
                            }
                            if (canInsert)
                            {
                                Genotype[j] = k;
                                break;
                            }
                        }
                    }
                }
        }
        public void CalculateFitness()
        {
            Fitness = Algorythm.Problem.CalculateFitness(Genotype);
        }
        
        public Pair<Individual> Cross(Individual secondIndividual)
        {
            var children = new Pair<Individual>();
            children.First = this;
            children.Second = secondIndividual;
            children.First.Repair();
            children.Second.Repair();
            return children;
        }
        public override string ToString()
        {
            string value="";
            foreach(var gen in Genotype)
            {
                value += gen.ToString()+";";
            }
            return value;
        }
    }

    public class Pair<T>
    {
        public T First { get; set; }
        public T Second { get; set; }
        public Pair()
        {
        }
        public Pair(T first, T second)
        {
            First = first;
            Second = second;
        }
    }
}
