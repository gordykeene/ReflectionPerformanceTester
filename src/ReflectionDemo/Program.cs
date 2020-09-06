using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ReflectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 4; ++i)
            {
                var message = i == 0 ? " ignore this pass, while it primes the jitter" : "";
                Console.WriteLine($"Pass {i}{message}");
#if USE
                tester.test01();
                tester.test02();
                tester.test03();
                tester.test04();
                tester.test05();
                tester.test06();
                tester.test07();
                tester.test08();
                tester.test09();
                tester.test10();
#else
                tester.test01();
                tester.test09();
                tester.test10();
#endif
            }
        }
    }


    /*
    public class MyTestClass
    {
        public int Foo;
        public string Bar;
        public Point Baz;

        public Dictionary<string, Delegate>
            Setters =
                PerformanceReflectionDelegates<MyTestClass>()
                .Build();
    }
    */

    /*
    public class PerformanceReflectionDelegates<T>
    {
        public Dictionary<string, Delegate> _setters;

        Action<T, TData> setter<TData> = (Action<T, TData>)
            Delegate.CreateDelegate(
                    typeof(Action<T, TData>),
                    null,
                    typeof(T)
                        .GetProperty("Number")
                        .GetSetMethod());

        Func<T, int> getter =
            (Func<T, int>)
            Delegate.CreateDelegate(typeof(Func<T, int>),
                null,
                typeof(T)
                    .GetProperty("Number")
                    .GetGetMethod());

        public static Dictionary<string, Delegate> Build()
        {

        }

        public static TValue Get<TObj, TValue>(this TObj obj, string name)
        {
            PropertyInfo info = typeof(TObj).GetProperty(name);
            return (TValue)info.GetValue(obj);
        }

        public static void Set<TObj, TValue>(this TObj obj, string name, TValue value)
        {
            PropertyInfo info = typeof(TObj).GetProperty(name);
            info.SetValue(obj, value);
        }
    }
    */

    public class MyClass
    {
        public int Number { get; set; }
    }

    public class MyGlorifiedDictionary
    {
        public Dictionary<string, object> values = new Dictionary<string, object>();
    }

    public static class tester
    {
        /// <summary>
        ///  https://www.c-sharpcorner.com/article/boosting-up-the-reflection-performance-in-c-sharp/#:~:text=Boosting%20up%20the%20performance,This%20is%20accomplished%20using%20Delegates.
        /// </summary>
        public static void test01()
        {
            List<MyClass> myClassList =
                Enumerable.Repeat(new MyClass(), 10000000).ToList();
            object aux = 0;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            foreach (var obj in myClassList)
            {
                //aux = obj.Number;
                obj.Number = 3;
            }
            watch.Stop();
            Console.WriteLine($"test 01 Execution Time: {watch.ElapsedMilliseconds} ms (direct property access)");
        }


        public static void test02()
        {
            List<MyClass> myClassList =
                Enumerable.Repeat(new MyClass(), 10000000).ToList();
            object aux = 0;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            PropertyInfo info = typeof(MyClass).GetProperty("Number");
            foreach (var obj in myClassList)
            {
                //aux = info.GetValue(obj);
                info.SetValue(obj, 3);
            }
            watch.Stop();
            Console.WriteLine($"test 02 Execution Time: {watch.ElapsedMilliseconds} ms (simple reflection)");
        }

        public static void test03()
        {
            List<MyClass> myClassList =
                Enumerable.Repeat(new MyClass(), 10000000).ToList();
            object aux = 0;

            Action<MyClass, int> setter = (Action<MyClass, int>)Delegate.CreateDelegate(typeof(Action<MyClass, int>), null, typeof(MyClass).GetProperty("Number").GetSetMethod());
            Func<MyClass, int> getter = (Func<MyClass, int>)Delegate.CreateDelegate(typeof(Func<MyClass, int>), null, typeof(MyClass).GetProperty("Number").GetGetMethod());

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            foreach (var obj in myClassList)
            {
                //aux = getter(obj);
                setter(obj, 3);
            }
            watch.Stop();
            Console.WriteLine($"test 03 Execution Time: {watch.ElapsedMilliseconds} ms (delegated reflection)");
        }

        public static void test04()
        {
            List<MyClass> myClassList =
                Enumerable.Repeat(new MyClass(), 10000000).ToList();
            object aux = 0;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Action<MyClass, int> setter = (Action<MyClass, int>)Delegate.CreateDelegate(typeof(Action<MyClass, int>), null, typeof(MyClass).GetProperty("Number").GetSetMethod());
            Func<MyClass, int> getter = (Func<MyClass, int>)Delegate.CreateDelegate(typeof(Func<MyClass, int>), null, typeof(MyClass).GetProperty("Number").GetGetMethod());

            var setters = new Dictionary<string, Action<MyClass, int>>();
            var getters = new Dictionary<string, Func<MyClass, int>>();

            setters.Add("Number", setter);
            getters.Add("Number", getter);

            foreach (var obj in myClassList)
            {
                //aux = getters["Number"](obj);
                setters["Number"](obj, 3);
            }
            watch.Stop();
            Console.WriteLine($"test 04 Execution Time: {watch.ElapsedMilliseconds} ms (delegated reflections in a dictionary)");
        }

        public static void test05()
        {
            List<MyGlorifiedDictionary> myGlorifiedDictionary =
                Enumerable.Repeat(new MyGlorifiedDictionary(), 10000000).ToList();
            object aux = 0;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            foreach (var obj in myGlorifiedDictionary)
            {
                //obj.values.TryGetValue("Number", out aux);
                obj.values["Number"] = 3;
            }
            watch.Stop();
            Console.WriteLine($"test 05 Execution Time: {watch.ElapsedMilliseconds} ms (direct values in a dictionary)");
        }

        public static void test06()
        {
            List<MyClass> myClassList =
                Enumerable.Repeat(new MyClass(), 10000000).ToList();
            object aux = 0;

            Action<MyClass, int> setter = (Action<MyClass, int>)Delegate.CreateDelegate(typeof(Action<MyClass, int>), null, typeof(MyClass).GetProperty("Number").GetSetMethod());
            Func<MyClass, int> getter = (Func<MyClass, int>)Delegate.CreateDelegate(typeof(Func<MyClass, int>), null, typeof(MyClass).GetProperty("Number").GetGetMethod());

            var setters = new ConcurrentDictionary<string, Action<MyClass, int>>();
            var getters = new ConcurrentDictionary<string, Func<MyClass, int>>();

            setters.TryAdd("Number", setter);
            getters.TryAdd("Number", getter);

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            foreach (var obj in myClassList)
            {
                //aux = getters["Number"](obj);
                setters["Number"](obj, 3);
            }
            watch.Stop();
            Console.WriteLine($"test 06 Execution Time: {watch.ElapsedMilliseconds} ms (delegated reflections in a concurrent dictionary)");
        }

        public class MyDualModeClass
        {
            public int Number { get; set; }

            public Dictionary<string, MethodInfo> Setters = new Dictionary<string, MethodInfo>();
            public Dictionary<string, MethodInfo> Getters = new Dictionary<string, MethodInfo>();

            public MyDualModeClass()
            {
                PropertyInfo[] properties = typeof(MyDualModeClass).GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    Setters.TryAdd(property.Name, property.GetSetMethod());
                    Getters.TryAdd(property.Name, property.GetGetMethod());
                }

            }
        }

        public static void test07()
        {
            List<MyDualModeClass> myClassList =
                Enumerable.Repeat(new MyDualModeClass(), 10000000).ToList();
            object aux = 0;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            foreach (var obj in myClassList)
            {
                //aux = obj.Getters["Number"].Invoke(obj, null);
                obj.Setters["Number"].Invoke(obj, new object[] { 3 });
            }
            watch.Stop();
            Console.WriteLine($"test 07 Execution Time: {watch.ElapsedMilliseconds} ms (reflections in a dictionary 2)");
        }

        public class MyDualModeWithDelegatsClass
        {
            public int Number { get; set; }
            private int Number2 { get; set; }

            public string String { get; set; }

            // public Dictionary<string, Action<MyDualModeWithDelegatsClass, int>> Setters = new Dictionary<string, Action<MyDualModeWithDelegatsClass, int>>();
            // public Dictionary<string, Func<MyDualModeWithDelegatsClass, int>> Getters = new Dictionary<string, Func<MyDualModeWithDelegatsClass, int>>();
            public Dictionary<string, object> Setters = new Dictionary<string, object>();
            public Dictionary<string, object> Getters = new Dictionary<string, object>();


            public MyDualModeWithDelegatsClass()
            {
                PropertyInfo[] properties = typeof(MyDualModeWithDelegatsClass).GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    if (property.CanWrite)
                    {
                        if (property.PropertyType == typeof(int))
                            Setters.TryAdd(property.Name, CreateSetterDelegate<MyDualModeWithDelegatsClass, int>(property));
                        else if (property.PropertyType == typeof(string))
                            Setters.TryAdd(property.Name, CreateSetterDelegate<MyDualModeWithDelegatsClass, string>(property));
                    }

                    if (property.CanRead)
                    {
                        if (property.PropertyType == typeof(int))
                            Getters.TryAdd(property.Name, CreateGetterDelegate<MyDualModeWithDelegatsClass, int>(property));
                        else if (property.PropertyType == typeof(string))
                            Getters.TryAdd(property.Name, CreateGetterDelegate<MyDualModeWithDelegatsClass, string>(property));
                    }
                }
            }

            private static Action<TClass, TValue> CreateSetterDelegate<TClass, TValue>(PropertyInfo property)
                => (Action<TClass, TValue>)Delegate.CreateDelegate(
                    typeof(Action<TClass, TValue>),
                    null,
                    property.GetSetMethod());

            private static Func<TClass, TValue> CreateGetterDelegate<TClass, TValue>(PropertyInfo property)
                => (Func<TClass, TValue>)Delegate.CreateDelegate(
                    typeof(Func<TClass, TValue>), 
                    null, 
                    property.GetGetMethod());
        }

        public static void test08()
        {
            List<MyDualModeWithDelegatsClass> myClassList =
                Enumerable.Repeat(new MyDualModeWithDelegatsClass(), 10000000).ToList();
            object aux = 0;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            foreach (var obj in myClassList)
            {
                //aux = ((Func<MyDualModeWithDelegatsClass, int>)obj.Getters["Number"])(obj);
                ((Action<MyDualModeWithDelegatsClass, int>)obj.Setters["Number"])(obj, 3);

                // aux = ((Func<MyDualModeWithDelegatsClass, string>)obj.Getters["String"])(obj);
                // ((Action<MyDualModeWithDelegatsClass, string>)obj.Setters["String"])(obj, "3");
            }
            watch.Stop();
            Console.WriteLine($"test 08 Execution Time: {watch.ElapsedMilliseconds} ms (delegated reflections in a dictionary 2)");
        }

        /* ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ----  */

