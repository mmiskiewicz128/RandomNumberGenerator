using RandomNumberGenerator.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model
{
    public class RemoveMehodNumbersGenerator : INumbersGenerator
    {
        private IGeneratorParams parameters;

        public RemoveMehodNumbersGenerator(IGeneratorParams generatorData)
        {
            parameters = generatorData;
        }

        public Task<List<int>> Generate(CancellationToken token)
        {
            return Task<List<int>>.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                Random rand = new Random();
                List<int> candidates = new List<int>();
                candidates.AddRange(Enumerable.Range(parameters.RangeStart, parameters.RangeEnd));

                candidates = candidates.Except(parameters.GeneratedNumbers).ToList();
                List<int> result = new List<int>();
  
                do
                {
                    token.ThrowIfCancellationRequested();
                    parameters.ProgressObserver.ProgressInfoAction?.Invoke(1);
                    int index = rand.Next(0, candidates.Count - 1);
                    result.Add(candidates[index]);
                    candidates.RemoveAt(index);
              
                } while (result.Count != parameters.NumbersToGenerate && candidates.Count != 0);


                return result;
            });

     
        }        
    }
}
