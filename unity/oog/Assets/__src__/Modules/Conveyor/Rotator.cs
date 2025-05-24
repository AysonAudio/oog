using System;
using UnityEngine;
namespace Oog.Modules.Conveyor {
public class Rotator : MonoBehaviour {
    public float secondsPerPeriod = 0.02f;
    public Vector3 eulersPerPeriod = new(0, 3f, 0);
    float _secondsSinceLastPeriod;

    void FixedUpdate() {
        _secondsSinceLastPeriod += Time.deltaTime;
        if (_secondsSinceLastPeriod < secondsPerPeriod) return;
        _secondsSinceLastPeriod -= secondsPerPeriod;
        transform.Rotate(eulersPerPeriod);
    }
}
}
