

//Receaver
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}



public void Main(string argument)
{




    string tag = "satellite1";
    IMyBroadcastListener Listener = IGC.RegisterBroadcastListener(tag) as IMyBroadcastListener;
    IMyReactor reactor = (IMyReactor)GridTerminalSystem.GetBlockWithName("Главный реактор");
    IMyAirVent vent = (IMyAirVent)GridTerminalSystem.GetBlockWithName("Вентиляция жилой зоны 1");
    
    //for gas
    IMyGasTank gas1 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 1");
    IMyGasTank gas2 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 2");
    IMyGasTank gas3 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 3");
    IMyGasTank gas4 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 4");
    IMyGasTank gas5 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 5");
    IMyGasTank gas6 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 6");
    IMyGasTank gas7 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 7");
    IMyGasTank gas8 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 8");
    IMyGasTank gas9 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 9");
    IMyGasTank gas10 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 10");
    IMyGasTank gas11 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 11");
    IMyGasTank gas12 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 12");
    IMyGasTank gas13 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 13");
    IMyGasTank gas14 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 14");
    IMyGasTank gas15 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 15");
    IMyGasTank gas16 = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Кислородный бак жилой зоны 16");


    //for elevator
    IMyPistonBase piston1 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 1");
    IMyPistonBase piston2 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 2");
    IMyPistonBase piston3 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 3");
    IMyPistonBase piston4 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 4");
    IMyPistonBase piston5 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 5");
    IMyPistonBase piston6 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 6");
    IMyPistonBase piston7 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 7");
    IMyDoor lift_door_top = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь лифта сверху");
    IMyDoor lift_door_bottom = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь лифта снизу");
    IMyDoor lift_door_lift = (IMyDoor)GridTerminalSystem.GetBlockWithName("Дверь в лифте");
    IMyProgrammableBlock programmable_block = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("Программа лифта");
    float lift_pos = piston1.CurrentPosition + piston2.CurrentPosition + piston3.CurrentPosition + piston4.CurrentPosition + piston5.CurrentPosition + piston6.CurrentPosition + piston7.CurrentPosition;
    string lift_pos_inp = (lift_pos * 1.42857142857142f).ToString("#");
    string lift_pos_inp_inv = (100 - lift_pos * 1.42857142857142f).ToString("#");



    
    IMyTextSurfaceProvider display1 = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Status display 1");
    string display = "INFO:";

    //Satellite status
    if(Listener.HasPendingMessage)
    {  
        display = display + System.Environment.NewLine + "Satellite: connected (" + (Listener.AcceptMessage()).Data + ")";
    }
    else
    {
        display = display + System.Environment.NewLine + "Satellite: no connection!";
    }
    
    //reactor status
    if(reactor != null)
    {
        if(reactor.IsWorking)
        {
       
            if(reactor.CurrentOutput / 3 > 0 )
            {
                display = display + System.Environment.NewLine + "Reactor: active (" + (reactor.CurrentOutput / 3).ToString("#") + "%)";
            }
            else
            {
                display = display + System.Environment.NewLine + "Reactor: idle";
            }
    
        }
        else
        {
            display = display + System.Environment.NewLine + "Reactor: off";
        }
    }
    else
    {
        display = display + System.Environment.NewLine + "Reactor: error!";
    }


    //air pressure
    if(vent.GetOxygenLevel()*100 > 10f)
    {
        display = display + System.Environment.NewLine + "Air pressure: " + (vent.GetOxygenLevel() * 100).ToString("##") + "%";
    }
    else
    {
        display = display + System.Environment.NewLine + "No air pressure";
    }


    //o2 stored
    double gas_total = gas1.FilledRatio + gas2.FilledRatio + gas3.FilledRatio + gas4.FilledRatio + gas5.FilledRatio + gas6.FilledRatio + gas7.FilledRatio + gas8.FilledRatio + gas9.FilledRatio + gas10.FilledRatio + gas11.FilledRatio + gas12.FilledRatio + gas13.FilledRatio + gas14.FilledRatio + gas15.FilledRatio + gas16.FilledRatio;
    if(gas_total * 100 / 16 > 1)
    {
    display = display + System.Environment.NewLine + "O² stored: " + (gas_total * 100 / 16).ToString("##.##") + "% (" + ((gas_total) * gas1.Capacity).ToString("#") + "L)";
    }
    else
    {
        display = display + System.Environment.NewLine + "No O² stored";
    }

    //elevator
    
    if(lift_pos < 0.2f & programmable_block.CustomData == "go_down")
    {
        display = display + System.Environment.NewLine + "Elevator: down";
    }
    if(lift_pos > 69.8f & programmable_block.CustomData == "go_up")
    {
       display = display + System.Environment.NewLine + "Elevator: up";
    }
    if(lift_door_top.Status == DoorStatus.Closed & lift_door_bottom.Status == DoorStatus.Closed & lift_door_lift.Status == DoorStatus.Closed)
    {
        if(programmable_block.CustomData == "go_down")
        {
            display = display + System.Environment.NewLine + "Elevator: moving down (" + lift_pos_inp_inv + "%)";
        }
        if(programmable_block.CustomData == "go_up")
        {                
            display = display + System.Environment.NewLine + "Elevator: moving up (" + lift_pos_inp + "%)";
        }
    }
    if(!(lift_pos < 0.2f & programmable_block.CustomData == "go_down") & !(lift_pos > 69.8f & programmable_block.CustomData == "go_up") & !(lift_door_top.Status == DoorStatus.Closed & lift_door_bottom.Status == DoorStatus.Closed & lift_door_lift.Status == DoorStatus.Closed))
    {
        display = display + System.Environment.NewLine + "Elevator: processing...";
    }



    display1.GetSurface(0).WriteText(display, false);
}