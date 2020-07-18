using RandomNumberGenerator.ViewModel.Core;
using System.Collections.Generic;

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
