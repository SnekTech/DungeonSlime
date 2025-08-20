using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime;

public class Game1() : Core("Dungeon Slime", 1280, 720, false)
{
    private AnimatedSprite _slime = null!;
    private AnimatedSprite _bat = null!;

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // var atlasTexture = Content.Load<Texture2D>("images/atlas");
        // var atlas = new TextureAtlas { Texture = atlasTexture };

        // atlas.AddRegion("slime", new Rectangle(0, 0, 20, 20));
        // atlas.AddRegion("bat", new Rectangle(20, 0, 20, 20));

        var atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4f, 4f);
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4f, 4f);

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _slime.Update(gameTime);
        _bat.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        using (SpriteBatch.DrawContext(samplerState: SamplerState.PointClamp))
        {
            _slime.Draw(SpriteBatch, Vector2.One);
            _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));
        }

        base.Draw(gameTime);
    }
}