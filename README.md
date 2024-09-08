# BKSMTool (Under Development)
BKSMTool is an open source C# GUI/CLI created by [Saltwatersam](https://github.com/Saltwatersam).

This GUI/CLI allows users to modify *.bnk files used by Wwise.

## License

This project is licensed under the [GNU General Public License v3.0 (GPLv3)](gpl-3.0.txt).

### Libraries Used

- **ObjectListView**: Licensed under GPLv3.
- **BASS audio library**: Proprietary library provided by Un4seen Developments. You must comply with the BASS licensing terms when using this library in commercial applications.
- **ManagedBass**: Licensed under MIT License.
- **Owner-drawn Trackbar/Slider**: Licensed under LGPLv3 by Chris Maunder from CodeProject. The version used in this project has been modified.
- **WEMSharp**: Licensed under GPLv3 by Crauzer the version used in this project has been modified.
- **RevorbStd**: Licensed under the MIT License by OverTools. This project includes a modified file from RevorbStd.


## Third-Party Libraries

This application uses the BASS audio library, which is a proprietary library provided by Un4seen Developments. You must comply with the BASS licensing terms when using this library in commercial applications. For more information, visit the [BASS website](http://www.un4seen.com/bass.html#license).

## Features

**GUI**
- [x] List all local audio files (.*wem) within *.bnk files by their ID
- [x] Read/Play local audio files (.*wem) inside *.bnk
- [x] Extract and save locally selected audio file (.*wem) in different formats:
  - WAV
  - MP3
  - OGG
  - WEM
- [x] (Quick) extract and save all audio files (.*wem) in different formats:
  - WAV
  - MP3
  - OGG
  - WEM
- [x] Replace selected audio file within *.bnk with a local audio file (.*wem, .*wav, *.mp3, *.ogg)
- [x] .txt file assignment to add event names corresponding to audio files ID (works for the game Trove)

**CLI**
- [x] Allow extraction through command line interface (CLI)

## Getting Started

The easiest way to use BKSMTool is to download the latest release from [here](https://github.com/Saltwatersam/BKSMTool/releases).

The GUI has been made to be simple to understand by users. Add file, select audio, options, etc.

## FAQ

**What is BKSMTool?**

BKSMTool is an open source C# GUI created by Saltwatersam. It provides the ability for users to modify *.bnk files mainly created by WWise.

**What does BKSMTool mean?**

BKSMTool stands for **B**an**k** **S**ound **M**odding **Tool**.

**How can I get help?**

You can raise an issue here on GitHub. This is the best option when you've written some code and want to ask why it's not working as you expect. I attempt to answer all questions, but since this is a spare time project, occasionally I get behind.

## Credits

**Many thanks to:**
- [XeNTaX](http://wiki.xentax.com/index.php/Wwise_SoundBank_(*.bnk)) (Game File Format Central) for providing structure of .*bnk files.
- [VGMStream](https://github.com/vgmstream) for providing excellent work with .*wem files format.
- [BASS audio library](https://www.un4seen.com/) for providing an audio library for .NET.
- [ManagedBass](https://github.com/ManagedBass/ManagedBass) for providing a managed wrapper for BASS audio library.
- [ObjectListView](https://objectlistview.sourceforge.net/cs/index.html) for providing an amazing control.
- [Chris Maunder](https://www.codeproject.com/Articles/17395/Owner-drawn-trackbar-slider) for providing a wonderful trackbar control.
- [Revorbstd](https://github.com/overtools/revorbstd) for providing a tool to restructure .ogg files packets.
- [Crauzer](https://github.com/Crauzer/WEMSharp) for providing WEMSharp library.
- [kmg design on freepik.com](https://www.freepik.com/author/kmgdesignid) for providing the icon for BKSMTool.]