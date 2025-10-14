//bike script V8
public string seat = "APGsG.Bike.Seat";
public int display_id = 0;

public double power_stored = 0;
public double total_power = 0;
public string display_text;
public Color display_color = new Color(0,100,165,255);
public bool isCharging = true;
public bool connected = false;
public int flash = 1;

public Program(){Runtime.UpdateFrequency = UpdateFrequency.Update10;}

public void Main(string arg){

flash++;
if(flash > 4) flash = 1;

List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(blocks);

List<IMyBatteryBlock> bats = new List<IMyBatteryBlock>();
bats = blocks.ConvertAll(x => (IMyBatteryBlock)x);

List<IMyTerminalBlock> seats = new List<IMyTerminalBlock>();
GridTerminalSystem.SearchBlocksOfName(seat,seats);
List<IMyTextSurfaceProvider> disps = new List<IMyTextSurfaceProvider>();

bats.ForEach(delegate(IMyBatteryBlock battery){if(battery.CubeGrid != Me.CubeGrid) connected = true;});

bats.ForEach(delegate(IMyBatteryBlock battery){
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

    double battery_charge = 0;
    if(total_power!=0) battery_charge = power_stored / total_power*100;

    display_text = getChargeText(battery_charge);

    display_text = display_text + System.Environment.NewLine + getBattery(battery_charge);
    display_color = getColor(battery_charge,isCharging);
   
    seats.ForEach(delegate(IMyTerminalBlock seat_){
    if(seat_.CubeGrid == Me.CubeGrid){

        IMyTextSurfaceProvider disp = (IMyTextSurfaceProvider)seat_;
        disp.GetSurface(display_id).WriteText(display_text, false);
        disp.GetSurface(display_id).BackgroundColor = display_color;
        }
    });


power_stored = 0;
battery_charge = 0;
total_power = 0;
display_text = "";
isCharging = true;
connected = false;

}

public string getBattery(double input){
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
public Color getColor(double input, bool isCharging){
    
    if(!isCharging){
        if(input <3 && flash <=2) return new Color(0,0,0,255);
        if(input < 10) return new Color(150,0,0,255);
        else if(input <35) return new Color(150,100,0,255);
        else return new Color(0,100,165,255);
    }
    else{
        if(input > 99.95) return new Color(0,200,0,255);
        else return new Color(255,255,0,255);
    }
}
public string getChargeText(double input){
    if(input > 99.9) return "100%";
    if(input == 0) return "0.00%";

    if(input < 10){
        return input.ToString("0.0") + "%";
    }
    else{
        return input.ToString("00.0") + "%";
    }

}
