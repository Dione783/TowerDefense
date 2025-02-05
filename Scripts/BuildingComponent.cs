using Game.Manager;
using Godot;

namespace Game.Component;
public partial class BuildingComponent : Node2D
{
	[Export]
	public int buildingRadius {get;private set;}
	public override void _Ready()
    {
        AddToGroup(nameof(BuildingComponent));
		Callable.From(() => GameManager.EmitBuildingPlaced(this)).CallDeferred();
    }

    public Vector2I GetGridCellPosition()
	{
		var gridPosition = GlobalPosition;
		Vector2 res = gridPosition;
		return new Vector2I((int)res.X,(int)res.Y)/64;
	}
}