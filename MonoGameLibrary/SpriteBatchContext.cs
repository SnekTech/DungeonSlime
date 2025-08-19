using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary;

public class SpriteBatchContext : IDisposable
{
    private readonly SpriteBatch _batch;

    public SpriteBatchContext(SpriteBatch batch, SpriteSortMode sortMode = SpriteSortMode.Deferred)
    {
        _batch = batch;
        batch.Begin(sortMode);
    }

    public void Dispose()
    {
        _batch.End();
    }
}

public static class SpriteBatchContextExtension
{
    public static SpriteBatchContext DrawContext(this SpriteBatch batch,
        SpriteSortMode sortMode = SpriteSortMode.Deferred) => new(batch, sortMode);
}