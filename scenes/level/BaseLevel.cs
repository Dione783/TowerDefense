<<<<<<< HEAD
using Game.Manager;
using Godot;

namespace Game;

public partial class BaseLevel : Node
{
	private GridManager gridManager;
	private GoldMine goldMine;
	private GameCamera gameCamera;
	private Node2D baseBuilding;
	private TileMapLayer baseTerrainTilemapLayer;

	public override void _Ready()
	{
		gridManager = GetNode<GridManager>("GridManager");
		goldMine = GetNode<GoldMine>("%GoldMine");
		gameCamera = GetNode<GameCamera>("GameCamera");
		baseTerrainTilemapLayer = GetNode<TileMapLayer>("%BaseTerrainTileMapLayer");
		baseBuilding = GetNode<Node2D>("%Base");

		gameCamera.SetBoundingRect(baseTerrainTilemapLayer.GetUsedRect());
		gameCamera.CenterOnPosition(baseBuilding.GlobalPosition);

		gridManager.GridStateUpdated += OnGridStateUpdated;
	}

	private void OnGridStateUpdated()
	{
		var goldMineTilePosition = gridManager.ConvertWorldPositionToTilePosition(goldMine.GlobalPosition);
		if (gridManager.IsTilePositionBuildable(goldMineTilePosition))
		{
			goldMine.SetActive();
			GD.Print("Win!");
		}
	}
}
=======
using Godot;

public partial class BaseLevel : Node
{
    private GridManager gridmanager;
    private GoldMine goldMine;
    public override void _Ready()
    {
        gridmanager = GetNode<GridManager>("GridManager");
        goldMine = GetNode<GoldMine>("%GoldMine");
        gridmanager.updateGridManager += OnUpdateGridManager;
    }

    private void OnUpdateGridManager()
    {
        var goldMinePosition = gridmanager.getWorldGridPosition(goldMine.GlobalPosition);
        if(gridmanager.isBuildableTile(goldMinePosition)){
            goldMine.setActive();
            GD.Print("Congratulations you Win");
        }
    }
}
>>>>>>> 8bad94781ba1cf0beb7289bfe6f0bde5598607a2
