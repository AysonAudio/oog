using UnityEngine;
namespace Oog.Modules.CG {

/// <summary>
///     Handles interactions between camera space and other spaces.
/// </summary> <remarks>
///     Camera space follows the OpenGL convention:
///     Camera's forward is the negative Z axis.
/// </remarks>

public static class CamSpace {

                                            ////////////////////////////////////
                                            #region Extension Methods
                                            ////////////////////////////////////

    /// <summary>
    ///     Transforms a point from screen space into world space.
    ///     Distance from camera is same as worldDepthPoint parameter.
    /// </summary> <remarks> <para>
    ///     Constructs a plane at camera in world space.
    ///     The plane's normal is the camera's forward.
    /// </para> <para>
    ///     Distance from plane to worldDepthPoint is the z coordinate.
    ///     In ScreenToWorldPoint(Vector3 position), it's positive z in position.
    ///     In camera space, such as when using cameraToWorldMatrix, it's negative z.
    ///     It's negative because it follows the OpenGL convention.
    /// </para> <para>
    ///     In effect, constructs a parallel plane at worldDepthPoint.
    ///     From screenPoint, finds closest point on parallel plane.
    ///     The closest point is the new object position.
    /// </para> </remarks>
    public static Vector3 ScreenToWorldPoint(this Camera cam, Vector2 screenPoint, Vector3 worldDepthPoint) {
        var plane = new Plane(cam.transform.forward, cam.transform.position);
        var z = plane.GetDistanceToPoint(worldDepthPoint);
        var buffer = new Vector3(screenPoint.x, screenPoint.y, z);
        return cam.ScreenToWorldPoint(buffer);
    }

                                                                      #endregion
}
}
