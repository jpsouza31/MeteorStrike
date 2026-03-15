using Godot;
using System;

public partial class Main : Node
{
	[Export]
    public PackedScene MobScene { get; set; }

    private int _score;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NewGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GameOver()
	{
		GetNode<Timer>("MeteorTimer").Stop();
		GetNode<Label>("HUD/GameOverLabel").Visible = true;
	}

	public void NewGame()
	{
		_score = 0;
		UpdateScore();
		GetNode<Label>("HUD/GameOverLabel").Visible = false;

		var player = GetNode<Ship>("Ship");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();
	}

	private void OnMeteorExploded()
	{
		_score++;
		UpdateScore();
	}

	private void UpdateScore()
	{
		GetNode<Label>("HUD/ScoreLabel").Text = "Score: " + _score;
	}

	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("MeteorTimer").Start();
	}

	private void OnMeteorTimerTimeout()
	{
		Meteor meteor = MobScene.Instantiate<Meteor>();
		var meteorSpawnLocation = GetNode<PathFollow2D>("MeteorPath/MeteorSpawnLocation");
		meteorSpawnLocation.ProgressRatio = GD.Randf();

		float direction = Mathf.Pi;
		direction += (float)GD.RandRange(-Mathf.Pi / 6, Mathf.Pi / 6);

		meteor.Position = meteorSpawnLocation.Position;

		meteor.Rotation = direction;

		Vector2 velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		meteor.LinearVelocity = velocity.Rotated(direction);

		meteor.Exploded += OnMeteorExploded;

		AddChild(meteor);
	}
}
