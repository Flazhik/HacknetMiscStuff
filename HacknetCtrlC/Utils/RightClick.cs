using Hacknet;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HacknetCtrlC.Utils
{
    public static class RightClick
    {
        private static ulong hot;
        private static ulong active;
        
        public static bool DoRightClick(ulong myID, int x, int y, int width, int height, bool hax = false)
        {
            bool flag = false;
            if (hot == myID && !GuiData.blockingInput && active == myID && (GuiData.lastMouse.RightButton == ButtonState.Pressed && GuiData.mouse.RightButton == ButtonState.Released))
            {
                flag = true;
                GuiData.active = -1;
            }

            var point = hax ? new Point(GuiData.mouse.X, GuiData.mouse.Y) : GuiData.getMousePoint();
            if ((GuiData.tmpRect with
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height
                }).Contains(point) && !GuiData.blockingInput)
            {
                hot = myID;
                if (GuiData.mouse.RightButton == ButtonState.Pressed && (!Button.DisableIfAnotherIsActive || active == 0))
                    active = myID;
            }
            else
            {
                if (hot == myID)
                    hot = 0;
                if (GuiData.mouse.RightButton == ButtonState.Pressed && active == myID && active == myID)
                    active = 0;
            }
            
            return flag;
        }
    }
}