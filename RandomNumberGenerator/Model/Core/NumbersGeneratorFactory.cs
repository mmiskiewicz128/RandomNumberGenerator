using RandomNumberGenerator.ViewModel.Core;
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
            IGeneratorParams parameters = new GeneratorParams()
            {
                NumbersToGenerate = numbersToGenerate,
                RangeStart = rangeStart,
                RangeEnd = rangeEnd,
                GeneratedNumbers = usedNumbers,
                ProgressObserver = progressObserver
            };

            int poolOfNumbers = rangeEnd - rangeStart - usedNumbers.Count;

            if (numbersToGenerate == 0 || (decimal)numbersToGenerate / (decimal)poolOfNumbers < 0.05m)
            {
                return new CheckMethodNumbersGenerator(parameters);
            }

            return new RemoveMehodNumbersGenerator(parameters);
        }
    }
}
