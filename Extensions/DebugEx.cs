using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


public static class DebugEx
{
    [System.Flags]
    public enum MemberType
    {
        Field = 1 << 1,
        Property = 1 << 2
    }

    public static string GetStringDebug(this object self, MemberType memberTypes = MemberType.Field | MemberType.Property, int levels = 3)
    {
        if (self == null) return "[null]";
        System.Type type = self.GetType();

        if (type.IsEnum || type.IsPrimitive || type == typeof(string))
        {
            return "[" + (self.ToString() == "\0" ? "null" : self.ToString()) + "]";
        }
        else if (type.IsArray)
        {
            string res = "{";
            var arr = self as System.Array;
            foreach (var a in arr)
                res += GetStringDebug(a, memberTypes, levels - 1) + " ";
            return (res.Length > 3 ? res.Remove(res.Length - 1) : res) + "}";
        }
        else if(type.IsGenericType && !type.IsValueType)
        {
            string res = "{";
            var arr = self as ICollection;
            foreach(var a in arr)
                res += GetStringDebug(a, memberTypes, levels - 1) + " ";
            return (res.Length > 3 ? res.Remove(res.Length - 1) : res) + "}";
        }
        else if (type.IsClass || type.IsValueType)
        {
            if (levels > 0)
            {
                string res = "{";
                var members = type.GetMembers();
                foreach (var member in members)
                {
                    if (member.MemberType == MemberTypes.Field && (memberTypes & MemberType.Field) != 0)
                    {
                        var field = member as FieldInfo;
                        res += field.Name + GetStringDebug(field.GetValue(self), memberTypes, levels - 1) + " ";
                    }
                    if (member.MemberType == MemberTypes.Property && (memberTypes & MemberType.Property) != 0)
                    {
                        var prop = member as PropertyInfo;
                        if (prop.CanRead)
                            res += prop.Name + GetStringDebug(prop.GetValue(self, null), memberTypes, levels - 1) + " ";
                    }
                }
                res = res.Remove(res.Length - 1) + "}";

                if (res.Length > 100)
                    return "\n" + res;
                else
                    return res;
            }
            else return "{}";
        }

        return "";
    }
}
