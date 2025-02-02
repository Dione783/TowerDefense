using Game.Component;
using Game.Manager;
using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node
{
	private Sprite2D cursor;
	private Sprite2D mouse;
	private TileMapLayer highLightTileMapLayer;
	private Button buttonTower;
	private PackedScene scene;
	private Vector2 gridCellPosition;
	private HashSet<Vector2> occupiedCells = new();

	private bool is_Dragging = false;
	public override void _Ready()
	{
		mouse = GetNode<Sprite2D>("Mouse");
		highLightTileMapLayer = GetNode<TileMapLayer>("HighLightTileMapLayer");
		buttonTower = GetNode<Button>("ButtonTower");
		scene = GD.Load<PackedScene>("res://scenes/tower.tscn");
		buttonTower.Pressed += buttonTowerPressed;
		GameManager.instance.buildingPlaced += buildingPlacing;
	}

	public override void _Process(double delta)
	{
		if(is_Dragging == true){
			mouse.Visible=true;
			gridCellPosition=getMousePosition();
			mouse.GlobalPosition = GetGridCellPosition();
			HighLightBuildableTiles();
		}
	}

    public override void _UnhandledInput(InputEvent evt)
    {
        if(evt.IsActionPressed("Mouse_Fire") && is_Dragging == true && !occupiedCells.Contains(gridCellPosition)){
			Node2D tower = scene.Instantiate<Node2D>();
			GD.Print(gridCellPosition);
			AddChild(tower);
			tower.GlobalPosition = GetGridCellPosition();
			is_Dragging=false;
		}
    }

	private Vector2I GetGridCellPosition(){
		var gridPosition = getMousePosition();
		return gridPosition * 64;
	}

	private Vector2I getMousePosition()
	{
		var mousePosition = highLightTileMapLayer.GetGlobalMousePosition();
		return (Vector2I)mousePosition / 64;
	}

	private void HighLightBuildableTiles()
	{
		highLightTileMapLayer.Clear();
		for(int i=(int)gridCellPosition.X - 3;i <= gridCellPosition.X + 3;i++){
			for(int j=(int)gridCellPosition.Y -3;j <= gridCellPosition.Y +3;j++){
				highLightTileMapLayer.SetCell(new Vector2I(i,j),0,Vector2I.Zero);
			}
		}
	}

	private void HighLightNotOcupiedTiles(){
		highLightTileMapLayer.Clear();
	}

	private void buttonTowerPressed()
	{
		is_Dragging = true;
	}

	private void markTileOccupied(Vector2I tilePosition){
		occupiedCells.Add(tilePosition);
	}

	private void buildingPlacing(BuildingComponent buildingComponent){
		markTileOccupied(buildingComponent.GetGridCellPosition());
		mouse.Visible=false;
	}
}