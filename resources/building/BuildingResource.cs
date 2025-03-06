using Godot;

namespace Game.Resources.Building;

[GlobalClass]
public partial class BuildingResource : Resource
{
	[Export]
	public string DisplayName { get; private set; }
	[Export]
	public Vector2I Dimension {get;private set;} = Vector2I.One;
	[Export]
	public bool isDeletable {get;private set;} = true;
	[Export]
	public int ResourceCost { get; private set; }
	[Export]
	public int BuildableRadius { get; private set; }
	[Export]
	public int ResourceRadius { get; private set; }
	[Export]
	public PackedScene BuildingScene { get; private set; }
	[Export]
	public PackedScene SpriteScene { get; private set; }
}
