using Game.Manager;
using Godot;

namespace Game.Component;
public partial class BuildingComponent : Node2D
{
	public override void _Ready()
    {
        AddToGroup(nameof(BuildingComponent));
		GameManager.EmitBuildingPlaced(this);
    }

    public Vector2I GetGridCellPosition()
	{
		var gridPosition = GlobalPosition;
		GD.Print(GlobalPosition);
		Vector2 res = gridPosition;
		return new Vector2I((int)res.X,(int)res.Y);
	}
}