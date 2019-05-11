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
            if (current != destination)
            {
                current = Mathf.Lerp(current, destination, speed * deltaTime);
                return true;
            }
            else return false;
        }
    }

}

