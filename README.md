# Audio's Amigo
![image](https://cloud.githubusercontent.com/assets/7611406/16860139/bff3dfa8-4a0f-11e6-835b-6098c684277a.png)
Say Adiós Amigo to Windows volume mixer and say hello to Audio's Amigo.


Audio's Amigo provides a mobile interface for controlling per-application volume for every sound device connected to your Windows system.
The Windows application interfaces with the [NAudio](https://github.com/naudio/NAudio) Windows API wrappers, to control and monitor sound devices or processes. The Android application is written in C# using [Xamarin](https://www.xamarin.com/) libraries. Messages are sent back and forth between the Windows and Android application using a JSON format, making this interface easily extendable!

---------------
### Overview

This solution contains four sub-projects:
 - **AudiosAmigo** A shared project which all projects in the solution need to reference.
 - **AudiosAmigo.Generate** Used to generate the main application interface used by both the Windows and Android applications. These files are linked in the `AudiosAmigo.Droid/Links` and `AudiosAmigo.Windows/Links` directories.
 - **AudiosAmigo.Windows** Contains the desktop application which controls the audio via Windows API
 - **AudiosAmigo.Droid** Contains the android application which provides a user interface to the system.
