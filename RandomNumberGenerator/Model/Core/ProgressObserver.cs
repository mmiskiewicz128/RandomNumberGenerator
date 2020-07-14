using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model.Core
{
    public class ProgressObserver : IProgressObserver
    {
        public Action AfterGenerateResult { get; set; }
    
        public ProgressObserver(Action actionAfterResult)
        {
            AfterGenerateResult = actionAfterResult;
        }
    }
}
