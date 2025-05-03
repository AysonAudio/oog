
//-----------------------------------------------------------------------------
// Scales an input along a blend curve.
// Input is elapsed percentage of one input period (0 to 1).
// Output is transformation scalar (0 to 1).
// For BlendIn functions, output is clamped (0 to 1).
// For BlendInOut functions, output alternates between going up and down (0 to 1, 1 to 0...)
//-----------------------------------------------------------------------------

real BlendQuadratic(real elapsedPeriods, bool isInOut = true) {
    if (!isInOut) elapsedPeriods = clamp(elapsedPeriods, 0, 1);
    if (elapsedPeriods <= 0.5) return 2 * elapsedPeriods * elapsedPeriods;
    elapsedPeriods -= 0.5;
    return 2 * elapsedPeriods * (1 - elapsedPeriods) + 0.5;
}
real BlendQuadraticIn(real elapsedPeriods) {
    return BlendQuadratic(elapsedPeriods, false);
}
real BlendQuadraticInOut(real elapsedPeriods) {
    float periodRemainder = elapsedPeriods % 1;
    bool goingDown = floor(elapsedPeriods % 2) != 0;
    return goingDown
        ? BlendQuadratic(1 - periodRemainder)
        : BlendQuadratic(periodRemainder);
}

real BlendBezierIn(real elapsedPeriods) {
    return elapsedPeriods * elapsedPeriods * (3 - 2 * elapsedPeriods);
}
real BlendBezierInOut(real elapsedPeriods) {
    float periodRemainder = elapsedPeriods % 1;
    bool goingDown = floor(elapsedPeriods % 2) != 0;
    return goingDown ? BlendBezierIn(1 - periodRemainder) : BlendBezierIn(periodRemainder);
}

real BlendParametricIn(real elapsedPeriods) {
    float sq = elapsedPeriods * elapsedPeriods;
    return sq / (2 * (sq - elapsedPeriods) + 1);
}
real BlendParametricInOut(real elapsedPeriods) {
    float periodRemainder = elapsedPeriods % 1;
    bool goingDown = floor(elapsedPeriods % 2) != 0;
    return goingDown ? BlendParametricIn(1 - periodRemainder) : BlendParametricIn(periodRemainder);
}
