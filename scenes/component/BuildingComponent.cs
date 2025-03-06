using System.Collections.Generic;
using System.Linq;
using Game.Autoload;
using Game.Resources.Building;
using Godot;

namespace Game.Component;

public partial class BuildingComponent : Node2D
{
	[Export(PropertyHint.File, "*.tres")]
	private string buildingResourcePath;

	private HashSet<Vector2I> occupiedTiles = new();

	public BuildingResource BuildingResource { get; private set; }

	public override void _Ready()
	{
		if (buildingResourcePath != null)
		{
			BuildingResource = GD.Load<BuildingResource>(buildingResourcePath);
		}
		AddToGroup(nameof(BuildingComponent));
		Callable.From(Initialize).CallDeferred();
	}

	public Vector2I GetGridCellPosition()
	{
		var gridPosition = GlobalPosition / 64;
		gridPosition = gridPosition.Floor();
		return new Vector2I((int)gridPosition.X, (int)gridPosition.Y);
	}

	public bool isTileInBuildingArea(Vector2I tilePosition)
	{
		return occupiedTiles.Contains(tilePosition);
	}

	public HashSet<Vector2I> getOccupiedCells()
	{
		return occupiedTiles.ToHashSet();
	}

	public void Destroy()
	{
		GameEvents.EmitBuildingDestroyed(this);
		Owner.QueueFree();
	}

	private void setOccupiedTiles()
	{
		var gridPosition = GetGridCellPosition();
		for (int x = gridPosition.X; x < gridPosition.X + BuildingResource.Dimension.X; x++)
		{
			for (int y = gridPosition.Y; y < gridPosition.Y + BuildingResource.Dimension.Y; y++)
			{
				occupiedTiles.Add(new Vector2I(x, y));
			}
		}
	}

	private void Initialize()
	{
		setOccupiedTiles();
		GameEvents.EmitBuildingPlaced(this);
	}
}
