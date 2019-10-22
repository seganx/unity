namespace SeganX.DiffieHellman
{
    public class Bigint
    {
        byte[] number = new byte[128];

        public uint Length
        {
            get
            {
                uint res = 0;
                while (number[res] < 10) ++res;
                return res;
            }
        }

        public Bigint()
        {
            number[0] = 0;
            number[1] = 10;
        }

        public Bigint(uint v)
        {
            Assign(v);
        }

        public uint Mode(uint v)
        {
            uint res = 0;
            uint len = Length;
            for (int i = (int)len - 1; i >= 0; --i)
                res = (number[i] + res * 10) % v;
            return res;
        }

        //! compute the power and store that in this
        // original code from: https://discuss.codechef.com/questions/7349/computing-factorials-of-a-huge-number-in-cc-a-tutorial
        public Bigint Power(uint i, uint n)
        {
            number[0] = 1;	    // initializes array with only 1 digit, the digit 1.
            int m = 1;			// initializes digit counter
            int k = 1;			// k is a counter that goes from 1 to n.
            uint temp = 0;		// initializes carry variable to 0.
            while (k <= n)
            {
                for (int j = 0; j < m; j++)
                {
                    uint x = number[j] * i + temp;	    // x contains the digit by digit product
                    number[j] = (byte)(x % 10);			// contains the digit to store in position j
                    temp = x / 10;				        // contains the carry value that will be stored on later indexes
                }
                while (temp > 0)    // while loop that will store the carry value on array.
                {
                    number[m] = (byte)(temp % 10);
                    temp = temp / 10;
                    m++; // increments digit counter
                }
                k++;
            }
            number[m] = 10;

            return this;
        }

        private void Assign(uint v)
        {
            uint i = 0;
            while (v > 0)
            {
                number[i++] = (byte)(v % 10);
                v /= 10;
            }
            number[i] = 10;
        }
    };
}