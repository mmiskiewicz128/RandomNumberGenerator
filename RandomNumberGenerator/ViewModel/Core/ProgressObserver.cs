using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private int _progressMaxValue;

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


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Invoke(int Value, int maxValue)
        {
            ProgressInfoAction.Invoke(Value, maxValue);
        }

        public void Reset()
        {
            ProgressValue = 0;
            ProgressMaxValue = 0;
            PercentageValue = 0;
        }

        #endregion

        #region Private

        private decimal GetPercentageValue()
        {
            return ProgressMaxValue == 0 ? 100m : (int)(((decimal)ProgressValue / (decimal)ProgressMaxValue) * 100m);
        }

        #endregion
    }
}
