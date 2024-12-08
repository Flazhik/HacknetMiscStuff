using System.Windows.Forms;
using Hacknet;
using Microsoft.Xna.Framework;

namespace HacknetCtrlC.GUI;

public static class CopyDropdownMenu
{
    public static CopyDropdownMenuArea Area = CopyDropdownMenuArea.None;
    private static int _x;
    private static int _y;
    private static bool _active;
    private static string _singleLine;
    private static string[] _allLines;
    private static Color _color;

    public static void Spawn(int xCoord, int yCoord, string single, string[] all, Color color)
    {
        if (string.IsNullOrEmpty(single) || all == null)
            return;
        
        _active = true;
        _x = xCoord - 20;
        _y = yCoord - 20;
        _singleLine = single;
        _allLines = all;
        _color = color;
    }

    public static void DrawDropdownMenu()
    {
        if (!_active || _x == 0 || _y == 0)
            return;
        
        if (!(GuiData.tmpRect with
            {
                X = _x,
                Y = _y,
                Width = 150,
                Height = 60
            }).Contains(GuiData.mouse.X, GuiData.mouse.Y) || GuiData.blockingInput)
        {
            _active = false;
            return;
        }

        if (ScrollIndependentButton.doButton(82378502, _x, _y, 150, 30, "Copy Line", _color))
        {
            Clipboard.SetText(_singleLine);
            _active = false;
            return;
        }

        if (ScrollIndependentButton.doButton(82378503, _x, _y + 30, 150, 30, "Copy All", _color))
        {
            Clipboard.SetText(string.Join("\n", _allLines));
            _active = false;
            return;
        }
    }

    public static void Despawn() => _active = false;
    
    public enum CopyDropdownMenuArea
    {
        None,
        MailInbox,
        Terminal,
        DisplayCat,
        Irc
    }
}