using RandomNumberGenerator.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model
{
    public class RemoveMehodNumbersGenerator : INumbersGenerator
    {
        IGeneratorData _data;

        public RemoveMehodNumbersGenerator(IGeneratorData generatorData)
        {
            _data = generatorData;
        }

        public List<int> Generate()
        {
           Random rand = new Random();
           List<int> candidates = new List<int>();
           candidates.AddRange(Enumerable.Range(_data.RangeStart, _data.RangeEnd));

           candidates = candidates.Except(_data.GeneratedNumbers).ToList();
           List<int> result = new List<int>();

           do
           {
               int index = rand.Next(0, candidates.Count - 1);
               result.Add(candidates[index]);
               candidates.RemoveAt(index);

           } while (result.Count != _data.NumbersToGenerate && candidates.Count != 0);

           return result;           
        }
    }
}
