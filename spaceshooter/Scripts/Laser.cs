using System;
using Godot;

namespace Spaceshooter.Scripts;

public sealed partial class Laser: Sprite2D
{
    private static float? _destructionY;

    [Export] private float _speed = 200F;

    public override void _Ready()
    {
        _destructionY ??= CalcDestructionY();
        
        return;
    }

    float CalcDestructionY()
    {
        var textureHeight = Texture.GetSize().Y * Scale.Y;

        return -1 * (textureHeight * 0.5F + 1F);
    }

    public override void _Process(double delta)
    {
        Translate(Vector2.Up * (float) (delta * _speed));

        if (Position.Y < (_destructionY ?? throw new InvalidOperationException("Destruction Y not set")))
        {
            QueueFree();
        }
    }
}