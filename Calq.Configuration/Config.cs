using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Calq.Configuration {

    public class Config {

        private static Dictionary<Type, object> instances = new Dictionary<Type, object>();

        static Config() {
            if (Directory.Exists("config") == false) {
                Directory.CreateDirectory("config");
            }
        }

        public static T Load<T>() where T : new() {
            if (instances.ContainsKey(typeof(T))) {
                return (T)instances[typeof(T)];
            }

            var instance = new T(); ;
            Load(ref instance);

            return instance;
        }

        public static void Load<T>(ref T instance) {
            var jsonPath = GetJsonPath<T>();
            var jsonText = File.ReadAllText(jsonPath);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonText);
            var reader = new Utf8JsonReader(jsonBytes, new JsonReaderOptions {
                CommentHandling = JsonCommentHandling.Skip
            });

            Deserialize(reader, ref instance);
            instances[typeof(T)] = instance!;
        }

        private static string GetJsonPath<T>() {
            return $"config/{typeof(T).FullName}.json";
        }

        private static object? GetFieldOrPropertyValue(Type type, object obj, string fieldOrPropertyName) {
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

        private static object? GetOrInitializeFieldOrPropertyValue(Type type, object obj, string fieldOrPropertyName) {
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

        private static object? InitializeFieldOrPropertyValue(Type type, object obj, string fieldOrPropertyName) {
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

        private static void SetFieldOrPropertyValue(Type type, object obj, string fieldOrPropertyName, object? value) {
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

        public static void Deserialize<T>(Utf8JsonReader reader, ref T instance) {
            if (instance == null) {
                throw new ArgumentException("instance can't be null");
            }

            object? currentInstance = instance;
            var currentType = typeof(T);
            var instanceStack = new Stack<object>();

            reader.Read();
            if (reader.TokenType != JsonTokenType.StartObject) {
                throw new JsonException("json must be an object");
            }

            while (true) {
                reader.Read();
                string propertyName;
                switch (reader.TokenType) {
                    case JsonTokenType.PropertyName:
                        propertyName = reader.GetString()!;
                        break;
                    case JsonTokenType.EndObject:
                        if (instanceStack.Count == 0) {
                            if (reader.Read()) {
                                throw new JsonException();
                            }
                            return;
                        }
                        currentInstance = instanceStack.Pop();
                        currentType = currentInstance.GetType();
                        continue;
                    default:
                        throw new JsonException();
                }

                reader.Read();
                object? value;
                switch (reader.TokenType) {
                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        value = reader.GetBoolean();
                        break;
                    case JsonTokenType.String:
                        value = reader.GetString();
                        break;
                    case JsonTokenType.Number:
                        value = reader.GetInt32();
                        break;
                    case JsonTokenType.Null:
                        value = null;
                        break;
                    default:
                        switch (reader.TokenType) {
                            case JsonTokenType.StartObject:
                                instanceStack.Push(currentInstance);
                                currentInstance = GetOrInitializeFieldOrPropertyValue(currentType, currentInstance, propertyName);
                                if (currentInstance == null) {
                                    throw new JsonException();
                                }
                                currentType = currentInstance.GetType();
                                break;
                            case JsonTokenType.StartArray:
                                break;
                            default:
                                throw new JsonException();
                        }
                        continue;
                }
                SetFieldOrPropertyValue(currentType, currentInstance, propertyName, value);
            }
        }
    }
}
