using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TileMap(TileSet tileSet, int columns, int rows)
{
    private readonly int[] _tiles = new int[rows * columns];

    public int Columns { get; } = columns;
    public int Rows { get; } = rows;

    public int Count { get; } = rows * columns;

    public Vector2 Scale { get; set; } = Vector2.One;

    public float TileWidth => tileSet.TileWidth * Scale.X;
    public float TileHeight => tileSet.TileHeight * Scale.Y;

    public void SetTile(int index, int tileSetId)
    {
        _tiles[index] = tileSetId;
    }

    public void SetTile(int column, int row, int tileSetId)
    {
        _tiles[row * Columns + column] = tileSetId;
    }

    public TextureRegion GetTile(int index) => tileSet.GetTile(_tiles[index]);
    public TextureRegion GetTile(int column, int row) => GetTile(row * Columns + column);

    public void Draw(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < Count; i++)
        {
            var tile = GetTile(i);

            var (x, y) = (i % Columns, i / Columns);
            var position = new Vector2(x * TileWidth, y * TileHeight);
            tile.Draw(spriteBatch, position, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
        }
    }

    public static TileMap FromFile(ContentManager content, string fileName)
    {
        var filePath = Path.Combine(content.RootDirectory, fileName);

        using var stream = TitleContainer.OpenStream(filePath);
        using var reader = XmlReader.Create(stream);
        var doc = XDocument.Load(reader);
        var root = doc.Root!;

        // The <Tileset> element contains the information about the tileset
        // used by the tilemap.
        //
        // Example
        // <Tileset region="0 0 100 100" tileWidth="10" tileHeight="10">contentPath</Tileset>
        //
        // The region attribute represents the x, y, width, and height
        // components of the boundary for the texture region within the
        // texture at the contentPath specified.
        //
        // the tileWidth and tileHeight attributes specify the width and
        // height of each tile in the tileset.
        //
        // the contentPath value is the contentPath to the texture to
        // load that contains the tileset
        var tilesetElement = root.Element("Tileset")!;

        var regionAttribute = tilesetElement.Attribute("region")!.Value;
        var split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var x = int.Parse(split[0]);
        var y = int.Parse(split[1]);
        var width = int.Parse(split[2]);
        var height = int.Parse(split[3]);

        var tileWidth = int.Parse(tilesetElement.Attribute("tileWidth")!.Value);
        var tileHeight = int.Parse(tilesetElement.Attribute("tileHeight")!.Value);
        var contentPath = tilesetElement.Value;

        // Load the texture 2d at the content path
        var texture = content.Load<Texture2D>(contentPath);

        // Create the texture region from the texture
        var textureRegion = new TextureRegion
        {
            Texture = texture,
            SourceRectangle = new Rectangle(x, y, width, height),
        };

        // Create the tileset using the texture region
        var tileset = new TileSet(textureRegion, tileWidth, tileHeight);

        // The <Tiles> element contains lines of strings where each line
        // represents a row in the tilemap.  Each line is a space
        // separated string where each element represents a column in that
        // row.  The value of the column is the id of the tile in the
        // tileset to draw for that location.
        //
        // Example:
        // <Tiles>
        //      00 01 01 02
        //      03 04 04 05
        //      03 04 04 05
        //      06 07 07 08
        // </Tiles>
        var tilesElement = root.Element("Tiles")!;

        // Split the value of the tiles data into rows by splitting on
        // the new line character
        var rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

        // Split the value of the first row to determine the total number of columns
        var columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

        // Create the tilemap
        var tilemap = new TileMap(tileset, columnCount, rows.Length);

        // Process each row
        for (var row = 0; row < rows.Length; row++)
        {
            // Split the row into individual columns
            var columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            // Process each column of the current row
            for (var column = 0; column < columnCount; column++)
            {
                // Get the tileset index for this location
                var tilesetIndex = int.Parse(columns[column]);

                // Get the texture region of that tile from the tileset
                // var region = tileset.GetTile(tilesetIndex);

                // Add that region to the tilemap at the row and column location
                tilemap.SetTile(column, row, tilesetIndex);
            }
        }

        return tilemap;
    }
}