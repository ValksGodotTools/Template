namespace GodotUtils;

public static class CMath
{
    /// <summary>
    /// <para>Returns the sum of the first n natural numbers</para>
    /// <para>For example if n = 4 then this would return 0 + 1 + 2 + 3</para>
    /// </summary>
    public static int SumNaturalNumbers(int n)
    {
        return (n * (n - 1)) / 2;
    }

    public static uint UIntPow(this uint x, uint pow)
    {
        uint ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
            {
                ret *= x;
            }

            x *= x;
            pow >>= 1;
        }

        return ret;
    }
}
