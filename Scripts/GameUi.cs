using Game.Building.Resources;
using Godot;

namespace Game.UI;
public partial class GameUi : MarginContainer
{
	[Signal]
	public delegate void placeBuildingEventHandler(BuildingResource resource);
	[Export]
	private BuildingResource[] buildingResources;
	private HBoxContainer hBox;
	public override void _Ready()
	{
		hBox = GetNode<HBoxContainer>("HBoxContainer");
		foreach(BuildingResource resource in buildingResources){
			var button = new Button();
			button.Text = $"Create {resource.name}";
			hBox.AddChild(button);
			button.Pressed += ()=>{
				EmitSignal(SignalName.placeBuilding,resource);
			};
		}
	}
}
