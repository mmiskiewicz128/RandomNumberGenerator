using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model.Core
{
    public static class NumbersGeneratorFactory
    {
        public static INumbersGenerator CreateGenerator(int numbersToGenerate, int rangeStart, int rangeEnd, List<int> usedNumbers, IProgressObserver progressObserver)
        {
            IGeneratorData data = new GeneratorData()
            {
                NumbersToGenerate = numbersToGenerate,
                RangeStart = rangeStart,
                RangeEnd = rangeEnd,
                GeneratedNumbers = usedNumbers,
                ProgressObserver = progressObserver
            };

            int poolOfNumbers = rangeEnd - rangeStart - usedNumbers.Count;

            if(numbersToGenerate == 0 || poolOfNumbers / numbersToGenerate > 1)
            {
                return new CheckMethodNumbersGenerator(data);
            }

            return new RemoveMehodNumbersGenerator(data);
        }
    }
}
