﻿using RandomNumberGenerator.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model
{
    public class SwapMethodNumbersGenerator : INumbersGenerator
    {
        private IGeneratorParams parameters;

        public SwapMethodNumbersGenerator(IGeneratorParams generatorData)
        {
            parameters = generatorData;
        }

        public Task<List<int>> Generate(CancellationToken cancelatonToken)
        {
            return Task<List<int>>.Run(() =>
            {
                Random rand = new Random();

                List<int> candidates = new List<int>();
                candidates.AddRange(Enumerable.Range(parameters.RangeStart, parameters.RangeEnd));
                candidates = candidates.Except(parameters.GeneratedNumbers).ToList();

                List<int> result = new List<int>();

                int candidatesCount = candidates.Count;
                for (int i = 0; i < parameters.NumbersToGenerate; i++)
                {
                    cancelatonToken.ThrowIfCancellationRequested();

                    int roundValue = rand.Next(candidatesCount);

                    result.Add(candidates[roundValue]);

                    candidates[roundValue] = candidates[candidatesCount - 1];

                    candidatesCount--;

                    parameters.ProgressObserver.InvokeAction(result.Count, parameters.NumbersToGenerate);
                }

                return result;
            });

        }
    }
}
