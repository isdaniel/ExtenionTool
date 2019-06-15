﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtensionTool
{
    public static class AttributeExtension
    {

        public static TValue GetAttributeValue<TAttribute, TValue>(
           this Type attrType,
           Func<TAttribute, TValue> selector) where TAttribute : Attribute
        {
            var attr = attrType.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            return GetValueOrDefault(selector, attr);
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(
             this FieldInfo field,
             Func<TAttribute, TValue> selector)
             where TAttribute : Attribute
        {
            TAttribute attr = Attribute.GetCustomAttribute(field, typeof(TAttribute)) as TAttribute;
            return GetValueOrDefault(selector, attr);
        }

        private static TValue GetValueOrDefault<TAttribute, TValue>
            (Func<TAttribute, TValue> selector, TAttribute attr)
            where TAttribute : Attribute
        {
            if (attr != null)
            {
                return selector(attr);
            }

            return default(TValue);
        }

        public static TAttribute GetAttribute<TAttribute>(
                this PropertyInfo prop,
                bool isInherit = true
            ) 
            where TAttribute : Attribute
        {
            return GetAttributeValue(prop, (TAttribute attr)=> attr, isInherit);
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(
                this PropertyInfo prop,
                Func<TAttribute, TValue> selector,
                bool isInherit = true
            )
            where TAttribute : Attribute
        {
            TAttribute attr = Attribute.GetCustomAttribute(prop, typeof(TAttribute), isInherit) as TAttribute;
            return GetValueOrDefault(selector, attr);
        }

        /// <summary>
        /// 是否使用此標籤
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsUseAttribute<TAttribute>(this PropertyInfo prop)
        {
            return Attribute.GetCustomAttribute(prop, typeof(TAttribute)) != null;
        }
    }
}