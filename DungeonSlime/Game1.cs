using DungeonSlime.Scenes;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;

namespace DungeonSlime;

public class Game1() : Core("Dungeon Slime", 1280, 720, false)
{
    private Song _themeSong = null!;

    protected override void Initialize()
    {
        base.Initialize();

        Audio.PlaySong(_themeSong);

        ChangeScene(new TitleScreen());
    }

    protected override void LoadContent()
    {
        _themeSong = Content.Load<Song>("audio/theme");
    }
}