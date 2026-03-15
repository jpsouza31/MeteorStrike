using Godot;
using System;

public partial class Meteor : RigidBody2D
{
    [Signal]
    public delegate void ExplodedEventHandler();

    public override void _Ready()
    {
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Play();
    }

    public void Explode()
    {
        EmitSignal(SignalName.Exploded);
        LinearVelocity = Vector2.Zero;
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Play("explosion");
    }

    private void OnAnimatedSprite2DAnimationFinished()
    {
        if (GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation == "explosion")
        {
            QueueFree();
        }
    }

    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}
