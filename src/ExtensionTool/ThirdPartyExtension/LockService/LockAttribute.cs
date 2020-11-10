using System;

namespace ThirdPartyExtension
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LockAttribute : Attribute
    {
        public string Key { get; set; }

        public LockMode Mode { get; set; } = LockMode.XLock;

        public int Order{ get; set; }
    }
}