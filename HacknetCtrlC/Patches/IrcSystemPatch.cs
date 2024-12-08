using System;
using System.Linq;
using Hacknet;
using Hacknet.Daemons.Helpers;
using HacknetCtrlC.GUI;
using HacknetCtrlC.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util = Hacknet.Utils;

namespace HacknetCtrlC.Patches
{
    [HarmonyPatch(typeof(IRCSystem))]
    public class IrcSystemPatch
    {
        private static Rectangle _bounds;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(IRCSystem), "DrawLog")]
        public static bool IRCSystem_DrawLog_Prefix(Rectangle dest)
        {
            _bounds = dest;
            return true;
        }
        
      [HarmonyPrefix] 
      [HarmonyPatch(typeof(IRCSystem), "DrawLogEntry")]
      public static bool IRCSystem_DrawLogEntry_Prefix(
            IRCSystem __instance, IRCSystem.IRCLogEntry log,
            Rectangle startingDest,
            SpriteBatch sb,
            int lineHeight,
            int linesRemaining,
            int yNotToPass,
            out Rectangle dest)
        {
            dest = startingDest;
            var num1 = 55;
            var val1 = Settings.ActiveLocale != "en-us" ? 76 : 78;
            var num2 = 4;

            val1 = GuiData.ActiveFontConfig.name.ToLower() switch
            {
                "medium" => 92,
                "large" => 115,
                _ => val1
            };
            
            var num4 = Math.Max(val1, (int) (GuiData.tinyfont.MeasureString("<" + log.Author + ">").X + (double) num2));
            var width = dest.Width - (num1 + num2 + num4);
            var message = log.Message;
            var strArray = new[] { message };
            
            if (!log.Message.StartsWith("!ATTACHMENT:"))
              strArray = Util.SuperSmartTwimForWidth(message, width, GuiData.tinyfont).Split(Util.newlineDelim, StringSplitOptions.None);
            
            var dest1 = new Rectangle(dest.X + num1 + num2 + num4, dest.Y, dest.Width - (num2 + num4), dest.Height);
            var mouseIsInsidePanel = _bounds.Contains(GuiData.mouse.X, GuiData.mouse.Y);

            for (var index = strArray.Length - 1; index >= 0 && linesRemaining > 0; --index)
            {
              var vector2 = Util.ClipVec2ForTextRendering(new Vector2(dest.X, dest.Y + dest.Height - (GuiData.ActiveFontConfig.tinyFontCharHeight + 1f)));
              
              if (RightClick.DoRightClick(
                      MessageHashUtil.Hash(strArray[index]),
                      dest.X + num4 + 33,
                      (int)vector2.Y, dest1.Width,
                      (int)GuiData.ActiveFontConfig.tinyFontCharHeight + 4)
                  && mouseIsInsidePanel)
                  CopyDropdownMenu.Spawn(GuiData.mouse.X, GuiData.mouse.Y, strArray[index], strArray.Reverse().ToArray(), OS.currentInstance.shellButtonColor);

              if (mouseIsInsidePanel)
              {
                  CopyDropdownMenu.Area = CopyDropdownMenu.CopyDropdownMenuArea.Irc;
                  CopyDropdownMenu.DrawDropdownMenu();
              } else if (CopyDropdownMenu.Area == CopyDropdownMenu.CopyDropdownMenuArea.Irc)
              {
                  CopyDropdownMenu.Area = CopyDropdownMenu.CopyDropdownMenuArea.None;
                  CopyDropdownMenu.Despawn();
              }
              
              dest.Height -= lineHeight;
              --linesRemaining;
              if (dest.Y + dest.Height - 6 <= yNotToPass)
                break;
            }
            
            return true;
        }
    }
}