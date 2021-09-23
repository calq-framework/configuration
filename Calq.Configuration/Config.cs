using Calq.Configuration.Attributes;
using Ghbvft6.Calq.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Calq.Configuration {

    public class Config {

        private static readonly Dictionary<Type, object> instances = new();

        static Config() {
            if (Directory.Exists("config") == false) {
                Directory.CreateDirectory("config");
            }
        }

        public static T Load<T>() where T : notnull, new() {
            if (instances.ContainsKey(typeof(T))) {
                return (T)instances[typeof(T)];
            }

            var instance = new T(); ;
            Load(ref instance);
            instances[typeof(T)] = instance;

            return instance;
        }

        public static void Load<T>(ref T instance) where T : notnull {

            string GetJsonPath() {
                return $"config/{typeof(T).FullName}.json";
            }

            void Deserialize(Utf8JsonReader reader, ref T instance) {
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
                                    currentInstance = Reflection.GetOrInitializeFieldOrPropertyValue(currentType, currentInstance, propertyName);
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
                    Reflection.SetFieldOrPropertyValue(currentType, currentInstance, propertyName, value);
                }
            }

            var jsonPath = GetJsonPath();
            var jsonText = File.ReadAllText(jsonPath);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonText);
            var reader = new Utf8JsonReader(jsonBytes, new JsonReaderOptions {
                CommentHandling = JsonCommentHandling.Skip
            });

            Deserialize(reader, ref instance);

            if (Attribute.GetCustomAttribute(typeof(T), typeof(OptionsAttribute)) != null) {
                Opts.LoadSkipUnknown(instance);
            }
        }
    }
}
