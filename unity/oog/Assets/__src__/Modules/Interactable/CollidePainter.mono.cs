using System.Collections.Generic;
using UnityEngine;
namespace Oog.Modules.Interactable {

/// <summary>
///     When another object enters collision, overwrite its materials.
///     And store its previous materials.
///     When it exits collision, restore its previous materials.
/// </summary> <remarks> <para>
///     On Awake, disables Rigidbody gravity and kinematics for this object.
/// </para> <para>
///     On Awake, ensures the Collider for this object is a trigger.
///     If the Collider is a MeshCollider, Unity requires it to be convex.
///     If it is not convex, disables this CollidePainter.
/// </para> </remarks>

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CollidePainter : MonoBehaviour {

                                            ////////////////////////////////////
                                            #region Public Fields and Properties
                                            ////////////////////////////////////

    // Serialized Fields
    public List<Material> entranceMaterials;
    /// <remarks> If empty, all objects are affected. </remarks>
    public List<string> tagFilters = new();

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Fields
                                            ////////////////////////////////////

    // References
    readonly Dictionary<Renderer, List<Material>> _prevMaterials = new();

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region MonoBehaviour Methods
                                            ////////////////////////////////////

    /// <summary>
    /// Ensures the collider is a trigger.
    /// Disables Rigidbody physics.
    /// </summary>
    void Awake() {
        var col = GetComponent<Collider>();
        if (col is MeshCollider { convex: false }) {
            Debug.LogError("Requires convex collider to enable trigger.", col);
            enabled = false;
            return;
        } col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
    }

    /// <summary>
    /// Overwrites materials if entering object matches tag filter.
    /// Stores previous materials.
    /// </summary>
    void OnTriggerEnter(Collider other) {
        if (tagFilters.Count > 0 && !tagFilters.Contains(other.tag)) return;
        if (!other.TryGetComponent<Renderer>(out var r)) return;
        _prevMaterials.Add(r, new List<Material>(r.sharedMaterials));
        r.SetSharedMaterials(entranceMaterials);
    }

    /// <summary>
    /// Restores previous materials if exiting object matches tag filter.
    /// Frees up memory.
    /// </summary>
    void OnTriggerExit(Collider other) {
        if (tagFilters.Count > 0 && !tagFilters.Contains(other.tag)) return;
        if (!other.TryGetComponent<Renderer>(out var r)) return;
        if (!_prevMaterials.TryGetValue(r, out var mats)) return;
        r.SetSharedMaterials(mats);
        _prevMaterials.Remove(r);
    }

                                                                      #endregion
}
}
