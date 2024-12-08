using Hacknet;
using Hacknet.Gui;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Entrypoint
{
    [HarmonyPatch(typeof(MainMenu))]
    public static class MainMenuPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainMenu), "DrawBackgroundAndTitle")]
        public static void MainMenu_DrawBackgroundAndTitle_Postfix(MainMenu __instance)
        {
            TextItem.doFontLabel(new Vector2(20, __instance.ScreenManager.GraphicsDevice.Viewport.Height - 30), "Hacknet Mod Kit v1.0.0", GuiData.UISmallfont, Color.White, 600f, 22f);
        }
    }
}