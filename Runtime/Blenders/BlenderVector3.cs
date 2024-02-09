using UnityEngine;

namespace SeganX
{
    public struct BlenderVector3
    {
        public float speed;
        public Vector3 value;
        public Vector3 current;

        public BlenderVector3(float x, float y, float z, float blendSpeed)
        {
            speed = blendSpeed;
            value.x = current.x = x;
            value.y = current.y = y;
            value.z = current.z = z;
        }

        public void Setup(float x, float y, float z)
        {
            value.Set(x, y, z);
            current.Set(x, y, z);
        }

        public void Setup(Vector3 initValue)
        {
            current = value = initValue;
        }

        public bool Update(float deltaTime)
        {
            if (current != value)
            {
                var d = value - current;
                current += d * speed * deltaTime;
                if (d.magnitude <= deltaTime)
                    current = value;
                return true;
            }
            else return false;
        }

        public static BlenderVector3 zero = new BlenderVector3(0, 0, 0, 1);
        public static BlenderVector3 one = new BlenderVector3(1, 1, 1, 1);
    }
}