#if !USE_TEST09
        public class JustSomeClass
        {
            public int PropertyOne { get; set; }
            private int PropertyTwo { get; set; }
            public string PropertyThree { get; set; }

            public Dictionary<string, AccessorFactory<JustSomeClass>.Accessor> Accessors;
            public JustSomeClass()
            {
                Accessors = AccessorFactory<JustSomeClass>.Build(this);
            }
        }

        public static class AccessorFactory<T>
        {
            public class Accessor // <TValue>
            {
                public Func<object> Getter { set; private get; }
                public object Get() => Getter.Invoke();

                public Action<object> Setter { set; private get; }
                public void Set(object value) => Setter.Invoke(value);
            }

            public static Dictionary<string, Accessor> Build(T obj) {
                PropertyInfo[] properties = typeof(T).GetProperties();

                var result = new Dictionary<string, Accessor>();
                foreach (PropertyInfo property in properties)
                    if (property.CanWrite || property.CanRead)
                        result.Add(
                            property.Name,
                            new Accessor{
                                Getter = property.CanRead ? BuildGetter<T>(obj, property) : null,
                                Setter = property.CanWrite ? BuildSetter<T>(obj, property) : null
                            });
                return result;
            }

            private static Func<object> BuildGetter<TClass>(TClass obj, PropertyInfo property)
            {
                if (property.PropertyType == typeof(bool))
                {
                    var getter = CreateGetterDelegate<TClass, bool>(property);
                    return () => getter.Invoke(obj);
                }
                else if (property.PropertyType == typeof(int))
                {
                    var getter = CreateGetterDelegate<TClass, int>(property);
                    return () => getter.Invoke(obj);
                }
                else if (property.PropertyType == typeof(double))
                {
                    var getter = CreateGetterDelegate<TClass, double>(property);
                    return () => getter.Invoke(obj);
                }
                else if (property.PropertyType == typeof(string))
                {
                    var getter = CreateGetterDelegate<TClass, string>(property);
                    return () => getter.Invoke(obj);
                }
                else if (property.PropertyType == typeof(Point))
                {
                    var getter = CreateGetterDelegate<TClass, Point>(property);
                    return () => getter.Invoke(obj);
                }
                else if (property.PropertyType == typeof(Size))
                {
                    var getter = CreateGetterDelegate<TClass, Size>(property);
                    return () => getter.Invoke(obj);
                }
                else if (property.PropertyType == typeof(Color))
                {
                    var getter = CreateGetterDelegate<TClass, Color>(property);
                    return () => getter.Invoke(obj);
                }
                else if (property.PropertyType == typeof(Color[]))
                {
                    var getter = CreateGetterDelegate<TClass, Color[]>(property);
                    return () => getter.Invoke(obj);
                }
                else
                    Console.WriteLine($"Getter - unrecognized property type {property.PropertyType.Name}");
                return null;
            }

            private static Func<TClass, TValue> CreateGetterDelegate<TClass, TValue>(PropertyInfo property)
                => (Func<TClass, TValue>)Delegate.CreateDelegate(
                    typeof(Func<TClass, TValue>),
                    null,
                    property.GetGetMethod());

            private static Action<object> BuildSetter<TClass>(TClass obj, PropertyInfo property)
            {
                if (property.PropertyType == typeof(bool))
                {
                    var setter = CreateSetterDelegate<TClass, bool>(property);
                    return (value) => setter.Invoke(obj, (bool)value);
                }
                else if (property.PropertyType == typeof(int))
                {
                    var setter = CreateSetterDelegate<TClass, int>(property);
                    return (value) => setter.Invoke(obj, (int)value);
                }
                else if (property.PropertyType == typeof(double))
                {
                    var setter = CreateSetterDelegate<TClass, double>(property);
                    return (value) => setter.Invoke(obj, (double)value);
                }
                else if (property.PropertyType == typeof(string))
                {
                    var setter = CreateSetterDelegate<TClass, string>(property);
                    return (value) => setter.Invoke(obj, (string)value);
                }
                else if (property.PropertyType == typeof(Point))
                {
                    var setter = CreateSetterDelegate<TClass, Point>(property);
                    return (value) => setter.Invoke(obj, (Point)value);
                }
                else if (property.PropertyType == typeof(Size))
                {
                    var setter = CreateSetterDelegate<TClass, Size>(property);
                    return (value) => setter.Invoke(obj, (Size)value);
                }
                else if (property.PropertyType == typeof(Color))
                {
                    var setter = CreateSetterDelegate<TClass, Color>(property);
                    return (value) => setter.Invoke(obj, (Color)value);
                }
                else if (property.PropertyType == typeof(Color[]))
                {
                    var setter = CreateSetterDelegate<TClass, Color[]>(property);
                    return (value) => setter.Invoke(obj, (Color[])value);
                }
                else
                    Console.WriteLine($"BuildSetter - unrecognized property type {property.PropertyType.Name}");
                return null;
            }

            private static Action<TClass, TValue> CreateSetterDelegate<TClass, TValue>(PropertyInfo property)
                => (Action<TClass, TValue>)Delegate.CreateDelegate(
                    typeof(Action<TClass, TValue>),
                    null,
                    property.GetSetMethod());
        }

        public static void test09()
        {
            List<JustSomeClass> myClassList =
                Enumerable.Repeat(new JustSomeClass(), 10000000).ToList();
            object aux = 0;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            foreach (var obj in myClassList)
            {
                // aux = obj.Accessors["PropertyOne"].Get();
                // obj.Accessors["PropertyOne"].Set(3);
                obj.Accessors["PropertyThree"].Set("Hello");
            }
            watch.Stop();
            Console.WriteLine($"test 09 Execution Time: {watch.ElapsedMilliseconds} ms (accessor model)");
        }
