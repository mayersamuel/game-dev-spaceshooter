using Godot;
using System;

public partial class Player : Sprite2D
{
    private static readonly StringName moveUpAction = "move_up";
    private static readonly StringName moveDownAction = "move_down";
    private static readonly StringName moveLeftAction = "move_left";
    private static readonly StringName moveRightAction = "move_right";
    
    [Export]
    private float _startX = 500;
    
    [Export]
    private float _startY = 500;

    [Export]
    private float _speed = 100;

    private PlayerBoundaries _boundaries;
    
    public override void _Ready()
    {
        Position = new Vector2(_startX, _startY);
        _boundaries = DetermineBoundaries();
    }

    public override void _Process(double delta)
    {
        var movementDirection = Vector2.Zero;

        if (Input.IsActionPressed(moveUpAction) && Position.Y > _boundaries.MinY)
        {
            movementDirection += Vector2.Up;
        }
        
        if (Input.IsActionPressed(moveDownAction) && Position.Y < _boundaries.MaxY)
        {
            movementDirection += Vector2.Down;
        }
        
        if (Input.IsActionPressed(moveLeftAction) && Position.X > _boundaries.MinX)
        {
            movementDirection += Vector2.Left;
        }
        
        if (Input.IsActionPressed(moveRightAction) && Position.X < _boundaries.MaxX)
        {
            movementDirection += Vector2.Right;
        }

        if (movementDirection == Vector2.Zero)
        {
            return;
        }
        
        var movement = movementDirection * (_speed * (float) delta);
        Translate(movement);
    }

    private PlayerBoundaries DetermineBoundaries()
    {
        var windowSize = GetViewportRect().Size;
        var textureSize = Texture.GetSize();
        var scaledSize = textureSize * Scale;
        var halfSize = scaledSize / 2;

        return new(halfSize.X, windowSize.X - halfSize.X, halfSize.Y, windowSize.Y - halfSize.Y);
    }

    private readonly record struct PlayerBoundaries(float MinX, float MaxX, float MinY, float MaxY);
}
