using RandomNumberGenerator.App_System;
using RandomNumberGenerator.Model;
using RandomNumberGenerator.Model.Core;
using RandomNumberGenerator.Model.Extensions;
using RandomNumberGenerator.ViewModel.Commands;
using RandomNumberGenerator.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace RandomNumberGenerator.ViewModel
{
    public class RandomNumberGeneratorViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields

        private CancellationTokenSource cancellationTokenSource;
        private List<int> alreadyGeneratedNumbers = new List<int>();

        #endregion

        #region Properties

        public int RangeStart
        {
            get
            {
                return AppConfiguration.GetGeneratorNumbersRangeStart();
            }
        }

        public int RangeEnd
        {
            get
            {
                return AppConfiguration.GetGeneratorNumbersRangeEnd();
            }
        }

        private decimal _generatedNumbersPercentageUse;
        public decimal GeneratedNumbersPercentageUse
        {
            set
            {
                _generatedNumbersPercentageUse = value;
                OnPropertyChanged(nameof(GeneratedNumbersPercentageUse));
            }

            get
            {
                return _generatedNumbersPercentageUse;
            }
        }

        private int _numbersLeft;
        public int NumbersLeft
        {
            get
            {
                return _numbersLeft;
            }

            set
            {
                _numbersLeft = value;
                
                OnPropertyChanged(nameof(NumbersLeft));

            }
        }


        private bool _canRunGenerator = true;
        public bool CanRunGenerator
        {
            get
            {
                return _canRunGenerator;
            }

            set
            {
                _canRunGenerator = value;
                OnPropertyChanged(nameof(CanRunGenerator));

            }
        }

        private int _numbersToGenerate;
        public int NumbersToGenerate
        {
            get
            {
                return _numbersToGenerate;
            }

            set
            {
                _numbersToGenerate = value;
                ResetGenerator();
                OnPropertyChanged(nameof(NumbersToGenerate));

            }
        }


        private List<int> _generatorResult;
        public List<int> GeneratorResult
        {
            get
            {
                return _generatorResult;
            }

            set
            {
                _generatorResult = value;
                OnPropertyChanged(nameof(GeneratorResult));
            }
        }

        private bool _progressBarVisibility;
        public bool ProgressBarVisibility
        {
            get
            {
                return _progressBarVisibility;
            }

            set
            {
                _progressBarVisibility = value;
                OnPropertyChanged(nameof(ProgressBarVisibility));
            }
        }

        private int _progressBarGeneratorValue;
        public int ProgressBarGeneratorValue
        {
            get
            {
                return _progressBarGeneratorValue;
            }

            set
            {
                _progressBarGeneratorValue = value;
                OnPropertyChanged(nameof(ProgressBarGeneratorValue));
            }
        }

        private int _progressBarSaveDataValue;
        public int ProgressBarSaveDataValue
        {
            get
            {
                return _progressBarSaveDataValue;
            }

            set
            {
                _progressBarSaveDataValue = value;
                OnPropertyChanged(nameof(ProgressBarSaveDataValue));
            }

        }

        private int _progressBarSaveDataMaxValue = 100;
        public int ProgressBarSaveDataMaxValue
        {
            get
            {
                return _progressBarSaveDataMaxValue;
            }

            set
            {
                _progressBarSaveDataMaxValue = value;
                OnPropertyChanged(nameof(ProgressBarSaveDataMaxValue));
            }
        }


        private int _progressBarValuePercentageValue;
        public int ProgressBarValuePercentageValue
        {
            get
            {
                return _progressBarValuePercentageValue;
            }

            set
            {
                _progressBarValuePercentageValue = value;
                OnPropertyChanged(nameof(ProgressBarValuePercentageValue));
            }
        }

        private string _stringResult;
        public string StringResult
        {
            get
            {
                return _stringResult;
            }
            set
            {
                _stringResult = value;
                OnPropertyChanged(nameof(StringResult));
            }
        }

        private bool _isDataSaving;

        public bool IsDataSaving
        {
            get 
            { 
                return _isDataSaving; 
            }
            set 
            { 
                _isDataSaving = value;
                OnPropertyChanged(nameof(IsDataSaving));
            }
        }

        IProgressObserver GeneratorProgressObserver
        {
            get; set;
        }


        #endregion

        #region Commands
        public RunGeneratorCommand _runGenerator;
        public RunGeneratorCommand RunGenerator
        {
            get
            {
                return _runGenerator;
            }
            set
            {
                _runGenerator = value;
                OnPropertyChanged(nameof(RunGenerator));
            }
        }

        public CancelCommand Cancel
        {
            get; set;
        }

        #endregion

        #region Constructor

        public RandomNumberGeneratorViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InitializeData();
            }
                        
            GeneratorResult = new List<int>();

            RunGenerator = new RunGeneratorCommand(this);
            Cancel = new CancelCommand(this);
        }

        #endregion


        #region Public

        public async Task GenerateNumbers()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            GeneratorResult = new List<int>();
            
            ProgressBarGeneratorValue = ProgressBarSaveDataValue = 0;

            ProgressBarVisibility = true;
            CanRunGenerator = false;

            INumbersGenerator numbersGenerator = NumbersGeneratorFactory.CreateGenerator(GetGeneratorParams());

            try
            {
                _generatorResult = await Task.Run(() => numbersGenerator.Generate(token), token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await SaveResult();
            await InitailizeDataAsync();

        }

        public async Task SaveResult()
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                IsDataSaving = true;
                Action<int, int> action = new Action<int, int>((currentValue, maxValue) =>
                {
                    ProgressBarSaveDataValue = currentValue;
                    ProgressBarSaveDataMaxValue = maxValue;
                });

                await Task.Run(() =>
                {
                    dbContext.GeneratedNumbers.BulkInsert(_generatorResult.Select(i => new GeneratedNumber() { Number = i }), new ProgressObserver(action));
                }).ConfigureAwait(false);

                IsDataSaving = false;
            }
        }

        private async Task InitailizeDataAsync()
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                var numbers = await Task.Run(() =>
                {
                    return dbContext.GeneratedNumbers;
                   
                }).ConfigureAwait(false);

                alreadyGeneratedNumbers =  numbers.Select(r => r.Number).ToList();

                GeneratedNumbersPercentageUse = GetPercentageUse();
                NumbersLeft = GetNumbersLeft();
            }
        }

        public void ResetGenerator()
        {
     
            CanceGeneratorlTask();
            ProgressBarVisibility = false;
            ProgressBarGeneratorValue = ProgressBarValuePercentageValue = ProgressBarSaveDataValue = 0;
            CanRunGenerator = true;

        }

        public void AfterResult()
        {
            CanRunGenerator = true;
            StringResult =  string.Join(", ", _generatorResult);

   
        }

        public bool CanGenerateNumbers(int numbersToGenerate)
        {
            return NumbersToGenerate <= RangeEnd - RangeStart - alreadyGeneratedNumbers.Count;
        }



        #endregion

        #region Private

        private void CanceGeneratorlTask()
        {
            if (!cancellationTokenSource?.IsCancellationRequested ?? false)
            {
                cancellationTokenSource?.Cancel();
            }

        }

        private async void InitializeData()
        {
            await InitailizeDataAsync();
        }

        private IGeneratorParams GetGeneratorParams()
        {
            Action<int, int> action = new Action<int, int>((currentValue, maxValue) =>
            {
                ProgressBarGeneratorValue = currentValue;
                ProgressBarValuePercentageValue = GetPercentageProggress();
            });

            return ParamsFactory.CreateParams(NumbersToGenerate, RangeStart, RangeEnd, alreadyGeneratedNumbers, new ProgressObserver(action));
        }

        private int GetPercentageProggress()
        {
            return NumbersToGenerate == 0 ? 100 : (int)(((decimal)ProgressBarGeneratorValue / (decimal)NumbersToGenerate) * 100m);
        }

        private decimal GetPercentageUse()
        {
            return alreadyGeneratedNumbers.Count == 0 ? 0 : ((decimal)alreadyGeneratedNumbers.Count / (decimal)(RangeEnd - RangeStart)) * 100m;
        }

        private int GetNumbersLeft()
        {
            return RangeEnd - RangeStart - alreadyGeneratedNumbers.Count;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
