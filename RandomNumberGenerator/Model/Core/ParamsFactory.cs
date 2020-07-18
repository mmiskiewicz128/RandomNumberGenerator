using RandomNumberGenerator.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model.Core
{
    public class ParamsFactory
    {
        public static IGeneratorParams CreateParams(int numbersToGenerate, int rangeStart, int rangeEnd, List<int> usedNumbers, IProgressObserver progressObserver)
        {
            return new GeneratorParams()
            {
                NumbersToGenerate = numbersToGenerate,
                RangeStart = rangeStart,
                RangeEnd = rangeEnd,
                GeneratedNumbers = usedNumbers,
                ProgressObserver = progressObserver
            };
        }
    }
}
