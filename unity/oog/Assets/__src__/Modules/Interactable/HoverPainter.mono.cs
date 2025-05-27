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

                                            ////////////////////////////////////
                                            #region Public Fields and Properties
                                            ////////////////////////////////////

    // Serialized Fields
    public List<Material> materialsHighlightOff;
    public List<Material> materialsHighlightOn;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Fields
                                            ////////////////////////////////////

    // Cached Unity Components
    internal HoverSensor Sensor;
    internal Renderer Renderer => Sensor.Renderer;

    // Event Callbacks
    Action<InputAction.CallbackContext> _pointerEnterCallback;
    Action<InputAction.CallbackContext> _pointerExitCallback;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region MonoBehaviour Event Funcs
                                            ////////////////////////////////////

    /// <summary>
    /// Serializes materials.
    /// Registers HoverSensor callbacks.
    /// </summary>
    void OnEnable() {
        Sensor ??= GetComponent<HoverSensor>();
        Sensor.enabled = true;

        // If highlightOff materials are undefined, default to current Renderer materials.
        if (materialsHighlightOff.Count == 0)
            Renderer.GetSharedMaterials(materialsHighlightOff);

        // Prevent duplicate callbacks.
        if (_pointerEnterCallback == null) {
            _pointerEnterCallback = _ => Renderer.SetSharedMaterials(materialsHighlightOn);
            Sensor.OnPointerEnter += _pointerEnterCallback;
        }
        if (_pointerExitCallback != null) return;
        _pointerExitCallback = _ => Renderer.SetSharedMaterials(materialsHighlightOff);
        Sensor.OnPointerExit += _pointerExitCallback;
    }

    /// <summary>
    /// Frees up memory.
    /// Unregisters HoverSensor callbacks.
    /// </summary>
    void OnDisable() {
        Sensor.OnPointerEnter -= _pointerEnterCallback;
        Sensor.OnPointerExit -= _pointerExitCallback;
        _pointerEnterCallback = null;
        _pointerExitCallback = null;
    }

                                                                      #endregion
}
}
