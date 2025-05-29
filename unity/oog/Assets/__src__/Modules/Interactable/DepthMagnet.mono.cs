using UnityEngine;
using UnityEngine.InputSystem;
namespace Oog.Modules.Interactable  {

/// <summary>
///     While dragging, maps 2D offset to depth offset.
/// </summary> <remarks> <para>
///     If the Draggable component is set to clamp,
///     the clamp setting of this DepthMagnet component has no effect.
/// </para> <para>
///     When dragging starts, constructs an axis at this object.
///     +X and +Y are in the direction of a target point.
///     When dragged, Z is set to (X + Y) * scaleZ, including signs of X and Y.
/// </para> <para>
///     The target point is the position of a given object.
///     When the target moves, the axis moves the same amount,
///     so that the vector between target and axis remains constant.
///     This will result in sudden jumps in depth if the target moves while dragging.
/// </para> </remarks>

[RequireComponent(typeof(Draggable))]
public class DepthMagnet : MonoBehaviour {

                                            ////////////////////////////////////
                                            #region Public Fields and Properties
                                            ////////////////////////////////////

    // Serialized Fields
    public Transform target;
    public bool clampMinZ;
    public bool clampMaxZ;
    public float clampWorldMinZ;
    public float clampWorldMaxZ;
    public float scaleZ = 1f;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Fields
                                            ////////////////////////////////////

    // Cached Unity Components
    internal Draggable Drag { get; private set; }

    // Cached Values
    internal Vector3 AxisOffsetFromTarget { get; private set; }
    internal Vector3 AxisWorldOrigin => target.TransformPoint(AxisOffsetFromTarget);

    // State Flags
    internal bool EventsHooked { get; private set; }

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region MonoBehaviour Event Funcs
                                            ////////////////////////////////////

    /// <summary>
    ///     Caches Unity components.
    ///     Registers Draggable callbacks.
    /// </summary> <remarks>
    ///     Does nothing and remains disabled if: <list type="bullet">
    ///         <item> <description> An attached Draggable component is not found or is disabled. </description> </item>
    /// </list> </remarks>
    void OnEnable() {
        if ((Drag ??= GetComponent<Draggable>()) == null || !Drag.enabled) {
            Debug.LogWarning("DepthMagnet component failed to enable.", Drag);
            enabled = false;
            return;
        } if (EventsHooked) return;
        Drag.OnDragStart += GetOffset;
        Drag.OnDragMove += UpdateDepth;
        EventsHooked = true;
    }

    /// <summary>
    /// Unregisters Draggable callbacks.
    /// </summary>
    void OnDisable() {
        Drag.OnDragStart -= GetOffset;
        Drag.OnDragMove -= UpdateDepth;
        EventsHooked = false;
    }

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Methods
                                            ////////////////////////////////////

    /// <remarks>
    /// Callback for Draggable.OnDragStart. <br/>
    /// If target is not set, disables this object and does nothing.
    /// </remarks>
    void GetOffset(InputAction.CallbackContext ctx) {
        if (!target) {
            Debug.LogWarning("Depth Magnet failed to init. Disabling...", target);
            enabled = false;
            return;
        } AxisOffsetFromTarget = target.InverseTransformPoint(transform.position);
    }

    /// <remarks>
    /// Callback for Draggable.OnDragMove. <br/>
    /// Clamps if the Draggable component is not set to clamp.
    /// </remarks>
    void UpdateDepth(InputAction.CallbackContext ctx) {
        var displacement = transform.position - AxisWorldOrigin;
        displacement.z = (displacement.x + displacement.y) * scaleZ;
        if (!Drag.clampMin && clampMinZ) displacement.z = Mathf.Max(displacement.z, clampWorldMinZ);
        if (!Drag.clampMax && clampMaxZ) displacement.z = Mathf.Min(displacement.z, clampWorldMaxZ);
        transform.position = AxisWorldOrigin + displacement;
    }

                                                                        #endregion
}
}
