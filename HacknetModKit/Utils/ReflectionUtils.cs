using System.Reflection;

namespace HacknetModKit.Utils
{
    public static class ReflectionUtils
    {
        public static readonly string AssemblyDirectory = Assembly.GetExecutingAssembly().GetDirectoryPath();
    }
}