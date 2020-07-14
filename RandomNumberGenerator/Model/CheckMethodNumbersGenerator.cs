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
        private IGeneratorParams data;

        public CheckMethodNumbersGenerator(IGeneratorParams generatorData)
        {
            data = generatorData;
        }

        public Task<List<int>> Generate(CancellationToken token)
        {
            return Task<List<int>>.Run(() =>
            {
                Random rand = new Random();
                int[] numbers = new int[data.NumbersToGenerate];
                List<int> usedNumbersTemp = new List<int>();
                usedNumbersTemp.AddRange(data.GeneratedNumbers);

                bool test;
                int number;
                for (int i = 0; i < data.NumbersToGenerate; i++)
                {
                    do
                    {
                        token.ThrowIfCancellationRequested();

                        test = true;
                        number = rand.Next(data.RangeStart, data.RangeEnd + 1);

                        if (usedNumbersTemp.Contains(number))
                        {
                            test = false;
                        }

                    } while (!test);

                    data.ProgressObserver.ProgressInfoAction?.Invoke(1);
                    usedNumbersTemp.Add(number);
                    numbers[i] = number;
                }


                return numbers.ToList();
                
            });
           

        }
    }
}
