// The MIT License (MIT)
// 
// Copyright (c) 2015 Capsor Consulting AB
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WindowsPhoneSample.Core.Extensions;

namespace WindowsPhoneSample.Core
{
    public enum IocScope
    {
        Singleton,
        Instance
    }

    public sealed class IoC : IDisposable
    {
        private readonly Dictionary<Type, Tuple<Type, IocScope>> dependencyMap;
        private readonly Dictionary<Type, Tuple<object, IocScope>> factoryMap;
        private readonly ConcurrentDictionary<Type, Object> instanceCache;

        public IoC()
        {
            dependencyMap = new Dictionary<Type, Tuple<Type, IocScope>>();
            factoryMap = new Dictionary<Type, Tuple<object, IocScope>>();
            instanceCache = new ConcurrentDictionary<Type, object>();
        }

        ~IoC()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // managed types
                instanceCache.Where(x => x.Value is IDisposable).Select(x => x.Value).Cast<IDisposable>().ToList().ForEach(d =>
                {
                    if (d != null) d.Dispose();
                });
            }
            // unmanaged types
            instanceCache.Clear();
            factoryMap.Clear();
            dependencyMap.Clear();
        }

        public T Resolve<T>(Type t)
        {
            return (T)Resolve(t);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public void Register<TFrom, TTo>(IocScope iocScope = IocScope.Singleton)
        {
            Type type = typeof(TFrom);
            if (dependencyMap.ContainsKey(type))
            {
                object obj;
                instanceCache.TryRemove(type, out obj);
                dependencyMap.Remove(type);
            }
            dependencyMap[typeof(TFrom)] = Tuple.Create(typeof(TTo), iocScope);
        }

        public void Register<TFrom>(Func<TFrom> factory, IocScope iocScope = IocScope.Singleton)
        {
            factoryMap[typeof(TFrom)] = Tuple.Create((object)factory, iocScope);
        }

        public void RegisterInstance<TFrom>(TFrom instance)
        {
            instanceCache[typeof(TFrom)] = instance;
            dependencyMap[typeof(TFrom)] = Tuple.Create(typeof(TFrom), IocScope.Singleton);
        }

        public void UnregisterInstance<TFrom>(TFrom instance)
        {
            var key = typeof(TFrom);
            object existing;
            if (instanceCache.TryGetValue(key, out existing) && ReferenceEquals(existing, instance))
            {
                object dummy;
                instanceCache.TryRemove(key, out dummy);
                dependencyMap.Remove(key);
            }
        }

        private object Resolve(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            Tuple<Type, IocScope> resolvedType = LookUpDependency(type);

            object instance;
            if (resolvedType == null)
            {
                instance = ResolveFactory(type);
                if (instance == null)
                {
                    string typeName = type.FullName;
                    throw new ArgumentOutOfRangeException("type", type, typeName + " could not be resolved by " + GetType().Name);
                }
            }
            else if (resolvedType.Item2 == IocScope.Instance || !instanceCache.TryGetValue(type, out instance))
            {
                // Check instance cache            
                object createInstance;
                ConstructorInfo constructor = resolvedType.Item1.GetTypeInfo().DeclaredConstructors.FirstOrDefault(x => !x.IsStatic);
                if (constructor == null)
                {
                    createInstance = Activator.CreateInstance(resolvedType.Item1);
                }
                else
                {
                    ParameterInfo[] parameters = constructor.GetParameters();

                    if (!parameters.Any())
                    {
                        createInstance = Activator.CreateInstance(resolvedType.Item1);
                    }
                    else
                    {
                        createInstance = constructor.Invoke(ResolveParameters(parameters).ToArray());
                    }
                }

                if (resolvedType.Item2 == IocScope.Singleton)
                {
                    instance = instanceCache.GetOrAdd(type, createInstance);
                }
                else
                {
                    instance = createInstance;
                }
            }

            return instance;
        }

        private object ResolveFactory(Type type)
        {
            object instance = null;
            Tuple<object, IocScope> factory;
            if (factoryMap.TryGetValue(type, out factory))
            {
                Func<object> typeFactory = factory.Item1 as Func<object>;
                if (typeFactory != null)
                {
                    if (factory.Item2 == IocScope.Instance || !instanceCache.TryGetValue(type, out instance))
                    {
                        instance = typeFactory();

                        if (factory.Item2 == IocScope.Singleton)
                        {
                            instance = instanceCache.GetOrAdd(type, instance);
                        }
                    }
                }
            }
            return instance;
        }

        private Tuple<Type, IocScope> LookUpDependency(Type type)
        {
            Tuple<Type, IocScope> result;
            dependencyMap.TryGetValue(type, out result);
            return result;
        }

        private IEnumerable<object> ResolveParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(p => Resolve(p.ParameterType));
        }

        public IEnumerable<Type> FindMatchingTypes<T>()
        {
            Type matchType = typeof(T);
            foreach (Type tp in dependencyMap.Keys)
            {
                if (matchType.GetTypeInfo().IsAssignableFrom(tp.GetTypeInfo()))
                {
                    yield return tp;
                }
            }
        }

        public IEnumerable<T> ResolveMatchingInstances<T>()
        {
            Type matchType = typeof(T);
            foreach (Type tp in factoryMap.Keys)
            {
                if (matchType.GetTypeInfo().IsAssignableFrom(tp.GetTypeInfo()))
                {
                    yield return (T)Resolve(tp);
                }
            }
        }
    }
}