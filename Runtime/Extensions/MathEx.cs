using UnityEngine;

namespace SeganX
{
    public static class MathEx
    {
        #region Float 
        public static float RandomRadius(float startRadius, float endRadius)
        {
            return Random.Range(startRadius, endRadius) * Random.value < 0.5f ? -1 : 1;
        }
        #endregion

        #region Vector
        public static bool Any(this Vector2 self)
        {
            return
                self.x > Mathf.Epsilon || self.x < -Mathf.Epsilon ||
                self.y > Mathf.Epsilon || self.y < -Mathf.Epsilon;
        }

        public static bool Any(this Vector3 self)
        {
            return
                self.x > Mathf.Epsilon || self.x < -Mathf.Epsilon ||
                self.y > Mathf.Epsilon || self.y < -Mathf.Epsilon ||
                self.z > Mathf.Epsilon || self.z < -Mathf.Epsilon;
        }


        public static Vector2 RandomOnRectEdge(this Vector2 self)
        {
            return RandomOnRectEdge(self.x, self.y);
        }

        public static Vector2 RandomOnRectEdge(float width, float height)
        {
            var w = width * 0.5f;
            var h = height * 0.5f;

            if (Random.value < 0.5f)
            {
                if (Random.value < 0.5f)
                    return new Vector2(Random.Range(-w, w), h);
                else
                    return new Vector2(Random.Range(-w, w), -h);
            }
            else
            {
                if (Random.value < 0.5f)
                    return new Vector2(w, Random.Range(-h, h));
                else
                    return new Vector2(-w, Random.Range(-h, h));
            }
        }

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

        public static Vector2 Randomize(this Vector2 self)
        {
            Vector2 res;
            res.x = Random.Range(-self.x, self.x);
            res.y = Random.Range(-self.y, self.y);
            return res;
        }

        public static Vector3 Randomize(this Vector3 self)
        {
            Vector3 res;
            res.x = Random.Range(-self.x, self.x);
            res.y = Random.Range(-self.y, self.y);
            res.z = Random.Range(-self.z, self.z);
            return res;
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

        public static Quaternion LerpTo(this Quaternion quaternion1, Quaternion quaternion2, Vector4 amount)
        {
            Vector4 num2 = Vector4.one - amount;

            Quaternion quaternion = new Quaternion();
            float num5 = (((quaternion1.x * quaternion2.x) + (quaternion1.y * quaternion2.y)) + (quaternion1.z * quaternion2.z)) + (quaternion1.w * quaternion2.w);
            if (num5 >= 0f)
            {
                quaternion.x = (num2.x * quaternion1.x) + (amount.x * quaternion2.x);
                quaternion.y = (num2.y * quaternion1.y) + (amount.y * quaternion2.y);
                quaternion.z = (num2.z * quaternion1.z) + (amount.z * quaternion2.z);
                quaternion.w = (num2.w * quaternion1.w) + (amount.w * quaternion2.w);
            }
            else
            {
                quaternion.x = (num2.x * quaternion1.x) - (amount.x * quaternion2.x);
                quaternion.y = (num2.y * quaternion1.y) - (amount.y * quaternion2.y);
                quaternion.z = (num2.z * quaternion1.z) - (amount.z * quaternion2.z);
                quaternion.w = (num2.w * quaternion1.w) - (amount.w * quaternion2.w);
            }
            float num4 = (((quaternion.x * quaternion.x) + (quaternion.y * quaternion.y)) + (quaternion.z * quaternion.z)) + (quaternion.w * quaternion.w);
            float num3 = 1f / Mathf.Sqrt(num4);
            quaternion.x *= num3;
            quaternion.y *= num3;
            quaternion.z *= num3;
            quaternion.w *= num3;
            return quaternion;
        }
        #endregion
    }
}