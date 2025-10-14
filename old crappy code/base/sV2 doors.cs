//remote door opener with distance
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update100;}

public void Main(string args)
{

    string tag = "doorsystemV2";
    IMyBroadcastListener Listener = IGC.RegisterBroadcastListener(tag) as IMyBroadcastListener;
    







    if(Listener.HasPendingMessage)
    {
        MyIGCMessage message = Listener.AcceptMessage();
        double distance;

        //door import
        checkDoor(message,"Ангарные двери 0",10,1004617.26,182205.82,1650976.75);
        checkDoor(message,"Крыша",10,1004606.08,182218.81,1650982.82);
        checkDoor(message,"Ангарные двери 1",10,1004598.6,182217.46,1650938.03);
        checkDoor(message,"DronePortDoor",8,1004625.82,182206.1,1650987.4);
        checkDoor(message,"Ангарные двери склад1",10,1004645.66,182226.52,1650971.08);

        //GPS:DoorPos1:1004598.6:182217.46:1650938.03:#FF75C9F1:
        //GPS:DoorPosDronePort:1004625.82:182206.1:1650987.4:#FF75C9F1:
        //Ангарные двери склад1
        //GPS:DoorPosStorage1:1004645.66:182226.52:1650971.08:#FF75C9F1:

        //custom
        IMyBlockGroup welds_g0 = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Принтер сварщики");
        List<IMyTerminalBlock> welds_l0 = new List<IMyTerminalBlock>();
        welds_g0.GetBlocks(welds_l0);
        distance = Vector3D.Distance(new Vector3D(1004636.38,182234.51,1650942.2), (Vector3D)message.Data);
        if(distance < 15) foreach(var door in welds_l0){
            var dors = door as IMyShipToolBase;
            if(dors.Enabled)
            dors.Enabled = false;
            else
            dors.Enabled = true;
        }
    }
}

public bool checkDoor(MyIGCMessage message,string name,int rad,double x,double y,double z){
    IMyBlockGroup doors_g = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName(name);
    List<IMyTerminalBlock> doors_l = new List<IMyTerminalBlock>();
    doors_g.GetBlocks(doors_l);
    double distance = Vector3D.Distance(new Vector3D(x,y,z), (Vector3D)message.Data);
    if(distance < rad) foreach(var door in doors_l){
        var dors = door as IMyDoor;
        if(dors.Status == DoorStatus.Closed || dors.Status == DoorStatus.Closing)
        dors.OpenDoor();
        else
        dors.CloseDoor();
    }
    else return false;
    return true;
}
// GPS:DoorPosRoof:1004606.08:182218.81:1650982.82:#FF75C9F1:
// GPS:WeldPosActivator0:1004636.38:182234.51:1650942.2:#FF75C9F1: