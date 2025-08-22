using Dumpify;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using Gum.DataTypes;
using Gum.Wireframe;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;

namespace DungeonSlime.Scenes;

public class GameScene : Scene
{
    // Defines the slime animated sprite.
    private AnimatedSprite _slime = null!;

// Defines the bat animated sprite.
    private AnimatedSprite _bat = null!;

// Tracks the position of the slime.
    private Vector2 _slimePosition;

// Speed multiplier when moving.
    private const float MovementSpeed = 5.0f;

// Tracks the position of the bat.
    private Vector2 _batPosition;

// Tracks the velocity of the bat.
    private Vector2 _batVelocity;

// Defines the tilemap to draw.
    private TileMap _tilemap = null!;

// Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;

// The sound effect to play when the bat bounces off the edge of the screen.
    private SoundEffect _bounceSoundEffect = null!;

// The sound effect to play when the slime eats a bat.
    private SoundEffect _collectSoundEffect = null!;

// The SpriteFont Description used to draw text
    private SpriteFont _font = null!;

// Tracks the players score.
    private int _score;

// Defines the position to draw the score text at.
    private Vector2 _scoreTextPosition;

// Defines the origin used when drawing the score text.
    private Vector2 _scoreTextOrigin;

    // A reference to the pause panel UI element so we can set its visibility
// when the game is paused.
    private Panel _pausePanel = null!;

// A reference to the resume button UI element so we can focus it
// when the game is paused.
    private Button _resumeButton = null!;

// The UI sound effect to play when a UI event is triggered.
    private SoundEffect _uiSoundEffect = null!;


    protected override void OnInitialize()
    {
        Core.ExitOnEscape = false;

        var screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight,
            screenBounds.Width - (int)_tilemap.TileWidth * 2,
            screenBounds.Height - (int)_tilemap.TileHeight * 2
        );

