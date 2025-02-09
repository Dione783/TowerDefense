using Godot;
using System;

namespace Game.UI;
public partial class GameUi : MarginContainer
{
	[Signal]
	public delegate void placeVillageEventHandler();
	[Signal]
	public delegate void placeTowerEventHandler();
	private Button buttonTower;
	private Button buttonVillage;
	public override void _Ready()
	{
		buttonTower = GetNode<Button>("%Place Tower");
		buttonVillage = GetNode<Button>("%Place Village");
		buttonTower.Pressed += OnTowerButtonPressed;
		buttonVillage.Pressed += OnVillageButtonPressed;
	}

	public void OnVillageButtonPressed(){
		EmitSignal(SignalName.placeVillage);
	}

	public void OnTowerButtonPressed(){
		EmitSignal(SignalName.placeTower);
	}
}
