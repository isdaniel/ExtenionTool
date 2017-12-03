using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtensionTool
{
    public static class ExtensionAttribute
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
          this Type attrType,
          Func<TAttribute, TValue> selecotr) where TAttribute : Attribute
        {
            var attr = attrType.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            if (attr != null)
            {
                return selecotr(attr);
            }
            return default(TValue);
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(
            this FieldInfo field,
            Func<TAttribute, TValue> selector)
            where TAttribute : Attribute
        {
            TAttribute attr = Attribute.GetCustomAttribute(field, typeof(TAttribute)) as TAttribute;
            if (attr != null)
            {
                return selector(attr);
            }
            return default(TValue);
        }
    }
}