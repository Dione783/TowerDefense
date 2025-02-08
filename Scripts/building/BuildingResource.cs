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
    public PackedScene scene {get;private set;}
}
