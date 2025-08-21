using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime;

public class Game1() : Core("Dungeon Slime", 1280, 720, false)
{
    private AnimatedSprite _slime = null!;
    private AnimatedSprite _bat = null!;

    private Vector2 _slimePosition;
    private const float MovementSpeed = 5;

    private Vector2 _batPosition;
    private Vector2 _batVelocity;

    private TileMap _tileMap = null!;
    private Rectangle _roomBounds;

    private SoundEffect _bounceSoundEffect = null!;
    private SoundEffect _collectSoundEffect = null!;

    protected override void Initialize()
    {
        base.Initialize();

        var screenBounds = GraphicsDevice.PresentationParameters.Bounds;
        _roomBounds = new Rectangle(
            (int)_tileMap.TileWidth,
            (int)_tileMap.TileHeight,
            screenBounds.Width - (int)_tileMap.TileWidth * 2,
            screenBounds.Height - (int)_tileMap.TileHeight * 2
        );

        var (centerRow, centerColumn) = (_tileMap.Rows / 2, _tileMap.Columns / 2);
        _slimePosition = new Vector2(centerColumn * _tileMap.TileWidth, centerRow * _tileMap.TileHeight);

        _batPosition = new Vector2(_roomBounds.Left, _roomBounds.Top);

        AssignRandomBatVelocity();
    }

    protected override void LoadContent()
    {
        var atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4f, 4f);
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4f, 4f);

        _tileMap = TileMap.FromFile(Content, "images/tilemap-definition.xml");
        _tileMap.Scale = new Vector2(4, 4);

        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        LoadAndPlayTheThemeSong();
        return;

        void LoadAndPlayTheThemeSong()
        {
            var theme = Content.Load<Song>("audio/theme");

            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(theme);
            MediaPlayer.IsRepeating = true;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        _slime.Update(gameTime);
        _bat.Update(gameTime);

        CheckKeyboardInput();
        CheckGamePadInput();

        var (screenWidth, screenHeight) = GraphicsDevice.Resolution();

        var slimeBoundsCenter = new Vector2(
            _slimePosition.X + _slime.Width * 0.5f,
            _slimePosition.Y + _slime.Height * 0.5f
        ).ToPoint();
        var slimeRadius = (int)(_slime.Width * 0.5f);
        var slimeBounds = new Circle(slimeBoundsCenter, slimeRadius);

        CheckAndMoveBackSlimeIfOffScreen();

        var newBatPosition = _batPosition + _batVelocity;

        var batBoundsCenter = new Vector2(
            _batPosition.X + _bat.Width * 0.5f,
            _batPosition.Y + _bat.Height * 0.5f
        ).ToPoint();
        var batRadius = _bat.Width * 0.5f;
        var batBounds = new Circle(batBoundsCenter, (int)batRadius);

        var normal = Vector2.Zero;

        CheckAndReflectBatIfOffScreen();

        HandleSlimeEatTheBat();

        base.Update(gameTime);

        return;

        void HandleSlimeEatTheBat()
        {
            if (slimeBounds.Intersects(batBounds))
            {
                var tileWidth = (int)_bat.Width;
                var totalColumns = screenWidth / tileWidth;
                var totalRows = screenHeight / tileWidth;

                var randomColumn = Random.Shared.Next(0, totalColumns);
                var randomRow = Random.Shared.Next(0, totalRows);

                _batPosition = new Vector2(randomColumn * _bat.Width, randomRow * _bat.Height);

                AssignRandomBatVelocity();

                _collectSoundEffect.Play();
            }
        }

        void CheckAndMoveBackSlimeIfOffScreen()
        {
            if (slimeBounds.Left < _roomBounds.Left)
            {
                _slimePosition.X = _roomBounds.Left;
            }
            else if (slimeBounds.Right > _roomBounds.Right)
            {
                _slimePosition.X = _roomBounds.Right - _slime.Width;
            }

            if (slimeBounds.Top < _roomBounds.Top)
            {
                _slimePosition.Y = _roomBounds.Top;
            }
            else if (slimeBounds.Bottom > _roomBounds.Bottom)
            {
                _slimePosition.Y = _roomBounds.Bottom - _slime.Height;
            }
        }

        void CheckAndReflectBatIfOffScreen()
        {
            if (batBounds.Left < _roomBounds.Left)
            {
                normal.X = Vector2.UnitX.X;
                newBatPosition.X = _roomBounds.Left;
            }
            else if (batBounds.Right > _roomBounds.Right)
            {
                normal.X = -Vector2.UnitX.X;
                newBatPosition.X = _roomBounds.Right - _bat.Width;
            }

            if (batBounds.Top < _roomBounds.Top)
            {
                normal.Y = Vector2.UnitY.Y;
                newBatPosition.Y = _roomBounds.Top;
            }
            else if (batBounds.Bottom > _roomBounds.Bottom)
            {
                normal.Y = -Vector2.UnitY.Y;
                newBatPosition.Y = _roomBounds.Bottom - _bat.Height;
            }

            if (normal != Vector2.Zero)
            {
                _batVelocity = Vector2.Reflect(_batVelocity, normal);

                _bounceSoundEffect.Play();
            }

            _batPosition = newBatPosition;
        }
    }

    private void AssignRandomBatVelocity()
    {
        var angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        var direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

        _batVelocity = direction * MovementSpeed;
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
            _tileMap.Draw(SpriteBatch);
            _slime.Draw(SpriteBatch, _slimePosition);
            _bat.Draw(SpriteBatch, _batPosition);
        }

        base.Draw(gameTime);
    }
}