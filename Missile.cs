using Godot;

public partial class Missile : Area2D
{
	[Export]
	public int Speed { get; set; } = 600;

	public override void _Process(double delta)
	{
		Position += new Vector2(Speed * (float)delta, 0);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Meteor meteor)
		{
			meteor.Explode();
			QueueFree();
		}
	}

	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}
