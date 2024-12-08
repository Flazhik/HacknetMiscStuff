using System;
using Hacknet;
using HacknetCtrlC.GUI;
using HacknetCtrlC.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Util = Hacknet.Utils;

namespace HacknetCtrlC.Patches
{
    [HarmonyPatch(typeof(DisplayModule))]
    public class DisplayModulePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DisplayModule), "doCatDisplay")]
        public static void MailServer_doCatDisplay_Prefix(DisplayModule __instance)
        {
          if (!__instance.os.hasConnectionPermission(false))
            return;
          
          var tmpRect = GuiData.tmpRect with
          {
            Width = __instance.bounds.Width,
            X = __instance.bounds.X,
            Y = __instance.bounds.Y + 1,
            Height = __instance.bounds.Height - 2
          };
          
          if (__instance.os.connectedComp != null &&
              __instance.os.connectedComp.ip != __instance.LastDisplayedFileSourceIP &&
              __instance.LastDisplayedFileSourceIP != __instance.os.thisComputer.ip)
            return;
          
          var data = "";
          for (var index = 1; index < __instance.commandArgs.Length; ++index)
            data = data + __instance.commandArgs[index] + " ";
          var text1 = LocalizedFileLoader.SafeFilterString(data);
          
          if (__instance.LastDisplayedFileFolder.searchForFile(text1.Trim()) == null)
            return;
          var num = 55;
          var dest = new Rectangle(tmpRect.X + 4, tmpRect.Y + num + 3, tmpRect.Width - 6, tmpRect.Height - num - 2);
          var displayCache = __instance.os.displayCache;
          var text2 = Util.SuperSmartTwimForWidth(LocalizedFileLoader.SafeFilterString(displayCache), __instance.bounds.Width - 40, GuiData.tinyfont);

          var strArray = text2.Split(new []
          {
            "\r\n",
            "\n"
          }, StringSplitOptions.None);

          var tinyFontCharHeight = GuiData.ActiveFontConfig.tinyFontCharHeight;
          var input = new Vector2(dest.X, dest.Y);
          var mouseIsInsidePanel = dest.Contains(GuiData.mouse.X, GuiData.mouse.Y);
          foreach (var str in strArray)
          {
            var pos = Util.ClipVec2ForTextRendering(input);
            if (RightClick.DoRightClick(MessageHashUtil.Hash(str), (int)pos.X, (int)pos.Y, dest.Width, (int)tinyFontCharHeight) && mouseIsInsidePanel)
              CopyDropdownMenu.Spawn(GuiData.mouse.X, GuiData.mouse.Y, str, strArray, __instance.os.shellButtonColor);

            if (mouseIsInsidePanel)
            {
              CopyDropdownMenu.Area = CopyDropdownMenu.CopyDropdownMenuArea.DisplayCat;
              CopyDropdownMenu.DrawDropdownMenu();
            } else if (CopyDropdownMenu.Area == CopyDropdownMenu.CopyDropdownMenuArea.DisplayCat)
            {
              CopyDropdownMenu.Area = CopyDropdownMenu.CopyDropdownMenuArea.None;
              CopyDropdownMenu.Despawn();
            }
            
            //__instance.spriteBatch.Draw(__instance.lockSprite, new Rectangle((int)pos.X, (int)pos.Y, dest.Width, (int)tinyFontCharHeight), new Color(255, 0, 0, 100));
            input.Y += tinyFontCharHeight + 2f; 
          }
        }
    }
}