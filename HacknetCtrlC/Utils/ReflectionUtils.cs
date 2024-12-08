using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HacknetCtrlC.Models;

namespace HacknetCtrlC.Utils
{
    public static class ReflectionUtils
    {
        private static readonly Assembly HacknetAssembly = Assembly.GetEntryAssembly();
        
        public static readonly Type OsType = HacknetAssembly.GetType("Hacknet.OS");
        public static readonly Type NetworkMapType = HacknetAssembly.GetType("Hacknet.NetworkMap");
        public static readonly Type ComputerType = HacknetAssembly.GetType("Hacknet.Computer");
        public static readonly Type MailServerType = HacknetAssembly.GetType("Hacknet.MailServer");
        
        private static readonly FieldInfo NetMapField = OsType.GetField("netMap", PublicInstance);
        private static readonly FieldInfo NetMapNodesField = NetworkMapType.GetField("nodes", PublicInstance);
        private static readonly FieldInfo NetMapVisibleNodesField = NetworkMapType.GetField("visibleNodes", PublicInstance);
        private static readonly FieldInfo ComputerNameField = ComputerType.GetField("name", PublicInstance);
        private static readonly FieldInfo ComputerIpField = ComputerType.GetField("ip", PublicInstance);
        
        private static readonly MethodInfo OsWriteMethod = OsType.GetMethod("write", PublicInstance);

        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

        public static List<Computer> GetVisibleHosts(object osObj)
        {
            var netMap = NetMapField.GetValue(osObj);
            var nodes = (IEnumerable<object>)NetMapNodesField.GetValue(netMap);
            var visibleNodes = (List<int>)NetMapVisibleNodesField.GetValue(netMap);
            var computers = new List<Computer>(nodes.Count());
            var i = 0;
            foreach (var node in nodes)
            {
                if (visibleNodes.Contains(i))
                    computers.Add(new Computer
                    { 
                        Name = (string)ComputerNameField.GetValue(node),
                        Ip = (string)ComputerIpField.GetValue(node)
                    });
                i++;
            }

            return computers;
        }

        public static void OsWrite(object osObj, string msg)
        {
            OsWriteMethod.Invoke(osObj, new object[] { msg });
        }
    }
}