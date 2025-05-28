using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Oog.Settings;
using Oog.Modules.Math;
using Oog.Modules.CG;
namespace Oog.Modules.Interactable {

/// <summary>
///     Allows a screen space pointer to drag-and-drop this object in world space. <br/>
///     Invokes C# events when: <list type="bullet">
///         <item> <description> Dragging starts. </description> </item>
///         <item> <description> Dragging stops. </description> </item>
///         <item> <description> Position changes while dragging. </description> </item>
/// </list> </summary> <remarks> <para>
///     Quickly tapping the pointer toggles dragging.
///     Holding the pointer enables dragging until release.
/// </para> <para>
///     Drags on a plane parallel to the camera clip plane.
///     When position changes, constructs a new parallel plane containing object.
///     In effect, maintains distance between object and closest point on clip plane.
/// </para> </remarks>

[RequireComponent(typeof(HoverSensor))]
public class Draggable : MonoBehaviour, InputSet.IDragActions {

                                            ////////////////////////////////////
                                            #region Public Fields and Properties
                                            ////////////////////////////////////

    // Serialized Fields
    public DragTrigger dragTriggers = DragTrigger.Click;
    public bool clampMin;
    public bool clampMax;
    public Vector3 clampWorldMin = Vector3.zero;
    public Vector3 clampWorldMax = Vector3.zero;

    // Encapsulated Properties
    public Vector2 PointerPosition { get; private set; }
    public bool DragIsToggledOn => DragStatus.HasFlag(DragMode.Toggle);
    public bool DragIsHeldOn => DragStatus.HasFlag(DragMode.Hold);
    public bool IsDragging => DragStatus != DragMode.None;

    // C# Events
    public event Action<InputAction.CallbackContext> OnDragStart;
    public event Action<InputAction.CallbackContext> OnDragMove;
    public event Action<InputAction.CallbackContext> OnDragEnd;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Fields
                                            ////////////////////////////////////

    // Cached Unity Components
    internal HoverSensor Sensor { get; private set; }
    internal InputSet Inputs { get; private set; }

    // State Flags
    internal DragMode DragStatus;

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
    ///         <item> <description> An attached HoverSensor component is not found or is disabled. </description> </item>
    /// </list> </remarks>
    void OnEnable() {
        if ((Sensor ??= GetComponent<HoverSensor>()) == null || !Sensor.enabled) {
            Debug.LogWarning("Draggable component failed to enable.", Sensor);
            enabled = false;
            return;
        } if (Inputs == null) {
            Inputs = new InputSet();
            Inputs.Drag.SetCallbacks(this);
        } Inputs.Drag.Enable();
    }

    /// <summary>
    ///     Disables input listeners in Unity Input System package.
    /// </summary> <remarks>
    ///     For faster re-enabling, callbacks remain cached.
    /// </remarks>
    void OnDisable() {
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
        PointerPosition = context.ReadValue<Vector2>();
        if (DragStatus == DragMode.None) return;
        var cam = Sensor.MainCam;
        var worldPoint = cam.ScreenToWorldPoint(PointerPosition, transform.position);
        if (clampMin) {
            worldPoint.x = Mathf.Max(worldPoint.x, clampWorldMin.x);
            worldPoint.y = Mathf.Max(worldPoint.y, clampWorldMin.y);
            worldPoint.z = Mathf.Max(worldPoint.z, clampWorldMin.z);
        } if (clampMax) {
            worldPoint.x = Mathf.Min(worldPoint.x, clampWorldMax.x);
            worldPoint.y = Mathf.Min(worldPoint.y, clampWorldMax.y);
            worldPoint.z = Mathf.Min(worldPoint.z, clampWorldMax.z);
        } transform.position = worldPoint;
        OnDragMove?.Invoke(context);
    }

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Methods
                                            ////////////////////////////////////

    /// <summary>
    ///     Begins dragging, with logic to prevent duplicate events.
    /// </summary> <remarks>
    ///     #1: Invokes OnDragStart. <br/>
    ///     #2: Pauses HoverSensor. <br/>
    ///     #3: Adds a DragMode state flag. <br/>
    ///     If any DragMode flag is already present, only does #3.
    /// </remarks>
    void TurnOnDragMode(InputAction.CallbackContext context, DragMode dragMode) {
        if (DragStatus == DragMode.None) {
            Sensor.IsPaused = true;
            OnDragStart?.Invoke(context);
        } DragStatus.AddFlag(dragMode);
    }

    /// <summary>
    ///     Begins dragging, with logic to prevent duplicate events.
    /// </summary> <remarks>
    ///     #1: Invokes OnDragStart. <br/>
    ///     #2: Pauses HoverSensor. <br/>
    ///     #3: Adds all DragMode state flags. <br/>
    ///     If any DragMode flag is already present, only does #3.
    /// </remarks>
    void TurnOnDrag(InputAction.CallbackContext context) {
        if (DragStatus == DragMode.None) {
            Sensor.IsPaused = true;
            OnDragStart?.Invoke(context);
        } DragStatus.AddAllFlags();
    }

    /// <summary>
    ///     Ends dragging, with logic to prevent duplicate events.
    /// </summary> <remarks>
    ///     #1: Invokes OnDragEnd. <br/>
    ///     #2: Pauses HoverSensor. <br/>
    ///     #3: Removes a DragMode state flag. <br/>
    ///     If no DragMode flag is already present, does nothing.
    /// </remarks>
    void TurnOffDragMode(InputAction.CallbackContext context, DragMode dragMode) {
        if (DragStatus == DragMode.None) return;
        DragStatus.RemoveFlag(dragMode);
        if (DragStatus != DragMode.None) return;
        Sensor.IsPaused = false;
        OnDragEnd?.Invoke(context);
    }

    /// <summary>
    ///     Ends dragging, with logic to prevent duplicate events.
    /// </summary> <remarks>
    ///     #1: Invokes OnDragEnd. <br/>
    ///     #2: Pauses HoverSensor. <br/>
    ///     #3: Removes all DragMode states flag. <br/>
    ///     If no DragMode flag is already present, does nothing.
    /// </remarks>
    void TurnOffDrag(InputAction.CallbackContext context) {
        if (DragStatus == DragMode.None) return;
        DragStatus = DragMode.None;
        Sensor.IsPaused = false;
        OnDragEnd?.Invoke(context);
    }

    /// <summary>
    ///     Using parameters: <list type="bullet">
    ///         <item> <description> Determines what the pointer just did. </description> </item>
    ///         <item> <description> Starts or stops dragging if the event calls for it. </description> </item>
    /// </list> </summary> <param name="context">
    ///     A context object from the Unity Input System package.
    ///     Provided by methods implemented in IDragActions.
    /// </param> <param name="trigger">
    ///     A bit flag indicating an Action in the Unity Input System package.
    /// </param>
    void ParseInteraction(InputAction.CallbackContext context, DragTrigger trigger) {
        if (!dragTriggers.HasFlag(trigger)) return;
        if (!Sensor.IsHovered) return;
        switch (context.interaction) {
            case PressInteraction when context.started: TurnOnDragMode(context, DragMode.Hold); break;
            case PressInteraction when context.performed: TurnOffDragMode(context, DragMode.Hold); break;
            case TapInteraction when context.started: TurnOnDragMode(context, DragMode.Hold); break;
            case TapInteraction when context.canceled: TurnOffDragMode(context, DragMode.Toggle); break;
            case TapInteraction when context.performed:
                if (DragIsToggledOn) TurnOffDrag(context);
                else TurnOnDragMode(context, DragMode.Toggle);
                break;
        }
    }

                                                                      #endregion
}
}
