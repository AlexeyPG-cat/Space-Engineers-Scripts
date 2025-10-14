//

int tick = 0;
long sender = 0L;
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update100;
IMyTextSurfaceProvider disp = (IMyTextSurfaceProvider)Me;
disp.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
disp.GetSurface(0).FontSize = 1.5f;
disp.GetSurface(0).TextPadding = 30f;
disp.GetSurface(0).Alignment = TextAlignment.CENTER;
disp.GetSurface(0).WriteText("Starting");
Me.CustomData = IGC.Me + "";
Me.CustomName = "Emergency Control System";
}

public void Main(string args)
{
    IMyTextSurfaceProvider disp = (IMyTextSurfaceProvider)Me;
    disp.GetSurface(0).WriteText("Online: " + tick); tick++;
    IMyUnicastListener Listener = IGC.UnicastListener as IMyUnicastListener;
    if(Listener.HasPendingMessage)
    {
        try{
            MyIGCMessage message = Listener.AcceptMessage();
            sender = message.Source;
            ImmutableArray<string> arr = (ImmutableArray<string>)message.Data;
            if(arr[0]=="EmergencyAction"){
                if(arr[1]=="Preset1"){
                    presetAction(arr);
                }
            }
        } catch {
            disp.GetSurface(0).WriteText("Message unknown!");
        }
    }
}
public void presetAction(ImmutableArray<string> act){
    IMyTextSurfaceProvider disp = (IMyTextSurfaceProvider)Me;
    disp.GetSurface(0).WriteText(act[2]);
    switch(act[2]){
        case "MaxAntennas":
            Echo("Maxing Antennas");
            break;
        case "MaxBeacons":
            Echo("Maxing Beacons");
            break;
        case "ActivateAllPowerSources":
            Echo("Activating power");
            break;
        case "ActivateBattaries":
            Echo("Activating all batteries");
            break;
        case "ShutDown":
            Echo("Shutting Down");
            break;
        case "WeaponsON":
            Echo("Starting weapons");
            break;
        case "WeaponsOFF":
            Echo("Stopping weapons");
            break;
        case "WeaponsMax":
            Echo("Maxing Weapons destruction");
            break;
        case "OpenTanks":
            Echo("Opening all tanks");
            break;
        case "EnableDampeners":
            Echo("Enabling dampeners");
            break;
        case "SendGPS":
            Echo("Sending gps");
            break;
        case "ListenOnly":
            Echo("Hiding");
            break;
    }
}