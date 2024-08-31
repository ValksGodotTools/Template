using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MySourceGenerator
{
    public static class StringExtensions
    {
        public static string SnakeCaseToPascalCase(this string str)
        {
            return string.Concat(
                str.Split('_')
                .Select(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase)
            );
        }
    }
}
