//sender

public void Main(string args)
{
string tag = "doorsystemV2";
    IGC.SendBroadcastMessage<Vector3D>(tag,Me.GetPosition());
    //hi
}