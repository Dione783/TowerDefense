using Godot;
using System;
using System.Dynamic;

public partial class BuildingGhostScene : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public void SetValid(){
		Modulate = Colors.White;
	}

	public void SetInvalid(){
		Modulate = Colors.Red;
	}
}
