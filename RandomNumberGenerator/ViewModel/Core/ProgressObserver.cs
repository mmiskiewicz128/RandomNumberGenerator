using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace RandomNumberGenerator.ViewModel.Core
{
    public class ProgressObserver : IProgressObserver, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields
        public Action<int, int> ProgressInfoAction { get; set; }

        #endregion

        #region Properties

        private int _progressValue;

        public int ProgressValue
        {
            get 
            {
                return _progressValue; 
            }
            set 
            { 
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        private int _progressMaxValue = 100;

        public int ProgressMaxValue
        {
            get
            {
                return _progressMaxValue;
            }
            set
            {
                _progressMaxValue = value;
                OnPropertyChanged(nameof(ProgressMaxValue));
            }
        }

        private decimal _percentageValue;

        public decimal PercentageValue
        {
            get
            {
                return _percentageValue;
            }
            set
            {
                _percentageValue = value;
                OnPropertyChanged(nameof(PercentageValue));
            }
        }

        private bool _isInProgress;
        public bool IsInProgress
        {
            get
            {
                return _isInProgress;
            }
            set
            {
                _isInProgress = value;
                OnPropertyChanged(nameof(IsInProgress));
            }
        }
        #endregion

        #region Constructor

        public ProgressObserver()
        {
            ProgressInfoAction = new Action<int, int>((currentValue, maxValue) =>
            {
                ProgressValue = currentValue;
                ProgressMaxValue = maxValue;
                PercentageValue = GetPercentageValue();
            });
        }

        #endregion

        #region Public

        public ProgressObserver(Action<int, int> actionAfterResult)
        {
            ProgressInfoAction = actionAfterResult;
        }

        public void InvokeAction(int Value, int maxValue)
        {
            ProgressInfoAction?.Invoke(Value, maxValue);

            bool progresTemp = IsInProgress;
            IsInProgress = Value > 0 && Value != maxValue;

            if (progresTemp != IsInProgress)
            {
                RaiseCanExecuteChanged();
            }
        }

        public void Reset()
        {
            ProgressValue = 0;
            ProgressMaxValue = 100;
            PercentageValue = 0;
            IsInProgress = false;
        }

        #endregion

        #region Private

        private decimal GetPercentageValue()
        {
            return ProgressMaxValue == 0 ? 100m : (int)(((decimal)ProgressValue / (decimal)ProgressMaxValue) * 100m);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        #endregion
    }
}
