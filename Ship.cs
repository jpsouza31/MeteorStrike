using Godot;
using System;

public partial class Ship : Area2D
{

	[Export]
    public int Speed { get; set; } = 400;

	[Export]
	public PackedScene MissileScene { get; set; }

	[Export]
	public float FireCooldown { get; set; } = 0.3f;

    public Vector2 ScreenSize;

	[Signal]
	public delegate void HitEventHandler();

	private float _fireCooldownTimer = 0f;	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_right")) {
			velocity.X += 1;
		}
		if (Input.IsActionPressed("move_left")) {
			velocity.X -= 1;
		}
		if (Input.IsActionPressed("move_down")) {
			velocity.Y += 1;
		}
		if (Input.IsActionPressed("move_up")) {
			velocity.Y -= 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "moving";

		if (velocity.Length() > 0) {
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		} else {
			animatedSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		// Shooting
		if (_fireCooldownTimer > 0)
			_fireCooldownTimer -= (float)delta;

		if (Input.IsActionPressed("shoot") && _fireCooldownTimer <= 0 && MissileScene != null)
		{
			_fireCooldownTimer = FireCooldown;
			var missile = MissileScene.Instantiate<Missile>();
			missile.Position = Position + new Vector2(30, 0);
			GetTree().CurrentScene.AddChild(missile);
		}
	}

	// We also specified this function name in PascalCase in the editor's connection window.
	private void OnBodyEntered(Node2D body)
	{
		Hide(); // Player disappears after being hit.
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}
