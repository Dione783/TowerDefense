using Game.Building.Resources;
using Game.UI;
using Godot;

public partial class Main : Node
{
	private Sprite2D cursor;
	private Sprite2D mouse;
	private BuildingResource towerResource;
	private BuildingResource villageResource;
	private Vector2I gridPosition;
	private GridManager gridManager;
	private Vector2I? hoveredGridCellPosition;
	private Node2D ySortRoot;
	private BuildingResource toPlaceResource;
	private GameUi gameUI;
	private bool is_Dragging = false;
	public override void _Ready()
	{
		mouse = GetNode<Sprite2D>("Mouse");
		gameUI = GetNode<GameUi>("GameUi");
		towerResource = GD.Load<BuildingResource>("res://Resources/tower.tres");
		villageResource = GD.Load<BuildingResource>("res://Resources/village.tres");
		ySortRoot = GetNode<Node2D>("YSortRoot");
		gridManager = GetNode<GridManager>("GridManager");
		gameUI.placeTower += buttonTowerPressed;
		gameUI.placeVillage += buttonVillagePressed;
		gridManager.updateResources += OnUpdateResources;
	}

	public override void _Process(double delta)
	{
		if (is_Dragging == true && (!hoveredGridCellPosition.HasValue || hoveredGridCellPosition != gridPosition))
		{
			mouse.Visible = true;
			mouse.GlobalPosition = gridManager.getGridCellPosition();
			gridPosition = gridManager.getGridCellPosition();
			gridManager.clearHighlight();
			gridManager.HighLightExpandedBuildableTile(gridManager.getMousePosition(),toPlaceResource.buildableRadius);
			gridManager.highLightResourceTiles(gridManager.getMousePosition(),toPlaceResource.resourceRadius);
		}
	}
	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("Mouse_Fire") && is_Dragging == true && gridManager.isBuildableTile(gridManager.getMousePosition()) && gridManager.isBuildableTile(gridManager.getMousePosition()))
		{
			placeBulding();
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
		gridManager.clearHighlight();
	}
	private void buttonTowerPressed()
	{
		toPlaceResource=towerResource;
		is_Dragging = true;
	}

	private void buttonVillagePressed(){
		toPlaceResource=villageResource;
		is_Dragging=true;
	}

	private void OnUpdateResources(int count){
		GD.Print(count);
	}
}