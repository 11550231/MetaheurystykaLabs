using Lab1AlgorytmGenetyczny.GeneticAlgorythmNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Lab1AlgorytmGenetyczny
{
    public class Individual : ICloneable
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
            //var random = Algorythm.RandomGenerator.NextDouble();
            ////exchange 2 gens
            //if(random>0.5)
            //    SwapMutate();
            //else
            //    InversionMutate();
            //if(random > 0.5)
            //if(Algorythm.MUTATION_BY_SWAP)
                SwapMutate();
            //else
                InversionMutate();
        }
        public void InversionMutate()
        {
            var children = new Pair<Individual>();
            var smallerCutIndex = Algorythm.RandomGenerator.Next(0, Genotype.Length - 2);
            var biggerCutIndex = Algorythm.RandomGenerator.Next(smallerCutIndex + 1, Genotype.Length - 1);
            
            for(int i=0;i< (biggerCutIndex- smallerCutIndex+1)/2; i++)
            {
                int exchange = Genotype[smallerCutIndex+i];
                Genotype[smallerCutIndex+i] = Genotype[biggerCutIndex-i];
                Genotype[biggerCutIndex - i] = exchange;
            }
            
        }
        public void SwapMutate()
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
                        
                        for (int k = 0; k < Genotype.Length; k++)
                        {
                            bool canInsert = true;
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
            if(!Algorythm.CROSS_OX)
            return CrossPMX(secondIndividual);
            else
                return CrossOX(secondIndividual);
        }
        public Pair<Individual> CrossPMX(Individual secondIndividual)
        {
            var children = new Pair<Individual>();
            var smallerCutIndex = Algorythm.RandomGenerator.Next(0, Genotype.Length - 2);
            var biggerCutIndex = Algorythm.RandomGenerator.Next(smallerCutIndex + 1, Genotype.Length - 1);
            var firstChildGenotype = new int[Genotype.Length];
            var secondChildGenotype = new int[Genotype.Length];
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if (i < smallerCutIndex || i > biggerCutIndex)
                {
                    firstChildGenotype[i] = Genotype[i];
                    secondChildGenotype[i] = secondIndividual.Genotype[i];
                }
                else
                {
                    firstChildGenotype[i] = secondIndividual.Genotype[i];
                    secondChildGenotype[i] = Genotype[i];
                }
            }
            //jeśli jest konflikt kopiuję z drugiego rodzica
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if (i < smallerCutIndex || i > biggerCutIndex)
                {
                    if(firstChildGenotype.Count(x=> x == firstChildGenotype[i])>1)
                    {
                        firstChildGenotype[i] = secondIndividual.Genotype[i];
                    }
                    if (secondChildGenotype.Count(x => x == secondChildGenotype[i]) > 1)
                    {
                        secondChildGenotype[i] = Genotype[i];
                    }
                }
            }

            children.First = new Individual(Algorythm, firstChildGenotype);
            children.Second = new Individual(Algorythm, secondChildGenotype);
            children.First.Repair();
            children.Second.Repair();
            return children;
        }
        public Pair<Individual> CrossCuttingPartFromInsideOfIndiv(Individual secondIndividual)
        {
            var children = new Pair<Individual>();
            var smallerCutIndex = Algorythm.RandomGenerator.Next(0, Genotype.Length - 2);
            var biggerCutIndex = Algorythm.RandomGenerator.Next(smallerCutIndex+1, Genotype.Length - 1);
            var firstChildGenotype = new int[Genotype.Length];
            var secondChildGenotype = new int[Genotype.Length];
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if (i < smallerCutIndex || i > biggerCutIndex)
                {
                    firstChildGenotype[i] = Genotype[i];
                    secondChildGenotype[i] = secondIndividual.Genotype[i];
                }
                else
                {
                    firstChildGenotype[i] = secondIndividual.Genotype[i];
                    secondChildGenotype[i] = Genotype[i];
                }
            }
            children.First = new Individual(Algorythm, firstChildGenotype);
            children.Second = new Individual(Algorythm, secondChildGenotype);
            children.First.Repair();
            children.Second.Repair();
            return children;
        }
        public Pair<Individual> CrossSwapingGens(Individual secondIndividual)
        {
            var children = new Pair<Individual>();
            var swapedGenPosition = Algorythm.RandomGenerator.Next(0, Genotype.Length - 1);
            var firstChildGenotype = new int[Genotype.Length];
            var secondChildGenotype = new int[Genotype.Length];
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                    firstChildGenotype[i] = Genotype[i];
                    secondChildGenotype[i] = secondIndividual.Genotype[i];
            }
            firstChildGenotype[swapedGenPosition] = secondIndividual.Genotype[swapedGenPosition];
            secondChildGenotype[swapedGenPosition] = Genotype[swapedGenPosition];
            children.First = new Individual(Algorythm, firstChildGenotype);
            children.Second = new Individual(Algorythm, secondChildGenotype);
            children.First.Repair();
            children.Second.Repair();
            return children;
        }
        public Pair<Individual> CrossOX(Individual secondIndividual)
        {
            var children = new Pair<Individual>();
            var smallerCutIndex = Algorythm.RandomGenerator.Next(0, Genotype.Length - 2);
            var biggerCutIndex = Algorythm.RandomGenerator.Next(smallerCutIndex + 1, Genotype.Length - 1);
            var firstChildGenotype = new int[Genotype.Length];
            var secondChildGenotype = new int[Genotype.Length];

            for (int i = 0; i < this.Genotype.Length; i++)
            {
                    firstChildGenotype[i] = -1;
                    secondChildGenotype[i] = -1;
            }

            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if (! (i < smallerCutIndex || i > biggerCutIndex))
                {
                    firstChildGenotype[i] = secondIndividual.Genotype[i];
                    secondChildGenotype[i] = Genotype[i];
                }
            }
            //tworzę listę z niepodmienionych fragmentów genotypu i dla każdego dziecka używam genów które nie występują w nim


            List<int> usunedGens = new List<int>();
            //copy usused gens from first indiv
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if ((i < smallerCutIndex || i > biggerCutIndex))
                    usunedGens.Add(Genotype[i]);
            }
            //copy usused gens from second indiv
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if ((i < smallerCutIndex || i > biggerCutIndex))
                    usunedGens.Add(secondIndividual.Genotype[i]);
            }
            List<int> usunedGensInFirstChild = usunedGens.Where(x=> !firstChildGenotype.Contains(x)).GroupBy(y => y).Select(z => z.First()).ToList();
            List<int> usunedGensInSecondChild = usunedGens.Where(x => !secondChildGenotype.Contains(x)).GroupBy(y => y).Select(z => z.First()).ToList();
            int lastUsedFromFirstList = 0, lastUsedFromSecondList = 0;
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if ((i < smallerCutIndex || i > biggerCutIndex))
                {
                            firstChildGenotype[i] = usunedGensInFirstChild[lastUsedFromFirstList++];
                            secondChildGenotype[i] = usunedGensInSecondChild[lastUsedFromSecondList++];
                }
            }
            children.First = new Individual(Algorythm, firstChildGenotype);
            children.Second = new Individual(Algorythm, secondChildGenotype);
            return children;
        }
        public Pair<Individual> CrossDividiangIntoTwoParts(Individual secondIndividual)
        {
            var children = new Pair<Individual>();
            var cutIndex = Algorythm.RandomGenerator.Next(1, Genotype.Length - 2);
            var firstChildGenotype = new int[Genotype.Length];
            var secondChildGenotype = new int[Genotype.Length];
            for (int i = 0; i < this.Genotype.Length; i++)
            {
                if (i < cutIndex)
                {
                    firstChildGenotype[i] = Genotype[i];
                    secondChildGenotype[i] = secondIndividual.Genotype[i];
                }
                else
                {
                    firstChildGenotype[i] = secondIndividual.Genotype[i];
                    secondChildGenotype[i] = Genotype[i];
                }
            }
            children.First = new Individual(Algorythm, firstChildGenotype);
            children.Second = new Individual(Algorythm, secondChildGenotype);
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

        public object Clone()
        {
            var genotype = new int[Genotype.Length];
            for (int i = 0; i < Genotype.Length; i++)
                genotype[i] = Genotype[i];
            var newObj = new Individual(Algorythm, genotype);
            return newObj;
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
