using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sour;

public sealed class Editor
{
    public bool RequestObjectIdRender;
    private GameObject? _currentlySelected;

    public Editor()
    {
        Engine.UpdateEmitter.OnUpdateStage += Update;
        Engine.RenderEmitter.OnRenderStage += Render;
    }

    private void Update( UpdateStage stage, FrameEventArgs args )
    {
        if ( stage is not UpdateStage.PreRender )
            return;

        if ( !Engine.Mouse.State.IsButtonReleased( MouseButton.Left ) )
            return;

        RequestObjectIdRender = true;
    }

    private void Render( RenderStage stage, FrameEventArgs args )
    {
        if ( stage is not RenderStage.PostSceneRender )
            return;

        if ( RequestObjectIdRender )
        {
            RequestObjectIdRender = false;

            var coords = Engine.Mouse.Position;
            // TODO: Why? 
            coords.Y = Engine.ScreenSize.Y - coords.Y;

            var wantsToSelect = GameObject.TryGetAt( coords );

            if ( wantsToSelect is not null )
            {
                if ( _currentlySelected is null )
                {
                    // Nothing was selected but now one thing is.
                    _currentlySelected = wantsToSelect;
                    _currentlySelected.ToggleSelected();
                }
                else if ( wantsToSelect == _currentlySelected )
                {
                    // We selected the same thing twice, so unselect it.
                    _currentlySelected.ToggleSelected();

                    // Remove it from the buffer.
                    _currentlySelected = null;
                }
                else
                {
                    // Switching from one GameObject to another.
                    _currentlySelected.ToggleSelected();
                    _currentlySelected = wantsToSelect;
                    _currentlySelected.ToggleSelected();
                }
            }
            else
            {
                if ( _currentlySelected is not null )
                {
                    // We selected on emtpy space, deselect what we have selected.
                    _currentlySelected.ToggleSelected();

                    // Remove it from the buffer.
                    _currentlySelected = null;
                }
            }
        }
    }
}
