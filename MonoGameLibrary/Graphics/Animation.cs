namespace MonoGameLibrary.Graphics;

public class Animation
{
    public List<TextureRegion> Frames { get; set; } = [];

    public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(100);

    public static Animation Empty => new();
}