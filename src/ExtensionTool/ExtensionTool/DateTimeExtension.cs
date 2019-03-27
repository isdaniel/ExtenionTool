using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionTool
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 比較兩個時間的年差
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns>年差</returns>
        public static int YeaDifferent(this DateTime startDate,DateTime endDate)
        {
            DateTime timezone = new DateTime(1,1,1);
            
            TimeSpan span = endDate - startDate;

            return (timezone + span).Year;
        }
    }
}
