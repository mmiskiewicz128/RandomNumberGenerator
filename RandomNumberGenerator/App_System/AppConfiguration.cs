using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomNumberGenerator.App_System
{
    public static class AppConfiguration
    {
        private static readonly string _generatorNumbersRangeStart;
        private static readonly string _generatorNumbersRangeEnd;
        private static readonly string _connectionTimeout;
        private static readonly string _connectionString; 

        static AppConfiguration()
        {
            _generatorNumbersRangeStart = ConfigurationManager.AppSettings["generatorNumbersRangeStart"];
            _generatorNumbersRangeEnd = ConfigurationManager.AppSettings["generatorNumbersRangeEnd"];
            _connectionTimeout = ConfigurationManager.AppSettings["connectionTimeout"];
            _connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"]?.ConnectionString;
        }



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

        public static int GetConnectionTimeout()
        {
            int value = 0;
            Int32.TryParse(_connectionTimeout, out value);

            return value;
        }

        public static string GetConnectionString()
        {
            return _connectionString;
        }


    }
}
