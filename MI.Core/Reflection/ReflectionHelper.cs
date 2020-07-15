using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MI.Core.Reflection
{
    /// <summary>
    ///     定义用于反射的帮助方法
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        ///     检查 <paramref name="givenType" /> 是否实现或者继承自 <paramref name="genericType" />.
        /// </summary>
        /// <param name="givenType">需要检查的类型</param>
        /// <param name="genericType">需要匹配的目标类型</param>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var giveTypeInfo = givenType.GetTypeInfo();

            if (giveTypeInfo.IsGenericType && giveTypeInfo.GetGenericTypeDefinition() == genericType)
                return true;

            foreach (var interfaceType in giveTypeInfo.ImplementedInterfaces)
            {
                var interfaceTypeInfo = interfaceType.GetTypeInfo();
                if (interfaceTypeInfo.IsGenericType && interfaceTypeInfo.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (giveTypeInfo.BaseType == null)
                return false;

            return IsAssignableToGenericType(giveTypeInfo.BaseType, genericType);
        }

        /// <summary>
        ///     获取一个类型成员它包含的attributes以及它自身的类型所包含的attributes
        /// </summary>
        /// <typeparam name="TAttribute">特性的类型</typeparam>
        /// <param name="memberInfo">该类型成员</param>
        public static List<TAttribute> GetAttributesOfMemberAndDeclaringType<TAttribute>(MemberInfo memberInfo)
            where TAttribute : Attribute
        {
            var attributeList = new List<TAttribute>();

            //Add attributes on the member
            if (memberInfo.IsDefined(typeof(TAttribute), true))
                attributeList.AddRange(memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());

            //Add attributes on the class
            if (memberInfo.DeclaringType != null &&
                memberInfo.DeclaringType.GetTypeInfo().IsDefined(typeof(TAttribute), true))
                attributeList.AddRange(
                    memberInfo.DeclaringType.GetTypeInfo()
                        .GetCustomAttributes(typeof(TAttribute), true)
                        .Cast<TAttribute>());

            return attributeList;
        }

        /// <summary>
        ///     尝试获取一个类是否有定义指定的Attribute.如无则返回默认值
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="memberInfo"></param>
        /// <param name="defaultValue"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default(TAttribute), bool inherit = true)
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        /// <summary>
        /// 获取一个类的成员及其类型所包含的特性.
        /// </summary>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="type">Type</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        public static List<object> GetAttributesOfMemberAndType(MemberInfo memberInfo, Type type, bool inherit = true)
        {
            var attributeList = new List<object>();
            attributeList.AddRange(memberInfo.GetCustomAttributes(inherit));
            attributeList.AddRange(type.GetTypeInfo().GetCustomAttributes(inherit));
            return attributeList;
        }

        internal static bool IsPropertyGetterSetterMethod(MethodInfo method, Type type)
        {
            if (!method.IsSpecialName)
            {
                return false;
            }

            if (method.Name.Length < 5)
            {
                return false;
            }

            return type.GetProperty(method.Name.Substring(4), BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic) != null;
        }
    }
}
