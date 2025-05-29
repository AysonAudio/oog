#nullable enable
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Oog.Modules.LazyLoad {

/// <summary>
/// An asynchronously loaded asset.
/// On first access, load data.
/// On subsequent accesses, use cached data.
/// </summary>

public class SafeCache<T> where T : ScriptableObject {

                                            ////////////////////////////////////
                                            #region Fields and Properties
                                            ////////////////////////////////////

    public string AssetAddress { get; }
    AsyncOperationHandle<T>? _asyncLoader;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Constructors and Destructors
                                            ////////////////////////////////////

    public SafeCache(string address) => AssetAddress = address;
    ~SafeCache() => _asyncLoader?.Release();

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Public Methods
                                            ////////////////////////////////////

    public bool TryGetAsset(out T? asset) {
        asset = null;
        _asyncLoader ??= Addressables.LoadAssetAsync<T>(AssetAddress);
        if (!_asyncLoader.Value.IsValid()) return false;

        // If called again after previous failure, try again
        if (_asyncLoader.Value is { IsDone: true, Status: AsyncOperationStatus.Failed })
            _asyncLoader = Addressables.LoadAssetAsync<T>(AssetAddress);

        // Get results
        _asyncLoader.Value.WaitForCompletion();
        if (_asyncLoader.Value.Status == AsyncOperationStatus.Failed) return false;
        asset = _asyncLoader.Value.Result;
        return true;
    }

                                                                      #endregion
}
}
