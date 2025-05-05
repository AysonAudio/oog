using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Oog.Modules.Math;
using Oog.Settings;
namespace Oog.Modules.Interactable {

/// <summary>
///     Invokes C# events when the main pointer enters and exits a 3D object.
/// </summary> <remarks>
///     Gets pointer position in world space (the point on the camera's far clipping plane / clip space).
///     Casts a ray from pointer to main camera. Activates events if ray hits this object.
///     If another object is blocking this object, the hover event will not activate.
/// </remarks>

[RequireComponent(typeof(Renderer))]
public class EventHover : MonoBehaviour, InputSet.IHoverActions {

                                            ////////////////////////////////////
                                            #region Public Fields and Properties
                                            ////////////////////////////////////

    // Serialized Fields
    public LayerMask raycastLayerMask = INTERACTABLE.LAYER.DEFAULT.ToLayerMask();

    // Encapsulated Properties
    public bool IsHovered { get; private set; }

    // C# Events
    public event Action<InputAction.CallbackContext> OnPointerEnter;
    public event Action<InputAction.CallbackContext> OnPointerExit;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Fields
                                            ////////////////////////////////////

    // Cached Unity Components
    internal Camera MainCam { get; private set; }
    internal Renderer Renderer { get; private set; }
    internal InputSet Inputs { get; private set; }

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region MonoBehaviour Event Funcs
                                            ////////////////////////////////////

    /// <summary>
    ///     Caches Unity components.
    ///     Registers callbacks for Unity Input System package.
    ///     Enables input listeners.
    /// </summary> <remarks>
    ///     No-ops if main camera not found or disabled.
    ///     No-ops if Renderer not found or disabled.
    ///     If no-op, this EventHover remains disabled.
    /// </remarks>
    void OnEnable() {
        if ((MainCam ??= Camera.main) == null
        || !MainCam.enabled
        || (Renderer ??= GetComponent<Renderer>()) == null
        || !Renderer.enabled) {
            enabled = false;
            return;
        } if (Inputs == null) {
            Inputs = new InputSet();
            Inputs.Hover.SetCallbacks(this);
        } Inputs.Hover.Enable();
    }

    /// <summary>
    /// Disables input listeners.
    /// </summary>
    void OnDisable() {
        Inputs.Hover.Disable();
    }

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region IHoverActions (Unity Input)
                                            ////////////////////////////////////

    /// <summary>
    ///     Checks if pointer hovers this object.
    ///     Updates encapsulated properties.
    /// </summary> <remarks>
    ///     Triggers on pointer movement.
    /// </remarks>
    public void OnPoint(InputAction.CallbackContext context) {
        var screenUV = context.ReadValue<Vector2>();
        var camRay = MainCam.ScreenPointToRay(screenUV);
        var rayHitSomething = Physics.Raycast(camRay, out var rayHit, Mathf.Infinity, raycastLayerMask);
        var rayHitThis = rayHit.transform == transform;

        if (rayHitSomething && rayHitThis) {
            if (IsHovered) return;
            IsHovered = true;
            OnPointerEnter?.Invoke(context);
        } else {
            if (!IsHovered) return;
            IsHovered = false;
            OnPointerExit?.Invoke(context);
        }
    }

                                                                      #endregion
}
}
