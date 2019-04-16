using UnityEngine;

namespace SeganX
{
    public struct BlenderVector2
    {
        public enum BlendMode { StaticSpeed, Acceleration }

        public float speed;
        public Vector2 value;
        public Vector2 current;
        public BlendMode blendMode;


        public BlenderVector2(float x, float y, float blendSpeed, BlendMode mode = BlendMode.Acceleration)
        {
            speed = blendSpeed;
            value.x = current.x = x;
            value.y = current.y = y;
            blendMode = mode;
        }

        public void Setup(float x, float y)
        {
            value.x = current.x = x;
            value.y = current.y = y;
        }

        public BlenderVector2 Setup(Vector2 initValue)
        {
            current = value = initValue;
            return this;
        }

        public bool Update(float deltaTime)
        {
            if (current != value)
            {
                if (blendMode == BlendMode.StaticSpeed)
                {
                    current = Vector2.MoveTowards(current, value, 30 * speed * deltaTime);
                }
                else
                {
                    var d = value - current;
                    current += d * speed * deltaTime;
                    if (d.sqrMagnitude <= deltaTime) current = value;
                }
                return true;
            }
            else return false;
        }

        public static BlenderVector2 zero = new BlenderVector2(0, 0, 1);
        public static BlenderVector2 one = new BlenderVector2(1, 1, 1);
    }
}
