using Game.Building.Resources;
using Game.UI;
using Godot;
using System;

public partial class BuildingManager : Node
{
	private Vector2I gridPosition;
	[Export]
	private GridManager gridManager;
	private Vector2I? hoveredGridCellPosition;
	[Export]
	private Node2D ySortRoot;
	private BuildingResource toPlaceResource;
	[Export]
	private GameUi gameUI;
	private int initialResources = 8;
	private int usedResources;
	private int currentCollectedResources;

	private int AvailabeResources => initialResources + currentCollectedResources - usedResources;

	[Export]
	private PackedScene ghostBuildingScene;
	private BuildingGhostScene ghostBuild;

	public override void _Ready()
	{
		gridManager.updateResources += OnUpdateResources;
		gameUI.placeBuilding += OnPlaceBuildingPressed;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsInstanceValid(ghostBuild)) return;

		hoveredGridCellPosition = gridManager.getGridCellPosition();
		ghostBuild.GlobalPosition = gridManager.getGridCellPosition();
		if (toPlaceResource != null && !hoveredGridCellPosition.HasValue || hoveredGridCellPosition != gridPosition)
		{
			gridDisplay();
		}
	}

	private void gridDisplay()
	{
		ghostBuild.SetValid();
		gridPosition = gridManager.getGridCellPosition();
		gridManager.clearHighlight();
		gridManager.HighLightExpandedBuildableTile(gridManager.getMousePosition(), toPlaceResource.buildableRadius);
		gridManager.highLightResourceTiles(gridManager.getMousePosition(), toPlaceResource.resourceRadius);
		if (gridManager.isBuildableTile(gridManager.getMousePosition()))
		{
			ghostBuild.SetValid();
		}
		else
		{
			ghostBuild.SetInvalid();
		}
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("Mouse_Fire") &&
		hoveredGridCellPosition.HasValue &&
		toPlaceResource != null &&
		AvailabeResources >= toPlaceResource.useCost &&
		gridManager.isBuildableTile(gridManager.getMousePosition()))
		{
			placeBulding();
		}
	}

	private void placeBulding()
	{
		if (!hoveredGridCellPosition.HasValue) return;
		Node2D build = toPlaceResource.scene.Instantiate<Node2D>();
		ySortRoot.AddChild(build);
		build.GlobalPosition = gridManager.getGridCellPosition();
		usedResources += toPlaceResource.useCost;
		hoveredGridCellPosition = null;
		gridManager.clearHighlight();
		ghostBuild.QueueFree();
		ghostBuild = null;
	}

	private void OnPlaceBuildingPressed(BuildingResource resource)
	{
		if (IsInstanceValid(ghostBuild))
		{
			ghostBuild.QueueFree();
		}
		ghostBuild = ghostBuildingScene.Instantiate<BuildingGhostScene>();
		ySortRoot.AddChild(ghostBuild);
		var buildingSprite = resource.sprite.Instantiate<Sprite2D>();
		ghostBuild.AddChild(buildingSprite);
		toPlaceResource = resource;
	}

	private void OnUpdateResources(int count)
	{
		currentCollectedResources = count;
	}
}
