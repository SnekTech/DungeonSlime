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

    private Vector2 _slimePosition;
    private const float MovementSpeed = 5;

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
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

        CheckKeyboardInput();
        CheckGamePadInput();

        base.Update(gameTime);
    }

    private void CheckKeyboardInput()
    {
        var keyboardState = Keyboard.GetState();

        var speed = MovementSpeed;
        if (keyboardState.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
        {
            _slimePosition.Y -= speed;
        }

        if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
        {
            _slimePosition.Y += speed;
        }

        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
        {
            _slimePosition.X -= speed;
        }

        if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
        {
            _slimePosition.X += speed;
        }
    }

    private void CheckGamePadInput()
    {
        var gamePadState = GamePad.GetState(PlayerIndex.One);

        var speed = MovementSpeed;
        if (gamePadState.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            GamePad.SetVibration(PlayerIndex.One, 1f, 1f);
        }
        else
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
        }

        // thumbstick has priority
        if (gamePadState.ThumbSticks.Left != Vector2.Zero)
        {
            _slimePosition.X += gamePadState.ThumbSticks.Left.X * speed;
            _slimePosition.Y -= gamePadState.ThumbSticks.Left.Y * speed; // invert Y
            return;
        }

        if (gamePadState.IsButtonDown(Buttons.DPadUp))
        {
            _slimePosition.Y -= speed;
        }

        if (gamePadState.IsButtonDown(Buttons.DPadDown))
        {
            _slimePosition.Y += speed;
        }

        if (gamePadState.IsButtonDown(Buttons.DPadLeft))
        {
            _slimePosition.X -= speed;
        }

        if (gamePadState.IsButtonDown(Buttons.DPadRight))
        {
            _slimePosition.X += speed;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        using (SpriteBatch.DrawContext(samplerState: SamplerState.PointClamp))
        {
            _slime.Draw(SpriteBatch, _slimePosition);
            _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));
        }

        base.Draw(gameTime);
    }
}