IMyDefensiveCombatBlock detector;
bool alarm = false;
string alarmType = "none";
string currentSound = "none";
string prevSound = "none";

List<IMySoundBlock> sounds;
int seqTick = 100;

int lightTick = 0;
List<IMyLightingBlock> lights;
Dictionary<IMyLightingBlock, Color> lightStateSaver = new Dictionary<IMyLightingBlock, Color>();
bool lightsCustom = false;

List<IMyDoor> doors;
Dictionary<IMyDoor, DoorStatus> doorStateSaver = new Dictionary<IMyDoor, DoorStatus>();
bool lockDown = false;
bool doorAllOpen = false;
bool doorAllClosed = false;


//to-do:
//debug lockdown

public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    scan();
    saveLightColors();
    playSound("Alarm EnabledId");
}
public void Main(string argument, UpdateType updateSource) { seqTick++;
    if (lightTick < 10) lightTick++; else lightTick = 0;

    if (argument != "") { seqTick = 0; lightTick = 0; }
    Echo(alarmType);
    switch (argument.ToLower())
    { //this one called once
        case "alarm enemy":
            alarmType = "enemy";
            alarm = true;
            break;
        case "alarm damage":
            alarmType = "damage";
            alarm = true;
            break;
        case "alarm":
            alarmType = "alarm";
            alarm = true;
            break;
        case "stop":
            alarm = false;
            break;
        case "shut up":
            alarm = false;
            break;
        case "what":
            whatIsThisSound();
            alarm = false;
            break;
        case "scan":
            scan();
            break;
    }

    if (detect())
    {
        if (!alarm)
        {
            alarmType = "enemy auto";
            seqTick = 0;
            lightTick = 0;
            alarm = true;
        }
    }
    else if (alarmType == "enemy auto")
    {
        alarmType = "none";
        alarm = false;
        seqTick = 0;
    }


    if (alarm)
    { //this one ticks
        switch (alarmType)
        {
            case "enemy": playEnemyAlarm(); break;
            case "enemy auto": playEnemyAlarm(); break;
            case "alarm": playAlarm(); break;
        }
    }
    else
    {
        stopSound();
    }
    supportLockDown();
    

    prevSound = currentSound;
    if(alarm){
        lightSeq();
    } else if(lightsCustom){
        loadLightColors();
    }
}

//SoundBlockAlert1 / 2 / 3
//SoundBlockEnemyDetected
//EmergencyId
//Alarm EnabledId


public bool detect() {
    if (detector != null)
    {
        return detector.SearchEnemyComponent.FoundEnemyId != null;
    }
    return false;
}
public void scan() {
    scanAudioSources();
    scanLights();
    scanDoors();
    detector = (IMyDefensiveCombatBlock)GridTerminalSystem.GetBlockWithName("Enemy Scanner");
    foreach (IMySoundBlock sound in sounds) {
        Echo("Got " + sound.SelectedSound);
    }
}

public void scanDoors(){
    Echo("Scanning doors");
    var allBlocks = new List<IMyDoor>();
    GridTerminalSystem.GetBlocksOfType<IMyDoor>(allBlocks);
    doors = new List<IMyDoor>{};
    foreach(IMyDoor door in allBlocks){
        if(door != null) if(door.IsWorking == true) if(door.CubeGrid == Me.CubeGrid)
        if(door.CustomData.Contains("Apply lockdown")){
            Echo("Found " + door.CustomName);
            doors.Add(door);
        }
    }
}

public void saveDoorStates(){
    if(lockDown) return;
    doorStateSaver = new Dictionary<IMyDoor, DoorStatus>();
    foreach(IMyDoor door in doors){
        if(door != null)
        doorStateSaver.Add(door,door.Status);
    }
}
public void loadDoorStates(){
    foreach(KeyValuePair<IMyDoor, DoorStatus> pair in doorStateSaver){
        if(pair.Key != null)
        if(pair.value == DoorStatus.Closed || pair.Value == DoorStatus.Closing) pair.Key.Close();
        else pair.Key.Open();
    }
    lockDown = false;
}
public void initLockdown(){
    if(lockDown) return;
    scanDoors();
    saveDoorStates();
    doorAllClosed = false;
    foreach(IMyDoor door in doors){
        door.Close();
    }
    
}
public void undoLockDown(){
    if(!lockDown)return;
    scanDoors();
    doorAllOpen = false;
    foreach(IMyDoor door in doors){
        door.Enabled = true;
    }
    loadDoorStates();
}
public void supportLockDown(){
    bool allOpen = true;
    bool allClosed = true;

    if(lockDown){
        if(!doorAllClosed)
        foreach(IMyDoor door in doors){
            if(door != null) if(door.IsWorking)
            if(door.Status == DoorStatus.Closed) door.Enabled = false;
            else(door.Enabled = true; allClosed = false;)
        }
    } else {
        if(!doorAllOpen)
        foreach(IMyDoor door in doors){
            if(door != null) if(door.IsWorking)
            if(door.Status == DoorStatus.Closed) door.Enabled = true;
            else(allOpen = false);
        }
    }
    doorAllOpen = allOpen;
    doorAllClosed = allClosed;
}

