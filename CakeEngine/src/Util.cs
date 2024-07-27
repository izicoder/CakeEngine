using Raylib_cs;
using System.Numerics;

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

    public static void DrawRect(Rectangle rect, Color tint) {
        Raylib.DrawRectangleLines(
            (int) rect.X,
            (int) rect.Y,
            (int) rect.Width,
            (int) rect.Height,
            tint
        );
    }

    public static void DrawPoint(Vector2 point, Color tint) {
        Raylib.DrawPixel(
            (int) point.X,
            (int) point.Y,
            tint
        );
    }

    public static Vector2 WorldMousePosition(Camera2D cam) {
        return Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), cam);

    }

    public static int WrapAround(int value, int max) {
        if (value == 0) {
            return 0;
        } else if (value > 0) {
            return value % max;
        } else if (value < 0 && value > (-max)) {
            return max - Math.Abs(value);
        } else {
            return Math.Abs(value) % max;
        }
    }
}
