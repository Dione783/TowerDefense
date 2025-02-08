using Game.Building.Resources;
using Godot;

public partial class Main : Node
{
	private Sprite2D cursor;
	private Sprite2D mouse;
	private Button buttonTower;
	private Button buttonVillage;
	private BuildingResource towerResource;
	private BuildingResource villageResource;
	private Vector2I gridPosition;
	private GridManager gridManager;
	private Vector2I? hoveredGridCellPosition;
	private Node2D ySortRoot;
	private BuildingResource toPlaceResource;


	private bool is_Dragging = false;
	public override void _Ready()
	{
		mouse = GetNode<Sprite2D>("Mouse");
		buttonTower = GetNode<Button>("ButtonTower");
		buttonVillage = GetNode<Button>("ButtonVillage");
		towerResource = GD.Load<BuildingResource>("res://Resources/tower.tres");
		villageResource = GD.Load<BuildingResource>("res://Resources/village.tres");
		ySortRoot = GetNode<Node2D>("YSortRoot");
		gridManager = GetNode<GridManager>("GridManager");
		buttonTower.Pressed += buttonTowerPressed;
		buttonVillage.Pressed += buttonVillagePressed;
	}

	public override void _Process(double delta)
	{
		if (is_Dragging == true && (!hoveredGridCellPosition.HasValue || hoveredGridCellPosition != gridPosition))
		{
			mouse.Visible = true;
			mouse.GlobalPosition = gridManager.getGridCellPosition();
			gridPosition = gridManager.getGridCellPosition();
			gridManager.HighLightExpandedBuildableTile(gridManager.getMousePosition(),toPlaceResource.buildableRadius);
		}
	}
	public override void _UnhandledInput(InputEvent evt)
	{
		if (evt.IsActionPressed("Mouse_Fire") && is_Dragging == true && gridManager.isBuildableValid(gridManager.getMousePosition()) && gridManager.isBuildableTile(gridManager.getMousePosition()))
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
}