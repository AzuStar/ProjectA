using Godot;

public partial class DoneViewTexture : TextureRect
{
	[Export] private SubViewport _subViewport;

	public override void _Ready()
	{
		Texture = _subViewport.GetTexture();
		_subViewport.TransparentBg = false;
	}
}
