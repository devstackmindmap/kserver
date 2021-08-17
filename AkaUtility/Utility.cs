using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AkaUtility
{
    public static class Utility
    {        
        public static TValue CopyFrom<TValue>(TValue originValue   )  where TValue : class, new() {

            if (originValue == null)
                return null;

            var fields = typeof(TValue).GetFields();
            var newObject = new TValue();
            foreach( var field in fields)
            {
                try
                {
                    if (field.FieldType.IsValueType)
                    {
                        field.SetValue(newObject, field.GetValue(originValue));
                    }
                    else
                    {
                        var obj = field.GetValue(originValue);
                        if (obj == null)
                            continue;

                        if (obj is ICloneable)
                        {
                            var cloneable = obj as ICloneable;
                            field.SetValue(newObject, cloneable.Clone());
                        }    
                        else if (obj is System.Collections.IEnumerable)
                        {
                            field.SetValue(newObject, Activator.CreateInstance(obj.GetType(), obj));
                        }
                    }
                }
               catch(Exception e)
                {

                }
            }

            var properties = typeof(TValue).GetProperties().Where( property => property.CanRead && property.CanWrite);
            foreach (var property in properties)
            {
                try
                {
                    if (property.PropertyType.IsValueType)
                    {
                        property.SetValue(newObject, property.GetValue(originValue));
                    }
                    else
                    {
                        var obj = property.GetValue(originValue);
                        if (obj == null)
                            continue;

                        if (obj is ICloneable)
                        {
                            var cloneable = obj as ICloneable;
                            property.SetValue(newObject, cloneable.Clone());
                        }
                        else if (obj is System.Collections.IEnumerable)
                        {
                            property.SetValue(newObject, Activator.CreateInstance(obj.GetType(), obj));
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return newObject;
        } 

        public static bool In<TValue>(this TValue item, params TValue[] values)
        {
            return values.Contains(item);
        }

        public static bool In<TValue>(this TValue item, IEnumerable<TValue> values)
        {
            return values?.Contains(item) ?? false;
        }

        public static Task CallDelayAfter(int milisecond, Action<Task> func)
        {
            return Task.Delay(milisecond).ContinueWith(func);
        }

        public static string RandomString(int length = 16)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[AkaRandom.Random.Next(s.Length)]).ToArray());
        }
    }
}
