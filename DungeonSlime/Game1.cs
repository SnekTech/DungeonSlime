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
    }

    protected override void Update(GameTime gameTime)
    {
        _slime.Update(gameTime);
        _bat.Update(gameTime);

        CheckKeyboardInput();
        CheckGamePadInput();

        base.Update(gameTime);
    }

    private void CheckKeyboardInput()
    {
        var speed = MovementSpeed;
        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            _slimePosition.Y -= speed;
        }

        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            _slimePosition.Y += speed;
        }

        if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            _slimePosition.X -= speed;
        }

        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            _slimePosition.X += speed;
        }
    }

    private void CheckGamePadInput()
    {
        var gamePadOne = Input.GetGamePad(PlayerIndex.One);

        var speed = MovementSpeed;
        if (gamePadOne.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            gamePadOne.SetVibration(1.0f, TimeSpan.FromSeconds(1));
        }
        else
        {
            gamePadOne.StopVibration();
        }

        // thumbstick has priority
        if (gamePadOne.LeftThumbStick != Vector2.Zero)
        {
            _slimePosition.X += gamePadOne.LeftThumbStick.X * speed;
            _slimePosition.Y -= gamePadOne.LeftThumbStick.Y * speed; // invert Y
            return;
        }

        if (gamePadOne.IsButtonDown(Buttons.DPadUp))
        {
            _slimePosition.Y -= speed;
        }

        if (gamePadOne.IsButtonDown(Buttons.DPadDown))
        {
            _slimePosition.Y += speed;
        }

        if (gamePadOne.IsButtonDown(Buttons.DPadLeft))
        {
            _slimePosition.X -= speed;
        }

        if (gamePadOne.IsButtonDown(Buttons.DPadRight))
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