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

    public ScriptableObject? TryGetAsset() {
        _asyncLoader ??= Addressables.LoadAssetAsync<T>(AssetAddress);
        if (!_asyncLoader.Value.IsValid())
            return null;
        if (_asyncLoader.Value is { IsDone: true, Status: AsyncOperationStatus.Failed })
            _asyncLoader = Addressables.LoadAssetAsync<T>(AssetAddress);
        _asyncLoader.Value.WaitForCompletion();
        return _asyncLoader.Value.Result;
    }

    public TT? TryGetAsset<TT>() where TT : ScriptableObject => TryGetAsset() as TT;

                                                                      #endregion
}
}
