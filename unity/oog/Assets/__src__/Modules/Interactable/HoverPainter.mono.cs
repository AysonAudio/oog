using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Oog.Modules.Interactable {

/// <summary>
/// Overwrites Renderer materials when any pointer device enters or exits mesh bounds.
/// </summary>

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(HoverSensor))]
public class HoverPainter : MonoBehaviour {
    public List<Material> materialsHighlightOff;
    public List<Material> materialsHighlightOn;
    Action<InputAction.CallbackContext> _pointerEnterCallback;
    Action<InputAction.CallbackContext> _pointerExitCallback;
    HoverSensor _hoverSensor;

    /// <summary>
    /// Serializes materials.
    /// Registers HoverSensor callbacks.
    /// </summary>
    protected void OnEnable() {
        _hoverSensor ??= GetComponent<HoverSensor>();
        _hoverSensor.enabled = true;

        // If highlightOff materials are undefined, default to current Renderer materials.
        if (materialsHighlightOff.Count == 0)
            _hoverSensor.Renderer.GetSharedMaterials(materialsHighlightOff);

        // Prevent duplicate callbacks.
        if (_pointerEnterCallback == null) {
            _pointerEnterCallback = _ => _hoverSensor.Renderer.SetSharedMaterials(materialsHighlightOn);
            _hoverSensor.OnPointerEnter += _pointerEnterCallback;
        }
        if (_pointerExitCallback != null) return;
        _pointerExitCallback = _ => _hoverSensor.Renderer.SetSharedMaterials(materialsHighlightOff);
        _hoverSensor.OnPointerExit += _pointerExitCallback;
    }

    /// <summary>
    /// Frees up memory.
    /// Unregisters HoverSensor callbacks.
    /// </summary>
    protected void OnDisable() {
        _hoverSensor.OnPointerEnter -= _pointerEnterCallback;
        _hoverSensor.OnPointerExit -= _pointerExitCallback;
        _pointerEnterCallback = null;
        _pointerExitCallback = null;
    }
}
}
