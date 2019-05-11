using UnityEngine;

namespace SeganX
{
    public struct BlenderVector2
    {
        public enum BlendMode { Acceleration, StaticSpeed }

        public float speed;
        public Vector2 destination;
        public Vector2 current;
        public BlendMode blendMode;


        public BlenderVector2(float x, float y, float blendSpeed, BlendMode mode = BlendMode.Acceleration)
        {
            speed = blendSpeed;
            destination.x = current.x = x;
            destination.y = current.y = y;
            blendMode = mode;
        }

        public void Setup(float x, float y)
        {
            destination.x = current.x = x;
            destination.y = current.y = y;
        }

        public BlenderVector2 Setup(Vector2 initValue)
        {
            current = destination = initValue;
            return this;
        }

        public bool Update(float deltaTime)
        {
            if (current != destination)
            {
                if (blendMode == BlendMode.StaticSpeed)
                {
                    current = Vector2.MoveTowards(current, destination, speed * deltaTime);
                }
                else
                {
                    current = Vector2.Lerp(current, destination, speed * deltaTime);
                }
                return true;
            }
            else return false;
        }

        public static BlenderVector2 zero = new BlenderVector2(0, 0, 1);
        public static BlenderVector2 one = new BlenderVector2(1, 1, 1);
    }
}
