using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite
{
    private int _currentFrame;
    private TimeSpan _elapsed;
    private Animation _animation = Animation.Empty;

    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }

    public void Update(GameTime gameTime)
    {
        _elapsed += gameTime.ElapsedGameTime;

        if (_elapsed < Animation.Delay) return;

        _elapsed -= Animation.Delay;
        _currentFrame++;

        if (_currentFrame >= Animation.Frames.Count)
        {
            _currentFrame = 0;
        }

        Region = Animation.Frames[_currentFrame];
    }
}