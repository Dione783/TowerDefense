using Godot;

public partial class GoldMine : Node2D
{
	[Export]
	private Texture2D activeTexture;
	private Sprite2D texture;

    public override void _Ready()
    {
        texture=GetNode<Sprite2D>("Sprite2D");
    }

    public void setActive(){
		texture.Texture = activeTexture;
	}
}
