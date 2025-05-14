using System;
namespace Oog.Modules.Math {

/// <summary>
///     Static functions to scale an input along a blend curve.
/// </summary> <remarks> <list>
///     <listheader> <description> Domain </description> </listheader>
///     <item> <description> Input is number of elapsed interpolation periods. </description> </item>
///     <item> <description> One period maps to [0, 1]. </description> </item>
/// </list> <list>
///     <listheader> <description> Range </description> </listheader>
///     <item> <description> Output is a transformation scalar to interpolate another value. </description> </item>
///     <item> <description> One amplitude maps to [0, 1]. </description> </item>
/// </list> <list>
///     <listheader> <description> Periodicity </description> </listheader>
///     <item> <description> Functions can accept inputs outside of [0, 1]. </description> </item>
///     <item> <description> For BlendIn functions, input and output are clamped to [0, 1]. </description> </item>
///     <item> <description> For BlendInOut functions, output alternates up and down: </description> </item>
///     <item> <description> [0 to 1], [1 to 0], [0 to 1], [1 to 0]... </description> </item>
/// </list> </remarks>

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
