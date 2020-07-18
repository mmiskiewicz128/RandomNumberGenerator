using RandomNumberGenerator.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model.Core
{
    public class GeneratorParams : IGeneratorParams
    {
        public int RangeStart { get ; set ; }
        public int RangeEnd { get ; set ; }
        public int NumbersToGenerate { get ; set ; }
        public List<int> GeneratedNumbers { get ; set ; }
        public IProgressObserver ProgressObserver { get; set; }

    }
}
