﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.Model.Core
{
    public interface IProgressObserver
    {
        Action AfterGenerateResult { get; set; }
    }
}
