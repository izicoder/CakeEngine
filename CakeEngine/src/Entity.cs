using Raylib_cs;
using System.Numerics;

namespace CakeEngine;

class Entity {
    public AnimatedSprite Sprite;
    public Vector2 Position;
    public Vector2 CollisionPoint {
        get {
            Vector2 result = Vector2.Zero;
            result.X = Sprite.ViewSprite.dest.X - Sprite.ViewSprite.origin.X;
            result.Y = Sprite.ViewSprite.dest.Y - Sprite.ViewSprite.origin.Y;
            return result;
        }
    }

    public Rectangle CollisionRect => new Rectangle(CollisionPoint, Sprite.ViewSprite.dest.Size);

    public Entity(Vector2 position, AnimatedSprite sprite) {
        Position = position;
        Sprite = sprite;
    }

    public static void Draw(Entity entity) {
        entity.Sprite.ViewSprite.dest.X = entity.Position.X;
        entity.Sprite.ViewSprite.dest.Y = entity.Position.Y;
        AnimatedSprite.Draw(entity.Sprite);
    }
}