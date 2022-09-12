using System;

namespace SolarGames.FSM
{
    public static class Utils
    {
        public static bool IsSimpleType(this object t)
        {
            Type type = t.GetType();
            return type.IsPrimitive || type.Equals(typeof(string));
        }
    }
}

