using Godot;
public partial class GameCamera : Camera2D
{
	[Export]
	private int speed = 500;
	private readonly StringName INPUT_DOWN = "pan_down";
	private readonly StringName INPUT_UP = "pan_up";
	private readonly StringName INPUT_LEFT = "pan_left";
	private readonly StringName INPUT_RIGHT = "pan_right";
	public override void _Process(double delta)
	{
		GlobalPosition = GetScreenCenterPosition();
		var position = Input.GetVector(INPUT_LEFT, INPUT_RIGHT, INPUT_UP, INPUT_DOWN);
		GlobalPosition += position * speed * (float)delta;
	}

	public void setBoundingRects(Rect2I rect)
	{
		var left = rect.Position.X * 64;
		var right = rect.End.X * 64;
		var top = rect.Position.Y * 64;
		var bottom = rect.End.Y * 64;
	}

	public void setCenterPosition(Vector2 Position){
		GlobalPosition = Position;
	}
}