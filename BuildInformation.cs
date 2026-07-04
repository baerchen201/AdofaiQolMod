using System;

namespace AdofaiQolMod;

// ReSharper disable once PartialTypeWithSinglePart
internal static partial class BuildInformation
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class BuildTimeAttribute(long buildTime) : Attribute
    {
        public readonly long BuildTime = buildTime;

        public override string ToString()
        {
            return $"{new DateTime(BuildTime).ToLocalTime():yyyy-MM-dd HH:mm:ss}";
        }
    }

    public const bool DEBUG_BUILD =
#if DEBUG
        true;
#else
        false;
#endif
}
