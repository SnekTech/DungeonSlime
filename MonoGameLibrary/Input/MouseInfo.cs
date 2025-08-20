using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class MouseInfo
{
    public MouseState PreviousState { get; private set; }
    public MouseState CurrentState { get; private set; } = Mouse.GetState();

    public Point Position
    {
        get => CurrentState.Position;
        set => SetPosition(value.X, value.Y);
    }

    public int X
    {
        get => CurrentState.X;
        set => SetPosition(value, Y);
    }

    public int Y
    {
        get => CurrentState.Y;
        set => SetPosition(X, value);
    }

    public Point PositionDelta => CurrentState.Position - PreviousState.Position;

    public int XDelta => CurrentState.X - PreviousState.X;
    public int YDelta => CurrentState.Y - PreviousState.Y;

    public bool WasMoved => PositionDelta != Point.Zero;

    public int ScrollWheel => CurrentState.ScrollWheelValue;
    public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Mouse.GetState();
    }

    private delegate bool ButtonStatePredicate(ButtonState current, ButtonState previous);

    public bool IsButtonDown(MouseButton button) =>
        DoesButtonSatisfy(button, (currentButtonState, _) => currentButtonState == ButtonState.Pressed);

    public bool IsButtonUp(MouseButton button) =>
        DoesButtonSatisfy(button, (currentButtonState, _) => currentButtonState == ButtonState.Released);

    public bool WasButtonJustPressed(MouseButton button) =>
        DoesButtonSatisfy(button,
            (current, previous) => current == ButtonState.Pressed && previous == ButtonState.Released);

    public bool WasButtonJustReleased(MouseButton button) =>
        DoesButtonSatisfy(button,
            (current, previous) => current == ButtonState.Released && previous == ButtonState.Pressed);

    private bool DoesButtonSatisfy(MouseButton button, ButtonStatePredicate predicate) =>
        button switch
        {
            MouseButton.Left => predicate(CurrentState.LeftButton, PreviousState.LeftButton),
            MouseButton.Middle => predicate(CurrentState.MiddleButton, PreviousState.MiddleButton),
            MouseButton.Right => predicate(CurrentState.RightButton, PreviousState.RightButton),
            MouseButton.XButton1 => predicate(CurrentState.XButton1, PreviousState.XButton1),
            MouseButton.XButton2 => predicate(CurrentState.XButton2, PreviousState.XButton2),
            _ => false,
        };

    private void SetPosition(int x, int y)
    {
        Mouse.SetPosition(x, y);
        CurrentState = new MouseState(
            x,
            y,
            CurrentState.ScrollWheelValue,
            CurrentState.LeftButton,
            CurrentState.MiddleButton,
            CurrentState.RightButton,
            CurrentState.XButton1,
            CurrentState.XButton2
        );
    }
}