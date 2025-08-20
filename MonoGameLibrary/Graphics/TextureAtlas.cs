using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    public Texture2D? Texture { get; set; }

    private readonly Dictionary<string, TextureRegion> _regions = [];
    private readonly Dictionary<string, Animation> _animations = [];

    public void AddRegion(string name, Rectangle rect)
    {
        var region = new TextureRegion { Texture = Texture, SourceRectangle = rect };
        _regions.Add(name, region);
    }

    public TextureRegion? GetRegion(string name) => _regions.GetValueOrDefault(name);

    public bool RemoveRegion(string name) => _regions.Remove(name);

    public void Clear() => _regions.Clear();

    public void AddAnimation(string animationName, Animation animation) => _animations.Add(animationName, animation);

    public Animation? GetAnimation(string animationName) => _animations.GetValueOrDefault(animationName);

    public bool RemoveAnimation(string animationName) => _animations.Remove(animationName);

    public Sprite CreateSprite(string regionName)
    {
        var region = GetRegion(regionName) ?? TextureRegion.Empty;
        return new Sprite { Region = region };
    }

    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        var animation = GetAnimation(animationName) ?? Animation.Empty;
        return new AnimatedSprite { Animation = animation };
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
            var name = region.Attribute("name")!.Value;
            var x = int.Parse(region.Attribute("x")?.Value ?? "0");
            var y = int.Parse(region.Attribute("y")?.Value ?? "0");
            var width = int.Parse(region.Attribute("width")?.Value ?? "0");
            var height = int.Parse(region.Attribute("height")?.Value ?? "0");

            atlas.AddRegion(name, new Rectangle(x, y, width, height));
        }

        // The <Animations> element contains individual <Animation> elements, each one describing
        // a different animation within the atlas.
        //
        // Example:
        // <Animations>
        //      <Animation name="animation" delay="100">
        //          <Frame region="spriteOne" />
        //          <Frame region="spriteTwo" />
        //      </Animation>
        // </Animations>
        //
        // So we retrieve all the <Animation> elements then loop through each one
        // and generate a new Animation instance from it and add it to this atlas.
        var animationElements = root.Element("Animations")?.Elements("Animation");
        if (animationElements == null)
            return atlas;

        foreach (var animationElement in animationElements)
        {
            var name = animationElement.Attribute("name")!.Value;
            var delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
            var delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

            var frameElements = animationElement.Elements("Frame");

            var frames = frameElements
                .Select(frameElement => frameElement.Attribute("region")!.Value)
                .Select(regionName => atlas.GetRegion(regionName)).OfType<TextureRegion>().ToList();

            var animation = new Animation { Frames = frames, Delay = delay };
            atlas.AddAnimation(name, animation);
        }

        return atlas;
    }
}