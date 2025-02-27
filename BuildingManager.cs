using System;
using System.Linq;
using Game.Building.Resources;
using Game.Component;
using Game.Manager;
using Game.UI;
using Godot;

public partial class BuildingManager : Node
{
	[Export]
	private int initialResources = 4;
	private Vector2I gridPosition;
	[Export]
	private GridManager gridManager;
	private Vector2I hoveredGridCellPosition;
	[Export]
	private Node2D ySortRoot;
	private BuildingResource toPlaceResource;
	[Export]
	private GameUi gameUI;
	private int usedResources;
	private int currentCollectedResources;
	private readonly StringName FIRE_BUTTON = "Mouse_Fire";
	private readonly StringName CANCEL_BUTTON = "cancel";

	private readonly StringName RIGHT_CLICK_BUTTON = "right_click";

	private enum State
	{
		Normal,
		placingBuilding
	}
	private State currentState;

	private int AvailabeResources => initialResources + currentCollectedResources - usedResources;

	[Export]
	private PackedScene ghostBuildingScene;
	private BuildingGhostScene ghostBuild;

	public override void _Ready()
	{
		gridManager.updateResources += OnUpdateResources;
		gameUI.placeBuilding += OnPlaceBuildingPressed;
	}


	public override void _Process(double delta)
	{
		hoveredGridCellPosition = gridManager.getGridCellPosition();

		if (hoveredGridCellPosition != gridPosition)
		{
			hoveredGridCellPosition = gridPosition;
			UpdateHoveredGridCell();
		}

		switch (currentState)
		{
			case State.Normal:

				break;
			case State.placingBuilding:
				ghostBuild.GlobalPosition = gridManager.getGridCellPosition();
				break;
		}
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		switch (currentState)
		{
			case State.Normal:
				if (evt.IsActionPressed(RIGHT_CLICK_BUTTON))
				{
					DestroyBuildingAtHoveredPosition();
				}
				break;
			case State.placingBuilding:

				if (evt.IsActionPressed(CANCEL_BUTTON))
				{
					ChangeState(State.Normal);
					clearSelectedBuild();
				}
				else if (evt.IsActionPressed(FIRE_BUTTON) &&
				AvailabeResources >= toPlaceResource.useCost &&
				gridManager.isBuildableTile(gridPosition))
				{
					placeBulding();
				}
				break;
		}
	}

	private void DestroyBuildingAtHoveredPosition()
	{
		var building = GetTree().GetNodesInGroup(nameof(BuildingComponent)).Cast<BuildingComponent>()
		.FirstOrDefault((component) => component.GetGridCellPosition() == gridManager.getMousePosition());
		if (building == null) return;
		usedResources -= building.buildingResource.useCost;
		building.Destroy();
		GD.Print(AvailabeResources);
	}

	private void gridDisplay()
	{
		gridManager.clearHighlight();
		ghostBuild.SetValid();
		gridPosition = gridManager.getMousePosition();
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

	private void placeBulding()
	{
		Node2D build = toPlaceResource.scene.Instantiate<Node2D>();
		ySortRoot.AddChild(build);
		build.GlobalPosition = gridManager.getGridCellPosition();
		usedResources += toPlaceResource.useCost;
		ChangeState(State.Normal);
	}

	private void clearSelectedBuild()
	{
		gridManager.clearHighlight();
		if (IsInstanceValid(ghostBuild))
		{
			ghostBuild.QueueFree();
		}
		ghostBuild = null;
	}




	private void UpdateHoveredGridCell()
	{
		switch (currentState)
		{
			case State.Normal:
				clearSelectedBuild();
				break;
			case State.placingBuilding:
				gridDisplay();
				break;
		}
	}
	private void ChangeState(State toState)
	{
		switch (currentState)
		{
			case State.Normal:
				break;
			case State.placingBuilding:
				clearSelectedBuild();
				toPlaceResource = null;
				break;
		}

		currentState = toState;

		switch (currentState)
		{
			case State.Normal:
				break;
			case State.placingBuilding:
				ghostBuild = ghostBuildingScene.Instantiate<BuildingGhostScene>();
				ySortRoot.AddChild(ghostBuild);
				break;
		}
	}


	private void OnPlaceBuildingPressed(BuildingResource resource)
	{
		ChangeState(State.placingBuilding);
		var buildingSprite = resource.sprite.Instantiate<Sprite2D>();
		ghostBuild.AddChild(buildingSprite);
		toPlaceResource = resource;
		gridManager.clearHighlight();
	}

	private void OnUpdateResources(int count)
	{
		currentCollectedResources = count;
	}
}
