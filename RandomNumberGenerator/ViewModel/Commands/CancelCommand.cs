using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RandomNumberGenerator.ViewModel.Commands
{
    public class CancelCommand : ICommand
    {
        public RandomNumberGeneratorViewModel ViewModel { get; private set; }

        public CancelCommand(RandomNumberGeneratorViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public bool CanExecute(object parameter)
        {
            return ViewModel.GeneratorProgressObserver.IsInProgress;
            
        }

        public void Execute(object parameter)
        {
            ViewModel.ResetGenerator();
        }


    }
}
