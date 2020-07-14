using RandomNumberGenerator.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model
{
    public class CheckMethodNumbersGenerator : INumbersGenerator
    {
        private IGeneratorData data;

        public CheckMethodNumbersGenerator(IGeneratorData generatorData)
        {
            data = generatorData;
        }

        public List<int> Generate()
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
                   test = true;
                   number = rand.Next(data.RangeStart, data.RangeEnd + 1);

                   if (usedNumbersTemp.Contains(number))
                   {
                       test = false;
                   }

               } while (!test);

               usedNumbersTemp.Add(number);
               numbers[i] = number;
           }

           data.ProgressObserver.AfterGenerateResult?.Invoke();
           return numbers.ToList();
           

        }
    }
}
