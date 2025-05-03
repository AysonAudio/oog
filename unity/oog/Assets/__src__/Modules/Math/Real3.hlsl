
//-----------------------------------------------------------------------------
// Checks if a given vector contains at least one component
// with a value less than or equal to a given maximum.
//-----------------------------------------------------------------------------

bool VectContainsMax(real2 vect, real max) {
    if (vect.x <= max) return true;
    if (vect.y <= max) return true;
    return false;
}

bool VectContainsMax(real3 vect, real max) {
    if (vect.x <= max) return true;
    if (vect.y <= max) return true;
    if (vect.z <= max) return true;
    return false;
}

bool VectContainsMax(real4 vect, real max) {
    if (vect.x <= max) return true;
    if (vect.y <= max) return true;
    if (vect.z <= max) return true;
    if (vect.w <= max) return true;
    return false;
}

//-----------------------------------------------------------------------------
// Gets the smallest component of a vector.
//-----------------------------------------------------------------------------

real MinComponent(real2 vect) {
    return min(vect.x, vect.y);
}

real MinComponent(real3 vect) {
    return min(MinComponent(vect.xy), vect.z);
}

real MinComponent(real4 vect) {
    return min(MinComponent(vect.xyz), vect.w);
}
