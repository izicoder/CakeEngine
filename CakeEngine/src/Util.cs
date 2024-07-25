using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CakeEngine;

static class Util {

    const string englishchars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string russianchars = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
    const string specialchars = "~`!@#$%^&*()_+-={[}]|\\:;\"'<,>.?/";
    const string digits = "1234567890";

    const string allchars = digits + specialchars + englishchars + russianchars;

    public static int[] GetCodepoints() {
        var codepoints = new List<int>();
        for (int i = 0; i < allchars.Length; i++) {
            codepoints.Add(char.ConvertToUtf32(allchars, i));
        }
        return codepoints.ToArray();
    }
    public static Vector2 txrow(int row, int fontsize, int x = 10, int y = 10) {
        return new Vector2(x, y + fontsize * row);
    }

    public static double ms2s(double ms) {
        return ms / 1000;
    }
}

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

    public void Print(string message, int row) {
        Raylib.DrawTextEx(_font, message, Util.txrow(row, _fontSize), _fontSize, _spacing, _color);
    }
}