        // Initial slime position will be the center tile of the tile map.
        var centerRow = _tilemap.Rows / 2;
        var centerColumn = _tilemap.Columns / 2;
        _slimePosition = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);

        // Initial bat position will the in the top left corner of the room.
        _batPosition = new Vector2(_roomBounds.Left, _roomBounds.Top);

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        var scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

        AssignRandomBatVelocity();
        
        InitializeUI();
    }

    public override void LoadContent()
    {
        var atlas = TextureAtlas.FromFile(GlobalContent, "images/atlas-definition.xml");

        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);

        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);

        _tilemap = TileMap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        _font = GlobalContent.Load<SpriteFont>("fonts/04B_30");
        
        _uiSoundEffect = GlobalContent.Load<SoundEffect>("audio/ui");
    }

    private void CreatePausePanel()
    {
        _pausePanel = new Panel();
        _pausePanel.Anchor(Anchor.Center);
        _pausePanel.Visual.WidthUnits = DimensionUnitType.Absolute;
        _pausePanel.Visual.HeightUnits = DimensionUnitType.Absolute;
        _pausePanel.Visual.Height = 70;
        _pausePanel.Visual.Width = 264;
        _pausePanel.IsVisible = false;
        _pausePanel.AddToRoot();

        var background = new ColoredRectangleRuntime();
        background.Dock(Dock.Fill);
        background.Color = Color.DarkBlue;
        _pausePanel.AddChild(background);

        var textInstance = new TextRuntime();
        textInstance.Text = "PAUSED";
        textInstance.X = 10f;
        textInstance.Y = 10f;
        _pausePanel.AddChild(textInstance);

        _resumeButton = new Button();
        _resumeButton.Text = "RESUME";
        _resumeButton.Anchor(Anchor.BottomLeft);
        _resumeButton.Visual.X = 9f;
        _resumeButton.Visual.Y = -9f;
        _resumeButton.Visual.Width = 80;
        _resumeButton.Click += HandleResumeButtonClicked;
        _pausePanel.AddChild(_resumeButton);

        var quitButton = new Button();
        quitButton.Text = "QUIT";
        quitButton.Anchor(Anchor.BottomRight);
        quitButton.Visual.X = -9f;
        quitButton.Visual.Y = -9f;
        quitButton.Width = 80;
        quitButton.Click += HandleQuitButtonClicked;

        _pausePanel.AddChild(quitButton);
    }

    private void HandleQuitButtonClicked(object? sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        Core.ChangeScene(new TitleScene());
    }

    private void HandleResumeButtonClicked(object? sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        _pausePanel.IsVisible = false;
    }

    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();
        
        CreatePausePanel();
    }


    protected override void OnUnloadContent()
    {
    }

    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
        
        if (_pausePanel.IsVisible) return;
        
        _slime.Update(gameTime);
        _bat.Update(gameTime);

        CheckKeyboardInput();
        CheckGamePadInput();

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

        return;

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

                Core.Audio.PlaySoundEffect(_bounceSoundEffect);
            }

            _batPosition = newBatPosition;
        }

        void HandleSlimeEatTheBat()
        {
            if (!slimeBounds.Intersects(batBounds)) return;

            var randomColumn = Random.Shared.Next(1, _tilemap.Columns - 1);
            var randomRow = Random.Shared.Next(1, _tilemap.Rows - 1);

            _batPosition = new Vector2(randomColumn * _bat.Width, randomRow * _bat.Height);

            AssignRandomBatVelocity();

            Core.Audio.PlaySoundEffect(_collectSoundEffect);
            _score += 100;
        }
    }

    private void AssignRandomBatVelocity()
    {
        // Generate a random angle.
        var angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Convert angle to a direction vector.
        var x = (float)Math.Cos(angle);
        var y = (float)Math.Sin(angle);
        var direction = new Vector2(x, y);

        // Multiply the direction vector by the movement speed
        _batVelocity = direction * MovementSpeed;
    }

    private void CheckKeyboardInput()
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            PauseGame();
        }

        var keyboard = Core.Input.Keyboard;

        CheckMoveInput();
        CheckAudioInput();

        return;

        void CheckMoveInput()
        {
            var speed = MovementSpeed;
            if (keyboard.IsKeyDown(Keys.Space))
            {
                speed *= 1.5f;
            }

            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            {
                _slimePosition.Y -= speed;
            }

            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            {
                _slimePosition.Y += speed;
            }

            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            {
                _slimePosition.X -= speed;
            }

            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            {
                _slimePosition.X += speed;
            }
        }

        void CheckAudioInput()
        {
            if (keyboard.WasKeyJustPressed(Keys.M))
            {
                Core.Audio.ToggleMute();
            }

            if (keyboard.WasKeyJustPressed(Keys.OemPlus))
            {
                Core.Audio.SongVolume += 0.1f;
                Core.Audio.SoundEffectVolume += 0.1f;
            }

            if (keyboard.WasKeyJustPressed(Keys.OemMinus))
            {
                Core.Audio.SongVolume -= 0.1f;
                Core.Audio.SoundEffectVolume -= 0.1f;
            }
        }
    }

    private void CheckGamePadInput()
    {
        var gamePadOne = Core.Input.GetGamePad(PlayerIndex.One);

        if (gamePadOne.WasButtonJustPressed(Buttons.Start))
        {
            PauseGame();
        }

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

    private void PauseGame()
    {
        _pausePanel.IsVisible = true;
        _resumeButton.IsVisible = true;
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        using (Core.SpriteBatch.DrawContext(samplerState: SamplerState.PointClamp))
        {
            _tilemap.Draw(Core.SpriteBatch);
            _slime.Draw(Core.SpriteBatch, _slimePosition);
            _bat.Draw(Core.SpriteBatch, _batPosition);

            Core.SpriteBatch.DrawString(
                _font,
                $"Score: {_score}",
                _scoreTextPosition,
                Color.White,
                0,
                _scoreTextOrigin,
                1,
                SpriteEffects.None,
                0
            );
        }
        
        GumService.Default.Draw();
    }
}