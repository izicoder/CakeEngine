using Raylib_cs;
using System.Numerics;

namespace CakeEngine;

enum OriginType {
    TopLeft,
    TopCenter,
    TopRight,
    Left,
    Center,
    Right,
    BottomLeft,
    BottomCenter,
    BottomRight
}

struct StaticSprite {
    public required Texture2D texture;
    public required Rectangle source;
    public required Rectangle dest;
    public required Vector2 origin;
    public required float rotation;
    public required Color tint;


    public static void Draw(StaticSprite obj) {
        Raylib.DrawTexturePro(obj.texture, obj.source, obj.dest, obj.origin, obj.rotation, obj.tint);
    }

    public static StaticSprite FromImage(Image image, Rectangle? offsetRect = null) {
        var texture = Raylib.LoadTextureFromImage(image);
        Rectangle sourceRect = offsetRect is null ? new Rectangle(0, 0, texture.Width, texture.Height) : offsetRect.Value;
        return new StaticSprite {
            texture = texture,
            source = sourceRect,
            dest = new Rectangle(0, 0, texture.Width, texture.Height),
            origin = Vector2.Zero,
            rotation = 0,
            tint = Color.White
        };
    }

    public static Vector2 SetOrigin(StaticSprite sprite, OriginType originType) {

        Vector2 newOrigin = originType switch {
            OriginType.TopLeft => Vector2.Zero,
            OriginType.TopCenter => new Vector2(sprite.dest.Width / 2, 0),
            OriginType.TopRight => new Vector2(sprite.dest.Width, 0),
            OriginType.Left => new Vector2(0, sprite.dest.Height / 2),
            OriginType.Center => new Vector2(sprite.dest.Width / 2, sprite.dest.Height / 2),
            OriginType.Right => new Vector2(sprite.dest.Width, sprite.dest.Height / 2),
            OriginType.BottomLeft => new Vector2(0, sprite.dest.Height),
            OriginType.BottomCenter => new Vector2(sprite.dest.Width / 2, sprite.dest.Height),
            OriginType.BottomRight => new Vector2(sprite.dest.Width, sprite.dest.Height),
            _ => Vector2.Zero,
        };
        return newOrigin;
    }
}

struct AnimatedSprite {
    public required Rectangle[] Frames;

    /// <summary>
    /// source rectangle will be modified each draw call,
    /// texture is your spritesheet
    /// </summary>
    public required StaticSprite ViewSprite;
    public required int CurrentFrame;

    public static void Draw(AnimatedSprite anim) {
        anim.ViewSprite.source = anim.Frames[anim.CurrentFrame];
        StaticSprite.Draw(anim.ViewSprite);
    }

    public static AnimatedSprite FromSpriteSheet(Image spriteSheet, Rectangle frameRect, int initialFrame) {
        StaticSprite tempview = new StaticSprite {
            texture = Raylib.LoadTextureFromImage(spriteSheet),
            source = frameRect,
            dest = frameRect,
            origin = Vector2.Zero,
            rotation = 0,
            tint = Color.White
        };
        return new AnimatedSprite {
            ViewSprite = tempview,
            CurrentFrame = initialFrame,
            Frames = CalculateFrames(spriteSheet, frameRect)
        };
    }

    public static Rectangle[] CalculateFrames(Image sheet, Rectangle frameRect) {
        int horizontalCount = (int) Math.Floor(sheet.Width / frameRect.Width);
        int verticalCount = (int) Math.Floor(sheet.Height / frameRect.Height);
        Rectangle[] result = new Rectangle[horizontalCount * verticalCount];
        int fc = 0;
        for (int v = 0; v < verticalCount; v++) {
            for (int h = 0; h < horizontalCount; h++) {
                Rectangle temp = new(new Vector2(frameRect.Width * h, frameRect.Height * v), frameRect.Size);
                Console.WriteLine($"v={v} h={h} temp={temp}");
                result[fc] = temp;
                fc += 1;
            }
        }

        return result;
    }
}