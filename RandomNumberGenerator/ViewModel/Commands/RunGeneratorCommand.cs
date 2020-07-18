using System;
using System.Windows.Input;

namespace RandomNumberGenerator.ViewModel.Commands
{
    public class RunGeneratorCommand : ICommand
    {
        public RandomNumberGeneratorViewModel ViewModel { get; private set; }
        
        public RunGeneratorCommand(RandomNumberGeneratorViewModel viewModel)
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
            if(parameter == null || !ViewModel.CanRunGenerator)
            {
                return false;
            }

            int value = 0;
            int.TryParse(parameter.ToString(), out value);

            return ViewModel.CanGenerateNumbers(value);
        }

        public async void Execute(object parameter)
        {        
            await ViewModel.GenerateNumbers();
  
            ViewModel.AfterResult();

            CanExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
