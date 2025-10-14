//H refuel station
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update100;

}


public void Main(string args)
{

    IMyBlockGroup tower_h_1 = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("h_tower1");
    IMyBlockGroup tower_h_2 = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("h_tower2");
    IMyBlockGroup tower_h_3 = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("h_tower3");
    IMyBlockGroup tower_h_4 = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("h_tower4");
    IMyBlockGroup tower_h_5 = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("h_tower5");
    IMyBlockGroup tower_h_6 = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("h_tower6");
    IMyBeacon beacon = (IMyBeacon)GridTerminalSystem.GetBlockWithName("Маяк");
    List<IMyTerminalBlock> tower_1_list = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> tower_2_list = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> tower_3_list = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> tower_4_list = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> tower_5_list = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> tower_6_list = new List<IMyTerminalBlock>();
    IMyTextPanel display = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("Column_status");
    


    double[] tower_fill = new double[6]; 



    tower_h_1.GetBlocks(tower_1_list);
    foreach(var tank_v in tower_1_list)
    {
        var tank = tank_v as IMyGasTank;
        tower_fill[0] += tank.FilledRatio;
    }

    tower_h_2.GetBlocks(tower_2_list);
    foreach(var tank_v in tower_2_list)
    {
        var tank = tank_v as IMyGasTank;
        tower_fill[1] += tank.FilledRatio;
    }

    tower_h_3.GetBlocks(tower_3_list);
    foreach(var tank_v in tower_3_list)
    {
        var tank = tank_v as IMyGasTank;
        tower_fill[2] += tank.FilledRatio;
    }

    tower_h_4.GetBlocks(tower_4_list);
    foreach(var tank_v in tower_4_list)
    {
        var tank = tank_v as IMyGasTank;
        tower_fill[3] += tank.FilledRatio;
    }

    tower_h_5.GetBlocks(tower_5_list);
    foreach(var tank_v in tower_5_list)
    {
        var tank = tank_v as IMyGasTank;
        tower_fill[4] += tank.FilledRatio;
    }

    tower_h_6.GetBlocks(tower_6_list);
    foreach(var tank_v in tower_6_list)
    {
        var tank = tank_v as IMyGasTank;
        tower_fill[5] += tank.FilledRatio;
    }


    display.WriteText("TOWER 1   TOWER 3   TOWER 5" + System.Environment.NewLine,false);
    display.WriteText(upd(tower_fill[0]) + "      " + upd(tower_fill[2]) + "      " + upd(tower_fill[4]) + System.Environment.NewLine + System.Environment.NewLine,true);
    display.WriteText("TOWER 2   TOWER 4   TOWER 6" + System.Environment.NewLine,true);
    display.WriteText(upd(tower_fill[1]) + "      " + upd(tower_fill[3]) + "      " + upd(tower_fill[5]) + System.Environment.NewLine,true);

    
    display.WriteText(System.Environment.NewLine + "Total: " + upd((tower_fill[0] + tower_fill[1] + tower_fill[2] + tower_fill[3] + tower_fill[4] + tower_fill[5])/6),true);
    
    if(beacon.HudText != "Refuel station: " + upd((tower_fill[0] + tower_fill[1] + tower_fill[2] + tower_fill[3] + tower_fill[4] + tower_fill[5])/6))
    beacon.HudText = "Refuel station: " + upd((tower_fill[0] + tower_fill[1] + tower_fill[2] + tower_fill[3] + tower_fill[4] + tower_fill[5])/6);
}

public string upd(double input)
{
    if(input==5){
        return "100%";
    }
    else{
        if(input<0.00001)
        return "0.00%";
        else
        if(input*20>0.999)
        return (input*20).ToString("##.##")+"%";
        else
        return "0"+(input*20).ToString("##.##")+"%";
    }
}