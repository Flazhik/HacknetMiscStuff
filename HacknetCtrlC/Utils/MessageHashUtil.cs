using System;

namespace HacknetCtrlC.Utils;

public static class MessageHashUtil
{
    public static ulong Hash(string text)
    {
        var s1 = text.Substring(0, text.Length / 2);
        var s2 = text.Substring(text.Length / 2);

        var MS4B = BitConverter.GetBytes(s1.GetHashCode());
        var LS4B = BitConverter.GetBytes(s2.GetHashCode());
        return (ulong)MS4B[0] << 56 | (ulong)MS4B[1] << 48 | 
                   (ulong)MS4B[2] << 40 | (ulong)MS4B[3] << 32 |
                   (ulong)LS4B[0] << 24 | (ulong)LS4B[1] << 16 | 
                   (ulong)LS4B[2] << 8  | LS4B[3];
    }
}