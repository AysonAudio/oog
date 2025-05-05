using System;
using System.Linq;
namespace Oog.Modules.Math {
public static class Bitwise {

                                            ////////////////////////////////////
                                            #region Extension Methods
                                            ////////////////////////////////////

    /// <summary> Converts a Unity layer index to a layer mask. </summary>
    /// <remarks> Does not mutate the original variable. </remarks>
    /// <example> Converts 0 to 0001. <br/> </example>
    /// <example> Converts 1 to 0010. <br/> </example>
    /// <example> Converts 2 to 0100. <br/> </example>
    /// <example> Converts 3 to 1000. <br/> </example>
    public static int ToLayerMask(this int layer) => layer.ToBitMask();

    /// <summary> Converts an integer to a bitwise mask. </summary>
    /// <remarks> Does not mutate the original variable. </remarks>
    /// <example> Converts 0 to 0001. <br/> </example>
    /// <example> Converts 1 to 0010. <br/> </example>
    /// <example> Converts 2 to 0100. <br/> </example>
    /// <example> Converts 3 to 1000. <br/> </example>
    public static int ToBitMask<T>(this T enumVar) where T : struct, IConvertible => 1 << (int)(object)enumVar;




    /// <summary> Sets a specific bit to 1. </summary>
    /// <remarks> The underlying integrals should go 0, 2, 4, 8... <br/> </remarks>
    /// <remarks> Mutates the original variable. </remarks>
    public static T AddFlag<T>(this ref T enumVar, T flagBitmask) where T : struct, IConvertible {
        var enumVal = (int)(object)enumVar | (int)(object)flagBitmask;
        enumVar = (T)(object)enumVal;
        return enumVar;
    }

    /// <summary> Gets a new value with a specific bit set to 1. </summary>
    /// <remarks> The underlying integrals should go 0, 2, 4, 8... <br/> </remarks>
    /// <remarks> Does not mutate the original variable. </remarks>
    public static T WithFlag<T>(this T enumVar, T flagBitmask) where T : struct, IConvertible {
        var enumVal = (int)(object)enumVar | (int)(object)flagBitmask;
        return (T)(object)enumVal;
    }




    /// <summary> Sets a specific bit to 0. </summary>
    /// <remarks> The underlying integrals should go 0, 2, 4, 8... <br/> </remarks>
    /// <remarks> Mutates the original variable. </remarks>
    public static T RemoveFlag<T>(this ref T enumVar, T flagBitmask) where T : struct, IConvertible {
        var enumVal = (int)(object)enumVar & ~(int)(object)flagBitmask;
        enumVar = (T)(object)enumVal;
        return enumVar;
    }

    /// <summary> Gets a new value with a specific bit set to 0. </summary>
    /// <remarks> The underlying integrals should go 0, 2, 4, 8... <br/> </remarks>
    /// <remarks> Does not mutate the original variable. </remarks>
    public static T WithoutFlag<T>(this T enumVar, T flagBitmask) where T : struct, IConvertible {
        var enumVal = (int)(object)enumVar & ~(int)(object)flagBitmask;
        return (T)(object)enumVal;
    }




    /// <summary> Sets all bits to 1. </summary>
    /// <remarks> The underlying integrals should go 0, 2, 4, 8... <br/> </remarks>
    /// <remarks> Mutates the original variable. </remarks>
    public static T AddAllFlags<T>(this ref T enumVar) where T : struct, IConvertible {
        var flagBitmask = ((int[])Enum.GetValues(typeof(T))).Sum();
        var enumVal = (int)(object)enumVar | (int)(object)flagBitmask;
        enumVar = (T)(object)enumVal;
        return enumVar;
    }

    /// <summary> Gets a new value with all bits set to 1. </summary>
    /// <remarks> The underlying integrals should go 0, 2, 4, 8... <br/> </remarks>
    /// <remarks> Does not mutate the original variable. </remarks>
    public static T WithAllFlags<T>(this T enumVar) where T : struct, IConvertible {
        var flagBitmask = ((int[])Enum.GetValues(typeof(T))).Sum();
        var enumVal = (int)(object)enumVar | (int)(object)flagBitmask;
        return (T)(object)enumVal;
    }

                                                                      #endregion
}
}
