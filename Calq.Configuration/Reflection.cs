using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ghbvft6.Calq.Configuration {
    internal class Reflection {
        public static object? GetFieldOrPropertyValue(Type type, object obj, string fieldOrPropertyName) {
            var field = type.GetField(fieldOrPropertyName);
            if (field != null) {
                return field.GetValue(obj);
            } else {
                var property = type.GetProperty(fieldOrPropertyName);
                if (property != null) {
                    return property.GetValue(obj);
                }
            }
            throw new MissingMemberException();
        }

        public static object? GetOrInitializeFieldOrPropertyValue(Type type, object obj, string fieldOrPropertyName) {
            object? value;
            var field = type.GetField(fieldOrPropertyName);
            if (field != null) {
                value = field.GetValue(obj);
                if (value == null) {
                    value = Activator.CreateInstance(field.FieldType);
                    field.SetValue(obj, value);
                }
                return value;
            } else {
                var property = type.GetProperty(fieldOrPropertyName);
                if (property != null) {
                    value = property.GetValue(obj);
                    if (value == null) {
                        value = Activator.CreateInstance(property.PropertyType);
                        property.SetValue(obj, value);
                    }
                    return value;
                }
            }
            throw new MissingMemberException();
        }

        public static object? InitializeFieldOrPropertyValue(Type type, object obj, string fieldOrPropertyName) {
            object? value;
            var field = type.GetField(fieldOrPropertyName);
            if (field != null) {
                value = Activator.CreateInstance(field.FieldType);
                field.SetValue(obj, value);
                return value;
            } else {
                var property = type.GetProperty(fieldOrPropertyName);
                if (property != null) {
                    value = Activator.CreateInstance(property.PropertyType);
                    property.SetValue(obj, value);
                    return value;
                }
            }
            throw new MissingMemberException();
        }

        public static void SetFieldOrPropertyValue(Type type, object obj, string fieldOrPropertyName, object? value) {
            var field = type.GetField(fieldOrPropertyName);
            if (field != null) {
                field.SetValue(obj, value);
            } else {
                var property = type.GetProperty(fieldOrPropertyName);
                if (property != null) {
                    property.SetValue(obj, value);
                } else {
                    throw new MissingMemberException();
                }
            }
        }
    }
}
