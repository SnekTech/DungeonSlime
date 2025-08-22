using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace DungeonSlime.Scenes;

public class TitleScreen : Scene
{
    private const string DungeonText = "Dungeon";
    private const string SlimeText = "Slime";
    private const string PressEnterText = "Press Enter To Start";

    private SpriteFont _font = null!;
    // ReSharper disable once InconsistentNaming
    private SpriteFont _font5x = null!;

    private Vector2 _dungeonTextPos;
    private Vector2 _dungeonTextOrigin;

    private Vector2 _slimeTextPos;
    private Vector2 _slimeTextOrigin;

    private Vector2 _pressEnterPos;
    private Vector2 _pressEnterOrigin;

    private Texture2D _backgroundPattern = null!;
    private Rectangle _backgroundDestination;
    private Vector2 _backgroundOffset;
    private float _scrollSpeed = 50;

    protected override void OnInitialize()
    {
        Core.ExitOnEscape = true;

        var size = _font5x.MeasureString(DungeonText);
        _dungeonTextPos = new Vector2(640, 100);
        _dungeonTextOrigin = size * 0.5f;

        size = _font5x.MeasureString(SlimeText);
        _slimeTextPos = new Vector2(757, 207);
        _slimeTextOrigin = size * 0.5f;

        size = _font.MeasureString(PressEnterText);
        _pressEnterPos = new Vector2(640, 620);
        _pressEnterOrigin = size * 0.5f;

        _backgroundOffset = Vector2.Zero;
        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;
    }

    public override void LoadContent()
    {
        // this one can be reused, so use the global content manager
        // to cache it on load
        _font = GlobalContent.Load<SpriteFont>("fonts/04B_30");
        _font5x = Content.Load<SpriteFont>("fonts/04B_30_5x");

        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");
    }

    protected override void OnUnloadContent()
    {
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }

        var offset = _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _backgroundOffset.X -= offset;
        _backgroundOffset.Y -= offset;

        _backgroundOffset.X %= _backgroundPattern.Width;
        _backgroundOffset.Y %= _backgroundPattern.Height;
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        var spriteBatch = Core.SpriteBatch;

        using (spriteBatch.DrawContext(samplerState: SamplerState.PointWrap))
        {
            spriteBatch.Draw(_backgroundPattern, _backgroundDestination,
                new Rectangle(_backgroundOffset.ToPoint(), _backgroundDestination.Size), Color.White * 0.5f);
        }

        using (spriteBatch.DrawContext(samplerState: SamplerState.PointClamp))
        {
            var dropShadowColor = Color.Black * 0.5f;
            var dropShadowOffset = new Vector2(10, 10);

            spriteBatch.DrawString(
                _font5x,
                DungeonText,
                _dungeonTextPos + dropShadowOffset,
                dropShadowColor,
                0,
                _dungeonTextOrigin,
                1,
                SpriteEffects.None,
                1
            );
            spriteBatch.DrawString(
                _font5x,
                DungeonText,
                _dungeonTextPos,
                Color.White,
                0,
                _dungeonTextOrigin,
                1,
                SpriteEffects.None,
                1
            );

            spriteBatch.DrawString(
                _font5x,
                SlimeText,
                _slimeTextPos + dropShadowOffset,
                dropShadowColor,
                0,
                _slimeTextOrigin,
                1,
                SpriteEffects.None,
                1
            );
            spriteBatch.DrawString(
                _font5x,
                SlimeText,
                _slimeTextPos,
                Color.White,
                0,
                _slimeTextOrigin,
                1,
                SpriteEffects.None,
                1
            );

            spriteBatch.DrawString(_font,
                PressEnterText,
                _pressEnterPos,
                Color.White,
                0,
                _pressEnterOrigin,
                1,
                SpriteEffects.None,
                1
            );
        }
    }
}