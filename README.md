# StreamApp

A personal media streaming application built with ASP.NET Core MVC (.NET 10) that provides a Netflix-like interface for browsing and watching your local movie and TV series collection.

## Features

### 🎬 Media Management
- **Automatic Media Scanning**: Scans your local media folders and organizes movies and TV series automatically
- **Smart Organization**: Parses folder structures to detect series names, seasons, and episodes
- **Poster Support**: Displays custom poster images for movies and series

### 📺 Video Player
- **Custom HTML5 Video Player**: Netflix-style player with modern UI
- **Streaming Support**: HTTP range request support for smooth seeking and playback
- **Format Support**: MKV, MP4, and other browser-compatible formats
- **Responsive Controls**: Play/pause, seek, fullscreen, volume control
- **Auto-hide UI**: Controls automatically hide during playback

### 📝 Subtitle Support
- **Local Subtitle Loading**: Automatically loads subtitles from a `Subtitles` folder
- **Format Support**: SRT and VTT subtitle formats
- **Auto-matching**: Subtitles are matched by filename to the corresponding video

### ⌨️ Keyboard Shortcuts
- `Space` / `K` - Play/Pause
- `F` - Toggle Fullscreen
- `M` - Mute/Unmute
- `←` - Seek backward 10 seconds
- `→` - Seek forward 10 seconds

## Folder Structure

Your media files should be organized as follows:

```
E:\Media\
├── Movies\
│   └── Movie Name\
│       ├── Movie Name.mkv
│       ├── poster.jpg
│       └── Subtitles\
│           └── Movie Name.srt
│
└── Series\
    └── Series Name\
        ├── poster.jpg
        ├── Season 1\
        │   ├── Episode 01.mkv
        │   ├── Episode 02.mkv
        │   └── Subtitles\
        │       ├── Episode 01.srt
        │       └── Episode 02.srt
        └── Season 2\
            └── ...
```

### Requirements:
- **Movies**: Each movie in its own folder with format `Movie Name`
- **Series**: Organized by series → season → episodes
- **Subtitles**: Place in a `Subtitles` subfolder with the same name as the video file
- **Posters**: Optional `poster.jpg` in the movie/series root folder

## Configuration

Edit `appsettings.json` to set your media root path:

```json
{
  "MediaRootPath": "E:\\Media"
}
```

The application will automatically scan:
- `{MediaRootPath}\Movies` for movies
- `{MediaRootPath}\Series` for TV shows

## Getting Started

### Prerequisites
- .NET 10 SDK
- Web browser with HTML5 video support (Chrome, Edge, Firefox)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/asc-17/StreamApp.git
cd StreamApp
```

2. Configure your media path in `appsettings.json`

3. Run the application:
```bash
dotnet run
```

4. Open your browser and navigate to `https://localhost:7193`

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 10)
- **Frontend**: Razor Views, HTML5, CSS, Vanilla JavaScript
- **Styling**: Bootstrap 5 + Custom CSS
- **Video**: HTML5 `<video>` element with custom controls
- **Architecture**: MVC pattern with service layer

## Project Structure

```
StreamApp/
├── Controllers/
│   ├── HomeController.cs      # Landing page
│   ├── SeriesController.cs    # TV series browsing
│   └── VideoController.cs     # Video streaming & playback
├── Services/
│   └── MediaScannerService.cs # Media folder scanning
├── Models/
│   └── MediaModels.cs         # Movie, Series, Episode models
├── Views/
│   ├── Home/
│   │   └── Index.cshtml       # Main landing page
│   ├── Series/
│   │   └── Details.cshtml     # Series details & episodes
│   └── Video/
│       └── Play.cshtml        # Video player page
└── wwwroot/                   # Static assets
```

## Features in Detail

### Video Streaming
- Uses ASP.NET Core's `FileStreamResult` with range processing
- Enables seeking in large video files without loading the entire file
- Supports multiple concurrent streams

### Media Scanning
- Automatically runs on application startup
- Regex-based parsing for series names, season numbers, and episode numbers
- Supports various naming conventions (S01E01, Episode 01, etc.)

### User Interface
- Clean, modern design inspired by streaming platforms
- Grid layout for media browsing
- Responsive design (works on desktop and tablets)
- Smooth transitions and hover effects

## Known Limitations

- **Browser Codec Support**: Some MKV files may not play if they use codecs not supported by the browser (e.g., HEVC/H.265)
  - Solution: Convert videos to H.264/AAC for maximum compatibility
- **Security**: Designed for local/private network use only
- **Authentication**: No user authentication (suitable for single-user/trusted network scenarios)

## Future Enhancements

Potential features for future development:
- [ ] Watch progress tracking
- [ ] Recently watched section
- [ ] Search functionality
- [ ] Multiple subtitle language support
- [ ] Mobile responsive design improvements
- [ ] Thumbnail preview on seek
- [ ] Episode auto-play (next episode)

## License

This is a personal project for local media streaming. Use at your own discretion.

## Contributing

This is a personal project, but suggestions and improvements are welcome via issues or pull requests.

---

**Note**: This application is designed for streaming your personal media collection on a local network. Ensure you have the legal right to possess and stream any media files you use with this application.


