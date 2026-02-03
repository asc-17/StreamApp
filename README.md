# StreamApp

A lightweight, personal media streaming server built with **ASP.NET Core MVC (NET 10)**. Designed for local network streaming of your Movie and TV Series collection with a clean, modern interface.

> **Note**: StreamApp uses **Direct Play** for maximum performance. It natively supports **MKV containers**, but relies on your browser for **video decoding**. Ensure your files use compatible codecs (like H.264) as no transcoding occurs.

## Features

### 🎬 Media Management
- **Automated Scanning**: Scans configured folders for Movies and TV Series at startup.
- **Smart Parsing**: Detects seasons and episodes from folder structures.
- **Metadata**: Parses file names for titles/years and displays `poster.jpg` if present.

### 📺 Playback & Streaming
- **Smart MKV Streaming**: Specialized server-side handling for `.mkv` containers. Uses HTTP Byte-Range requests to enable smooth seeking and playback of large MKV files that typically fail on standard web servers.
- **HTML5 Custom Player**: Polished UI with auto-hiding controls, play/pause, seek, and volume management.
- **Resume Playback**: Automatically saves your watch progress (per video) so you can resume where you left off.
- **Embedded Subtitles**: Extracts and streams embedded subtitles (SRT/VTT) from MKV files (requires FFmpeg).
- **Direct Streaming**: Zero-transcoding pipeline for maximum performance.

### 🛠 Technical Highlights
- **Framework**: ASP.NET Core 10 MVC
- **Database**: SQLite (for tracking watch progress)
- **Frontend**: Vanilla JavaScript (no heavy frameworks), Bootstrap 5
- **Backend**: C# Service Layer for media scanning and subtitle extraction

---

## Prerequisites

1.  **.NET 10.0 SDK** (Preview/RC depending on availability)
2.  **FFmpeg & FFprobe**: Required for subtitle extraction.
    *   Download from [ffmpeg.org](https://ffmpeg.org/download.html).
    *   Ensure `ffmpeg.exe` and `ffprobe.exe` are available.
3.  **Modern Browser**: Chrome, Edge, or Firefox (must support HTML5 Video).

---

## Configuration

The application is configured via `appsettings.json`. You must specify your media root directory and the paths to FFmpeg tools.

```json
{
  "MediaRootPath": "E:\\Media",
  "FFmpegPath": "C:\\path\\to\\ffmpeg.exe",
  "FFprobePath": "C:\\path\\to\\ffprobe.exe"
}
```

### Folder Structure Expectation
The scanner expects a specific structure to organize media correctly:

```
MediaRoot/
├── Movies/
│   └── Movie Name (2024)/
│       ├── Movie.mkv
│       └── poster.jpg
│
└── Series/
    └── Series Name/
        ├── poster.jpg
        └── Season 01/
            ├── Episode 01.mkv
            └── Episode 02.mkv
```

---

## Setup & Run

1.  **Clone the Repository**
    ```bash
    git clone https://github.com/asc-17/StreamApp.git
    cd StreamApp
    ```

2.  **Update Configuration**
    Modify `appsettings.json` with your actual paths.

3.  **Run the Application**
    ```bash
    dotnet run
    ```
    *The app will perform an initial scan of your media folders on startup.*

4.  **Access in Browser**
    Navigate to `https://localhost:7193` (or the port specified in console).

---

## Limitations

-   **Subtitle Loading Delay**: Due to real-time FFmpeg extraction, subtitles may take **20-30 seconds** to appear after starting a video. This is a loading time only; once loaded, subtitles are perfectly synced with the audio.
-   **No Transcoding**: Files with unsupported codecs (like heavy HEVC/H.265 profiles on some browsers) may fail to play or have no video. Use H.264/AAC for best compatibility.
-   **No Authentication**: The app is open. Intended for use on a trusted local network only.
-   **Subtitle Support**: Currently only supports **embedded MKV subtitles**. External `.srt` file support is under development.

## Future Enhancements
-   [ ] External `.srt` file support
-   [ ] OpenSubtitles integration
-   [ ] Multi-user support

## License
Personal Project. Source code provided as-is.
