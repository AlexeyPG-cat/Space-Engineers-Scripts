//sender

public void Main(string args)
{
string tag = "EmergencyControlSystem";
    
    long reciever = 0L;
    long.TryParse(Me.CustomData,out reciever);
    if(reciever == 0L){
        Echo("Put reciever id in custom data!");
    }

    ImmutableArray<string> data = ImmutableArray.Create<string>("EmergencyAction","Preset1",args);
    IGC.SendUnicastMessage<ImmutableArray<string>>(reciever,tag,data);
}