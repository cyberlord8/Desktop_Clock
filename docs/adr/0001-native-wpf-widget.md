# ADR 0001: Use a native WPF application for the Windows widget

## Status

Accepted for the first production slice.

## Decision

Build the desktop clock as a native .NET WPF application targeting Windows. Keep timezone and clock-reading logic in a framework-independent core module, while the WPF layer owns window behavior and rendering.

## Context

The primary product is a very small draggable widget that can stay always on top. Native window behavior is central to the experience, and the project is being developed on Windows with the .NET Windows Desktop runtime already available.

## Consequences

- The widget can use native borderless-window, dragging, and topmost behavior without embedding a browser runtime.
- The clock-reading module can be tested independently of WPF.
- A future screen-saver mode can reuse the core clock model and reading formatter while using a separate full-screen host.
- The first implementation is Windows-focused; cross-platform support would require a later adapter decision.
