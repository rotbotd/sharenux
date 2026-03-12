# ShareNux

Cross-platform fork of [ShareX](https://github.com/ShareX/ShareX) v18.0.1. Replaces WinForms with Avalonia UI and GDI+ with SkiaSharp for Linux and macOS support.

## Status

**Early development.** Not usable yet.

We're incrementally porting ShareX's functionality to cross-platform libraries:

| Component | Original | Target | Status |
|-----------|----------|--------|--------|
| UI Framework | WinForms | Avalonia | 🔴 Not started |
| Graphics | GDI+ (System.Drawing) | SkiaSharp | 🔴 Not started |
| Screen Capture | Windows APIs | PipeWire/Wayland (Linux), CGDisplay (macOS) | 🔴 Not started |
| Hotkeys | Windows hooks | Avalonia + platform-specific | 🔴 Not started |
| Clipboard | Windows clipboard | Avalonia clipboard | 🔴 Not started |
| Config Storage | Windows Registry | JSON files | 🔴 Not started |
| Shell Integration | Windows shell | XDG (Linux), LaunchServices (macOS) | 🔴 Not started |
| OCR | Windows OCR API | Tesseract | 🔴 Not started |
| Screen Recording | FFmpeg | FFmpeg (already cross-platform) | 🟡 Needs testing |

## Building

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

## Dependency Graph

```
HelpersLib (base - DONE ✓)
    ↓
    ├── HistoryLib
    ├── UploadersLib  
    ├── MediaLib
    ├── IndexerLib
    ├── ImageEffectsLib
    │       ↓
    └── ScreenCaptureLib (depends on HelpersLib, ImageEffectsLib, MediaLib)
            ↓
        ShareX (main app - depends on everything)
```

## Contributing

This is a big project. If you want to help:

1. Pick a component from the table above
2. Create an interface that abstracts the Windows-specific implementation
3. Implement the interface for Linux/macOS
4. Add integration tests

## License

GPL-3.0 (same as ShareX)

## Acknowledgments

ShareX is an incredible piece of software. This fork exists because Linux users deserve nice things too.
