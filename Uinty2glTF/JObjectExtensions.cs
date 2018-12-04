using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uinty2glTF
{
    internal static class JObjectExtensions
    {
        public static void SetValue<T>(this JObject obj, string propertyName, T value, T defaultValue)
        {
            if (value.Equals(defaultValue))
            {
                obj.Remove(propertyName);
            }
            else
            {
                obj[propertyName] = JToken.FromObject(value);
            }
        }
    }
}