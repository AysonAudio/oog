using UnityEngine;
namespace Oog.Modules.Conveyor {

/// <summary>
/// Periodically displaces two objects in a direction.
/// Periodically teleports the 2nd object behind the 1st.
/// If the loop points are off screen, it looks like a seamless conveyor belt.
/// </summary>

public class WarpSlide : MonoBehaviour {

                                            ////////////////////////////////////
                                            #region Serialized Fields
                                            ////////////////////////////////////

    public Transform slidingObject1;
    public Transform slidingObject2;
    public Vector3 localStartPosition1;
    public Vector3 localStartPosition2;
    public Vector3 displacementPerPeriod;
    public float secondsPerPeriod;
    public float distancePerWarp;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Fields
                                            ////////////////////////////////////

    Vector3 _displacementSinceLastWarp = Vector3.zero;
    float _secondsSinceLastPeriod;
    bool _swapped;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region MonoBehaviour Event Funcs
                                            ////////////////////////////////////

    void FixedUpdate() {
        _secondsSinceLastPeriod += Time.deltaTime;
        if (_secondsSinceLastPeriod < secondsPerPeriod) return;
        _secondsSinceLastPeriod -= secondsPerPeriod;
        _displacementSinceLastWarp += displacementPerPeriod;

        // After moving enough distance, swap objects and reset positions, keeping overflowed displacement
        if (_displacementSinceLastWarp.magnitude > distancePerWarp) {
            _displacementSinceLastWarp -= displacementPerPeriod.normalized * distancePerWarp;
            _swapped = !_swapped;
            if (slidingObject1)
                slidingObject1.localPosition = _swapped ? localStartPosition2 : localStartPosition1;
            if (slidingObject2)
                slidingObject2.localPosition = _swapped ? localStartPosition1 : localStartPosition2;
        }

        // Every period, displace both objects
        if (slidingObject1)
            slidingObject1.localPosition = _displacementSinceLastWarp +
                (_swapped ? localStartPosition2 : localStartPosition1);
        if (slidingObject2)
            slidingObject2.localPosition = _displacementSinceLastWarp +
                (_swapped ? localStartPosition1 : localStartPosition2);
    }

                                                                      #endregion
}
}
