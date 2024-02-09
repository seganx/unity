namespace SeganX
{
    public class Randomer
    {
        //! generate a random number from the seed
        public static int Random(int seed)
        {
            long res = ((seed * 214013L + 2531011L) >> 16);
            return (int)(res & 0x7fff);
        }

        //! generate a random number from the seed in 0 - exclusive range
        public static int Random(int seed, int range)
        {
            return Random(seed) % range;
        }
    }
}
