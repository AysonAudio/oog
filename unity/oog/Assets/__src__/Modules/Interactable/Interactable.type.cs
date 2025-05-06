using System;
namespace Oog.Modules.Interactable {

[Flags] public enum HoverInteraction { None = 0, Point = 1 }
[Flags] public enum DragInteraction { None = 0, Point = 1, Click = 2, RightClick = 4, MiddleClick = 8 }
[Flags] public enum DragTrigger { None = 0, Click = 2, RightClick = 4, MiddleClick = 8 }
[Flags] public enum DragMode { None = 0, Hold = 1, Toggle = 2 }
}
