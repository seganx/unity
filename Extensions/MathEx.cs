using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathEx
{
    #region Vector3
    public static Vector3 LerpTo(this Vector3 self, Vector3 target, Vector3 t)
    {
        self.x = Mathf.Lerp(self.x, target.x, t.x);
        self.y = Mathf.Lerp(self.y, target.y, t.y);
        self.z = Mathf.Lerp(self.z, target.z, t.z);
        return self;
    }

    public static Vector3 LerpTo(this Vector3 self, Vector3 target, float tx, float ty, float tz)
    {
        self.x = Mathf.Lerp(self.x, target.x, tx);
        self.y = Mathf.Lerp(self.y, target.y, ty);
        self.z = Mathf.Lerp(self.z, target.z, tz);
        return self;
    }
    #endregion

    #region Quaternion
    public static Quaternion LerpTo(this Quaternion self, Quaternion target, float t)
    {
        return Quaternion.Lerp(self, target, t);
    }

    public static Quaternion LerpTo(this Quaternion self, float x, float y, float z, float t)
    {
        return Quaternion.Lerp(self, Quaternion.Euler(x, y, z), t);
    }
    #endregion
}
