﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.ViewModel.Core
{
    public interface IProgressObserver
    {
        Action<int, int> ProgressInfoAction { get; set; }
        int ProgressMaxValue { get; set; }
        int ProgressValue { get; set; }

        void Invoke(int value, int maxValue);
        void Reset();
    }
}
