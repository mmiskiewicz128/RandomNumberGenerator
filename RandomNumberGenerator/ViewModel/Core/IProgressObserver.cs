using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.ViewModel.Core
{
    public interface IProgressObserver
    {
        Action<int> ProgressInfoAction { get; set; }
    }
}
