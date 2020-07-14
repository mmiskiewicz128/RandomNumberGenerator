using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.App_System
{
    static class Configuration
    {
        private static readonly string _generatorNumbersRangeStart = ConfigurationManager.AppSettings["generatorNumbersRangeStart"];
        private static readonly string _generatorNumbersRangeEnd = ConfigurationManager.AppSettings["GeneratorNumbersRangeEnd"];

        public static int GetGeneratorNumbersRangeStart()
        {
            int value = 0;
            Int32.TryParse(_generatorNumbersRangeStart, out value);

            return value;
        }

        public static int GetGeneratorNumbersRangeEnd()
        {
            int value = 0;
            Int32.TryParse(_generatorNumbersRangeEnd, out value);

            return value;
        }
    }
}
