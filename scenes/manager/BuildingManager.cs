using System.Collections.Generic;
using System.Linq;
using Game.Building;
using Game.Component;
using Game.Resources.Building;
using Game.UI;
using Godot;

namespace Game.Manager;

public partial class BuildingManager : Node
{
	private readonly StringName ACTION_LEFT_CLICK = "left_click";
	private readonly StringName ACTION_CANCEL = "cancel";
	private readonly StringName ACTION_RIGHT_CLICK = "right_click";

	[Export]
	private int startingResourceCount = 4;
	[Export]
	private GridManager gridManager;
	[Export]
	private GameUI gameUI;
	[Export]
	private Node2D ySortRoot;
	[Export]
	private PackedScene buildingGhostScene;

	private enum State
	{
		Normal,
		PlacingBuilding
	}

	private int currentResourceCount;
	private int currentlyUsedResourceCount;
	private BuildingResource toPlaceBuildingResource;
	private Rect2I hoveredGridArea;
	private BuildingGhost buildingGhost;
	private State currentState;

	private int AvailableResourceCount => startingResourceCount + currentResourceCount - currentlyUsedResourceCount;

	public override void _Ready()
	{
		gridManager.ResourceTilesUpdated += OnResourceTilesUpdated;
		gameUI.BuildingResourceSelected += OnBuildingResourceSelected;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		switch (currentState)
		{
			case State.Normal:
				if (evt.IsActionPressed(ACTION_RIGHT_CLICK))
				{
					DestroyBuildingAtHoveredCellPosition();
				}
				break;
			case State.PlacingBuilding:
				if (evt.IsActionPressed(ACTION_CANCEL))
				{
					ChangeState(State.Normal);
				}
				else if (
					toPlaceBuildingResource != null &&
					evt.IsActionPressed(ACTION_LEFT_CLICK) &&
					IsBuildingPlaceableAtTile(hoveredGridArea)
					)
				{
					PlaceBuildingAtHoveredCellPosition();
				}
				break;
			default:
				break;
		}
	}

	public override void _Process(double delta)
	{
		var gridPosition = gridManager.GetMouseGridCellPosition();
		var rootCell = hoveredGridArea.Position;
		if (rootCell != gridPosition)
		{
			hoveredGridArea.Position = gridPosition;
			UpdateHoveredGridCell();
		}

		switch (currentState)
		{
			case State.Normal:
				break;
			case State.PlacingBuilding:
				buildingGhost.GlobalPosition = gridPosition * 64;
				break;
		}
	}

	private void UpdateGridDisplay()
	{
		gridManager.ClearHighlightedTiles();
		gridManager.HighlightBuildableTiles();
		if (IsBuildingPlaceableAtTile(hoveredGridArea))
		{
			gridManager.HighlightExpandedBuildableTiles(hoveredGridArea, toPlaceBuildingResource.BuildableRadius);
			gridManager.HighlightResourceTiles(hoveredGridArea, toPlaceBuildingResource.ResourceRadius);
			buildingGhost.SetValid();
		}
		else
		{
			buildingGhost.SetInvalid();
		}
	}

	private void PlaceBuildingAtHoveredCellPosition()
	{
		var positionArea = hoveredGridArea.Position;
		var building = toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
		ySortRoot.AddChild(building);

		building.GlobalPosition = positionArea * 64;

		currentlyUsedResourceCount += toPlaceBuildingResource.ResourceCost;

		ChangeState(State.Normal);
	}

	private void DestroyBuildingAtHoveredCellPosition()
	{
		var positionArea = hoveredGridArea.Position;
		var buildingComponent = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>()
			.FirstOrDefault((buildingComponent) =>
			 {
				return buildingComponent.BuildingResource.isDeletable && buildingComponent.isTileInBuildingArea(positionArea);
			 });
		if (buildingComponent == null) return;
		currentlyUsedResourceCount -= buildingComponent.BuildingResource.ResourceCost;
		buildingComponent.Destroy();
	}

	private void ClearBuildingGhost()
	{
		gridManager.ClearHighlightedTiles();

		if (IsInstanceValid(buildingGhost))
		{
			buildingGhost.QueueFree();
		}
		buildingGhost = null;
	}

	private bool IsBuildingPlaceableAtTile(Rect2I tileArea)
	{
		List<Vector2I> tilesInArea = GetTilesInPositionArea(tileArea);
		var allTilesBuildable = tilesInArea.All((tilePosition) => gridManager.IsTilePositionBuildable(tilePosition));
		return allTilesBuildable && AvailableResourceCount >= toPlaceBuildingResource.ResourceCost;
	}

	private List<Vector2I> GetTilesInPositionArea(Rect2I tileArea)
	{
		var result = new List<Vector2I>();
		for (int x = tileArea.Position.X; x < tileArea.End.X; x++)
		{
			for (int y = tileArea.Position.Y; y < tileArea.End.Y; y++)
			{
				result.Add(new Vector2I(x, y));
			}
		}
		return result;
	}

	private void UpdateHoveredGridCell()
	{
		switch (currentState)
		{
			case State.Normal:
				break;
			case State.PlacingBuilding:
				UpdateGridDisplay();
				break;
		}
	}

	private void ChangeState(State toState)
	{
		switch (currentState)
		{
			case State.Normal:
				break;
			case State.PlacingBuilding:
				ClearBuildingGhost();
				toPlaceBuildingResource = null;
				break;
		}

		currentState = toState;

		switch (currentState)
		{
			case State.Normal:
				break;
			case State.PlacingBuilding:
				buildingGhost = buildingGhostScene.Instantiate<BuildingGhost>();
				ySortRoot.AddChild(buildingGhost);
				break;
		}
	}

	private void OnResourceTilesUpdated(int resourceCount)
	{
		currentResourceCount = resourceCount;
	}

	private void OnBuildingResourceSelected(BuildingResource buildingResource)
	{
		ChangeState(State.PlacingBuilding);
		hoveredGridArea.Size = buildingResource.Dimension;
		var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
		buildingGhost.AddChild(buildingSprite);
		toPlaceBuildingResource = buildingResource;
		UpdateGridDisplay();
	}
}
