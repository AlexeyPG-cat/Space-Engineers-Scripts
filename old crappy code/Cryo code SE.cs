public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}

public void Main(string argument)
{
    //Cryo code by AlexeyPG
    String cryoID = "0";
    IMyCryoChamber cryo = (IMyCryoChamber)GridTerminalSystem.GetBlockWithName("Крио камера ID:" + cryoID);
    IMyBlockGroup batteries_group = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName("Крио батареи ID:" + cryoID);
    IMyGasTank oxygen = (IMyGasTank)GridTerminalSystem.GetBlockWithName("Крио кислород ID:" + cryoID);
    IMyShipConnector connector = (IMyShipConnector)GridTerminalSystem.GetBlockWithName("Крио коннектор ID:" + cryoID);
    IMyLightingBlock lamp = (IMyLightingBlock)GridTerminalSystem.GetBlockWithName("Крио индикатор ID:" + cryoID);
    IMyTextSurfaceProvider display = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("Крио программа ID:" + cryoID);
    IMyProgrammableBlock programmable_block = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("Крио программа ID:" + cryoID);
    IMyBeacon beacon = (IMyBeacon)GridTerminalSystem.GetBlockWithName("Крио маяк ID:" + cryoID);
    List<IMyTerminalBlock> batteries_list = new List<IMyTerminalBlock>();
    int warning_level = 0;
    //warning levels:
    //0 - all ok
    //1 - disconnected but all ok
    //2 - disconnected but have spme problems not critical for live or empty
    //3 - disconnected and person will probably die if not done anything

    //Cryo requiers setup!
    //After every component cryo`s ID
    //Each cryo must have different id else it fail calc
    //It does not matter when cryo detached
    //Only provides info on station
    //After renaming change variable "CryoID" in beginning
    //ID must be the same for all components
    string display_text = "";
    if(connector.Status == MyShipConnectorStatus.Connected & cryoID == "0")
    {
        display_text = display_text + "Can`t provide info" + System.Environment.NewLine + "Cryo is not" + System.Environment.NewLine + "configured" + System.Environment.NewLine + "Please contact" + System.Environment.NewLine + "cryo engineer";
        if(warning_level < 1)
        {warning_level = 1;}
    }
    else
    {
    if(cryo.IsUnderControl)
    {
        display_text = display_text + "Alive";
    }
    else
    {
        display_text = display_text + "Empty";
    }
    //Connector status
    if(connector.Status == MyShipConnectorStatus.Connected)
    {
        display_text = display_text + System.Environment.NewLine + "Connected";
    }
    if(connector.Status == MyShipConnectorStatus.Unconnected)
    {
        display_text = display_text + System.Environment.NewLine + "Disconnected";
        if(warning_level < 1 & cryo.IsUnderControl)
        {warning_level = 1;}
    }
    if(connector.Status == MyShipConnectorStatus.Connectable)
    {
        display_text = display_text + System.Environment.NewLine + "Can connect";
        if(warning_level < 1 & cryo.IsUnderControl)
        {warning_level = 1;}
    }
    //battery charge calculator
    float power_stored = 0f;
    float battery_charge = 0f;
    bool battery_fail = false;
    batteries_group.GetBlocks(batteries_list);
    foreach(var battery in batteries_list)
    {
        var bats = battery as IMyBatteryBlock;
        power_stored = power_stored + bats.CurrentStoredPower;
        battery_charge = power_stored / 0.006f;
        if(!bats.IsWorking)
        {battery_fail = true;}
    }
    //oxygen anount
    float oxygen_amount = 0f;
    oxygen_amount = (float)oxygen.FilledRatio * 100f;

    display_text = display_text + System.Environment.NewLine + "C:" + battery_charge.ToString("#") + "%, O²:" + oxygen_amount.ToString("#") + "%";

    //Warnings secrion
    //Damage detector
    if(cryo.IsWorking & oxygen.IsWorking & connector.IsWorking & lamp.IsWorking & battery_fail == false)
    {
        display_text = display_text + System.Environment.NewLine + "Systems ok";
    }
    else
    {
        display_text = display_text + System.Environment.NewLine + "Problems:";
        if(cryo.IsWorking == false)
        {
            display_text = display_text + System.Environment.NewLine + "Cryo damaged!";
            if(warning_level < 3)
            {warning_level = 3;}
        }
        if(battery_fail)
        {
            display_text = display_text + System.Environment.NewLine + "Battery damaged!";
            if(cryo.IsUnderControl)
            {
                if(warning_level < 3)
                {warning_level = 3;}
            }
            else
            {
                if(warning_level < 2)
                {warning_level = 2;}
            }
        }
        if(oxygen.IsWorking == false)
        {
            display_text = display_text + System.Environment.NewLine + "O² tank damaged!";
            if(cryo.IsUnderControl)
            {
                if(warning_level < 3)
                {warning_level = 3;}
            }
            else
            {
                if(warning_level < 2)
                {warning_level = 2;}
            }
        }
        if(connector.IsWorking == false)
        {
            display_text = display_text + System.Environment.NewLine + "Connector damaged!";
            if(cryo.IsUnderControl)
            if(warning_level < 2)
            {warning_level = 2;}

        }
        if(lamp.IsWorking == false)
        {
            display_text = display_text + System.Environment.NewLine + "Lamp damaged!";
            if(warning_level < 2)
            {warning_level = 2;}
        }
    }
    //Supplies checker
    if(oxygen_amount < 50f & oxygen_amount >= 10f)
    {
        if(warning_level < 2 & connector.Status != MyShipConnectorStatus.Connected)
        {warning_level = 2;}
        display_text = display_text + System.Environment.NewLine + "O² low";
    }
    if(oxygen_amount < 10f & connector.Status != MyShipConnectorStatus.Connected)
    {
        if(cryo.IsUnderControl)
            {
                if(warning_level < 3 & connector.Status != MyShipConnectorStatus.Connected)
                {warning_level = 3;}
            }
            else
            {
                if(warning_level < 2 & connector.Status != MyShipConnectorStatus.Connected)
                {warning_level = 2;}
            }
        display_text = display_text + System.Environment.NewLine + "O² LOW!";
    }
    if(battery_charge < 50f & battery_charge >= 10f)
    {
        if(warning_level < 2 & connector.Status != MyShipConnectorStatus.Connected)
        {warning_level = 2;}
        display_text = display_text + System.Environment.NewLine + "Battery low";
    }
    if(battery_charge < 10f & connector.Status != MyShipConnectorStatus.Connected)
    {
        if(cryo.IsUnderControl)
            {
                if(warning_level < 3 & connector.Status != MyShipConnectorStatus.Connected)
                {warning_level = 3;}
            }
            else
            {
                if(warning_level < 2 & connector.Status != MyShipConnectorStatus.Connected)
                {warning_level = 2;}
            }
        display_text = display_text + System.Environment.NewLine + "BATTERY LOW!";
    }
    }
    //warning solver
    if(warning_level == 0)
    {
        lamp.SetValue("Color", new Color(0, 150, 0, 255));
        lamp.SetValue("Radius", 1f);
        lamp.SetValue("Intensity", 0.5f);
        lamp.SetValue("Blink Lenght", 100f);
        beacon.Enabled = false;
    }
    if(warning_level == 1)
    {
        lamp.SetValue("Color", new Color(255, 150, 0, 255));
        lamp.SetValue("Radius", 1f);
        lamp.SetValue("Intensity", 0.5f);
        lamp.SetValue("Blink Lenght", 100f);
        beacon.Enabled = false;
    }
    if(warning_level == 2)
    {
        lamp.SetValue("Color", new Color(255, 150, 0, 255));
        lamp.SetValue("Radius", 4f);
        lamp.SetValue("Intensity", 5f);
        lamp.SetValue("Blink Lenght", 50f);
        beacon.Enabled = false;
    }
    if(warning_level == 3)
    {
        lamp.SetValue("Color", new Color(150, 0, 0, 255));
        lamp.SetValue("Radius", 4f);
        lamp.SetValue("Intensity", 5f);
        lamp.SetValue("Blink Lenght", 50f);
        beacon.Enabled = true;
    }
    display_text = display_text + System.Environment.NewLine + "WL: " + warning_level;
    display.GetSurface(0).WriteText(display_text, false);
    display_text = "";
}