public void scanLights(){
    Echo("Scanning lights");
    var allBlocks = new List<IMyLightingBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(allBlocks);
    lights = new List<IMyLightingBlock>{};
    foreach(IMyLightingBlock lightBlock in allBlocks){
        if(lightBlock != null) if(lightBlock.IsWorking == true)
        if(lightBlock.CubeGrid == Me.CubeGrid){
            Echo("Found " + lightBlock.CustomName);
            lights.Add(lightBlock);
        }
    }
}
public void saveLightColors(){
    if(lightsCustom) return;
    lightStateSaver = new Dictionary<IMyLightingBlock, Color>();
    foreach(IMyLightingBlock lightBl in lights){
        if(lightBl != null)
        lightStateSaver.Add(lightBl,lightBl.Color);
    }
}
public void loadLightColors(){
    foreach(KeyValuePair<IMyLightingBlock, Color> pair in lightStateSaver){
        if(pair.Key != null)
        pair.Key.Color = pair.Value;
    }
    lightsCustom = false;
}
public void chargeLightColors(Color clr){
    foreach(KeyValuePair<IMyLightingBlock, Color> pair in lightStateSaver){
        if(pair.Key != null)
        pair.Key.Color = clr;
    }
    lightsCustom = true;
}
public void scanAudioSources(){
    Echo("Scanning sounds");
    var allBlocks = new List<IMySoundBlock>();
    GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(allBlocks);
    sounds = new List<IMySoundBlock>{};
    foreach(IMySoundBlock sound in allBlocks){
        if(sound != null) if(sound.IsWorking == true) if(sound.CubeGrid == Me.CubeGrid)
        if(sound.CustomData.Contains("EmergencyPlayer")){
            Echo("Found " + sound.CustomName);
            sounds.Add(sound);
        }
    }
}
public void lightSeq(){
    switch(lightTick){
        case 1:
        chargeLightColors(new Color(200,0,0,255));
        break;
        case 8:
        loadLightColors();
        break;
    }
}
public void whatIsThisSound(){
    Echo("Sound check");
    foreach(IMySoundBlock sound in sounds){
        if(sound != null)
        if(sound.CustomData.Contains("EmergencyPlayer")){
            Echo("updated " + sound.CustomName);
            sound.CustomData = "EmergencyPlayer " + sound.SelectedSound;
        }
    }
}
public void playEnemyAlarm(){
    Echo("Enemy Alarm");
    switch(seqTick){
        case 1:
        playSound("SoundBlockAlert1");
        break;
        case 20:
        playSound("SoundBlockEnemyDetected");
        break;
        case 35:
        playSound("SoundBlockAlert2");
        break;
        case 45:
        playSound("SoundBlockEnemyDetected");
        break;
        case 60:
        playSound("SoundBlockAlert1");
        break;
        default:
        if(seqTick >= 111) seqTick = 0;
        break;
    }
}
public void playAlarm(){
    Echo("Alarm");
    switch(seqTick){
        case 1:
        playSound("SoundBlockAlert1");
        break;
        default:
        if(seqTick >= 7) seqTick = 0;
        break;
    }
}
public void stopSound(){
    Echo("stop");
    switch(seqTick){
        case 1:
        playSound("SoundBlockAlert3");
        break;
        default:
        if(seqTick == 7){
            currentSound = "none";
            foreach(IMySoundBlock sound in sounds){
                sound.Stop();
            }
        }
        break;
    }
}
public void playSound(string soundIn){
    foreach(IMySoundBlock sound in sounds){
        if(sound != null) if(sound.IsWorking)
        if(sound.CustomData.Contains("EmergencyPlayer")){
            sound.SelectedSound = soundIn;
            sound.Play();
        }
    }
}
public void playSound(){
    foreach(IMySoundBlock sound in sounds){
        if(sound != null) if(sound.IsWorking)
        if(sound.CustomData.Contains("EmergencyPlayer")){
            sound.SelectedSound = currentSound;
            sound.Play();
        }
    }
}