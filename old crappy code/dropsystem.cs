public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}

public void Main(string args)
{
    string tag = "dropsystem";
    string password = "456";

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    IMyBroadcastListener Listener = IGC.RegisterBroadcastListener(tag) as IMyBroadcastListener;
    IMyBeacon beacon = GridTerminalSystem.GetBlockWithName("Маяк") as IMyBeacon;
    
  
    MyIGCMessage message;
    message = new MyIGCMessage ();
    

    if(Listener.HasPendingMessage)
    {   
        message = Listener.AcceptMessage();
        if(message.Data as string == password)
        {
            beacon.Radius = 200000;
        }
        if(message.Data as string == password + "_kill")
        {
            beacon.Radius = 1;
        }
    }
}
