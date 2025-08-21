using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class TileSet
{
    private readonly TextureRegion[] _tiles;

    public int TileWidth { get; }
    public int TileHeight { get; }

    public int Columns { get; }
    public int Rows { get; }

    public int Count { get; }

    public TileSet(TextureRegion textureRegion, int tileWidth, int tileHeight)
    {
        (TileWidth, TileHeight) = (tileWidth, tileHeight);
        Columns = textureRegion.Width / tileWidth;
        Rows = textureRegion.Height / tileHeight;
        Count = Columns * Rows;

        _tiles = new TextureRegion[Count];
        for (var i = 0; i < Count; i++)
        {
            var x = i % Columns * tileWidth;
            var y = i / Columns * tileHeight;
            _tiles[i] = new TextureRegion
            {
                Texture = textureRegion.Texture,
                SourceRectangle = new Rectangle(
                    textureRegion.SourceRectangle.X + x,
                    textureRegion.SourceRectangle.Y + y,
                    tileWidth, tileHeight),
            };
        }
    }

    public TextureRegion GetTile(int index) => _tiles[index];
    public TextureRegion GetTile(int column, int row) => GetTile(row * Columns + column);
}