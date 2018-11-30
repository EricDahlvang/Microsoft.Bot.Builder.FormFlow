using System;
using System.Runtime.Serialization;

namespace Microsoft.Bot.Builder.Internals.Fibers
{
    public static partial class SetField
    {
        public static void NotNull<T>(out T field, string name, T value) where T : class
        {
            CheckNull(name, value);
            field = value;
        }

        public static void CheckNull<T>(string name, T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void NotNullFrom<T>(out T field, string name, SerializationInfo info) where T : class
        {
            var value = (T)info.GetValue(name, typeof(T));
            SetField.NotNull(out field, name, value);
        }

        public static void From<T>(out T field, string name, SerializationInfo info)
        {
            var value = (T)info.GetValue(name, typeof(T));
            field = value;
        }
    }
}
