using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionTool
{

    public static class BaseTypeExtension
    {
        /// <summary>
        /// 字串轉時間
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s)
        {
            DateTime time;
            return DateTime.TryParse(s, out time) ? time : default(DateTime?);
        }

        /// <summary>
        /// 字串轉整數
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt(this string s)
        {
            int val;
            return int.TryParse(s, out val) ? val : default(int);
        }

        /// <summary>
        /// 取得Enum欄位上的Attribute欄位
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetEnumDisplay<TEnum, TAttribute>(this byte val,Func<TAttribute, string> selector)
            where TEnum : struct
            where TAttribute : Attribute
        {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
                throw new ArgumentException("TEnum必須為enum");

            TEnum enumValue = (TEnum)Enum.ToObject(enumType, val);
            return typeof(TEnum).GetField(enumValue.ToString())
                .GetAttributeValue(selector);
        }

        /// <summary>
        /// 取得Enum欄位上的Attribute欄位
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string GetEnumDisplay<TEnum, TAttribute>(this byte? val, Func<TAttribute, string> selector)
            where TEnum : struct
            where TAttribute : Attribute
        {
            if (val.HasValue)
            {
                return val.Value.GetEnumDisplay<TEnum, TAttribute>(selector);
            }

            return string.Empty;
        }

        /// <summary>
        /// 字串轉Long
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long ToLongOrDefault(this string s)
        {
            long result = 0;
            return long.TryParse(s, out result) ? result : default(long);
        }

        /// <summary>
        /// 可空日期轉時間格式
        /// </summary>
        /// <param name="date"></param>
        /// <param name="Dformat"></param>
        /// <returns></returns>
        public static string DateStringOrEmpty(this DateTime? date, string Dformat)
        {
            return date.HasValue ? date.Value.ToString(Dformat) : string.Empty;
        }

        /// <summary>
        /// 物件轉字串
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ObjectToString(this object o)
        {
            return o == null ? string.Empty : o.ToString();
        }

        public static bool IsCollectionType(this object o)
        {
            if (o == null)
                throw new ArgumentNullException("parameter can't be null");

            return o is IEnumerable;
        }
    }
}
