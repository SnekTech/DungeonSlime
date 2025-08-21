using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MonoGameLibrary.Scenes;

public abstract class Scene : IDisposable
{
    protected ContentManager Content { get; }

    public bool IsDisposed { get; private set; }

    protected Scene()
    {
        Content = new ContentManager(Core.Content.ServiceProvider);
        Content.RootDirectory = Core.Content.RootDirectory;
    }

    ~Scene() => Dispose(false);

    public void Initialize()
    {
        LoadContent();
        OnInitialize();
    }

    public abstract void LoadContent();

    public void UnloadContent()
    {
        OnUnloadContent();
        Content.Unload();
    }

    public abstract void Update(GameTime gameTime);

    public abstract void Draw(GameTime gameTime);

    protected abstract void OnInitialize();
    protected abstract void OnUnloadContent();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        if (disposing)
        {
            UnloadContent();
            Content.Dispose();
        }

        IsDisposed = true;
    }
}