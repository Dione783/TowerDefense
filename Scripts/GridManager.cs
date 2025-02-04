using System.Collections.Generic;
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
        GameManager.instance.buildingPlaced += buildingPlacing;
    }

	public void setGridCellPosition(){
		gridCellPosition = getGridCellPosition();
	}

    private void clearHighlight()
	{
		highLightTileMapLayer.Clear();
	}

	private void buildingPlacing(BuildingComponent buildingComponent)
	{
		HighLightBuildableTiles(buildingComponent);

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

	public bool isBuildableTile(Vector2I tilePosition){
		return validBuildableTile.Contains(tilePosition);
	}

	public bool isBuildableValid(Vector2I tilePosition){
		var tile = baseTerraintTileMapLayer.GetCellTileData(tilePosition);
		if(tile == null){
			return false;
		}
		return (bool)tile.GetCustomData("is_buildable");
	}

	public void highLightTile(){
		foreach(var tilePosition in validBuildableTile){
			highLightTileMapLayer.SetCell(tilePosition,0,Vector2I.Zero);
		}
	}

	private void HighLightBuildableTiles(BuildingComponent buildingComponent)
	{
		var position = buildingComponent.GetGridCellPosition();
		for (int i = position.X - 3; i <= position.X + 3; i++)
		{
			for (int j = position.Y - 3; j <= position.Y + 3; j++)
			{
				if(!isBuildableValid(position)){
					continue;
				}
				validBuildableTile.Add(new Vector2I(i,j));
			}
		}
	}
}
