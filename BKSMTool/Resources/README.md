# BKSMTool (Under developpment, not ready yet)
BKSMTool is an open source C# GUI created by [Saltwatersam](https://github.com/Saltwatersam)

This GUI allow user to modify *.bnk files used by Wwise.

# Features
- [x] List all local audio files (.*wem) within *.bnk files by their ID
- [ ] Read/Play local audio files (.*wem) inside *.bnk
- [ ] extract and save localy selected audio file (.*wem)
  - WAV
  - MP3
  - OGG
  - WEM
- [ ] (quick) extract and save all audio files (.*wem)
  - WAV
  - MP3
  - OGG
  - WEM
- [ ] Replace selected audio file within *.bnk
- [x] .txt File assignment to add event names corresponding to audio files ID (works for the game Trove
- [ ] more parameters can be modified inside *.bnk files. Might add options for that in future.

# Getting Started
The easiest way to use BKSMTool is to download the latest release from [here](https://github.com/Saltwatersam/BKSMTool/releases)

The GUI has been made to be simple to understand by users. Add file, select audio, options,  etc...

# FAQ
**What is BKSMTool?**

BKSMTool is an open source C# GUI created by Saltwatersam. It provides ability for user to modify .*bnk files mainly created by WWise.

**What does BKSMTool mean?**

BKSMTool stand for <ins>B</ins>an<ins>k</ins> <ins>S</ins>ound <ins>M</ins>odding <ins>Tool</ins>.

**How can I get help?**

You can raise an issue here on GitHub. This is the best option when you've written some code and want to ask why it's not working as you expect. I attempt to answer all questions, but since this is a spare time project, occasionally I get behind.

**How do I submit a patch?**

I welcome contributions to BKSMTool, but if you want your code to be included, please familiarise yourself with the following guidelines:

- Your submission must be your own work, and able to be released under the MIT license.
- You will need to make sure your code conforms to the layout and naming conventions used elsewhere in BKSMTool.
- Remember that there are existing users of BKSMTool. A patch that changes the public interface is not likely to be accepted.
- Try to write "clean code" - avoid long functions and long classes. Try to add a new feature by creating a new class rather than putting loads of extra code inside an existing one.
- I don't usually accept contributions I can't test.
- If you are adding a new feature, please consider writing a short tutorial on how to use it.
- Unless your patch is a small bugfix, I will code review it and give you feedback. You will need to be willing to make the recommended changes before it can be integrated into the main code.
- Patches should be provided using the Pull Request feature of GitHub.
- Please also bear in mind that when you add a feature to BKSMTool, that feature will generate future support requests and bug reports. Are you willing to stick around on the forums and help out people using it?

# Credits

**Many thanks to:**
- [XeNTaX](http://wiki.xentax.com/index.php/Wwise_SoundBank_(*.bnk)) (Game File Format Central) for providing structure of .*bnk files.
- [VGMStream](https://github.com/vgmstream) for providing excellent work with .*wem files format.
- [NAudio](https://github.com/naudio) for providing an audio and MIDI library for .NET