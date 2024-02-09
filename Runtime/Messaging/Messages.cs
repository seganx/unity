using System;
using UnityEngine;

namespace SeganX
{
    public static class Messages
    {
        public class Param
        {
            public IComparable type = 0;
            public object value = null;

            public bool Is(IComparable expected) { return type.CompareTo(expected) == 0; }
            public bool Is<T>() { return value != null && value is T; }
            public T AsEnum<T>() where T : IComparable { return (T)type; }
            public T As<T>() { return (T)value; }
        }

        public const string MethodName = "OnMessage";

        public static void BroadcastMessage(Component sender, IComparable type, object parameter = null)
        {
            var data = new Param { type = type, value = parameter };
            sender.BroadcastMessage(MethodName, data, SendMessageOptions.DontRequireReceiver);
        }

        public static void Broadcast(this Component sender, IComparable type, object parameter = null)
        {
            BroadcastMessage(sender, type, parameter);
        }

        public static void Broadcast(this GameObject sender, IComparable type, object parameter = null)
        {
            BroadcastMessage(sender.transform, type, parameter);
        }

        public static void SendMessage(Component sender, IComparable type, object parameter = null)
        {
            var data = new Param { type = type, value = parameter };
            sender.SendMessage(MethodName, data, SendMessageOptions.DontRequireReceiver);
        }

        public static void Message(this Component sender, IComparable type, object parameter = null)
        {
            SendMessage(sender, type, parameter);
        }

        public static void Message(this GameObject sender, IComparable type, object parameter = null)
        {
            SendMessage(sender.transform, type, parameter);
        }

        public static void SendMessageUpward(this GameObject sender, IComparable type, object parameter = null)
        {
            var data = new Param { type = type, value = parameter };
            sender.SendMessageUpwards(MethodName, data, SendMessageOptions.DontRequireReceiver);
        }
    }
}