bool connected = false;
bool can_connect = false;
bool open = false;
bool error = false;
bool had_connection = false;
bool blinky_lights = false;

public void Main(string arg)
{
    IMyBlockGroup connectors_group = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Зарядка - коннекторы");
    IMyBlockGroup lights_group = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Зарядка - свет");
    IMyPistonBase piston = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Зарядка - поршень");
    IMyTimerBlock error_timer = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName("Зарядка - ошибка");
    IMyTimerBlock autoclose_timer = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName("Зарядка - автозакрытие");




    
    List<IMyTerminalBlock> connectors_list = new List<IMyTerminalBlock>();
    connectors_group.GetBlocks(connectors_list);
        connected = false;
        can_connect = false;
    foreach (var connector_switch in connectors_list)
    {
        var connector = (IMyShipConnector)connector_switch;
        if(connector.Status == MyShipConnectorStatus.Connected)
        {
            connected = true;
        }

        if(connector.Status == MyShipConnectorStatus.Connectable)
        {
            can_connect = true;
        }
    }


if(arg == "connected")
{
    Echo("connected");
    open = true;
    had_connection = true;
    autoclose_timer.StopCountdown();
    piston.Extend();
    
}
if(arg == "disconnected")
{
    Echo("Disconnected");
    open = true;
    if(!connected)
    {
    autoclose_timer.StartCountdown();
    }
}


if(arg == "close")
{
    if(connected || can_connect)
    {
    Echo("Cant close!");
    open = true;
    error = true;
    error_timer.StartCountdown();
    autoclose_timer.StopCountdown();
    }
    else
    {
    Echo("closing");
    piston.Retract();
    open = false;
    autoclose_timer.StopCountdown();
    had_connection = false;
    }
}
if(arg == "open")
{
    Echo("open");
    piston.Extend();
    open = true;
    autoclose_timer.StopCountdown();
}
if(arg == "error")
{
    error = false;
}
if(arg == "autoclose")
{
    if(connected || can_connect)
    {
    Echo("Cant auto-close!");
    open = true;
    error = true;
    had_connection = false;
    }
    else
    {
    Echo("auto-closing");
    piston.Retract();
    open = false;
    autoclose_timer.StopCountdown();
    had_connection = false;
    }
}


    if(had_connection & !connected)
    {
        error_timer.StartCountdown();
    }



    List<IMyTerminalBlock> lights_list = new List<IMyTerminalBlock>();
    lights_group.GetBlocks(lights_list);


    

    if(autoclose_timer.IsCountingDown)
    {
        blinky_lights = true;
    }
    else
    {
        blinky_lights = false;
    }

    foreach (var light_switch in lights_list)
    {
        var light = (IMyLightingBlock)light_switch; 
        if(blinky_lights)
        {
            light.SetValue("Blink Interval", 1f);
        }
        else
        {
            light.SetValue("Blink Interval", 0f);
        }





        if(open)
        {
            if(error)
            {
                light.SetValue("Color", new Color(150, 0, 0, 255));
                light.SetValue("Blink Interval", 0f);
                
            }
            else
            {
                if(connected)
                {
                    light.SetValue("Color", new Color(0, 150, 0, 255));
                    
                }
                else
                {
                    if(had_connection)
                    {
                        light.SetValue("Color", new Color(150, 150, 150, 255));
                        
                    }
                    else
                    {
                        
                        light.SetValue("Color", new Color(255, 255, 255, 255));
                        
                    }

                }
            }
        }
        else
        {
        light.SetValue("Color", new Color(0, 0, 0, 255));
        }
    }
}