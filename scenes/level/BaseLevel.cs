using System;
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