using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime;

public class Game1() : Core("Dungeon Slime", 1280, 720, false)
{
    private TextureRegion? _slime;
    private TextureRegion? _bat;

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

        _slime = atlas.GetRegion("slime");
        _bat = atlas.GetRegion("bat");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        using (SpriteBatch.DrawContext(samplerState: SamplerState.PointClamp))
        {
            if (_slime is not null && _bat is not null)
            {
                _slime.Draw(SpriteBatch, Vector2.Zero, Color.White, 0.0f, Vector2.One, 4.0f, SpriteEffects.None, 0f);
                _bat.Draw(SpriteBatch, new Vector2(_slime.Width * 4f + 10, 0), Color.White, 0f, Vector2.One, 4.0f,
                    SpriteEffects.None, 0f);
            }
        }

        base.Draw(gameTime);
    }
}