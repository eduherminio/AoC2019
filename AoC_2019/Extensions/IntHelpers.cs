namespace AoC_2019.Extensions
{
    public static class IntHelpers
    {
        public static int Factorial(int n)
        {
            return n > 0
                ? n * Factorial(n - 1)
                : 1;
        }
    }
}
