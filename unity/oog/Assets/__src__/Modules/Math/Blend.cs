using System;
namespace Oog.Modules.Math {

/// <summary>
/// Contains math functions for smoothly interpolating a value over time.
/// All functions have an input and output from 0 to 1.
/// </summary>
public static class Blend {

                                            ////////////////////////////////////
                                            #region Types
                                            ////////////////////////////////////

    public enum Mode {
        Quadratic,
        Bezier,
        Parametric
    }

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Extension Methods
                                            ////////////////////////////////////

    public static Func<float, float> ToFunc(this Mode mode) => mode switch {
        Mode.Quadratic => Quadratic,
        Mode.Bezier => Bezier,
        Mode.Parametric => Parametric,
        _ => t => t
    };

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Public Functions
                                            ////////////////////////////////////

    public static float Quadratic(float t) {
        if (t <= 0.5f) return 2f * t * t;
        t -= 0.5f;
        return 2f * t * (1f - t) + 0.5f;
    }
    public static float Bezier(float t) {
        return t * t * (3f - 2f * t);
    }
    public static float Parametric(float t) {
        var sqr = t * t;
        return sqr / (2f * (sqr - t) + 1f);
    }

                                                                      #endregion
}
}
