using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Oog.Settings;
using UnityEngine.InputSystem.Interactions;
namespace Oog.Modules.Interactable {

/// <summary>
///     Allows a screen-space pointer to drag-and-drop this object in world space.
///     Invokes C# events when it starts and stops dragging.
/// </summary> <remarks>
///     Quickly tapping the pointer toggles dragging.
///     Holding the pointer enables dragging until release.
/// </remarks>

[RequireComponent(typeof(EventHover))]
public class EventDrag : MonoBehaviour, InputSet.IDragActions {

                                            ////////////////////////////////////
                                            #region Public Fields and Properties
                                            ////////////////////////////////////

    // Serialized Fields
    public DragTrigger dragTriggers = DragTrigger.Click;

    // Encapsulated Properties
    public bool IsDragging { get; private set; }

    // C# Events
    public event Action<InputAction.CallbackContext> OnDragStart;
    public event Action<InputAction.CallbackContext> OnDragEnd;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Fields
                                            ////////////////////////////////////

    // Cached Unity Components
    internal EventHover HoverSensor { get; private set; }
    internal InputSet Inputs { get; private set; }

    // Unity Input System
    bool _tapIsToggledOn;
    float _distanceFromCamera;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region MonoBehaviour Event Funcs
                                            ////////////////////////////////////

    /// <summary>
    ///     Caches Unity components.
    ///     Registers callbacks for Unity Input System package.
    ///     Enables input listeners.
    ///     Initializes private fields.
    /// </summary> <remarks>
    ///     No-ops if EventHover not found or disabled.
    ///     If no-op, this EventDrag remains disabled.
    /// </remarks>
    protected void OnEnable() {
        if ((HoverSensor ??= GetComponent<EventHover>()) == null || !HoverSensor.enabled) {
            enabled = false;
            return;
        } if (Inputs == null) {
            Inputs = new InputSet();
            Inputs.Drag.SetCallbacks(this);
        } Inputs.Drag.Enable();
        _distanceFromCamera = Vector3.Distance(transform.position, HoverSensor.MainCam.transform.position);
    }

    /// <summary>
    /// Disconnects input listeners.
    /// </summary>
    protected void OnDisable() {
        Inputs.Drag.Disable();
    }

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region IDragActions (Unity Input)
                                            ////////////////////////////////////

    public void OnClick(InputAction.CallbackContext context) => ParseInteraction(context, DragTrigger.Click);
    public void OnMiddleClick(InputAction.CallbackContext context) => ParseInteraction(context, DragTrigger.MiddleClick);
    public void OnRightClick(InputAction.CallbackContext context) => ParseInteraction(context, DragTrigger.RightClick);

    /// <summary>
    ///     Updates encapsulated pointer position in screen space.
    ///     If dragging, move object relative to new pointer position.
    /// </summary> <remarks>
    ///     Triggers on pointer movement.
    /// </remarks>
    public void OnPoint(InputAction.CallbackContext context) {
        if (!IsDragging) return;
        DragToScreenPoint(context.ReadValue<Vector2>());
    }

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Methods
                                            ////////////////////////////////////

    /// <remarks>
    /// No-ops if drag already on.
    /// </remarks>
    void TurnOnDrag(InputAction.CallbackContext context) {
        if (!IsDragging) OnDragStart?.Invoke(context);
        IsDragging = true;
    }

    /// <remarks>
    /// No-ops if drag already off.
    /// </remarks>
    void TurnOffDrag(InputAction.CallbackContext context) {
        if (IsDragging) OnDragEnd?.Invoke(context);
        IsDragging = false;
    }

    /// <summary>
    /// Calculates new world position for dragged object.
    /// </summary>
    void DragToScreenPoint(Vector2 screenPt) {
        var buffer = new Vector3(screenPt.x, screenPt.y, _distanceFromCamera);
        transform.position = HoverSensor.MainCam.ScreenToWorldPoint(buffer);
    }

    /// <summary>
    ///     Checks serialized properties for which device inputs are enabled.
    ///     If parameter matches, toggle dragging based on current interaction phase.
    /// </summary> <remarks>
    ///     No-ops if pointer is not hovering this object.
    /// </remarks>
    void ParseInteraction(InputAction.CallbackContext context, DragTrigger trigger) {
        if (!dragTriggers.HasFlag(trigger)) return;
        if (!HoverSensor.IsHovered) return;
        switch (context.interaction) {
            case PressInteraction when context.started:
            case TapInteraction when context.started:
                TurnOnDrag(context);
                break;
            case PressInteraction when context.performed:
                _tapIsToggledOn = false;
                TurnOffDrag(context);
                break;
            case TapInteraction when context.performed:
                if (_tapIsToggledOn) {
                    _tapIsToggledOn = false;
                    TurnOffDrag(context);
                } else {
                    _tapIsToggledOn = true;
                    TurnOnDrag(context);
                } break;
        }
    }

                                                                      #endregion
}
}
