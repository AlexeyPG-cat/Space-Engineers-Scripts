List<IMyTerminalBlock> thrsBack;
List<IMyTerminalBlock> thrsForw;
float thrustOverrideScale = 0f;
float reqSpeed = 0;
int speedFailureCount = 0;
float prevSpeed;
bool autoPilot = false;



public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    scan();
}


public void Main(string args){
    handleArgs(args);
    if(autoPilot){
        cruise();
        failCheck();
    }
}

public void handleArgs(String args){
    //setForce <number>
    //setSpeed <number>
    //start
    //stop
    if(args.StartsWith("setForce ")){
        thrustOverrideScale = float.Parse(args.Substring("setForce ".Length))
    }
    if(args.StartsWith("setSpeed ")){
        reqSpeed = float.Parse(args.Substring("setSpeed ".Length))
    }
    if(args = "start"){
        autoPilot = true;
        speedFailureCount = 0;
    }
    if(args = "stop"){
        stop("User input");
    }
}

public void stop(string reason){
    autoPilot = false;
    for(IMyTerminalBlock thr in thrsForw){
        IMyThrust thrust = thr as IMyThrust;
        thrust.ThrustOverride = 0f;
    }
    for(IMyTerminalBlock thr in thrsBack){
        IMyThrust thrust = thr as IMyThrust;
        thrust.ThrustOverride = 0f;
    }
}

public void cruise(){
    float currentSpeed = getSpeed()
    float dir = 0;
    if(currentSpeed+currentSpeed*0.05f > reqSpeed){
        dir = -1f;
    } else if(currentSpeed > reqSpeed){
        dir = -0.1f;
    } else if(currentSpeed-currentSpeed*0.05f < reqSpeed){
        dir = 1f;
    } else if(currentSpeed < reqSpeed){
        dir = 0.1f;
    }
    for(IMyTerminalBlock thr in thrsForw){
        IMyThrust thrust = thr as IMyThrust;
        if(dir > 0f){
            thrust.ThrustOverride = thrustOverrideScale*dir;
        } else {
            thrust.ThrustOverride = 0f;
        }
    }
    for(IMyTerminalBlock thr in thrsBack){
        IMyThrust thrust = thr as IMyThrust;
        if(dir < 0f){
            thrust.ThrustOverride = thrustOverrideScale*dir*-1;
        } else{
            thrust.ThrustOverride = 0f;
        }
    }
}

public float getSpeed(){
    return Me.Cubegrid.Speed;
}

public void failCheck(){
    if(prevSpeed != null){
        if(prevSpeed < getSpeed){
            speedFailureCount++;
        } else {
            speedFailureCount = 0;
        }
    }
    if(speedFailureCount > 10){
        stop("Speed only increasing!");
    }
 
    prevSpeed = getSpeed;
}

public void scan(){
    List<IMyTerminalBlock> thrsBack = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> thrsForw = new List<IMyTerminalBlock>();
    ((IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Cruise Forward")).GetBlocks(thrsForw);
    ((IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Cruise Backward")).GetBlocks(thrsBack);
}


