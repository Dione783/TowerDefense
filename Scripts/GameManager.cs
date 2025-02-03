using Game.Component;
using Godot;


namespace Game.Manager;
public partial class GameManager : Node
{
	[Signal]
    public delegate void buildingPlacedEventHandler(BuildingComponent buildingComponent);
	public static GameManager instance;
    public override void _Notification(int what)
    {
        if(what == NotificationSceneInstantiated) {
			instance = this;
		}
    }

	public static void EmitBuildingPlaced(BuildingComponent buildingComponent){
		instance.EmitSignal(SignalName.buildingPlaced,buildingComponent);
	}
}
