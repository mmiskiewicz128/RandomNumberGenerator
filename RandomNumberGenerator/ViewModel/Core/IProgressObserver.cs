﻿using System;

namespace RandomNumberGenerator.ViewModel.Core
{
    public interface IProgressObserver
    {
        Action<int, int> ProgressInfoAction { get; set; }
        int ProgressMaxValue { get; set; }
        int ProgressValue { get; set; }
        bool IsInProgress { get; set; }
        decimal PercentageValue { get; set; }

        void InvokeAction(int value, int maxValue);

        void Reset();
    }
}
