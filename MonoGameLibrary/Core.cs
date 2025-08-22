﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Audio;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace MonoGameLibrary;

public class Core : Game
{
    private static Core _instance = null!;

    /// <summary>
    /// Gets a reference to the Core instance.
    /// </summary>
    public static Core Instance => _instance;

    private static Scene? _activeScene;
    private static Scene? _nextScene;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; } = null!;

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; } = null!;

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; } = null!;

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; } = null!;

    public static InputManager Input { get; private set; } = null!;

    public static bool ExitOnEscape { get; set; } = true;

    public static AudioController Audio { get; private set; } = null!;

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {
        // Ensure that multiple cores are not created.
        if (_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        // Store reference to engine for global member access.
        _instance = this;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults.
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullScreen;

        // Apply the graphic presentation changes.
        Graphics.ApplyChanges();

        // Set the window title.
        Window.Title = title;

        // Set the core's content manager to a reference of the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Mouse is visible by default.
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's
        // graphics device.
        GraphicsDevice = base.GraphicsDevice;

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        Input = new InputManager();

        Audio = new AudioController();
    }

    protected override void UnloadContent()
    {
        Audio.Dispose();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(gameTime);

        Audio.Update();

        if (ExitOnEscape && Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Exit();
        }

        if (_nextScene is not null)
        {
            TransitionScene();
        }

        _activeScene?.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _activeScene?.Draw(gameTime);

        base.Draw(gameTime);
    }

    public static void ChangeScene(Scene next)
    {
        if (_activeScene != next)
        {
            _nextScene = next;
        }
    }

    public static void TransitionScene()
    {
        _activeScene?.Dispose();

        GC.Collect();

        _activeScene = _nextScene;
        _nextScene = null;

        _activeScene?.Initialize();
    }
}