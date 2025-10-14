//elevator
int cooldown = 0;
bool tick = false;

public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update100;}

public void Main(string args){

    //drone interaction
    string tag = "doorsystemV2";
    IMyBroadcastListener Listener = IGC.RegisterBroadcastListener(tag) as IMyBroadcastListener;
    if(Listener.HasPendingMessage)
    {
        MyIGCMessage message = Listener.AcceptMessage();
        double distance;

        distance = Vector3D.Distance(new Vector3D(1004531.85,182198.82,1650886.55), (Vector3D)message.Data); //GPS:Elevator call point down:1004536.69:182203.48:1650879.6:#FF75C9F1:
        if(distance < 10) args = "down";
        distance = Vector3D.Distance(new Vector3D(1004517.24,182203.49,1650922.61), (Vector3D)message.Data); //GPS:elewator call point up:1004517.24:182203.49:1650922.61:#FF75C9F1:
        if(distance < 10) args = "up";

        distance = Vector3D.Distance(new Vector3D(1004536.69,182203.48,1650879.6), (Vector3D)message.Data); //GPS:elevator sw down 1:1004531.85:182198.82:1650886.55:#FF75C9F1:
        if(distance < 10) args = "sw";
        distance = Vector3D.Distance(new Vector3D(1004531.4,182189.93,1650908.52), (Vector3D)message.Data); //GPS:elevator sw down 2:1004531.4:182189.93:1650908.52:#FF75C9F1:
        if(distance < 10) args = "sw";
        distance = Vector3D.Distance(new Vector3D(1004521.78,182208.49,1650915.88), (Vector3D)message.Data); //GPS:elevator sw up 1:1004521.78:182208.49:1650915.88:#FF75C9F1:
        if(distance < 10) args = "sw";
        distance = Vector3D.Distance(new Vector3D(1004522.35,182217.31,1650893.74), (Vector3D)message.Data); //GPS:elevator sw up 2:1004522.35:182217.31:1650893.74:#FF75C9F1:
        if(distance < 10) args = "sw";   
    }

    //input coordinator
    if(args==""){
        cooldown++;
        tick = !tick;
    }
    if((args == "up" || args == "down" || args == "sw")&Me.CustomData=="Waiting"){
        if(args == "up" & pStat() != PistonStatus.Extended){
            Me.CustomData = "Going up";
            cooldown = 0;
        }
        if(args == "down" & pStat() != PistonStatus.Retracted){
            Me.CustomData = "Going down";
            cooldown = 0;
        }
        if(args == "sw"){
            if(getPos() < 0.0001){
                Me.CustomData = "Going up";
                cooldown = 0;
            }
            if(getPos() > 0.999){
                Me.CustomData = "Going down";
                cooldown = 0;
            }
        }
    }
    bool cdWait = false;

    //elevator controller
    if(Me.CustomData == "Going up"){
        if(pStat() == PistonStatus.Extended){
            if(switchDoor("Лифт двери верх",DoorStatus.Open)){
                cdWait = true;
                if(cooldown > 30) Me.CustomData = "Waiting";
            };
        }else{
            if(switchDoor("Лифт двери верх",DoorStatus.Closed) & switchDoor("Лифт двери низ",DoorStatus.Closed)){
                moveUp();
                pressurize(false);
            }
        }
        
    }
    if(Me.CustomData == "Going down"){
        if(pStat() == PistonStatus.Retracted){
            if(switchDoor("Лифт двери низ",DoorStatus.Open)){
                cdWait = true;
                if(cooldown > 30) Me.CustomData = "Waiting";
            };
        }else{
            if(switchDoor("Лифт двери верх",DoorStatus.Closed) & switchDoor("Лифт двери низ",DoorStatus.Closed)){
                moveDown();
                pressurize(true);
            }
        }
        
    }

    //display controller
    string dispPos = "XXXX";
    string toDisplay = "ERROR";
    if(Me.CustomData == "Waiting"){
        if(pStat() == PistonStatus.Extended){
            toDisplay = "0====0" + System.Environment.NewLine + "|    |" + System.Environment.NewLine +  "|    |";
        }
        if(pStat() == PistonStatus.Retracted){
            toDisplay = "|    |" + System.Environment.NewLine + "|    |" + System.Environment.NewLine +  "0====0";
        }
        if(pStat() != PistonStatus.Retracted & pStat() != PistonStatus.Extended){
            toDisplay = "|    |" + System.Environment.NewLine + "X    X" + System.Environment.NewLine +  "V    V";
        }
    }
    if(Me.CustomData == "Going down"){
        double where = ((1-doorRat("Лифт двери верх")/14)+(1-getPos())+(doorRat("Лифт двери низ")/14))*33.33;
        dispPos = where.ToString("000") + "%";
        if(tick){
            toDisplay = "|    |" + System.Environment.NewLine + "V" + dispPos + "V" + System.Environment.NewLine +  "      ";
        }
        else{
            toDisplay = "      " + System.Environment.NewLine + "|" + dispPos + "|" + System.Environment.NewLine +  "V    V";
        }
    }
    if(Me.CustomData == "Going up"){
        double where = ((doorRat("Лифт двери верх")/14)+getPos()+(1-doorRat("Лифт двери низ")/14))*33.33;
        dispPos = where.ToString("000") + "%";
        if(tick){
            toDisplay = "^    ^" + System.Environment.NewLine + "|" + dispPos + "|" + System.Environment.NewLine +  "      ";
        }
        else{
            toDisplay = "      " + System.Environment.NewLine + "^" + dispPos + "^" + System.Environment.NewLine +  "|    |";
        }
    }
    if(cdWait){
        if(Me.CustomData == "Going down")
            toDisplay = "|    |" + System.Environment.NewLine + "|WAIT|" + System.Environment.NewLine +  "0====0";
        if(Me.CustomData == "Going up")
            toDisplay = "0====0" + System.Environment.NewLine + "|WAIT|" + System.Environment.NewLine +  "|    |";
   
    }

    IMyTextSurfaceProvider display1 = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Лифт дисплей 1");
    if(display1!=null)
    display1.GetSurface(0).WriteText(toDisplay, false);
    IMyTextSurfaceProvider display2 = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Лифт дисплей 2");
    if(display2!=null)
    display2.GetSurface(0).WriteText(toDisplay, false);
    IMyTextSurfaceProvider display3 = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Лифт дисплей 3");
    if(display3!=null)
    display3.GetSurface(0).WriteText(toDisplay, false);
    IMyTextSurfaceProvider display4 = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Лифт дисплей 4");
    if(display4!=null)
    display4.GetSurface(0).WriteText(toDisplay, false);
}

