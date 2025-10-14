public void Main(string argument)
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    //Lift code by AlexeyPG
    IMyDoor lift_door_top = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь лифта сверху");
    IMyDoor lift_door_bottom = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь лифта снизу");
    IMyDoor lift_door_lift = (IMyDoor)GridTerminalSystem.GetBlockWithName("Дверь в лифте");
    IMyProgrammableBlock programmable_block = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("Программа лифта");
    IMyTextSurfaceProvider programm_block_in_lift = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Программа лифта");
    IMyTextSurface display_in_lift = programm_block_in_lift.GetSurface(0);
    IMyTextSurfaceProvider display_button_bottom = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Вызов лифта снизу");
    IMyTextSurface display_on_bottom = display_button_bottom.GetSurface(0);
    IMyTextSurfaceProvider display_button_top = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Вызов лифта сверху");
    IMyTextSurface display_on_top = display_button_top.GetSurface(0);
    IMyPistonBase piston1 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 1");
    IMyPistonBase piston2 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 2");
    IMyPistonBase piston3 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 3");
    IMyPistonBase piston4 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 4");
    IMyPistonBase piston5 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 5");
    IMyPistonBase piston6 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 6");
    IMyPistonBase piston7 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень лифт 7");
    IMySoundBlock speaker = (IMySoundBlock)GridTerminalSystem.GetBlockWithName("Динамик лифт");
    float lift_pos = piston1.CurrentPosition + piston2.CurrentPosition + piston3.CurrentPosition + piston4.CurrentPosition + piston5.CurrentPosition + piston6.CurrentPosition + piston7.CurrentPosition;
    string lift_pos_inp = (lift_pos * 1.42857142857142f).ToString("#");
    string lift_pos_inp_inv = (100 - lift_pos * 1.42857142857142f).ToString("#");
    if(argument == "down" & lift_pos > 69.8f)
    {
        programmable_block.CustomData = "go_down";
        lift_door_top.CloseDoor();
        lift_door_bottom.CloseDoor();
        lift_door_lift.CloseDoor();
        

    }
    if(argument == "up" & lift_pos < 0.2f)
    {
        programmable_block.CustomData = "go_up";
        lift_door_top.CloseDoor();
        lift_door_bottom.CloseDoor();
        lift_door_lift.CloseDoor();
        
    }
    if(argument != "up" || argument != "down")
    { 
        if(lift_pos < 0.2f & programmable_block.CustomData == "go_down")
        {
            lift_door_bottom.OpenDoor();
            lift_door_lift.OpenDoor();
            display_in_lift.WriteText("vvv" + System.Environment.NewLine + "100%",false);
            display_on_bottom.WriteText("Лифт" + System.Environment.NewLine + "<<<",false);
            display_on_top.WriteText("Лифт" + System.Environment.NewLine + "Ожидает",false);
            speaker.Stop();
        }
        if(lift_pos > 69.8f & programmable_block.CustomData == "go_up")
        {
            lift_door_top.OpenDoor();
            lift_door_lift.OpenDoor();
            display_in_lift.WriteText("^^^" + System.Environment.NewLine + "100%",false);
            display_on_bottom.WriteText("Лифт" + System.Environment.NewLine + "Ожидает",false);
            display_on_top.WriteText("Лифт" + System.Environment.NewLine + "<<<",false);
            speaker.Stop();

        }
        if(lift_door_top.Status == DoorStatus.Closed & lift_door_bottom.Status == DoorStatus.Closed & lift_door_lift.Status == DoorStatus.Closed)
        {
            if(programmable_block.CustomData == "go_down")
            {
                piston1.Retract();
                piston2.Retract();
                piston3.Retract();
                piston4.Retract();
                piston5.Retract();
                piston6.Retract();
                piston7.Retract();
                display_in_lift.WriteText("vvv" + System.Environment.NewLine + lift_pos_inp_inv + "%",false);
                display_on_bottom.WriteText("Лифт" + System.Environment.NewLine + "vvv" + System.Environment.NewLine + lift_pos_inp_inv + "%",false);
                display_on_top.WriteText("Лифт" + System.Environment.NewLine + "vvv" + System.Environment.NewLine + lift_pos_inp_inv + "%",false);

            }
            if(programmable_block.CustomData == "go_up")
            {
                piston1.Extend();
                piston2.Extend();
                piston3.Extend();
                piston4.Extend();
                piston5.Extend();
                piston6.Extend();
                piston7.Extend();
                display_in_lift.WriteText("^^^" + System.Environment.NewLine + lift_pos_inp + "%",false);
                display_on_bottom.WriteText("Лифт" + System.Environment.NewLine + "^^^" + System.Environment.NewLine + lift_pos_inp + "%",false);
                display_on_top.WriteText("Лифт" + System.Environment.NewLine + "^^^" + System.Environment.NewLine + lift_pos_inp + "%",false);
            }
        }
        if(lift_pos < 0.2f)
        {
            if(lift_door_top.Status == DoorStatus.Open & programmable_block.CustomData == "go_up")
            {
                lift_door_top.CloseDoor();
            }
            if(lift_door_lift.Status == DoorStatus.Open & programmable_block.CustomData == "go_up")
            {
                lift_door_lift.CloseDoor();
            }
            if(lift_door_bottom.Status == DoorStatus.Open & programmable_block.CustomData == "go_up")
            {
                lift_door_bottom.CloseDoor();
            }
            
        }
        if(lift_pos > 69.8f)
        {
            if(lift_door_top.Status == DoorStatus.Open & programmable_block.CustomData == "go_down")
            {
                lift_door_top.CloseDoor();
            }
            if(lift_door_lift.Status == DoorStatus.Open & programmable_block.CustomData == "go_down")
            {
                lift_door_lift.CloseDoor();
            }
            if(lift_door_bottom.Status == DoorStatus.Open & programmable_block.CustomData == "go_down")
            {
                lift_door_bottom.CloseDoor();
            }
            
        }
        
    }

    if((argument == "down" & lift_pos > 69.8f) || (argument == "up" & lift_pos < 0.2f))
    {
        speaker.Play();
    }

}