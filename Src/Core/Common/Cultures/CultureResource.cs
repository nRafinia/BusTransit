using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common.Mappers;

namespace Common.Cultures
{
    public class CultureResource<K, T> : List<T>
        where T : class, ICultureBase
        where K : class
    {
        public K ApplyCulture(K baseClass, string lang = "")
        {
            if (string.IsNullOrWhiteSpace(lang))
                lang = CultureInfo.DefaultThreadCurrentCulture.Name;

            var obj = this.FirstOrDefault(t =>
                string.Equals(t.CultureName, lang, StringComparison.CurrentCultureIgnoreCase));
            if (obj == null)
            {
                return baseClass;
            }

            baseClass = obj.MapTo(baseClass);
            return baseClass;
        }
    }
}