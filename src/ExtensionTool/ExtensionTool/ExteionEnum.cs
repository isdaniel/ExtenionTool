using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionTool
{
    public static class ExteionEnum
    {
        public static IEnumerable<string> GetEnumString<TEnum>(this TEnum source) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be enum");
            }
            var arrEnum = Enum.GetValues(typeof(TEnum));
            var val = Convert.ToInt32(source);
            foreach (var e in arrEnum)
            {
                if ((val & (int)e) > 0)
                {
                    yield return e.ToString();
                }
            }
        }

        public static bool CheckAuth<TEnum>(this TEnum authSource, TEnum source, Func<TEnum, TEnum, bool> selector)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be enum");
            }

            //int? auth = authSource;
            return selector(authSource, source);
        }
    }
}