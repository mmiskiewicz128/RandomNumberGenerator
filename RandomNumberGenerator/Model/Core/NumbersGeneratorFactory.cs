
namespace RandomNumberGenerator.Model.Core
{
    public static class NumbersGeneratorFactory
    {
        public static INumbersGenerator CreateGenerator(IGeneratorParams parameters)
        {
            return new SwapMethodNumbersGenerator(parameters);
        }
    }
}
