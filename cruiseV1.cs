List<IMyTerminalBlock> thrsBack;
List<IMyTerminalBlock> thrsForw;
float thrustOverrideScale = 0f;
float reqSpeed = 0;
int speedFailureCount = 0;
float prevSpeed;
bool autoPilot = false;

public Program(){
    Echo("start");
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    scan();
}

public void Main(string args){
    handleArgs(args);
    if(autoPilot){
        cruise();
        failCheck();
    }
    Echo("AP = " + autoPilot);
}

public void handleArgs(string args){
    //setForce <number>
    //setSpeed <number>
    //start
    //stop
    if(args.StartsWith("setForce ")){
        thrustOverrideScale = float.Parse(args.Substring("setForce ".Length));
        Echo("Force set to " + thrustOverrideScale);
    }
    if(args.StartsWith("setSpeed ")){
        reqSpeed = float.Parse(args.Substring("setSpeed ".Length));
        Echo("Speed set to " + reqSpeed);
    }
    if(args == "start"){
        autoPilot = true;
        speedFailureCount = 0;
        Echo("Starting");
    }
    if(args == "stop"){
        stop("User input");
        Echo("Stop by user");
    }
}

public void stop(string reason){
    autoPilot = false;
    foreach(IMyTerminalBlock thr in thrsForw){
        IMyThrust thrust = thr as IMyThrust;
        thrust.ThrustOverride = 0f;
    }
    foreach(IMyTerminalBlock thr in thrsBack){
        IMyThrust thrust = thr as IMyThrust;
        thrust.ThrustOverride = 0f;
    }
}

public void cruise(){
    float currentSpeed = getSpeed();
    float dir = 0;
    if(currentSpeed+currentSpeed*0.05f > reqSpeed){
        dir = -1f;
    } else if(currentSpeed > reqSpeed){
        dir = -0.3f;
    } else if(currentSpeed-currentSpeed*0.05f < reqSpeed){
        dir = 1f;
    } else if(currentSpeed < reqSpeed){
        dir = 0.3f;
    }
    foreach(IMyTerminalBlock thr in thrsForw){
        IMyThrust thrust = thr as IMyThrust;
        if(dir > 0f){
            thrust.ThrustOverride = (thrustOverrideScale*dir)/thrsForw.Count;
        } else {
            thrust.ThrustOverride = 1f;
        }
    }
    foreach(IMyTerminalBlock thr in thrsBack){
        IMyThrust thrust = thr as IMyThrust;
        if(dir < 0f){
            thrust.ThrustOverride = (thrustOverrideScale*dir*-1)/thrsBack.Count;
        } else{
            thrust.ThrustOverride = 1f;
        }
    }
}

public float getSpeed(){
    return Me.CubeGrid.Speed;
}

public void failCheck(){
    if(prevSpeed != null){
        if(prevSpeed < getSpeed()){
            speedFailureCount++;
        } else {
            speedFailureCount = 0;
        }
    }
    if(speedFailureCount > 15){
        stop("Speed only increasing!");
    }
 
    prevSpeed = getSpeed();
}

public void scan(){
    thrsBack = new List<IMyTerminalBlock>();
    thrsForw = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> thrsForwAll = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> thrsBackAll = new List<IMyTerminalBlock>();
    ((IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Cruise Forward")).GetBlocks(thrsForwAll);
    ((IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Cruise Backward")).GetBlocks(thrsBackAll);
    foreach(IMyTerminalBlock thrust in thrsBackAll){
        if(thrust.CubeGrid == Me.CubeGrid){
            thrsBack.Add(thrust);
        }
    }
    foreach(IMyTerminalBlock thrust in thrsForwAll){
        if(thrust.CubeGrid == Me.CubeGrid){
            thrsForw.Add(thrust);
        }
    }
}