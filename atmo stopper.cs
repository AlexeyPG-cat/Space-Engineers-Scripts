public void Main(string args)
{

    //new

    var atmo1 = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(atmo1);
    var atmo = atmo1[0] as IMyThrust;
    for(int i=0; i < atmo1.Count;i++){
        if(atmo1[i].CubeGrid == Me.CubeGrid)
        atmo1[i].Enabled = false;
    }

}


