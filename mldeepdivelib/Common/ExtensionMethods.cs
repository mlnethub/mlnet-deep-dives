﻿using System.Linq;
using System.Reflection;

using Microsoft.ML.Data;

namespace mldeepdivelib.Common
{
    public static class ExtensionMethods
    {
        public static TextLoader.Column[] ToColumns<T>(this T model)
        {
            var fields = model.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            return fields.Select((t, x) => new TextLoader.Column(t.Name, t.PropertyType == typeof(string) ? DataKind.String : DataKind.Single, x)).ToArray();
        }

        public static string[] ToColumnNames<T>(this T model)
        {
            var fields = model.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            return fields.Select(a => a.Name).ToArray();
        }

        public static (string Name, int Min, int Max) GetLabelAttributes<T>(this T model)
        {
            var fields = model.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.CustomAttributes.All(a => a.AttributeType != typeof(LabelAttribute)))
                {
                    continue;
                }

                var attribute =
                    (LabelAttribute) field.GetCustomAttributes().FirstOrDefault(a => a.GetType() == typeof(LabelAttribute));

                if (attribute == null)
                {
                    continue;
                }

                return (field.Name, attribute.Min, attribute.Max);
            }

            return (null, -1, -1);
        }
    }
}