//Stupid
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update100;

}


public void Main(string args)
{

    //new

    var atmo1 = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(atmo1);
    var atmo = atmo1[0] as IMyThrust;
    Random rnd = new Random();
    for(int i=0; i < atmo1.Count;i++){
        if(rnd.Next(2) == 1)
        atmo1[i].ThrustOverride = atmo1[i].MaxThrust;
        else
        atmo1[i].ThrustOverride = 10;
    }

}


