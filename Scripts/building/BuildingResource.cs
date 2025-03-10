using Godot;


namespace Game.Building.Resources;
[GlobalClass]
public partial class BuildingResource : Resource
{
    [Export]
    public int buildableRadius {get;private set;}
    [Export]
    public int resourceRadius {get;private set;}
    [Export]
    public int useCost {get;private set;}
    [Export]
    public string name {get;private set;}
    [Export]
    public PackedScene scene {get;private set;}
    [Export]
    public PackedScene sprite {get;private set;}
}
