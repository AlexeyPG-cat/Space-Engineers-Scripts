public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}

public void Main(string argument)
{
    //airlock 1
    //After lift
    IMyDoor airlock1_1 = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь airlock 1.1");
    IMyDoor airlock1_2 = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь airlock 1.2");
    IMyAirVent airlock1_vent = (IMyAirVent)GridTerminalSystem.GetBlockWithName("Полная вентиляция airlock1");
    
    Echo("Airlock 1:");
    if(argument == "airlock1")
    {
        airlock1_1.CloseDoor();
        airlock1_2.CloseDoor();
    }
    if(airlock1_1.Status == DoorStatus.Closed & airlock1_2.Status == DoorStatus.Closed)
    {
        if(airlock1_vent.CanPressurize)
        {
        airlock1_1.Enabled = true;
        airlock1_2.Enabled = true;
        Echo("Закрыт");
        }
        else
        {
        airlock1_1.Enabled = true;
        airlock1_2.Enabled = false;
        Echo("Закрыт, не герметичен!");
        }
    }
    if(airlock1_1.Status == DoorStatus.Closed & airlock1_2.Status != DoorStatus.Closed)
    {
        airlock1_1.Enabled = false;
        airlock1_2.Enabled = true;
        Echo("Дверь 2 открыта");
    }
    if(airlock1_1.Status != DoorStatus.Closed & airlock1_2.Status == DoorStatus.Closed)
    {
        airlock1_1.Enabled = true;
        airlock1_2.Enabled = false;
        Echo("Дверь 1 открыта");
    }
    if(airlock1_1.Status != DoorStatus.Closed & airlock1_2.Status != DoorStatus.Closed)
    {
        airlock1_1.Enabled = true;
        airlock1_2.Enabled = true;
        Echo("Не герметичен");
    }

    //airlock 2
    //Top exit
    IMyDoor airlock2_1 = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь airlock 2.1");
    IMyDoor airlock2_2 = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь airlock 2.2");
    IMyAirVent airlock2_vent = (IMyAirVent)GridTerminalSystem.GetBlockWithName("Полная вентиляция airlock2");
    Echo("Airlock 2:");
    if(argument == "airlock2")
    {
        airlock2_1.CloseDoor();
        airlock2_2.CloseDoor();
    }
    if(airlock2_1.Status == DoorStatus.Closed & airlock2_2.Status == DoorStatus.Closed)
    {
        if(airlock2_vent.CanPressurize)
        {
        airlock2_1.Enabled = true;
        airlock2_2.Enabled = true;
        Echo("Закрыт");
        }
        else
        {
        airlock2_1.Enabled = true;
        airlock2_2.Enabled = false;
        Echo("Закрыт, не герметичен!");
        }
    }
    if(airlock2_1.Status == DoorStatus.Closed & airlock2_2.Status != DoorStatus.Closed)
    {
        airlock2_1.Enabled = false;
        airlock2_2.Enabled = true;
        Echo("Дверь 2 открыта");
    }
    if(airlock2_1.Status != DoorStatus.Closed & airlock2_2.Status == DoorStatus.Closed)
    {
        airlock2_1.Enabled = true;
        airlock2_2.Enabled = false;
        Echo("Дверь 1 открыта");
    }
    if(airlock2_1.Status != DoorStatus.Closed & airlock2_2.Status != DoorStatus.Closed)
    {
        airlock2_1.Enabled = true;
        airlock2_2.Enabled = true;
        Echo("Не герметичен");
    }

    //airlock 3
    //Middle exit
    IMyDoor airlock3_1 = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь airlock 3.1");
    IMyDoor airlock3_2 = (IMyDoor)GridTerminalSystem.GetBlockWithName("Сдвижная дверь airlock 3.2");
    IMyAirVent airlock3_vent = (IMyAirVent)GridTerminalSystem.GetBlockWithName("Полная вентиляция airlock3");
    Echo("Airlock 3:");
    if(argument == "airlock3")
    {
        airlock3_1.CloseDoor();
        airlock3_2.CloseDoor();
    }
    if(airlock3_1.Status == DoorStatus.Closed & airlock3_2.Status == DoorStatus.Closed)
    {
        if(airlock3_vent.CanPressurize)
        {
        airlock3_1.Enabled = true;
        airlock3_2.Enabled = true;
        Echo("Закрыт");
        }
        else
        {
        airlock3_1.Enabled = false;
        airlock3_2.Enabled = true;
        Echo("Закрыт, не герметичен!");
        }
    }
    if(airlock3_1.Status == DoorStatus.Closed & airlock3_2.Status != DoorStatus.Closed)
    {
        airlock3_1.Enabled = false;
        airlock3_2.Enabled = true;
        Echo("Дверь 2 открыта");
    }
    if(airlock3_1.Status != DoorStatus.Closed & airlock3_2.Status == DoorStatus.Closed)
    {
        airlock3_1.Enabled = true;
        airlock3_2.Enabled = false;
        Echo("Дверь 1 открыта");
    }
    if(airlock3_1.Status != DoorStatus.Closed & airlock3_2.Status != DoorStatus.Closed)
    {
        airlock3_1.Enabled = true;
        airlock3_2.Enabled = true;
        Echo("Не герметичен");
    }

    //airlock 4
    //advanced script
    //drone way
    Echo("Airlock 4:");
    IMyAirVent vent = (IMyAirVent)GridTerminalSystem.GetBlockWithName("Вентиляция airlock4");
    IMyAirVent vent_livezone = (IMyAirVent)GridTerminalSystem.GetBlockWithName("Вентиляция жилой зоны 1");
    IMyDoor airlock4_1 = (IMyDoor)GridTerminalSystem.GetBlockWithName("Ворота airlock4.1");
    IMyDoor airlock4_2 = (IMyDoor)GridTerminalSystem.GetBlockWithName("Ворота airlock4.2");
    if(vent.GetOxygenLevel()*100 > 80f)
    {
        airlock4_1.Enabled = true;
        airlock4_2.Enabled = false;
        Echo("Есть кислород");
    }
    if(vent.GetOxygenLevel()*100 < 5f)
    {
        airlock4_1.Enabled = false;
        airlock4_2.Enabled = true;
        Echo("Нет кислорода");
    }
    if(!(vent.GetOxygenLevel()*100 > 90f) & !(vent.GetOxygenLevel()*100 < 10f))
    {
        airlock4_1.Enabled = false;
        airlock4_2.Enabled = false;
        Echo("Меняется");
    }
    if(argument == "airlock4")
    {
        if(airlock4_1.Status == DoorStatus.Closed & airlock4_2.Status == DoorStatus.Closed)
        {
            if(vent.GetOxygenLevel()*100 > 80f)
            {
                vent.Depressurize = true;
            }
            if(vent.GetOxygenLevel()*100 < 5f)
            {
                vent.Depressurize = false;
            }
        }
        else
        {
            airlock4_1.CloseDoor();
            airlock4_2.CloseDoor();
        }
    }
}