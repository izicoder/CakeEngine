using Raylib_cs;
using System.Numerics;

namespace CakeEngine;

class TextDrawer {
    private Font _font;
    private int _fontSize;
    private float _spacing;
    private Color _color;
    public TextDrawer(Font font, int fontSize, float spacing, Color color) {
        _font = font;
        _fontSize = fontSize;
        _spacing = spacing;
        _color = color;
    }

    public void Print(int row, string message) {
        Raylib.DrawTextEx(_font, message, Util.txrow(row, _fontSize), _fontSize, _spacing, _color);
    }

    public void PrintAt(Vector2 pos, string message) {
        Raylib.DrawTextEx(_font, message, pos, _fontSize, _spacing, _color);
    }
}