# C# Azure IOT starter
<b>Simple Azure IOT example with device and backend application</b>

This solution contains 2 separate console projects. There is 1 device console project that will act as an IOT device and 1 console application which will make direct method calls to the device application to simulate the distributed communication. 

This solution uses the Azure IOT direct methods where the caller can invoke direct methods within the device applications. This allows for communciation between the hub and the devices on Azure IOT hub.



<b>Instructions</b> (expectation is that you will be loading solution onto Raspberry Pi after testing on dev machine)
1. Ensure you have .net core 2.0 installed on the machine where you install application
2. Download or clone this .net core project to your machine
3. Update the files with supplied connection strings, hub uri (make sure to not prepend with http:// https://), and Device ID from provided Hackathon instructions 
4. Add your custom code
5. When ready to deploy to the RPi, create a 'publish' folder by executing this command from the console: <code>dotnet publish -r linux-arm</code>
6. Using file transfer software that supports SFTP, SCP, SSH, etc. (e.g. WinSCP, filezilla, etc.), copy the entire 'publish' folder (...Device\bin\Debug\netcoreapp2.0\linux-arm\publish ) to the pi, this publish folder might be in a different place based on your version of VS
7. On the RPi, ensure the executable you copied over has executable permissions (e.g. <code>chmod +x ./Device</code>)
8. From the command line, start the application (e.g.  "./Device")

<b>Note</b>: it is expected that some teams will also want to run their backend application (Iot project) on a pi as well and this is not an issue. Just repeat steps 5-8 for the other project on another device. Other teams may wish to use their laptop and this is fine as well.
