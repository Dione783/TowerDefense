using System.Collections.Generic;
using System.Linq;
using Game.Component;
using Game.Manager;
using Godot;

public partial class GridManager : Node
{
	[Export]
	private TileMapLayer highLightTileMapLayer;
	[Export]
	private TileMapLayer baseTerraintTileMapLayer;
	private Vector2I? gridCellPosition;
	private HashSet<Vector2I> validBuildableTile = new();
	public override void _Ready()
	{
		GameManager.instance.buildingPlaced += OnBuildingPlacing;
	}

	public void setGridCellPosition()
	{
		gridCellPosition = getGridCellPosition();
	}

	private void clearHighlight()
	{
		highLightTileMapLayer.Clear();
	}

	private void OnBuildingPlacing(BuildingComponent buildingComponent)
	{
		setBuildableTiles(buildingComponent);
		highLightTile();
	}

	public Vector2I getMousePosition()
	{
		var position = highLightTileMapLayer.GetGlobalMousePosition();
		return new Vector2I((int)position.X, (int)position.Y) / 64;
	}
	public Vector2I getGridCellPosition()
	{
		return getMousePosition() * 64;
	}

	public bool isBuildableTile(Vector2I tilePosition)
	{
		return validBuildableTile.Contains(tilePosition);
	}

	public bool isBuildableValid(Vector2I tilePosition)
	{
		var tile = baseTerraintTileMapLayer.GetCellTileData(tilePosition);
		if (tile == null) return false;
		return (bool)tile.GetCustomData("is_buildable");
	}

	public void highLightTile()
	{
		foreach (var tilePosition in validBuildableTile)
		{
			highLightTileMapLayer.SetCell(new Vector2I(tilePosition.X, tilePosition.Y), 0, Vector2I.Zero);
		}
	}

	public void HighLightExpandedBuildableTile(Vector2I coord, int radius)
	{
		clearHighlight();
		highLightTile();
		var validTiles = getValidTiledInRadius(coord, radius);
		var expandedTiles = validTiles.Except(validBuildableTile).Except(GetOcuppiedTiles());
		var atlasCoord = new Vector2I(1, 0);
		foreach (var tilePosition in expandedTiles)
		{
			highLightTileMapLayer.SetCell(new Vector2I(tilePosition.X, tilePosition.Y), 0, atlasCoord);
		}
	}

	private List<Vector2I> getValidTiledInRadius(Vector2I position, int radius)
	{
		var list = new List<Vector2I>();
		for (int i = position.X - radius; i <= position.X + radius; i++)
		{
			for (int j = position.Y - radius; j <= position.Y + radius; j++)
			{
				var tilePosition = new Vector2I(i, j);
				if (!isBuildableValid(tilePosition)) continue;
				list.Add(tilePosition);
			}
		}
		return list;
	}

	private void setBuildableTiles(BuildingComponent buildingComponent)
	{
		var position = buildingComponent.GetGridCellPosition();
		var validTiles = getValidTiledInRadius(position,buildingComponent.buildingRadius);
		validBuildableTile.UnionWith(validTiles);
		validBuildableTile.ExceptWith(GetOcuppiedTiles());
	}

	private IEnumerable<Vector2I> GetOcuppiedTiles(){
		var buildingComponents = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>();
		var exceptions = buildingComponents.Select(x => x.GetGridCellPosition());
		return exceptions;
	}
}