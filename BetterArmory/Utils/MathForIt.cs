using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BetterArmory.Utils
{
    internal class MathForIt
    {
        public static float RoundFloat(float value, int digits)
        {
            float mult = Mathf.Pow(10.0f, (float)digits);
            return Mathf.Round(value * mult) / mult;
        }
    }
}
