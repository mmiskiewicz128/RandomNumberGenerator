using RandomNumberGenerator.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model.Core
{
    public interface IGeneratorParams
    {
        int RangeStart { get; set; }

        int RangeEnd { get; set; }

        int NumbersToGenerate { get; set; }

        List<int> GeneratedNumbers { get; set; }

        IProgressObserver ProgressObserver { get; set; } 
    }
}
