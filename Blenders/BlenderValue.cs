namespace SeganX
{
    public struct BlenderValue
    {
        public float speed;
        public float value;
        public float current;

        public void Setup(float initValue)
        {
            current = value = initValue;
        }            
        
        public bool Update(float deltaTime)
        {
            if (current != value)
            {
                float d = value - current;
                current += d * speed * deltaTime;
                if (System.Math.Abs(d) <= deltaTime)
                    current = value;
                return true;
            }
            else return false;
        }
    }

}