#endif

        /* ---- ---- ---- ---- ---- */

        public class MyAttribtute : Attribute { }

        [MyAttribtute]
        public class JustSomeClassWithMyAttribtute
        {
            public int PropertyOne { get; set; }
            private int PropertyTwo { get; set; }
            public string PropertyThree { get; set; }
        }

        public class DynamicSetter
        {
#if NOAH1
            private static Dictionary<string, Dictionary<string, Action<object, string>>> CachedSetters = new Dictionary<string, Dictionary<string, Action<object, string>>>();
#elif !NOAH2
            private static Dictionary<string, Dictionary<string, Action<object, object>>> CachedSetters = new Dictionary<string, Dictionary<string, Action<object, object>>>();
#else
            private static Dictionary<string, Dictionary<string, Delegate>> CachedSetters = new Dictionary<string, Dictionary<string, Delegate>>();
#endif
            public static void Init()
            {
                Type[] types = TypesWithMyAttribute(typeof(MyAttribtute));
                foreach (var type in types)
                {
#if NOAH1
                    CachedSetters[type.FullName] = new Dictionary<string, Action<object, string>>();
#elif !NOAH2
                    CachedSetters[type.FullName] = new Dictionary<string, Action<object, object>>();
#else
                    CachedSetters[type.FullName] = new Dictionary<string, Delegate>();
#endif
                    var propInfos = type.GetProperties(); //  BindingFlags.Public | BindingFlags.SetProperty);
                    foreach (var propInfo in propInfos)
                    {
#if !NOAH1
                        if (propInfo.PropertyType == typeof(string))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, str) => propInfo.SetValue(obj, str);
                        else if (propInfo.PropertyType == typeof(int))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
#else
                        if (propInfo.PropertyType == typeof(string))
                            CachedSetters[type.FullName][propInfo.Name] = Delegate.CreateDelegate(
                                    typeof(Action<JustSomeClassWithMyAttribtute, string>),
                                    null,
                                    propInfo.GetSetMethod());
                        else if (propInfo.PropertyType == typeof(int))
                            CachedSetters[type.FullName][propInfo.Name] = Delegate.CreateDelegate(
                                    typeof(Action<JustSomeClassWithMyAttribtute, int>),
                                    null,
                                    propInfo.GetSetMethod());
#endif

#if TODO // (if we get that far)
                        else if (propInfo.PropertyType == typeof(bool))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
                        else if (propInfo.PropertyType == typeof(int))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
                        else if (propInfo.PropertyType == typeof(double))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
                        //else if (propInfo.PropertyType == typeof(string))
                        //    CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
                        else if (propInfo.PropertyType == typeof(Point))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
                        else if (propInfo.PropertyType == typeof(Size))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
                        else if (propInfo.PropertyType == typeof(Color))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
                        else if (propInfo.PropertyType == typeof(Color[]))
                            CachedSetters[type.FullName][propInfo.Name] = (obj, value) => propInfo.SetValue(obj, value);
#endif
                        else
                            Console.WriteLine($"Getter - unrecognized property type {propInfo.PropertyType.Name}");
                    }
                }
            }

            private static Type[] TypesWithMyAttribute(Type targetAttribute)
                => (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    let attributes = type.GetCustomAttributes(targetAttribute, true)
                    where attributes != null && attributes.Length > 0
                    select type)
                    .ToArray();

#if USE
            public static void Set(object obj, string propName, string value)
            {
                var type = obj.GetType();
                CachedSetters[type.FullName][propName](obj, value);
            }
#else
            public static void Set(object obj, string propName, string value)
            {
                var type = obj.GetType();
                //Delegate d = CachedSetters[type.FullName][propName];
                //d.DynamicInvoke(obj, value);
                // ((Action<object, string>)CachedSetters[type.FullName][propName])(obj, value);
                ((Action<JustSomeClassWithMyAttribtute, string>)CachedSetters[type.FullName][propName])((JustSomeClassWithMyAttribtute)obj, value);
            }
#endif
        }

        public static void test10()
        {
            List<JustSomeClassWithMyAttribtute> myClassList =
                Enumerable.Repeat(new JustSomeClassWithMyAttribtute(), 10000000).ToList();
            object aux = 0;

            DynamicSetter.Init();

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            foreach (var obj in myClassList)
            {
                // DynamicSetter.Set(obj, "PropertyOne", 3);
                DynamicSetter.Set(obj, "PropertyThree", "hello");
            }
            watch.Stop();
            Console.WriteLine($"test 10 Execution Time: {watch.ElapsedMilliseconds} ms (Noah's version)");
        }


    }
}
