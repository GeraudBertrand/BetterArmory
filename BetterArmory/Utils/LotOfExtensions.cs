using System;

namespace BetterArmory.Utils
{
    public static class LotOfExtensions
    {

        public static T NextEnum<T>(this Random rand) where T : Enum
        {
            return (T)Enum.GetValues(typeof(T)).GetValue(rand.Next(Enum.GetValues(typeof(T)).Length));
        }

        public static T RandomEnum<T>(this T en) where T : struct, Enum
        {
            if (!typeof(T).IsEnum) { throw new Exception("random enum variable is not an enum"); }

            var random = new Random();
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(random.Next(values.Length));
        }
    }
}
