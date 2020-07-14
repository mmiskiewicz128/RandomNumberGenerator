using RandomNumberGenerator.App_System;
using RandomNumberGenerator.Model;
using RandomNumberGenerator.Model.Core;
using RandomNumberGenerator.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;

namespace RandomNumberGenerator.ViewModel
{
    public class RandomNumberGeneratorViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields
        private static object _lock = new object();
        private ApplicationDbContext dbContext = new ApplicationDbContext();
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
                OnPropertyChanged(nameof(NumbersToGenerate));

            }
        }

        public RunGeneratorCommand RunGenerator
        {
            get; set;
        }

        #endregion

        #region Constructor

        public RandomNumberGeneratorViewModel()
        {
            GeneratorResult = new ObservableCollection<int>();

            RunGenerator = new RunGeneratorCommand(this);
            InitailizeData();

        }

        private ObservableCollection<int> _generatorResult;
        public ObservableCollection<int> GeneratorResult
        {
            get
            {
                return _generatorResult;
            }

            set
            {
                _generatorResult = value;
            }
        }

        #endregion


        #region Public

        private List<int> numns = new List<int>();

        public void GenerateNumbers()
        {
            GeneratorResult.Clear();
            INumbersGenerator numbersGenerator = NumbersGeneratorFactory.CreateGenerator(NumbersToGenerate, RangeStart, RangeEnd, alreadyGeneratedNumbers, new ProgressObserver(null));
            //var uiContext = SynchronizationContext.Current;
            //var test = await Task.Run(() => numbersGenerator.Generate())

            Task<List<int>> loadFileTask = Task.Run(() => numbersGenerator.Generate());

            Task.WaitAll(loadFileTask);
            loadFileTask.Wait();
            Task cleanupTask = loadFileTask.ContinueWith(
                (antecedent) => {

                    foreach (int num in antecedent.Result)
                    {
                        GeneratorResult.Add(num);
                    }
                },
                /* do not cancel this task */
                CancellationToken.None,
                /* run only if faulted main task */
                TaskContinuationOptions.OnlyOnFaulted,
                /* use main SynchronizationContext */
                TaskScheduler.FromCurrentSynchronizationContext());


            InitailizeData();
        }

        public bool CanGenerateNumbers(int numbersToGenerate)
        {
            return NumbersToGenerate < RangeEnd - RangeStart - alreadyGeneratedNumbers.Count;
        }

        #endregion

        #region Private

        private void InitailizeData()
        {
            var numbers = dbContext.GeneratedNumbers;
            alreadyGeneratedNumbers = numbers.Select(r => r.Number).ToList();
            GeneratedNumbersPercentageUse = alreadyGeneratedNumbers.Count == 0 ? 0 : (decimal)(RangeEnd - RangeStart) / (decimal)alreadyGeneratedNumbers.Count;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
