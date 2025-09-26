using Godot;
using System;

public partial class RotationController : Node2D
{
    [Export] private float _rotationSpeed = 20F;

    private Node2D? _parent;

    public override void _Ready()
    {
        _parent = GetParentOrNull<Node2D>()
                  ?? throw new InvalidOperationException("Rotation parent not found");
    }

    public override void _Process(double delta)
    {
        var degreesPerSecond = _rotationSpeed * delta;
        var radiansPerSecond = (float) Mathf.DegToRad(degreesPerSecond);
        _parent?.Rotate(radiansPerSecond);
    }
}
