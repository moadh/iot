# C# Azure IOT starter
<b>Simple Azure IOT example with device and backend application</b>

This solution contains 2 separate console projects. There is 1 device console project that will act as an IOT device and 1 console application which will make direct method calls to the device application to simulate the distributed communication. 

This solution uses the Azure IOT direct methods where the caller can invoke direct methods within the device applications. This allows for communciation between the hub and the devices on Azure IOT hub.



<b>Instructions</b> (expectation is that you will be loading solution onto Raspberry Pi after testing on dev machine)
1. Ensure you have .net core 2.0 installed on the machine where you install application
2. Download or clone this .net core project to your machine
3. Update the files with supplied connection strings, hub uri (make sure to not prepend with http:// https://), and Device ID from provided Hackathon instructions.  
4. Set your startup projects to both console apps in VS by right clicking on the solution within VS and choosing "Set Startup Projects". Select "Multiple start up projects" and ensure both Device and IOT are set to "Start".
5. Add your custom code
6. When ready to deploy to the RPi, create a 'publish' folder by executing this command from the console: <code>dotnet publish -r linux-arm</code>
7. Using file transfer software that supports SFTP, SCP, SSH, etc. (e.g. WinSCP, filezilla, etc.), copy the entire 'publish' folder (...Device\bin\Debug\netcoreapp2.0\linux-arm\publish ) to the pi, this publish folder might be in a different place based on your version of VS
8. On the RPi, ensure the executable you copied over has executable permissions (e.g. <code>chmod +x ./Device</code>)
9. From the command line, start the application (e.g.  "./Device")

<b>Note</b>: It is expected that the IOT backend application, the non device application, will run from a laptop computer on a developers machine. The devices apps are intended to run on the Raspberry Pi's.

