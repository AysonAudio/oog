using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Oog.Modules.Math;
using Oog.Settings;
namespace Oog.Modules.Interactable {

/// <summary>
///     Invokes C# events when any active pointer enters and exits attached object.
/// </summary> <remarks> <para>
///     Unity Input System package provides the pointer position in screen pixel space. <br/>
///     UnityEngine converts it to a point on camera's near clip plane, in world space. <br/>
///     Casts a ray from this point to main camera.
/// </para> <para>
///     Activates OnPointerEnter when ray starts hitting this object. <br/>
///     Activates OnPointerExit when ray stops hitting this object. <br/>
///     If another object is between cam and this object, OnPointerEnter will not activate.
/// </para> </remarks>

[RequireComponent(typeof(Renderer))]
public class HoverSensor : MonoBehaviour, InputSet.IHoverActions {

                                            ////////////////////////////////////
                                            #region Public Fields and Properties
                                            ////////////////////////////////////

    // Serialized Fields
    public LayerMask raycastLayerMask = INTERACTABLE.LAYER.DEFAULT.ToLayerMask();

    // Encapsulated Properties
    /// <summary> If true, stops polling for updates. IsHovered will freeze at it's current value. </summary>
    public bool IsPaused { get; set; }
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
    ///     Caches Unity components. <br/>
    ///     Hooks up Unity Input System package: <list type="bullet">
    ///         <item> <description> Registers callbacks if null. </description> </item>
    ///         <item> <description> Enables input listeners. </description> </item>
    /// </list> </summary> <remarks>
    ///     Does nothing and remains disabled if: <list type="bullet">
    ///         <item> <description> Main camera is not found or is disabled. </description> </item>
    ///         <item> <description> An attached Renderer component is not found or is disabled. </description> </item>
    /// </list> </remarks>
    void OnEnable() {
        if ((MainCam ??= Camera.main) == null
        || !MainCam.enabled
        || (Renderer ??= GetComponent<Renderer>()) == null
        || !Renderer.enabled) {
            Debug.LogWarning("HoverSensor component failed to enable.");
            enabled = false;
            return;
        } if (Inputs == null) {
            Inputs = new InputSet();
            Inputs.Hover.SetCallbacks(this);
        } Inputs.Hover.Enable();
    }

    /// <summary>
    ///     Disables input listeners in Unity Input System package.
    /// </summary> <remarks>
    ///     For faster re-enabling, callbacks remain cached.
    /// </remarks>
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
        if (IsPaused) return;
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
