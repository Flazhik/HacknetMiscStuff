using System;
using Hacknet;
using HacknetCtrlC.GUI;
using HacknetCtrlC.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Util = Hacknet.Utils;

namespace HacknetCtrlC.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "Draw")]
        public static bool MailServer_Draw_Prefix(
            Terminal __instance,
            float t)
        {
            __instance.drawFrame();
            var tinyFontCharHeight = GuiData.ActiveFontConfig.tinyFontCharHeight;
            __instance.spriteBatch.Draw(Util.white, __instance.bounds, __instance.os.displayModuleExtraLayerBackingColor);
            var num = Math.Min((int) ((__instance.bounds.Height - 12) / (tinyFontCharHeight + 1.0)) - 3, __instance.history.Count);
            var input = new Vector2(__instance.bounds.X + 4, __instance.bounds.Y + __instance.bounds.Height - tinyFontCharHeight * 5f);
            if (num > 0)
            {
                var mouseIsInsidePanel = __instance.bounds.Contains(GuiData.mouse.X, GuiData.mouse.Y);
                for (var count = __instance.history.Count; count > __instance.history.Count - num; --count)
                {
                    try
                    {
                        var pos = Util.ClipVec2ForTextRendering(input);
                        __instance.spriteBatch.DrawString(GuiData.tinyfont, __instance.history[count - 1], pos, __instance.os.terminalTextColor);
                        
                        if (RightClick.DoRightClick(MessageHashUtil.Hash(__instance.history[count - 1]), (int)pos.X, (int)pos.Y, __instance.bounds.Width,
                                (int)tinyFontCharHeight) && mouseIsInsidePanel)
                            CopyDropdownMenu.Spawn(GuiData.mouse.X, GuiData.mouse.Y, __instance.history[count - 1],
                                __instance.history.ToArray(), __instance.os.shellButtonColor);

                        if (mouseIsInsidePanel)
                        {
                            CopyDropdownMenu.Area = CopyDropdownMenu.CopyDropdownMenuArea.Terminal;
                            CopyDropdownMenu.DrawDropdownMenu();
                        } else if (CopyDropdownMenu.Area == CopyDropdownMenu.CopyDropdownMenuArea.Terminal)
                        {
                            CopyDropdownMenu.Area = CopyDropdownMenu.CopyDropdownMenuArea.None;
                            CopyDropdownMenu.Despawn();
                        }

                        input.Y -= tinyFontCharHeight + 1f;
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            __instance.doGui();

            return false;
        }
    }
}