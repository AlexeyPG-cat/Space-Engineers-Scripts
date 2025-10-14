List<IMyMotorSuspension> wheels = new List<IMyMotorSuspension>();

public Program()
{
    wheels = new List<>();
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(blocks);
    foreach(IMyTerminalBlock block in blocks){
        if(block.CustomName.Contains("[LG]")){
            wheels.Add((IMyMotorSuspension)block);
        }
    }
}


public void Main(string args, UpdateType updateSource)
{
    foreach(IMyMotorSuspension wheel in wheels){
        if(wheel != null){
            List<Sandbox.ModAPI.Interfaces.ITerminalProperty> PropList=new List<Sandbox.ModAPI.Interfaces.ITerminalProperty>();
            wheel.GetProperties(PropList);
            foreach(ITerminalProperty Prop in PropList){
                if(Prop.TypeName == "Single") {
                    //block.SetValue<float>(Prop.Id,rnd.Next((int)(block.GetMinimum<float>(Prop.Id)*100),(int)(block.GetMaximum<float>(Prop.Id)*100))/100);
                    Echo(Prop.Id + " : " + block.GetValue<float>(Prop.Id));
                    //not done, need a small script to get those properties quicker
                } 
            }
        }
    }
}