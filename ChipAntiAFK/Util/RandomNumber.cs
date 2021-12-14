using System;

namespace ChipAntiAFK
{
    public static class RandomNumber
    {
        private static Random random;
        public static int Random(int min, int max)
        {
            if (random == null) random = new Random();
            return random.Next(min, max);
        }
    }
}
