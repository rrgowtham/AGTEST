using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AHP.Core
{
    public static class EnumHelper
    {
        /// <summary>
        /// Provides Enum instance description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum element)
        {
            Type type = element.GetType();
            MemberInfo[] memberInfo = type.GetMember(element.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return element.ToString();
        }

        /// <summary>
        /// List of enum instances for an enum type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ToList<T>() 
        {
            Type enumType = typeof(T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");
            return Enum.GetValues(typeof(T)).Cast<T>();
            //return new List<T>();
        }
    }
}
