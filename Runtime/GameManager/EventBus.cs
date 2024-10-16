using System;

namespace SeganX
{
    public static class EventBus<T>
    {
        public static event Action OnEvent;
        public static event Action<T> OnEventWithArg;

        public static void Rise() => OnEvent?.Invoke();
        public static void Rise(T arg) => OnEventWithArg?.Invoke(arg);
    }
}
