using System;
using System.Collections.Generic;
using System.Linq;
using Game.Component;
using Game.Manager;
using Godot;

public partial class GridManager : Node
{
	private const string IS_BUILDABLE = "is_buildable";
	private const string IS_WOOD  = "is_wood";
	[Export]
	private TileMapLayer highLightTileMapLayer;
	[Export]
	private TileMapLayer baseTerraintTileMapLayer;
	private Vector2I? gridCellPosition;
	private HashSet<Vector2I> validBuildableTile = new();

	private List<TileMapLayer> allTileMapLayers = new();
	public override void _Ready()
	{
		GameManager.instance.buildingPlaced += OnBuildingPlacing;
		allTileMapLayers = getAllTileMapLayers(baseTerraintTileMapLayer);
	}

	public void setGridCellPosition()
	{
		gridCellPosition = getGridCellPosition();
	}

	public void clearHighlight()
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

	public bool isCustomData(Vector2I tilePosition, string dataName)
	{
		foreach (var layer in allTileMapLayers)
		{
			var data = layer.GetCellTileData(tilePosition);
			if (data == null) continue;
			return (bool)data.GetCustomData(dataName);
		}
		return false;
	}

	public void highLightTile()
	{
		foreach (var tilePosition in validBuildableTile)
		{
			highLightTileMapLayer.SetCell(new Vector2I(tilePosition.X, tilePosition.Y), 0, Vector2I.Zero);
		}
	}

	public void highLightResourceTiles(Vector2I coord, int radius)
	{
		var validTiles = getResourceInRadius(coord, radius);
		var atlasCoord = new Vector2I(1, 0);
		foreach (var tilePosition in validTiles)
		{
			highLightTileMapLayer.SetCell(new Vector2I(tilePosition.X, tilePosition.Y), 0, atlasCoord);
		}
	}

	public void HighLightExpandedBuildableTile(Vector2I coord, int radius)
	{
		highLightTile();
		var validTiles = getValidTilesInRadius(coord, radius).ToHashSet();
		var expandedTiles = validTiles.Except(validBuildableTile).Except(GetOcuppiedTiles());
		var atlasCoord = new Vector2I(1, 0);
		foreach (var tilePosition in expandedTiles)
		{
			highLightTileMapLayer.SetCell(new Vector2I(tilePosition.X, tilePosition.Y), 0, atlasCoord);
		}
	}

	private List<Vector2I> getResourceInRadius(Vector2I position, int radius)
	{
		return getTilesInRadius(position, radius, (tilePosition) =>
		{
			return isCustomData(tilePosition, IS_WOOD);
		});
	}


	private List<Vector2I> getTilesInRadius(Vector2I position, int radius, Func<Vector2I, bool> filterFn)
	{
		var list = new List<Vector2I>();
		for (int i = position.X - radius; i <= position.X + radius; i++)
		{
			for (int j = position.Y - radius; j <= position.Y + radius; j++)
			{
				var tilePosition = new Vector2I(i, j);
				if (!filterFn(tilePosition)) continue;
				list.Add(tilePosition);
			}
		}
		return list;
	}

	private List<Vector2I> getValidTilesInRadius(Vector2I position, int radius)
	{
		return getTilesInRadius(position, radius, (tilePosition) =>
		{
			return isCustomData(tilePosition, IS_BUILDABLE);
		});
	}

	private void setBuildableTiles(BuildingComponent buildingComponent)
	{
		var position = buildingComponent.GetGridCellPosition();
		var validTiles = getValidTilesInRadius(position, buildingComponent.buildingResource.buildableRadius);
		validBuildableTile.UnionWith(validTiles);
		validBuildableTile.ExceptWith(GetOcuppiedTiles());
	}

	private List<TileMapLayer> getAllTileMapLayers(TileMapLayer tileMapLayer)
	{
		var result = new List<TileMapLayer>();
		var children = tileMapLayer.GetChildren();
		children.Reverse();
		foreach (var layer in tileMapLayer.GetChildren())
		{
			if (layer is TileMapLayer child)
			{
				result.AddRange(getAllTileMapLayers(child));
			}
		}
		result.Add(tileMapLayer);
		return result;
	}

	private IEnumerable<Vector2I> GetOcuppiedTiles()
	{
		var buildingComponents = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>();
		var exceptions = buildingComponents.Select(x => x.GetGridCellPosition());
		return exceptions;
	}
}