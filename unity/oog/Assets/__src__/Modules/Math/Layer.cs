using System;
namespace Oog.Modules.Math {

/// <summary>
/// Utility functions for Unity Layers.
/// </summary>

public static class Layer {
                                            ////////////////////////////////////
                                            #region Extension Methods
                                            ////////////////////////////////////

    public static int ToLayerMask(this int layer) => 1 << layer;

                                                                      #endregion
}
}
