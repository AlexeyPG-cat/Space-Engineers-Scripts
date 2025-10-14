//remote door opener with distance
public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}

public void Main(string args)
{

    string tag = "doorsystemV2";
    IMyBroadcastListener Listener = IGC.RegisterBroadcastListener(tag) as IMyBroadcastListener;
    IMyProgrammableBlock programm_block = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("Программируемый блок");


    if(Listener.HasPendingMessage)
    {
        MyIGCMessage message = Listener.AcceptMessage();
        double distance = Vector3D.Distance(programm_block.GetPosition(), (Vector3D)message.Data);
        if(distance < 15)
        {
            //code here
        }

    }
}