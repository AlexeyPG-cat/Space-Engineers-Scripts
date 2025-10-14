float velocity = 0f;
string state = "off";


IMyPistonBase piston;
IMyShipMergeBlock mergeTop;
IMyShipMergeBlock mergeDown;
IMyShipConnector connector;

public Program(){
	if(!init()) return;
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    
}

public void Main(string args)
{
    if(piston == null || mergeTop == null || mergeDown == null) {Echo("Im dead"); return;}
    if(args == "goDown") {velocity = 0.2f;
	}
    if(args == "stop") state = "stop";
    if(args == "connect") state = "connect";
    
    if(velocity > 0) goDown();
    updateConnector();
    display();
    Echo("exec - ok");
}

public bool init(){
    piston = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("SB2 - Piston");
    mergeTop = (IMyShipMergeBlock)GridTerminalSystem.GetBlockWithName("SB2 - Merge Top");
    mergeDown = (IMyShipMergeBlock)GridTerminalSystem.GetBlockWithName("SB2 - Merge Bottom");
    connector = (IMyShipConnector)GridTerminalSystem.GetBlockWithName("SB2 - Connector");
    bool flag1 = piston == null;
    bool flag2 = mergeTop == null;
    bool flag3 = mergeDown == null;
    if(flag1) Echo("Piston not found!");
    if(flag2) Echo("Top merge not found!");
    if(flag3) Echo("Bottom merge not found!");
    return !(flag1 || flag2 || flag3);
}

public void goDown(){
    switch(state){
        case "off":
            state = "setup";
            break;
        case "setup":
            if(piston.CurrentPosition < 0.5){
               if(connectAll()) state = "decending";
            } else {
                if(!allConnected()) piston.Velocity = -piston.MaxVelocity;
                else mergeTop.Enabled = false;
            }
            break;
        case "decending":
            mergeDown.Enabled = (piston.CurrentPosition > 5);
            piston.Velocity = velocity;
            if(allConnected() & piston.CurrentPosition > 9) state = "returning";
            break;
        case "returning":
            mergeTop.Enabled = (piston.CurrentPosition < 1);
            piston.Velocity = -piston.MaxVelocity;
            if(allConnected() & piston.CurrentPosition < 1) state = "decending";
            break;
        case "stop":
            connectAll();
            piston.Velocity = 0;
            break;
        case "connect":
            piston.Velocity = -piston.MaxVelocity;
            if(piston.CurrentPosition <5) connectAll();
            break;
        default: 
            Echo("Unknown State!");
            break;
    }
}

public bool connectAll(){
    Echo((mergeTop.IsConnected & mergeDown.IsConnected) + ".");
    if(mergeTop.IsConnected & mergeDown.IsConnected) return true;
    else{mergeTop.Enabled = true; mergeDown.Enabled = true; return false;}
}
public bool allConnected(){
    return mergeTop.IsConnected & mergeDown.IsConnected;
}

public void display(){
    IMyTextSurfaceProvider disp = (IMyTextSurfaceProvider)Me;
    disp.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    disp.GetSurface(0).FontSize = 1.5f;
    disp.GetSurface(0).TextPadding = 30f;
    disp.GetSurface(0).Alignment = TextAlignment.CENTER;
    disp.GetSurface(0).WriteText(state);
}

public void updateConnector(){
    if(connector != null){
        if(mergeTop.IsConnected) connector.Connect();
        else connector.Disconnect();
    }
}