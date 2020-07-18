using RandomNumberGenerator.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model
{
    public class CheckMethodNumbersGenerator : INumbersGenerator
    {
        private IGeneratorParams parameters;

        public CheckMethodNumbersGenerator(IGeneratorParams generatorData)
        {
            parameters = generatorData;
        }

        public Task<List<int>> Generate(CancellationToken token)
        {
            return Task<List<int>>.Run(() =>
            {
                Random rand = new Random();
                int[] numbers = new int[parameters.NumbersToGenerate];
                List<int> usedNumbersTemp = new List<int>();
                usedNumbersTemp.AddRange(parameters.GeneratedNumbers);

                bool test;
                int number;
                for (int i = 0; i < parameters.NumbersToGenerate; i++)
                {
                    do
                    {
                        token.ThrowIfCancellationRequested();

                        test = true;
                        number = rand.Next(parameters.RangeStart, parameters.RangeEnd);

                        if (usedNumbersTemp.Contains(number))
                        {
                            test = false;
                        }

                    } while (!test);

                    parameters.ProgressObserver.ProgressInfoAction?.Invoke(i + 1, parameters.NumbersToGenerate);
                    usedNumbersTemp.Add(number);
                    numbers[i] = number;
                }


                return numbers.ToList();
                
            });
           

        }
    }
}
