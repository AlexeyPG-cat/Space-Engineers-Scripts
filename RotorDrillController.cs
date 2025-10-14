//H2 rotor master
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;

}


public void Main(string args)
{
    IMyPistonBase piston1 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень 1п");
    IMyPistonBase piston2 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень 2п");
    IMyPistonBase piston3 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень 3п");
    IMyPistonBase piston4 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень 4п");
    IMyMotorStator rotor = (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Улучшенный ротор вращ");



    if(rotor.CustomData == "")
    rotor.CustomData = "0";
    if(rotor.Angle < Math.PI){
        if(int.Parse(rotor.CustomData)%2==0){
            rotor.CustomData = ""+(int.Parse(rotor.CustomData)+1);
        }
    }
    else{
        if(int.Parse(rotor.CustomData)%2==1){
            rotor.CustomData = ""+(int.Parse(rotor.CustomData)+1);
        }
    }
    float distance = int.Parse(rotor.CustomData)*1.5f;
    if(distance<40){
        piston1.MaxLimit = distance/4;
        piston2.MaxLimit = distance/4;
        piston3.MaxLimit = distance/4;
        piston4.MaxLimit = distance/4;
        piston1.Extend();
        piston2.Extend();
        piston3.Extend();
        piston4.Extend();

    }else{
        piston1.Retract();
        piston2.Retract();
        piston3.Retract();
        piston4.Retract();
    }
    Echo("Lap: " + int.Parse(rotor.CustomData));
    Echo("Distance: " + distance);
}