using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace DungeonSlime.Scenes;

public class TitleScene : Scene
{
    private const string DungeonText = "Dungeon";
    private const string SlimeText = "Slime";

    // ReSharper disable once InconsistentNaming
    private SpriteFont _font5x = null!;

    private Vector2 _dungeonTextPos;
    private Vector2 _dungeonTextOrigin;

    private Vector2 _slimeTextPos;
    private Vector2 _slimeTextOrigin;

    private Texture2D _backgroundPattern = null!;
    private Rectangle _backgroundDestination;
    private Vector2 _backgroundOffset;
    private float _scrollSpeed = 50;

    private SoundEffect _uiSoundEffect = null!;
    private Panel _titleScreenButtonsPanel = null!;
    private Panel _optionsPanel = null!;
    private Button _optionsButton = null!;
    private Button _optionsBackButton = null!;

    protected override void OnInitialize()
    {
        Core.ExitOnEscape = true;

        var size = _font5x.MeasureString(DungeonText);
        _dungeonTextPos = new Vector2(640, 100);
        _dungeonTextOrigin = size * 0.5f;

        size = _font5x.MeasureString(SlimeText);
        _slimeTextPos = new Vector2(757, 207);
        _slimeTextOrigin = size * 0.5f;

        _backgroundOffset = Vector2.Zero;
        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;

        InitializeUI();
    }

    public override void LoadContent()
    {
        _font5x = Content.Load<SpriteFont>("fonts/04B_30_5x");

        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");

        _uiSoundEffect = GlobalContent.Load<SoundEffect>("audio/ui");
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

        GumService.Default.Update(gameTime);
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

        if (_titleScreenButtonsPanel.IsVisible)
        {
            DrawTitleTextSprites();
        }

        GumService.Default.Draw();

        return;

        void DrawTitleTextSprites()
        {
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
            }
        }
    }

    private void CreateTitlePanel()
    {
        // Create a container to hold all of our buttons
        _titleScreenButtonsPanel = new Panel();
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _titleScreenButtonsPanel.AddToRoot();

        var startButton = new Button();
        startButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        startButton.Visual.X = 50;
        startButton.Visual.Y = -12;
        startButton.Visual.Width = 70;
        startButton.Text = "Start";
        startButton.Click += HandleStartClicked;
        _titleScreenButtonsPanel.AddChild(startButton);

        _optionsButton = new Button();
        _optionsButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsButton.Visual.X = -50;
        _optionsButton.Visual.Y = -12;
        _optionsButton.Visual.Width = 70;
        _optionsButton.Text = "Options";
        _optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(_optionsButton);

        startButton.IsFocused = true;
    }

    private void HandleStartClicked(object? sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        Core.ChangeScene(new GameScene());
    }

    private void HandleOptionsClicked(object? sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        _titleScreenButtonsPanel.IsVisible = false;
        _optionsPanel.IsVisible = true;
        _optionsBackButton.IsFocused = true;
    }

    private void CreateOptionsPanel()
    {
        _optionsPanel = new Panel();
        _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _optionsPanel.IsVisible = false;
        _optionsPanel.AddToRoot();

        var optionsText = new TextRuntime();
        optionsText.X = 10;
        optionsText.Y = 10;
        optionsText.Text = "OPTIONS";
        _optionsPanel.AddChild(optionsText);

        var musicSlider = new Slider();
        musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
        musicSlider.Visual.Y = 30f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.Value = Core.Audio.SongVolume;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        musicSlider.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        _optionsPanel.AddChild(musicSlider);

        var sfxSlider = new Slider();
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Visual.Y = 93;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = Core.Audio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        _optionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new Button();
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsBackButton.X = -28f;
        _optionsBackButton.Y = -10f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        _optionsPanel.AddChild(_optionsBackButton);
    }

    private void HandleMusicSliderValueChanged(object? sender, EventArgs e)
    {
        var slider = (Slider)sender!;
        Core.Audio.SongVolume = (float)slider.Value;
    }

    private void HandleMusicSliderValueChangeCompleted(object? sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleSfxSliderChanged(object? sender, EventArgs e)
    {
        var slider = (Slider)sender!;
        Core.Audio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderChangeCompleted(object? sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleOptionsButtonBack(object? sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        _titleScreenButtonsPanel.IsVisible = true;
        _optionsPanel.IsVisible = false;
        _optionsButton.IsFocused = false;
    }

    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
        CreateOptionsPanel();
    }
}