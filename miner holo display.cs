//miner
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}

//Script by AlexeyPG
//Configure!

//Disable thrusters automaticly when connected
bool DockingSw = true;

//Enable flashing warnings
bool doWarn = true;

//Disable all warnings
bool noWarn = false;

//Storage group name
string storageGroup = "AM1R MassInfo";

//Display name
string dp = "AM1R Display";

//Critical mass that ship can lift
float criticalMass = 120770f;

//Description
//Script automaticly detects batteries, connectors and remote control
//display defined by variable dp
//To calculate storage add all containers and drills (any inventory) into MassInfo group (you can rename that in storageGroup variable)
//To correctly setup critical mass replace variable with mass at which ship can not lift anymore (in kg)

float mass;
bool prevSw = true;
int tick = 0;

public void Main(string arg)
{
    bool warn = false;
    tick++;
    if(tick/3%2==1) warn = true;
    if(!doWarn) warn = false;
    if(noWarn) warn = true;

    String warn_Battery = "";
    String warn_Weight = "";
    String warn_Volume = "";
    String warn_Default = "";

    string toDisplay = System.Environment.NewLine + System.Environment.NewLine + "||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||" + System.Environment.NewLine;
    string cnStatus = "Unknown";
    
    long CargoVol = 0;
    long CargoVolMax = 0;
    IMyBlockGroup cargo = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName(storageGroup);
    List<IMyTerminalBlock> cargo_l = new List<IMyTerminalBlock>();
    cargo.GetBlocks(cargo_l);
    foreach(var container_ in cargo_l)
    {
        var container = container_ as IMyTerminalBlock;
        CargoVol += container.GetInventory().CurrentVolume.RawValue;
        CargoVolMax += container.GetInventory().MaxVolume.RawValue;
    }
    int load = (int)(((float)CargoVol/(float)CargoVolMax)*100);

bool connected = false;
//battery detector / controller
double power_stored = 0;
double total_power = 0;
bool isCharging = true;
List<IMyTerminalBlock> blocks_bat = new List<IMyTerminalBlock>();
List<IMyBatteryBlock> bats = new List<IMyBatteryBlock>();
GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(blocks_bat);
bats = blocks_bat.ConvertAll(x => (IMyBatteryBlock)x);
bats.ForEach(delegate(IMyBatteryBlock battery){
    if(battery.CubeGrid != Me.CubeGrid) connected = true;
    if(battery.CubeGrid == Me.CubeGrid && battery.IsWorking && battery.Enabled){
        power_stored += battery.CurrentStoredPower;
        total_power += battery.MaxStoredPower; 
        if(battery.ChargeMode != ChargeMode.Recharge){
            isCharging = false;
        }
        if(arg == "charge"){
            if(isCharging) battery.ChargeMode = ChargeMode.Auto;
            else if(connected) battery.ChargeMode = ChargeMode.Recharge;
        }  
    }
});
int battery_ch = (int)(power_stored/total_power*100);

    //connector detector / controller
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(blocks);
    List<IMyShipConnector> connectors = new List<IMyShipConnector>();
    connectors = blocks.ConvertAll(x => (IMyShipConnector)x);
    connectors.ForEach(delegate(IMyShipConnector connector){
        if(connector.CubeGrid == Me.CubeGrid){
        if(connector.Status == MyShipConnectorStatus.Connected) cnStatus = "Connected";
        else if(connector.Status == MyShipConnectorStatus.Connectable & cnStatus != "Connected") cnStatus = "Ready to connect";
        else if(connector.Status == MyShipConnectorStatus.Unconnected & (cnStatus != "Connected" || cnStatus != "Ready to connect")) cnStatus = "Disconnected                                              ";
        }
    });

    List<IMyTerminalBlock> blocks_rc = new List<IMyTerminalBlock>();
    List<IMyShipController> rc = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType<IMyShipController>(blocks_rc);
    rc = blocks_rc.ConvertAll(x => (IMyShipController)x);
    rc.ForEach(delegate(IMyShipController remote){
    if(remote.CubeGrid == Me.CubeGrid && remote.IsWorking){
        mass = remote.CalculateShipMass().TotalMass;
        if(DockingSw) {
            if(prevSw != !(connected || cnStatus == "Connected")){
                remote.DampenersOverride = !(connected || cnStatus == "Connected");
                prevSw = !(connected || cnStatus == "Connected");
            }
        }
        string tag = "doorsystemV2";
        if(arg == "door") IGC.SendBroadcastMessage<Vector3D>(tag,Me.GetPosition());
    }});

    if(CargoVol > CargoVolMax/5*4) warn_Volume = "Storage space low!";
    if(CargoVol >= CargoVolMax/100*99) warn_Volume = "No storage space!";
    if(mass > criticalMass/5*4) warn_Weight = "Heavy!";
    if(mass > criticalMass) warn_Weight = "Overweight!";
    if(battery_ch < 15) warn_Battery = "Battery low!";
    if(battery_ch < 5) warn_Battery = "Battery critical!";

    string rf = ". ";
    if(tick%2==1) rf = ": "; 

    double mass_pc = mass/criticalMass*100;
    if(connected || cnStatus == "Connected") toDisplay = toDisplay + rf + "Mass: unknown | N/A% " + getBar(0d);
    else toDisplay = toDisplay + rf + "Mass: " + mass + " | " + mass_pc.ToString("##.##") + "% " + getBar(mass_pc);
    toDisplay = toDisplay + System.Environment.NewLine + "Load: " + load + "% " + getBar((double)load) + "  |  Charge: " + battery_ch + "% " + getBar(battery_ch);
    toDisplay = toDisplay + System.Environment.NewLine;

    if(warn){
        warn_Battery = "";
        warn_Weight = "";
        warn_Volume = "";
        warn_Default = "";
    }
    if(connected || cnStatus == "Connected") warn_Weight = "";

    toDisplay = toDisplay + System.Environment.NewLine + warn_Default; //unused
    toDisplay = toDisplay + System.Environment.NewLine + warn_Battery; //Battery
    toDisplay = toDisplay + System.Environment.NewLine + warn_Weight; //Weight
    toDisplay = toDisplay + System.Environment.NewLine + warn_Volume; //Volume

    toDisplay = toDisplay + System.Environment.NewLine + System.Environment.NewLine;
    if(cnStatus == "Ready to connect" & warn) cnStatus = "";
    toDisplay = toDisplay + System.Environment.NewLine + cnStatus;

    List<IMyTerminalBlock> display = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(dp,display);
    display.ForEach(delegate(IMyTerminalBlock display__){
    if(display__.CubeGrid == Me.CubeGrid){
        var display_ = display__ as IMyTextSurfaceProvider;
        IMyTextSurface display___ = display_.GetSurface(0);
        display___.WriteText(toDisplay);
    }});
}

public string getBar(double input){
    if(input <3) return "[        ]";
    else if(input <10) return "[        ]";
    else if(input <20) return "[|       ]";
    else if(input <30) return "[||       ]";
    else if(input <40) return "[|||      ]";
    else if(input <50) return "[||||     ]";
    else if(input <60) return "[|||||    ]";
    else if(input <70) return "[||||||   ]";
    else if(input <80) return "[|||||||  ]";
    else if(input <90) return "[|||||||| ]";
    else return "[|||||||||]";
}