using Game.Building.Resources;
using Game.UI;
using Godot;
using System;

public partial class BuildingManager : Node
{
	[Export]
	private Sprite2D mouse;
	private Vector2I gridPosition;
	[Export]
	private GridManager gridManager;
	private Vector2I? hoveredGridCellPosition;
	[Export]
	private Node2D ySortRoot;
	private BuildingResource toPlaceResource;
	[Export]
	private GameUi gameUI;
	private bool is_Dragging = false;
	private int initialResources = 4;
	private int usedResources;
	private int currentCollectedResources;
	private int AvailabeResources => initialResources + currentCollectedResources - usedResources;

	public override void _Ready()
	{
		gridManager.updateResources += OnUpdateResources;
		gameUI.placeBuilding += OnPlaceBuildingPressed;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (is_Dragging == true && (!hoveredGridCellPosition.HasValue || hoveredGridCellPosition != gridPosition))
		{
			mouse.Visible = true;
			mouse.GlobalPosition = gridManager.getGridCellPosition();
			gridPosition = gridManager.getGridCellPosition();
			gridManager.clearHighlight();
			gridManager.HighLightExpandedBuildableTile(gridManager.getMousePosition(), toPlaceResource.buildableRadius);
			gridManager.highLightResourceTiles(gridManager.getMousePosition(), toPlaceResource.resourceRadius);
		}
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("Mouse_Fire") &&
		AvailabeResources >= toPlaceResource.useCost &&
		toPlaceResource != null &&
		is_Dragging == true &&
		gridManager.isBuildableTile(gridManager.getMousePosition()) &&
		gridManager.isBuildableTile(gridManager.getMousePosition()))
		{
			placeBulding();
			GD.Print(AvailabeResources);
		}
	}

	private void placeBulding()
	{
		Node2D build = toPlaceResource.scene.Instantiate<Node2D>();
		ySortRoot.AddChild(build);
		build.GlobalPosition = gridManager.getGridCellPosition();
		is_Dragging = false;
		mouse.Visible = false;
		hoveredGridCellPosition = null;
		usedResources += toPlaceResource.useCost;
		gridManager.clearHighlight();
	}

	private void OnPlaceBuildingPressed(BuildingResource resource)
	{
		toPlaceResource = resource;
		is_Dragging = true;
	}

	private void OnUpdateResources(int count)
	{
		currentCollectedResources = count;
	}
}
