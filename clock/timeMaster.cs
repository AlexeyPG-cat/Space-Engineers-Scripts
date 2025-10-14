IMyTextSurfaceProvider display1;
IMySolarPanel panel;
IMyMotorStator rotor;

MyIni _ini = new MyIni();

int tick = 0;
string display = "";

int day;
int dayTick;
float dayStartAt;
float dayEndAt;
bool newDay = true;

int dayStartTick = 0;
int dayEndTick = 0;

bool inverse = false;
float prevAngle = 0;
float angle = 0;

bool passDayStart = false;
bool passDayEnd = false;

int lastDayTick = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100; //solar panels update at same time
    display1  = (IMyTextSurfaceProvider)Me;
    rotor = (IMyMotorStator)GridTerminalSystem.GetBlockWithName("ClockRotor");
    panel = (IMySolarPanel)GridTerminalSystem.GetBlockWithName("Panel");
    Load();
    passDayStart = true;
    passDayEnd = true;
}


public void Main(string args, UpdateType updateSource)
{
    if(prevAngle < angle){inverse = false;}
    angle = (float)(rotor.Angle * 180/Math.PI);
    if(prevAngle > angle){inverse = true;}
    prevAngle = angle;
    if(inverse){angle = (angle-360)*-1;}



    if(updateSource == UpdateType.Update100 || updateSource == UpdateType.Update10){
        Echo("" + (tick++ + 1));
        dayTick++;
    }
    args = args.ToLower().Trim();
    if(args == "setdaystart"){
        dayStartAt = angle;
    }
    if(args == "setdayend"){
        dayEndAt = angle;
    }

    if(newDay && angle < 10){
        newDay = false;
        lastDayTick = dayTick;
        dayTick = 0;
        passDayStart = false;
        passDayEnd = false;
        Me.CustomData += System.Environment.NewLine + ":" + lastDayTick;
    }
    if(!newDay){
        if(angle > 10){
            newDay = true;
        }
    }

    if(angle > dayStartAt && !passDayStart){
        passDayStart = true;
        dayStartTick = dayTick;
        day++;
    }
    if(angle > dayEndAt && !passDayEnd){
        passDayEnd = true;
        dayEndTick = dayTick;
    }

    dAdd("Server: " + IGC.Me);
    dAdd("Tick: " + tick + " day: " + day + " dTick: " + dayTick);
    dAdd("dayStart: " + dayStartAt + " dayEnd: " + dayEndAt);
    dAdd("angle: " + angle + " inverse: " + inverse);
    dAdd("startTick: " + dayStartTick + " endTick = " + dayEndTick);
    dAdd("length: " + lastDayTick);



    IMyUnicastListener Listener = IGC.UnicastListener as IMyUnicastListener;
    while(Listener.HasPendingMessage)
    {
        long sender = 0L;
        
        try{
            MyIGCMessage message = Listener.AcceptMessage();
            sender = message.Source;
            ImmutableArray<string> arr = (ImmutableArray<string>)message.Data;
            if(arr[0]=="TimeClient" || true){
                if(arr[1]=="sendSycnData"){
                    ImmutableArray<string> data = ImmutableArray.Create<string>("Time master","Time data","1",""+dayTick,""+lastDayTick,""+dayStartTick,""+dayEndTick);
                    IGC.SendUnicastMessage<ImmutableArray<string>>(sender,"reply",data);
                } else {
                    ImmutableArray<string> data = ImmutableArray.Create<string>("Time master","error","Unknown reques");
                    IGC.SendUnicastMessage<ImmutableArray<string>>(sender,"reply",data);
                }
            }
        } catch {
            Echo("Incoming message error");
            ImmutableArray<string> data = ImmutableArray.Create<string>("Time master","error","Incorrect request");
            IGC.SendUnicastMessage<ImmutableArray<string>>(sender,"reply",data);
        }
    }

    

    
    //after all
    if(display1!=null){display1.GetSurface(0).WriteText(display, false);}
    display = "";
}

public void dAdd(string str){
    display = display + System.Environment.NewLine + str;
}

public void Save(){
    _ini.Clear();
    _ini.Set("data","tick",tick);
    _ini.Set("data","day",day);
    _ini.Set("data","dTick",dayTick);
    _ini.Set("data","dayStartTick",dayStartTick);
    _ini.Set("data","dayEndTick",dayEndTick);
    _ini.Set("data","dayStartAt",dayStartAt);
    _ini.Set("data","dayEndAt",dayEndAt);
    _ini.Set("data","prevAngle",prevAngle);
    _ini.Set("data","length",lastDayTick);
    Storage = _ini.ToString();
}
public void Load(){
    _ini.TryParse(Storage);
    tick = _ini.Get("data","tick").ToInt32(0);
    day = _ini.Get("data","day").ToInt32(0);
    dayTick = _ini.Get("data","dTick").ToInt32(0);
    dayStartTick = _ini.Get("data","dayStartTick").ToInt32(0);
    dayEndTick = _ini.Get("data","dayEndTick").ToInt32(0);
    dayStartAt = _ini.Get("data","dayStartAt").ToSingle(0f);
    dayEndAt = _ini.Get("data","dayEndAt").ToSingle(0f);
    prevAngle = _ini.Get("data","prevAngle").ToSingle(0f);
    lastDayTick = _ini.Get("data","length").ToInt32(0);
}