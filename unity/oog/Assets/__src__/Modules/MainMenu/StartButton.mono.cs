using System;
using System.Collections;
using Oog.Modules.Math;
using UnityEngine;
using UnityEngine.UIElements;
namespace Oog.Modules.MainMenu {

/// <summary>
/// Registers callbacks for the main menu start button.
/// On click, slides the button off-screen over time.
/// At the end, disables the button's GameObject.
/// </summary> <remarks>
/// Attach this to the GameObject with a UIDocument that loads the start button.
/// </remarks>

[RequireComponent(typeof(UIDocument))]
public class StartButtonMono : MonoBehaviour {

                                            ////////////////////////////////////
                                            #region Serialized Fields
                                            ////////////////////////////////////

    [Header("Animation Options")]
    public int framesPerSec = 60;
    public int frameCount = 45;
    public Vector2 startOffset = Vector2.zero;
    public Vector2 endOffset = new(0f, -125f);
    public Blend.Mode blendMode = Blend.Mode.Bezier;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Public Events
                                            ////////////////////////////////////

    public event Action AnimationStart;
    public event Action AnimationEnd;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Fields
                                            ////////////////////////////////////

    Action _startCallback;
    Button _startButton;
    Vector2 _currentOffset;
    Coroutine _currentAnimation;

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region MonoBehaviour Event Funcs
                                            ////////////////////////////////////

    /// <summary>
    /// Inits private reference variables.
    /// Inits and enables UI events.
    /// </summary>
    void OnEnable() {
        _startButton ??= GetComponent<UIDocument>().rootVisualElement.Q<Button>("start-button");
        _startButton.SetEnabled(true);

        // Ensure callback is only registered once.
        if (_startCallback != null) return;
        _startCallback = () => _currentAnimation = StartCoroutine(Animate());
        _startButton.clicked += _startCallback;
    }

    /// <summary>
    /// Disables UI events.
    /// </summary>
    void OnDisable() => _startButton.SetEnabled(false);

                                                                      #endregion
                                            ////////////////////////////////////
                                            #region Private Methods
                                            ////////////////////////////////////

    /// <summary>
    /// Slides the button off-screen over time.
    /// At the end, disables the button's GameObject.
    /// </summary>
    IEnumerator Animate() {
        var frameFrequency = 1f / framesPerSec;
        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        // Disable UI events during animation.
        _startButton.SetEnabled(false);

        // Play animation.
        AnimationStart?.Invoke();
        for (var i = 0; i <= frameCount; i++) {
            var t = (float) i / frameCount;
            _currentOffset = Vector2.Lerp(startOffset, endOffset, blendMode.ToFunc()(t));
            _startButton.style.left = new StyleLength(new Length(_currentOffset.x));
            _startButton.style.bottom = new StyleLength(new Length(_currentOffset.y));
            yield return new WaitForSeconds(frameFrequency);
        } AnimationEnd?.Invoke();

        // Cleanup finished animation.
        _currentAnimation = null;
        gameObject.SetActive(false);
    }

                                                                      #endregion
}
}