public bool switchDoor(string name,DoorStatus newStatus){
    IMyBlockGroup doors_g = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName(name);
    List<IMyTerminalBlock> doors_l = new List<IMyTerminalBlock>();
    doors_g.GetBlocks(doors_l);
    bool done = true;
    foreach(var door in doors_l){
        var dors = door as IMyDoor;
        if(newStatus == DoorStatus.Closed){
            dors.CloseDoor();
            if(dors.Status != DoorStatus.Closed) done = false;
        }
        if(newStatus == DoorStatus.Open){
            dors.OpenDoor();
            if(dors.Status != DoorStatus.Open) done = false;
        }
    }
    return done;
}

public float doorRat(string name){ //0(clsed)-door amount(open)
    IMyBlockGroup doors_g = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName(name);
    List<IMyTerminalBlock> doors_l = new List<IMyTerminalBlock>();
    doors_g.GetBlocks(doors_l);
    float sum = 0f;
    foreach(var door in doors_l){
        var dors = door as IMyDoor;
        sum += dors.OpenRatio;
        }
    return sum;
}

public double getPos(){ // 0-44.1f -> 0-0.99999996539957
    IMyBlockGroup doors_g = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Лифт поршни");
    List<IMyTerminalBlock> doors_l = new List<IMyTerminalBlock>();
    doors_g.GetBlocks(doors_l);
    float sum = 0f;
    foreach(var door in doors_l){
        var pist = door as IMyPistonBase;
            sum += pist.CurrentPosition;
    }
    return sum/44.1;
}

public PistonStatus pStat(){
    bool allOk = true;
    IMyBlockGroup doors_g = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Лифт поршни");
    List<IMyTerminalBlock> doors_l = new List<IMyTerminalBlock>();
    doors_g.GetBlocks(doors_l);
    var pistT = doors_l[0] as IMyPistonBase;
    PistonStatus first = pistT.Status;
    foreach(var door in doors_l){
        var pist = door as IMyPistonBase;
        if(pist.Status != first) allOk = false;
    }
    if(allOk) return first;
    float sum = 0f;
    foreach(var door in doors_l){
        var pist = door as IMyPistonBase;
        sum += pist.Velocity;
    }
    if(sum >0)return PistonStatus.Extending;
    if(sum <0)return PistonStatus.Retracting;
    return PistonStatus.Stopped;
}

public void moveUp(){
    IMyBlockGroup doors_g = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Лифт поршни");
    List<IMyTerminalBlock> doors_l = new List<IMyTerminalBlock>();
    doors_g.GetBlocks(doors_l);
    var pistT = doors_l[0] as IMyPistonBase;
    PistonStatus first = pistT.Status;
    foreach(var door in doors_l){
        var pist = door as IMyPistonBase;
        pist.Extend();
    }
}
public void moveDown(){
    IMyBlockGroup doors_g = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Лифт поршни");
    List<IMyTerminalBlock> doors_l = new List<IMyTerminalBlock>();
    doors_g.GetBlocks(doors_l);
    var pistT = doors_l[0] as IMyPistonBase;
    PistonStatus first = pistT.Status;
    foreach(var door in doors_l){
        var pist = door as IMyPistonBase;
        pist.Retract();
    }
}

public void pressurize(bool press){
    IMyBlockGroup doors_g = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Лифт вентиляция");
    List<IMyTerminalBlock> doors_l = new List<IMyTerminalBlock>();
    doors_g.GetBlocks(doors_l);
    foreach(var door in doors_l){
        var vent = door as IMyAirVent;
        vent.Depressurize = !press;
    }
}