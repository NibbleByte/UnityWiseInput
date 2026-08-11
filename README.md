# Wise Input
A Unity Input System wrapper that handles UI hotkeys and resolves hotkey conflicts in the prefab hierarchy, instead of in code.

## Motivation
With the Unity Input System, enabling a few InputActions is simple enough. But once you add on-screen hotkey hints, multiple UI states, and system pop-ups, doing it all in code quickly gets messy. When a system pop-up opens you need to disable the character input actions, but which ones are the right ones? This creates nasty code dependencies. And while that pop-up is shown, any hotkey hints in the background should hide themselves to avoid confusing the player.

The correct input behaviour is easy to sum up: **only the focused window on screen should receive input**.

Managing this in code is hard because it's a visual problem: **UI is built with the visual editor in the Unity hierarchy, and so should its input be**.

## How to use
To understand the whole system, open the **`UIScopesUsage.unity`** sample scene, which has plenty of examples showcasing usage. This document covers the main concepts at a high level.

The system introduces these core concepts:
* Input Context - the core of the system that ties everything together. Your code enables input actions through it.
* Input Actions Mask Stack - a stack of masks that filters which input actions are currently enabled.
* UI Scope + Scope Elements - a component that enables its child scope elements when focused.
* Selection Controller - manages the selected interactable and can be driven by a UI Scope.
* UI Navigation Group - sets up navigation links between all child interactables.

### Input Context
The input context is represented by the `IInputContext` interface, which your game initializes and uses. It provides a simple API for input operations and connects the system to your custom `IInputActionCollection` controls. Wise Input uses it to listen for device changes and update on-screen hotkey hints. Enabling or disabling input actions should always go through this interface so they can be managed properly. Enabled actions are stored in the `Input Actions Mask Stack`, which updates whenever a UI Scope that wants to consume input is focused.

You can have multiple `IInputContext` instances at once, which is useful for split-screen gameplay.

### UI Input Scopes
`UIScope` is a component that enables or disables its child `IScopeElement` components depending on whether it's active. You can nest scopes, and one of them can be focused, making all its parents active while other inactive scopes disable their elements. An enabled `IScopeElement` typically enables specific hotkeys or other objects.

`UIScope` helps you resolve input conflicts and manage hotkeys in the prefab hierarchy, instead of in code.

You can easily debug UI Scopes in the scene with the UIScopes Debugger.

### Selection Controller
This component manages the current selection in the UI. Only one should be active at any time. You can define how the selection behaves when it's lost or inactive. It relies on the existing navigation links of Unity's `Selectable` class - to set those links up quickly, use the `UI Navigation Group`.

### UI Navigation Group
This component gathers all child `Selectable` components and generates proper navigation links between them. If a `Selectable` is added or removed, the links are re-evaluated. You can define what happens when navigation moves outside the group's boundaries - wrap, call a method, jump to another group, and so on.
