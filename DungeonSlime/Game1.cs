using DungeonSlime.Scenes;
using Gum.Forms;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Media;
using MonoGameGum;
using MonoGameLibrary;

namespace DungeonSlime;

public class Game1() : Core("Dungeon Slime", 1280, 720, false)
{
    private Song _themeSong = null!;

    protected override void Initialize()
    {
        base.Initialize();

        Audio.PlaySong(_themeSong);

        InitializeGum();
        
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {
        _themeSong = Content.Load<Song>("audio/theme");
    }

    private void InitializeGum()
    {
        GumService.Default.Initialize(this, DefaultVisualsVersion.V2);
        GumService.Default.ContentLoader!.XnaContentManager = Content;

        FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);
        FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);
        FrameworkElement.TabReverseKeyCombos.Add(new KeyCombo { PushedKey = Microsoft.Xna.Framework.Input.Keys.Up });
        FrameworkElement.TabKeyCombos.Add(new KeyCombo { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });

        GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4f;
        GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4f;
        GumService.Default.Renderer.Camera.Zoom = 4f;
    }
}