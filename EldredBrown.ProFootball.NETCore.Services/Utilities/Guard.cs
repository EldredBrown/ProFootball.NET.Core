using System;

namespace EldredBrown.ProFootball.NETCore.Services.Utilities
{
    public static class Guard
    {
        public static void ThrowIfNull(object argValue, string paramName)
        {
            if (argValue is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
