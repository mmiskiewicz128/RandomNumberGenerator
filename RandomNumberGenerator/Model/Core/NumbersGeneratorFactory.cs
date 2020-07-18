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
        public static INumbersGenerator CreateGenerator(IGeneratorParams parameters)
        {
            return new SwapMethodNumbersGenerator(parameters);
        }
    }
}
