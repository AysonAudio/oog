using System;
namespace Oog.Modules.Math {

/// <summary>
/// Scales an input along a blend curve.
/// Input is elapsed percentage of one input period (0 to 1).
/// Output is transformation scalar (0 to 1).
/// For BlendIn functions, input is clamped (0 to 1).
/// For BlendInOut functions, output alternates between going up and down (0 to 1, 1 to 0...)
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
        Mode.Quadratic => QuadraticIn,
        Mode.Bezier => BezierIn,
        Mode.Parametric => ParametricIn,
        _ => t => t
    };

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Public Funcs - Quadratic
                                            ////////////////////////////////////

    public static float Quadratic(float elapsedPeriods, bool isInOut = true) {
        if (!isInOut) elapsedPeriods = System.Math.Clamp(elapsedPeriods, 0f, 1f);
        if (elapsedPeriods <= 0.5f) return 2f * elapsedPeriods * elapsedPeriods;
        elapsedPeriods -= 0.5f;
        return 2f * elapsedPeriods * (1f - elapsedPeriods) + 0.5f;
    }
    public static float QuadraticInOut(float elapsedPeriods) {
        var periodRemainder = elapsedPeriods % 1;
        var goingDown = System.Math.Floor(elapsedPeriods % 2) != 0;
        return goingDown
            ? Quadratic(1 - periodRemainder)
            : Quadratic(periodRemainder);
    }
    public static float QuadraticIn(float elapsedPeriods) => Quadratic(elapsedPeriods, false);

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Public Funcs - Bezier
                                            ////////////////////////////////////

    public static float Bezier(float elapsedPeriods, bool isInOut = true) {
        if (!isInOut) elapsedPeriods = System.Math.Clamp(elapsedPeriods, 0f, 1f);
        return elapsedPeriods * elapsedPeriods * (3f - 2f * elapsedPeriods);
    }
    public static float BezierInOut(float elapsedPeriods) {
        var periodRemainder = elapsedPeriods % 1;
        var goingDown = System.Math.Floor(elapsedPeriods % 2) != 0;
        return goingDown
            ? Bezier(1 - periodRemainder)
            : Bezier(periodRemainder);
    }
    public static float BezierIn(float elapsedPeriods) => Bezier(elapsedPeriods, false);

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Public Funcs - Parametric
                                            ////////////////////////////////////

    public static float Parametric(float elapsedPeriods, bool isInOut = true) {
        if (!isInOut) elapsedPeriods = System.Math.Clamp(elapsedPeriods, 0f, 1f);
        var sq = elapsedPeriods * elapsedPeriods;
        return sq / (2f * (sq - elapsedPeriods) + 1f);
    }
    public static float ParametricInOut(float elapsedPeriods) {
        var periodRemainder = elapsedPeriods % 1;
        var goingDown = System.Math.Floor(elapsedPeriods % 2) != 0;
        return goingDown
            ? Parametric(1 - periodRemainder)
            : Parametric(periodRemainder);
    }
    public static float ParametricIn(float elapsedPeriods) => Parametric(elapsedPeriods, false);

                                                                      #endregion

}
}
