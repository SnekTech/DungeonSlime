using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    private readonly Dictionary<string, TextureRegion> _regions = [];

    public Texture2D? Texture { get; set; }

    public void AddRegion(string name, Rectangle rect)
    {
        var region = new TextureRegion { Texture = Texture, SourceRectangle = rect };
        _regions.Add(name, region);
    }

    public TextureRegion? GetRegion(string name) => _regions.GetValueOrDefault(name);

    public bool RemoveRegion(string name) => _regions.Remove(name);

    public void Clear() => _regions.Clear();

    public Sprite CreateSprite(string regionName)
    {
        var region = GetRegion(regionName) ?? TextureRegion.Empty;
        return new Sprite { Region = region };
    }

    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        var filePath = Path.Combine(content.RootDirectory, fileName);

        using var stream = TitleContainer.OpenStream(filePath);
        using var reader = XmlReader.Create(stream);

        var doc = XDocument.Load(reader);
        var root = doc.Root!;

        var texturePath = root.Element("Texture")!.Value;
        var atlas = new TextureAtlas { Texture = content.Load<Texture2D>(texturePath) };

        var regions = root.Element("Regions")?.Elements("Region");

        if (regions is null)
            return atlas;

        foreach (var region in regions)
        {
            var name = region.Attribute("name")?.Value;
            var x = int.Parse(region.Attribute("x")?.Value ?? "0");
            var y = int.Parse(region.Attribute("y")?.Value ?? "0");
            var width = int.Parse(region.Attribute("width")?.Value ?? "0");
            var height = int.Parse(region.Attribute("height")?.Value ?? "0");

            if (!string.IsNullOrEmpty(name))
            {
                atlas.AddRegion(name, new Rectangle(x, y, width, height));
            }
        }

        return atlas;
    }
}