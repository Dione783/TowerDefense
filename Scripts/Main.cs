using Godot;

public partial class Main : Node
{
	private Sprite2D cursor;
	private Sprite2D mouse;
	private Button buttonTower;
	private PackedScene scene;
	private Vector2I? gridPosition;
	private GridManager gridManager;
	private Vector2I? hoveredGridCellPosition;

	private bool is_Dragging = false;
	public override void _Ready()
	{
		mouse = GetNode<Sprite2D>("Mouse");
		buttonTower = GetNode<Button>("ButtonTower");
		scene = GD.Load<PackedScene>("res://scenes/tower.tscn");
		gridManager = GetNode<GridManager>("GridManager");
		buttonTower.Pressed += buttonTowerPressed;
	}

	public override void _Process(double delta)
	{
		if(is_Dragging == true && (!hoveredGridCellPosition.HasValue || hoveredGridCellPosition != gridPosition)){
			mouse.Visible=true;
			mouse.GlobalPosition = gridManager.getGridCellPosition();
			gridPosition = gridManager.getGridCellPosition();
			gridManager.highLightTile();
		}
	}
    public override void _UnhandledInput(InputEvent evt)
    {
        if(evt.IsActionPressed("Mouse_Fire") && is_Dragging == true && gridManager.isBuildableTile(gridManager.getMousePosition())){
			Node2D tower = scene.Instantiate<Node2D>();
			AddChild(tower);
			tower.GlobalPosition = gridManager.getGridCellPosition();
			is_Dragging=false;
			hoveredGridCellPosition=null;
		}
    }
	private void buttonTowerPressed()
	{
		is_Dragging = true;
	}
}