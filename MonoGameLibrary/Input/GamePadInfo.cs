﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input;

public class GamePadInfo(PlayerIndex playerIndex)
{
    private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;

    public PlayerIndex PlayerIndex { get; } = playerIndex;

    public GamePadState PreviousState { get; private set; }
    public GamePadState CurrentState { get; private set; } = GamePad.GetState(playerIndex);

    public bool IsConnected => CurrentState.IsConnected;

    public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;
    public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

    public float LeftTrigger => CurrentState.Triggers.Left;
    public float RightTrigger => CurrentState.Triggers.Right;

    public void Update(GameTime gameTime)
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if (_vibrationTimeRemaining > TimeSpan.Zero)
        {
            _vibrationTimeRemaining -= gameTime.ElapsedGameTime;

            if (_vibrationTimeRemaining <= TimeSpan.Zero)
            {
                StopVibration();
            }
        }
    }

    public bool IsButtonDown(Buttons button) => CurrentState.IsButtonDown(button);
    public bool IsButtonUp(Buttons button) => CurrentState.IsButtonUp(button);

    public bool WasButtonJustPressed(Buttons button) =>
        CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);

    public bool WasButtonJustReleased(Buttons button) =>
        CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);

    public void SetVibration(float strength, TimeSpan time)
    {
        _vibrationTimeRemaining = time;
        GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    public void StopVibration() => GamePad.SetVibration(PlayerIndex, 0, 0);
}