using System;

namespace AdofaiQolMod;

internal static partial class BuildInformation
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class BuildTimeAttribute(long buildTime) : Attribute
    {
        public readonly long BuildTime = buildTime;

        public override string ToString()
        {
            return $"{new DateTime(BuildTime).ToLocalTime():dd.MM.yyyy hh:mm:ss}";
        }
    }
}
