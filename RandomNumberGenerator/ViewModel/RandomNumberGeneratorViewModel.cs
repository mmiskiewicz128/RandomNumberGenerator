using RandomNumberGenerator.App_System;
using RandomNumberGenerator.Model;
using RandomNumberGenerator.Model.Core;
using RandomNumberGenerator.Model.Extensions;
using RandomNumberGenerator.ViewModel.Commands;
using RandomNumberGenerator.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RandomNumberGenerator.ViewModel
{
    public class RandomNumberGeneratorViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields

        private List<int> alreadyGeneratedNumbers = new List<int>();
        private CancellationTokenSource cancellationTokenSource;

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

        public int NumberOfResultsToShow
        {
            get
            {
                return AppConfiguration.GetNumberOfResultsToShow();
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


        private bool _canRunGenerator = false;
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

        private string _stringResult = string.Empty;
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

        private bool _isDataLoading;
        public bool IsDataLoading
        {
            get
            {
                return _isDataLoading;
            }

            set
            {
                _isDataLoading = value;
                OnPropertyChanged(nameof(IsDataLoading));
            }
        }

        public IProgressObserver GeneratorProgressObserver
        {
            get; set;
        }

        public IProgressObserver DataSavingProgressObserver
        {
            get; set;
        }

        #endregion

        #region Commands
        public RunGeneratorCommand RunGenerator
        {
            get; set;
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
            GeneratorProgressObserver = new ProgressObserver();
            DataSavingProgressObserver = new ProgressObserver();
        }

        #endregion


        #region Public

        public async Task GenerateNumbers()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            GeneratorResult = new List<int>();
            CanRunGenerator = false;
            INumbersGenerator numbersGenerator = NumbersGeneratorFactory.CreateGenerator(GetGeneratorParams());

            try
            {
                GeneratorResult = await Task.Run(() => numbersGenerator.Generate(token), token);
            }
            catch (OperationCanceledException)
            {
                cancellationTokenSource = null;
                return;
            }

            await SaveResult();
            await InitailizeDataAsync();
        }

        public async Task SaveResult()
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                await Task.Run(() =>
                {
                    dbContext.GeneratedNumbers.BulkInsert(GeneratorResult.Select(i => new GeneratedNumber() { Number = i }), DataSavingProgressObserver);
                }).ConfigureAwait(false);
            }
        }

        public void ResetGenerator()
        {
            StringResult = string.Empty;
            CanceGeneratorlTask();
            GeneratorProgressObserver.Reset();
            DataSavingProgressObserver.Reset();
            CanRunGenerator = true;
        }

        public void AfterResult()
        {
            CanRunGenerator = true;
            StringResult = string.Join(" ", _generatorResult.Take(AppConfiguration.GetNumberOfResultsToShow()));
            RaiseCanExecuteChanged();
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
            CanRunGenerator = true;
            RaiseCanExecuteChanged();
        }

        private async Task InitailizeDataAsync()
        {
            IsDataLoading = true;

            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                dbContext.Configuration.AutoDetectChangesEnabled = false;

                var numbers = await Task.Run(() =>
                {
                    return dbContext.GeneratedNumbers.Select(n => n.Number).ToList();

                }).ConfigureAwait(false);

                alreadyGeneratedNumbers = numbers;

                GeneratedNumbersPercentageUse = GetPercentageUse();
                NumbersLeft = GetNumbersLeft();
            }

            IsDataLoading = false;
        }

        private IGeneratorParams GetGeneratorParams()
        {
            return ParamsFactory.CreateParams(NumbersToGenerate, RangeStart, RangeEnd, alreadyGeneratedNumbers, GeneratorProgressObserver);
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
        private void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }
}
