using System;
using System.Reflection;

namespace ThirdPartyExtension
{
    public class Singleton<T>
    {
        public static T Instance
        {
            get
            {
                return Nested.Instance;
            }
        }

        private class Nested
        {
            public static T Instance;

            static Nested()
            {
                ConstructorInfo ctor = typeof(T).GetConstructor(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic,
                    null,
                    new Type[0],
                    new ParameterModifier[0]);

                if (ctor == null)
                {
                    throw new NotSupportedException("No constructor without parameter");
                }

                Instance = (T)ctor.Invoke(null);
            }

            private Nested()
            {
            }
        }
    }

    public class Singleton<T, I> : Singleton<T> where T : I
    {
        public static new I Instance
        {
            get
            {
                return (I)Singleton<T>.Instance;
            }
        }
    }
}