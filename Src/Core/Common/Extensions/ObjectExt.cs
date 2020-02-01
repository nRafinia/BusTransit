using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Common.Extensions
{
    public static class ObjectExt
    {
        public static bool IsList(this Type o)
        {
            return o.IsGenericType && o.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsIEnumerable(this Type o)
        {
            return o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static Array ConvertToArray<T>(this IEnumerable<T> obj, Type destType)
        {
            var sourceObj = obj as T[] ?? obj.ToArray();
            var items = Array.CreateInstance(destType, sourceObj.Count());
            for (var i = 0; i < sourceObj.Count(); i++)
            {
                items.SetValue(Convert.ChangeType(sourceObj.ElementAt(i), destType), i);
            }

            return items;
        }

        public static IList ConvertTo<T>(this IEnumerable<T> obj, Type destType)
        {
            var t = typeof(List<>).MakeGenericType(destType);
            var res = (IList) Activator.CreateInstance(t);

            foreach (var item in obj)
            {
                res.Add(Convert.ChangeType(item, destType));
            }

            return res;
        }

        public static object ConvertToEnum(this string obj, Type destType)
        {
            return Enum.Parse(destType, obj);
        }

        public static object ConvertToType<T>(this object obj, Type destType)
        {
            object res = null;
            if (destType.IsGenericType &&
                destType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                res = ConvertToType<T>(obj, destType.GenericTypeArguments[0]);
            }

            if (destType.IsEnum)
            {
                var v = obj;
                if (obj.GetType().IsArray)
                    v = ((IEnumerable<T>) obj).FirstOrDefault();

                res = ConvertToEnum(v?.ToString() ?? "0", destType);
            }

            if (destType.IsList() || destType.IsIEnumerable())
            {
                res = ConvertTo(obj as IEnumerable<T>, destType.GenericTypeArguments[0]);
            }

            if (destType.IsArray)
            {
                res = ConvertToArray(obj as IEnumerable<T>, destType.GetElementType());
            }

            if (res == null)
            {
                var v = obj;
                if (obj.GetType().IsArray)
                    v = ((IEnumerable<T>) obj).FirstOrDefault();

                res = Convert.ChangeType(v, destType);
            }

            return res;
        }

        public static void SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLamda,
            TValue value)
        {
            if (!(memberLamda.Body is MemberExpression memberSelectorExpression))
                return;

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property != null)
            {
                var str = memberSelectorExpression.ToString();
                SetPropertyValue(target, str.Substring(str.IndexOf('.') + 1), value);

                //property.SetValue(target, value, null);
            }
        }

        private static object SetPropertyValue(object src, string propName, object value)
        {
            if (src == null) //throw new ArgumentException("Value cannot be null.", "src");
                return value;

            if (propName == null) //throw new ArgumentException("Value cannot be null.", "propName");
                return value;

            if (propName.Contains(".")) //complex type nested
            {
                var temp = propName.Split(new char[] {'.'}, 2);
                return SetPropertyValue(GetPropertyValue(src, temp[0]), temp[1], value);
            }

            var prop = src.GetType().GetProperty(propName);
            if (prop != null)
            {
                prop.SetValue(src, value);
            }

            return value;
        }

        public static TValue GetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLamda)
        {
            if (!(memberLamda.Body is MemberExpression memberSelectorExpression))
                return default(TValue);

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property != null)
            {
                var str = memberSelectorExpression.ToString();
                var value = GetPropertyValue(target, str.Substring(str.IndexOf('.') + 1));

                return (TValue) value;
            }

            return default(TValue);
        }

        private static object GetPropertyValue(object src, string propName)
        {
            if (src == null) //throw new ArgumentException("Value cannot be null.", "src");
                return null;

            if (propName == null) //throw new ArgumentException("Value cannot be null.", "propName");
                return null;

            if (propName.Contains(".")) //complex type nested
            {
                var temp = propName.Split(new char[] {'.'}, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }

            var prop = src.GetType().GetProperty(propName);
            return prop != null ? prop.GetValue(src, null) : null;
        }

        public static T Cast<T>(this object myObj)
        {
            if (myObj == null)
                return default(T);

            var objectType = myObj.GetType();
            var target = typeof(T);
            var x = Activator.CreateInstance(target, false);
            var z = from source in objectType.GetMembers().ToList()
                where source.MemberType == MemberTypes.Property
                select source;
            var d = from source in target.GetMembers().ToList()
                where source.MemberType == MemberTypes.Property
                select source;
            var members = d.Where(memberInfo => z.Select(c => c.Name)
                .ToList().Contains(memberInfo.Name)).ToList();
            foreach (var memberInfo in members)
            {
                var propertyInfo = typeof(T).GetProperty(memberInfo.Name);
                var value = myObj.GetType().GetProperty(memberInfo.Name)?.GetValue(myObj, null);

                propertyInfo?.SetValue(x, value, null);
            }

            return (T) x;
        }

        public static bool IsValid<T>(this T model, out Dictionary<string, string> results)
            where T : class, new()
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, validationResults, true);

            results = validationResults.ToDictionary(v => v.MemberNames.FirstOrDefault(), v => v.ErrorMessage);
            return isValid;
        }

        public static bool IsValidList<T>(this IEnumerable<T> models, out Dictionary<string, string> results)
            where T : class, new()
        {
            results = null;
            var isValid = true;
            foreach (var model in models)
            {
                isValid = model.IsValid(out results);

                if (!isValid)
                    break;
            }

            return isValid;
        }

        public static string ToMd5(this string text)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(text));
                return Encoding.ASCII.GetString(result);
            }
        }
    }
}