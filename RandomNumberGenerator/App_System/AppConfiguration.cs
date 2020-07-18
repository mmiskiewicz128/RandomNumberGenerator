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
        private static readonly string _numberOfResultsToShow;
        private static readonly string _resultSaveBatchSize;

        static AppConfiguration()
        {
            _generatorNumbersRangeStart = ConfigurationManager.AppSettings["generatorNumbersRangeStart"];
            _generatorNumbersRangeEnd = ConfigurationManager.AppSettings["generatorNumbersRangeEnd"];
            _numberOfResultsToShow = ConfigurationManager.AppSettings["numberOfResultsToShow"];
            _connectionTimeout = ConfigurationManager.AppSettings["resultSaveConnectionTimeout"];
            _resultSaveBatchSize = ConfigurationManager.AppSettings["resultSaveBatchSize"];
            _connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"]?.ConnectionString;
        }



        public static int GetGeneratorNumbersRangeStart()
        {
            return GetIntValue(_generatorNumbersRangeStart);
        }

        public static int GetGeneratorNumbersRangeEnd()
        {
            return GetIntValue(_generatorNumbersRangeEnd);
        }

        public static int GetConnectionTimeout()
        {
            return GetIntValue(_connectionTimeout);
        }

        public static int GetNumberOfResultsToShow()
        {
            return GetIntValue(_numberOfResultsToShow);
        }

        public static int GetResultSaveBatchSize()
        {
            return GetIntValue(_resultSaveBatchSize);
        }

        public static string GetConnectionString()
        {
            return _connectionString;
        }


        private static int GetIntValue(string configValue)
        {
            int value = 0;
            Int32.TryParse(configValue, out value);

            return value;
        }


    }
}
