﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model.Core
{
    public interface INumbersGenerator
    {
        Task<List<int>> Generate(CancellationToken cancelatonToken);
    }
}
