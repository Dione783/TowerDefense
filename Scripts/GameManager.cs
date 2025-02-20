using Game.Component;
using Godot;


namespace Game.Manager;
public partial class GameManager : Node
{
	[Signal]
	public delegate void buildingPlacedEventHandler(BuildingComponent buildingComponent);
	[Signal]
	public delegate void DestroingBuildingEventHandler(BuildingComponent buildingComponent);
	public static GameManager instance;
	public override void _Notification(int what)
	{
		if (what == NotificationSceneInstantiated)
		{
			instance = this;
		}
	}

	public static void EmitBuildingPlaced(BuildingComponent buildingComponent)
	{
		instance.EmitSignal(SignalName.buildingPlaced, buildingComponent);
	}

	public static void EmitBuildingDestroyed(BuildingComponent buildingComponent)
	{
		instance.EmitSignal(SignalName.DestroingBuilding, buildingComponent);
	}
}