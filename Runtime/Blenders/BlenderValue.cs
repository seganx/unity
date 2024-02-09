using UnityEngine;

namespace SeganX
{
    public struct BlenderValue
    {
        public float speed;
        public float destination;
        public float current;

        public void Setup(float initValue)
        {
            current = destination = initValue;
        }

        public bool Update(float deltaTime)
        {
            if (Mathf.Approximately(current, destination) == false)
            {
                current = Mathf.Lerp(current, destination, speed * deltaTime);
                if (Mathf.Approximately(current, destination)) current = destination;
                return true;
            }
            else return false;
        }
    }

}

