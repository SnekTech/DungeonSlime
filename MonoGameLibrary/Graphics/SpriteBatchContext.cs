using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class SpriteBatchContext : IDisposable
{
    private readonly SpriteBatch _batch;

    public SpriteBatchContext(SpriteBatch batch,
        SpriteSortMode sortMode = SpriteSortMode.Deferred,
        BlendState? blendState = null,
        SamplerState? samplerState = null,
        DepthStencilState? depthStencilState = null,
        RasterizerState? rasterizerState = null,
        Effect? effect = null,
        Matrix? transformMatrix = null
    )
    {
        _batch = batch;
        batch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
    }

    public void Dispose()
    {
        _batch.End();
    }
}

public static class SpriteBatchContextExtension
{
    public static SpriteBatchContext DrawContext(this SpriteBatch batch,
        SpriteSortMode sortMode = SpriteSortMode.Deferred,
        BlendState? blendState = null,
        SamplerState? samplerState = null,
        DepthStencilState? depthStencilState = null,
        RasterizerState? rasterizerState = null,
        Effect? effect = null,
        Matrix? transformMatrix = null
    ) => new(batch, sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
}