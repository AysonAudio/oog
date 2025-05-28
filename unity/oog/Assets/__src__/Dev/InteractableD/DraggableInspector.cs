using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Oog.Modules.LazyLoad;
using Oog.Modules.Interactable;
namespace Oog.Dev.InteractableD {

/// <summary>
/// Hides clamp value fields unless clamp boolean is enabled.
/// </summary>

[CustomEditor(typeof(Draggable))]
public class DraggableInspector : Editor {
    public const string UXML = "DraggableInspector.xml.uxml";
    public static SafeCache<VisualTreeAsset> TreeLoader = new(UXML);

    public override VisualElement CreateInspectorGUI() {
        // TODO: cherry pick async loader branch, load and cache DraggableInspector.xml.uxml
        // VisualTreeAsset uxml = (load async)
        // (await loading)
        // return uxml.Instantiate();
        throw new NotImplementedException();
        var tree = TreeLoader.TryGetAsset<VisualTreeAsset>();
    }
}
}
