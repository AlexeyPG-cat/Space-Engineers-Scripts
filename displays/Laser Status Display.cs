public string status = "Unknown";
public bool fire = false;
public bool recharge = true;
public float consAtFull = 30f;
public int blink = 0;

public Program(){Runtime.UpdateFrequency = UpdateFrequency.Update10;}

//Consumes about 12 uranium per full recharge

public void Main(string argument, UpdateType updateSource)
{
    bool ls_error = false;
    IMyTerminalBlock gen1 = GridTerminalSystem.GetBlockWithName("Ion Beam Generator 1");
    IMyTerminalBlock gen2 = GridTerminalSystem.GetBlockWithName("Ion Beam Generator 2");
    IMyTerminalBlock gen3 = GridTerminalSystem.GetBlockWithName("Ion Beam Generator 3");
    IMyTerminalBlock cap1 = GridTerminalSystem.GetBlockWithName("Ion Capacitor 1");
    IMyTerminalBlock cap2 = GridTerminalSystem.GetBlockWithName("Ion Capacitor 2");
    IMyTerminalBlock emit = GridTerminalSystem.GetBlockWithName("Ion Beam Active Emitter");
    IMyTerminalBlock shield = GridTerminalSystem.GetBlockWithName("Large Shield Generator 1");
    if(gen1 == null || gen2 == null || gen3 == null || cap1 == null || cap2 == null || emit == null){
        status = "Laser System Error!";
        ls_error = true;
    }

    float cap1_charge = 0f;
    float cap2_charge = 0f;
    if(cap1 != null) cap1_charge = cap1.GetValue<float>("Beam_ChargedRate");
    if(cap2 != null) cap2_charge = cap2.GetValue<float>("Beam_ChargedRate");
    float totalCharge = (cap1_charge + cap2_charge) * 50;

    if(argument == ""){
        
    }
    if(argument == "fire"){
        fire = !fire;
    }
    if(argument == "rc_sw"){
        recharge = !recharge;
    }
    if(totalCharge > 99.99){
        status = "Full";
        if(gen1 != null) gen1.SetValue<float>("Beam_OutputPower",consAtFull/3);
        if(gen2 != null) gen2.SetValue<float>("Beam_OutputPower",consAtFull/3);
        if(gen3 != null) gen3.SetValue<float>("Beam_OutputPower",consAtFull/3);
    }
    if(totalCharge <= 99.99){
        if(recharge){
            status = "Charging";
            if(gen1 != null) gen1.SetValue<float>("Beam_OutputPower",100f);
            if(gen2 != null) gen2.SetValue<float>("Beam_OutputPower",100f);
            if(gen3 != null) gen3.SetValue<float>("Beam_OutputPower",100f);
        } else {
            if (!ls_error) status = "Offline";
            if(gen1 != null) gen1.SetValue<float>("Beam_OutputPower",0f);
            if(gen2 != null) gen2.SetValue<float>("Beam_OutputPower",0f);
            if(gen3 != null) gen3.SetValue<float>("Beam_OutputPower",0f);
        }
    }
    if(totalCharge < 0.5) fire = false;
    if(fire) status = "Firing!";
    if(emit !=null) emit.SetValue<bool>("OnOff",fire);


    blink = (blink < 5) ? blink+1 : 0;
    string toDisplay = "Laser System" + System.Environment.NewLine + "Status: " + status + System.Environment.NewLine + "Charge: " + totalCharge.ToString("#0.00") + "%" + System.Environment.NewLine + ((status != "Full" || blink < 3) ? getBar((cap1_charge + cap2_charge)/2,25) : "");

    IMyTextPanel panel = GridTerminalSystem.GetBlockWithName("Display Lasers") as IMyTextPanel;
    if(panel != null){
    panel.WriteText(toDisplay);}




    if(shield != null){
        toDisplay = ("Shields: " + getCIProp(shield, "Ship Shield: ") + System.Environment.NewLine + (getShieldPc(getCIProp(shield, "Ship Shield: "))*100).ToString("0") + "% " + getBar(getShieldPc(getCIProp(shield, "Ship Shield: ")),25));
    } else {
        toDisplay = "Shield not found";
    }
    panel = GridTerminalSystem.GetBlockWithName("Display Shields") as IMyTextPanel;
    if(panel != null){
    panel.WriteText(toDisplay);}
}

public string getBar(float input,int size){
    if (size == 0) return "[Error]";
    float divid = 1f/size;
    float active = input/divid;
    string result = "";
    for(int i = 1; i <= size; i++){
        if(i<=active) result += "|"; else result+=" ";
    }
    return "[" + result + "]";
}

public string getCIProp(IMyTerminalBlock block, string paramStart){
    string result = "";
    if(block != null){
        string[] l = block.CustomInfo.Split('\n');
        foreach(string text in l){
            if(text.StartsWith(paramStart)){
                result = text.Substring(paramStart.Length);
            }
        }
    }
    return result;
}

public float getShieldPc(string input){
    float result = 0f;
    try{
        string[] nums = input.Split('/');
        result = getkm(nums[0])/getkm(nums[1]);
    } catch{result = 0f; Echo("error");};
    return result;
}

public float getkm(string input){
    int mul = 1;
    if(input.Contains("MPt")){
        mul = 1000000;
    } else if(input.Contains("kPt")){
        mul = 1000;
    }
    string res = "";
    char[] filther = {'0','1','2','3','4','5','6','7','8','9','.'};
    foreach(char ch in input){
        if(filther.Contains(ch)) res += ch;
    }
    return float.Parse(res)*mul;
}

