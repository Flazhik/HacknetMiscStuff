using Hacknet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HNButton = Hacknet.Gui.Button;
using Util = Hacknet.Utils;
using BtnState = Microsoft.Xna.Framework.Input.ButtonState;

namespace HacknetCtrlC.GUI;

public static class ScrollIndependentButton
{
    public static bool doButton(
        int myID,
        int x,
        int y,
        int width,
        int height,
        string text,
        Color? selectedColor)
    {
        return doButton(myID, x, y, width, height, text, selectedColor, Util.white);
    }
    
    private static bool doButton(
        int myID,
        int x,
        int y,
        int width,
        int height,
        string text,
        Color? selectedColor,
        Texture2D tex)
    {
        bool flag = false;
        if (GuiData.hot == myID && !GuiData.blockingInput && GuiData.active == myID && (GuiData.mouseLeftUp() || GuiData.mouse.LeftButton == BtnState.Released))
        {
            flag = true;
            GuiData.active = -1;
        }
        if ((GuiData.tmpRect with
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            }).Contains(GuiData.mouse.X, GuiData.mouse.Y) && !GuiData.blockingInput)
        {
            GuiData.hot = myID;
            if (GuiData.isMouseLeftDown() && (!HNButton.DisableIfAnotherIsActive || GuiData.active == -1))
                GuiData.active = myID;
        }
        else
        {
            if (GuiData.hot == myID)
                GuiData.hot = -1;
            if (GuiData.isMouseLeftDown() && GuiData.active == myID && GuiData.active == myID)
                GuiData.active = -1;
        }
        HNButton.drawModernButton(myID, x, y, width, height, text, selectedColor, tex);
        return flag;
    }
}