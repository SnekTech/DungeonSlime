using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace DungeonSlime;

public class Game1() : Core("Dungeon Slime", 1280, 720, false)
{
    private Texture2D _logo = null!;

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _logo = Content.Load<Texture2D>("images/logo");

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

        var (windowWidth, windowHeight) = Window.ClientBounds.Size;

        var iconSourceRect = new Rectangle(0, 0, 128, 128);
        var wordmarkSourceRect = new Rectangle(150, 34, 458, 58);

        using (SpriteBatch.DrawContext(SpriteSortMode.FrontToBack))
        {
            var position = new Vector2(windowWidth, windowHeight) * 0.5f;
            var iconOrigin = iconSourceRect.Size.ToVector2() * 0.5f;
            SpriteBatch.Draw(
                _logo,
                position,
                iconSourceRect,
                Color.White,
                0f,
                iconOrigin,
                1.0f,
                SpriteEffects.None,
                1f
            );

            var wordmarkOrigin = wordmarkSourceRect.Size.ToVector2() * 0.5f;
            SpriteBatch.Draw(
                _logo,
                position,
                wordmarkSourceRect,
                Color.White,
                0f,
                wordmarkOrigin,
                1.0f,
                SpriteEffects.None,
                0f
            );
        }

        base.Draw(gameTime);
    }
}