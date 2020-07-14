using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.ViewModel.Core
{
    public class ProgressObserver : IProgressObserver
    {
        public Action<int> ProgressInfoAction { get; set; }
    
        public ProgressObserver(Action<int> actionAfterResult)
        {
            ProgressInfoAction = actionAfterResult;
        }
    }
}
