namespace Sour;

public sealed class MaterialManager
{
    List<Material> materials;

    public bool AnyShadersNeedHotload = false;

    public MaterialManager()
    {
        materials = new();
    }

    public bool AddMaterial( Material material )
    {
        if ( materials.Contains( material ) )
            return false;

        materials.Add( material );
        // This might be terrible?
        material.OnFileChanged += () =>
        {
            AnyShadersNeedHotload = true;
        };
        return true;
    }

    public bool TryHotloadShaders()
    {
        if ( !AnyShadersNeedHotload )
            return false;

        Log.InfoInternal( "Reloading shaders..." );
        var needed = materials.Where( x => x.NeedsHotload ).ToArray();
        for ( int i = 0; i < needed.Length; i++ )
        {
            needed[i].Reload();
        }

        AnyShadersNeedHotload = false;
        return true;
    }
}
