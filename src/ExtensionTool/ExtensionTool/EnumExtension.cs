using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionTool
{
    public static class EnumExtension
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

            return selector(authSource, source);
        }

        /// <summary>
        /// 取得Enum 欄位的Attribute
        /// </summary>
        /// <typeparam name="TEnum">enum</typeparam>
        /// <typeparam name="TAttribute">想要獲取的Attribute</typeparam>
        /// <typeparam name="TResult">返回結果</typeparam>
        /// <param name="e"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TResult GetAttributeValue<TEnum, TAttribute, TResult>(
            this TEnum e, Func<TAttribute, TResult> func)
             where TAttribute : Attribute
             where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("TEnum must be an enum!!");

            FieldInfo field = e.GetType().GetField(e.ToString());
            TAttribute attr = Attribute.GetCustomAttribute
                (field, typeof(TAttribute)) as TAttribute;

            if (attr != null)
            {
                return func(attr);
            }

            return default(TResult);
        }
    }
}