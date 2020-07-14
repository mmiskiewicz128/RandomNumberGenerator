using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if(parameter == null)
            {
                return false;
            }

            int value;
            int.TryParse(parameter.ToString(), out value);

            return ViewModel.CanGenerateNumbers(value);
        }

        public  void Execute(object parameter)
        {        
            ViewModel.GenerateNumbers();

        }
    }
}
