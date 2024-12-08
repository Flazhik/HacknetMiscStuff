using Hacknet;
using Hacknet.UIUtils;
using HacknetCtrlC.GUI;
using HacknetCtrlC.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HacknetCtrlC.Patches
{
    [HarmonyPatch(typeof(MailServer))]
    public class MailServerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MailServer), "DrawMailMessageText")]
        public static bool MailServer_DrawMailMessageText_Prefix(
            MailServer __instance,
            Rectangle textBounds,
            SpriteBatch sb,
            string[] text,
            ref int __result)
        {
            if (__instance.sectionedPanel == null || __instance.sectionedPanel.PanelHeight != __instance.GetRenderTextHeight())
                __instance.sectionedPanel = new ScrollableSectionedPanel(__instance.GetRenderTextHeight(), sb.GraphicsDevice);
            __instance.sectionedPanel.NumberOfPanels = text.Length;
            var itemsDrawn = 0;
            var mouseIsInsidePanel = textBounds.Contains(GuiData.mouse.X, GuiData.mouse.Y);
            __instance.sectionedPanel.Draw((index, dest, spBatch) =>
            {
                spBatch.DrawString(GuiData.tinyfont, LocalizedFileLoader.SafeFilterString(text[index]), new Vector2(dest.X, dest.Y), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.8f);
                ++itemsDrawn;

                if (RightClick.DoRightClick(MessageHashUtil.Hash(text[index]), dest.X, dest.Y + 4, dest.Width, dest.Height) && mouseIsInsidePanel)
                    CopyDropdownMenu.Spawn(GuiData.mouse.X, GuiData.mouse.Y, text[index], text, __instance.os.shellButtonColor);

                if (mouseIsInsidePanel)
                {
                    CopyDropdownMenu.Area = CopyDropdownMenu.CopyDropdownMenuArea.MailInbox;
                    CopyDropdownMenu.DrawDropdownMenu();
                } else if (CopyDropdownMenu.Area == CopyDropdownMenu.CopyDropdownMenuArea.MailInbox)
                {
                    CopyDropdownMenu.Area = CopyDropdownMenu.CopyDropdownMenuArea.None;
                    CopyDropdownMenu.Despawn();
                }
            }, sb, textBounds);
            __result = __instance.sectionedPanel.NumberOfPanels * __instance.sectionedPanel.PanelHeight < textBounds.Height ? __instance.sectionedPanel.NumberOfPanels * __instance.sectionedPanel.PanelHeight : textBounds.Height;
            return false;
        }
    }
}