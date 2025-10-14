//Fly test
float TotalMass;
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;

}


public void Main(string args)
{
    IMyThrust atmo = (IMyThrust)GridTerminalSystem.GetBlockWithName("atmo");
    IMyShipController ctrl = (IMyShipController)GridTerminalSystem.GetBlockWithName("Штурвалл");
    Echo(TotalMass + "");
    //Echo(ctrl.GetNaturalGravity().length());
    //atmo.ThrustOverride = whatever i`ll need;

}