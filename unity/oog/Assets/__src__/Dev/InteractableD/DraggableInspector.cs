using System.Linq;
using Debug = System.Diagnostics.Debug;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Oog.Modules.LazyLoad;
using Oog.Modules.Interactable;
namespace Oog.Dev.InteractableD {

/// <summary>
/// Hides clamp value fields unless clamp toggle is enabled.
/// </summary>

[CustomEditor(typeof(Draggable)), CanEditMultipleObjects]
public class DraggableInspector : Editor {
    public const string UXML = "DraggableInspector.xml.uxml";
    public static readonly SafeCache<VisualTreeAsset> TreeLoader = new(UXML);

    public override VisualElement CreateInspectorGUI() {
        TreeLoader.TryGetAsset(out var tree);
        Debug.Assert(tree != null, nameof(tree) + " != null");

        var root = tree.Instantiate();
        var minToggle = root.Q<Toggle>("clampMin");
        var maxToggle = root.Q<Toggle>("clampMax");
        var minField = root.Q<Vector3Field>("clampWorldMin");
        var maxField = root.Q<Vector3Field>("clampWorldMax");

        minToggle.RegisterValueChangedCallback(e => minField.visible = e.newValue);
        maxToggle.RegisterValueChangedCallback(e => maxField.visible = e.newValue);
        return root;
    }
}
}
