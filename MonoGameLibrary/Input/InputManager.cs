using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input;

public class InputManager
{
    private const int GamePadCount = 4;
    public KeyboardInfo Keyboard { get; private set; }
    public MouseInfo Mouse { get; private set; }
    public GamePadInfo[] GamePads { get; }

    public InputManager()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();

        GamePads = new GamePadInfo[GamePadCount];
        for (var i = 0; i < GamePadCount; i++)
        {
            GamePads[i] = new GamePadInfo((PlayerIndex)i);
        }
    }

    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();

        for (var i = 0; i < GamePadCount; i++)
        {
            GamePads[i].Update(gameTime);
        }
    }
}