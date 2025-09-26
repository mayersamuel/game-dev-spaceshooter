using Godot;
using System;
using Spaceshooter.Scripts;

public partial class Player : Sprite2D
{
    private static readonly StringName moveUpAction = "move_up";
    private static readonly StringName moveDownAction = "move_down";
    private static readonly StringName moveLeftAction = "move_left";
    private static readonly StringName moveRightAction = "move_right";

    [Export] private float _startX;

    [Export] private float _startY;

    [Export] private float _maxY = 580F;

    [Export] private float _minY = 400F;

    [Export] private float _speed;

    private PlayerBoundaries _boundaries;
    private AudioStreamPlayer2D? _laserSound;

    [Export] private PackedScene? _laserPrefab;

    [Export] private Vector2 _laserSpawnOffset = Vector2.Up * 20F;

    [Export] private double _shootCooldownSeconds = 0.5D;
    private double _lastShotTime;

    public override void _Ready()
    {
        _lastShotTime = 0D;
        Position = new Vector2(_startX, _startY);
        _boundaries = DetermineBoundaries();

        _laserSound = GetNode<AudioStreamPlayer2D>("LaserSound");
    }
    

    public override void _Process(double delta)
    {
        Move(delta);
        Shoot();
    }

    private void Shoot()
    {
        const string ShootInput = "shoot";

        if (!Input.IsActionPressed((ShootInput)))
        {
            return;
        }

        if (_laserPrefab is null)
        {
            throw new InvalidOperationException("Laser prefab not set");
        }

        var now = Time.GetTicksMsec() * 0.001D;
        if (now - _lastShotTime < _shootCooldownSeconds)
        {
            return;
        }

        var laser = _laserPrefab.Instantiate<Laser>();
        laser.Position = Position + _laserSpawnOffset;
        GetParent().AddChild(laser);
        _lastShotTime = now;
        
        _laserSound?.Play();
    }

    private void Move(double delta)
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

        if (Input.IsActionPressed(moveLeftAction))
        {
            movementDirection += Vector2.Left;
        }

        if (Input.IsActionPressed(moveRightAction))
        {
            movementDirection += Vector2.Right;
        }

        if (movementDirection == Vector2.Zero)
        {
            return;
        }

        var movement = movementDirection * (_speed * (float)delta);
        Translate(movement);

        if (Position.X < _boundaries.MinX)
        {
            Position = new Vector2(_boundaries.MaxX, Position.Y);
        }
        else if (Position.X > _boundaries.MaxX)
        {
            Position = new Vector2(_boundaries.MinX, Position.Y);
        }
    }

    private PlayerBoundaries DetermineBoundaries()
    {
        var windowSize = GetViewportRect().Size;
        var textureSize = Texture.GetSize();
        var scaledSize = textureSize * Scale;
        var halfSize = scaledSize / 2;

        return new(halfSize.X, windowSize.X - halfSize.X, _minY, _maxY);
    }

    private readonly record struct PlayerBoundaries(float MinX, float MaxX, float MinY, float MaxY);
}