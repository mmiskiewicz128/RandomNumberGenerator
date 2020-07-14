using RandomNumberGenerator.App_System;
using RandomNumberGenerator.Model;
using RandomNumberGenerator.Model.Core;
using RandomNumberGenerator.ViewModel.Commands;
using RandomNumberGenerator.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Entity;
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
        private static object _lock = new object();
        private CancellationTokenSource cancellationTokenSource;
        private List<int> alreadyGeneratedNumbers = new List<int>();

        #endregion

        #region Properties

        public int RangeStart
        {
            get
            {
                return Configuration.GetGeneratorNumbersRangeStart();
            }
        }

        public int RangeEnd
        {
            get
            {
                return Configuration.GetGeneratorNumbersRangeEnd();
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

        private int _progressBarValue;
        public int ProgressBarValue
        {
            get
            {
                return _progressBarValue;
            }

            set
            {
                _progressBarValue = value;
                OnPropertyChanged(nameof(ProgressBarValue));
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

        private string _StringResult;
        public string StringResult
        {
            get
            {
                return _StringResult;
            }
            set
            {
                _StringResult = value;
                OnPropertyChanged(nameof(StringResult));
            }

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
        }

        #endregion


        #region Public

        public async Task GenerateNumbers()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            GeneratorResult = new List<int>();
            
            ProgressBarValue = 0;
            ProgressBarVisibility = true;
            CanRunGenerator = false;

            Action<int> action = new Action<int>((value) =>
            {
                ProgressBarValue += value;
                ProgressBarValuePercentageValue = GetPercentageProggress();
            });

            INumbersGenerator numbersGenerator = NumbersGeneratorFactory.CreateGenerator(NumbersToGenerate, RangeStart, RangeEnd, alreadyGeneratedNumbers, new ProgressObserver(action));

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
                await Task.Run(() =>
                {
                    dbContext.GeneratedNumbers.AddRange(_generatorResult.Select(i => new GeneratedNumber() { Number = i }));
              
                });

                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task InitailizeDataAsync()
        {
            using (ApplicationDbContext dbContext = new ApplicationDbContext())
            {
                var numbers = await Task.Run(() =>
                {
                    return dbContext.GeneratedNumbers;
                   
                });


                alreadyGeneratedNumbers =  numbers.Select(r => r.Number).ToList();

                GeneratedNumbersPercentageUse = GetPercentageUse();
                NumbersLeft = GetNumbersLeft();
            }
        }

        public void ResetGenerator()
        {
     
            CanceGeneratorlTask();
            ProgressBarVisibility = false;
            ProgressBarValue = ProgressBarValuePercentageValue = 0;
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

    

        private int GetPercentageProggress()
        {
            return NumbersToGenerate == 0 ? 100 : (int)(((decimal)ProgressBarValue / (decimal)NumbersToGenerate) * 100m);
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
