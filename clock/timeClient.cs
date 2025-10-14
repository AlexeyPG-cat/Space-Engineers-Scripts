long server = 0L;
IMyTextSurface _drawingSurface;
RectangleF _viewport;
int tick = 0;

MyIni _ini = new MyIni();

//save
int dayTick = 0;
int dayLength = 0;
int startAfter = 0;
int endAfter = 0;
int restarts = 0;


//no save
int messagesLost = 0;
bool accurate = false;

float pos = 0;
bool isDay = false;
string clockDisplay = "--:--";
float lineStart = 0;
float lineEnd = 0;


public Program()
{
    accurate = false;
    _drawingSurface = Me.GetSurface(0);
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    _viewport = new RectangleF(
        (_drawingSurface.TextureSize - _drawingSurface.SurfaceSize) / 2f,
        _drawingSurface.SurfaceSize
    );

 
    // Make the text surface display sprites
    PrepareTextSurfaceForSprites(_drawingSurface);
    Load();
    if(restarts > 10){
        dayTick = 0;
        dayLength = 0;
        startAfter = 0;
        endAfter = 0;
    }
}

public void Main(string args, UpdateType updateSource)
{
    Echo("us: "+updateSource);
    tick++;
    if(dayLength != 0){
        if(dayTick + 1 < dayLength){
            dayTick++;
        } else {
            dayTick = 0;
        }
    }
        long.TryParse(Me.CustomData,out server);
        if(server == 0L){
            Echo("Put server id in custom data!");
        }
        ImmutableArray<string> data = ImmutableArray.Create<string>("TimeClient","sendSycnData");
        IGC.SendUnicastMessage<ImmutableArray<string>>(server,"request",data);
        messagesLost++;

    IMyUnicastListener Listener = IGC.UnicastListener as IMyUnicastListener;
    if(Listener.HasPendingMessage)
    {
        long sender = 0L;
        
        try{
            MyIGCMessage message = Listener.AcceptMessage();
            sender = message.Source;
            ImmutableArray<string> arr = (ImmutableArray<string>)message.Data;
            if(arr[0]=="TimeServer" || true){
                if(arr[1]=="Time data"){
                    if(arr[2]=="1"){
                        dayTick = int.Parse(arr[3]);
                        dayLength = int.Parse(arr[4]);
                        startAfter = int.Parse(arr[5]);
                        endAfter = int.Parse(arr[6]);
                        restarts = 0;
                        messagesLost = 0;
                        accurate = true;

                        if(dayLength == 0){
                            dayTick = 0;
                            dayLength = 0;
                            startAfter = 0;
                            endAfter = 0;
                            clockDisplay = "Sv:Nr";
                            accurate = false;
                        }
                    } else {
                        Echo("Protocol version mismatch");
                    }
                } else {
                    Echo("Server replied nonsence!");
                }
            }
        } catch {
            Echo("Incoming message error");
        }
    }



    int localDayTick = dayTick - startAfter;
    if(localDayTick < 0) localDayTick += dayLength;
    int localDayEnd = endAfter - startAfter;
    if(localDayEnd < 0) localDayEnd += dayLength;
    Echo("Since day start: "+localDayTick);
    Echo("Day end at: " + localDayEnd);
    Echo("Length: " + dayLength);

    if(localDayEnd!=0)pos = Math.Min((float)localDayTick/(float)localDayEnd,1);
    isDay = (localDayTick < localDayEnd);
    
    if(dayLength != 0) if(isDay){
        clockDisplay = formatTime((int)((localDayEnd-localDayTick)*1.66f));
    } else {
        clockDisplay = formatTime((int)(((localDayTick-localDayEnd)-(dayLength-localDayEnd))*-1.66f));
    }
    if(localDayEnd!=0) lineStart = Math.Max(Math.Min((float)(localDayTick-localDayEnd)/(dayLength-localDayEnd),1),0);
    lineEnd = pos;


    var frame = _drawingSurface.DrawFrame();

    DrawSprites(ref frame);

    frame.Dispose();
}


public void Save(){
    _ini.Clear();

    _ini.Set("data","dayTick",dayTick);
    _ini.Set("data","dayLength",dayLength);
    _ini.Set("data","startAfter",startAfter);
    _ini.Set("data","endAfter",endAfter);
    _ini.Set("data","restarts",restarts);

    Storage = _ini.ToString();
}
public void Load(){
    _ini.TryParse(Storage);

    dayTick = _ini.Get("data","dayTick").ToInt32(0);
    dayLength = _ini.Get("data","dayLength").ToInt32(0);
    startAfter = _ini.Get("data","startAfter").ToInt32(0);
    endAfter = _ini.Get("data","endAfter").ToInt32(0);
    restarts = _ini.Get("data","restarts").ToInt32(0);
    restarts++;
}


public string formatTime(int seconds){
    return (seconds/60).ToString("00") + ":" + (seconds%60).ToString("00");
}


public void DrawSprites(ref MySpriteDrawFrame frame)
{

    var ground = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SemiCircle",
        Position = new Vector2(_viewport.Center.X,_viewport.Position.Y+(_viewport.Size.Y/3f*2)+(_viewport.Size.Y/2)),
        Size = _viewport.Size,
        Color = _drawingSurface.ScriptForegroundColor,
        Alignment = TextAlignment.CENTER
    };
    
    frame.Add(ground);

    var trajectory = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "CircleHollow",
        Position = new Vector2(_viewport.Center.X,_viewport.Position.Y+(_viewport.Size.Y/3f*2)+(_viewport.Size.Y/2)),
        Size = _viewport.Size*1.5f,
        Color = new Color(150,150,150),
        Alignment = TextAlignment.CENTER
    };
    
    frame.Add(trajectory);


    float angle = pos*120-60;
    Vector2 pos1 = new Vector2((float)Math.Sin(angle*Math.PI/180f),(float)Math.Cos(angle*Math.PI/180f));


    var sun = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "Circle",
        Position = new Vector2(_viewport.Center.X+((pos1.X)*_viewport.Size.X*0.725f),(_viewport.Position.Y+(_viewport.Size.Y/3f*2)+(_viewport.Size.Y/2))-(pos1.Y*_viewport.Size.Y*0.725f)), //(_viewport.Size.Y))-(pos1.Y*_viewport.Size.Y*1.5f)
        Size = _viewport.Size*0.2f,
        Color = new Color(255,255,0),
        Alignment = TextAlignment.CENTER
    };
    frame.Add(sun);

    if(messagesLost > 2){
        var cross = new MySprite()
        {
        Type = SpriteType.TEXTURE,
        Data = accurate ? "Cross" : "Danger",
        Position = new Vector2(_viewport.Center.X,_viewport.Position.Y+(_viewport.Size.Y/3f*2)),
        Size = _viewport.Size*0.3f,
        Color = new Color(200,0,0),
        Alignment = TextAlignment.CENTER
        };
        
        frame.Add(cross);
    }

    if(isDay){
        
    var timeText = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = clockDisplay,
        Position = new Vector2(((_viewport.Size.X/4*3)/2),(_viewport.Size.Y/12)),
        
        RotationOrScale = _viewport.Size.X/100,
        Color = Color.White,
        Alignment = TextAlignment.CENTER,
        FontId = "White"
    };
    
    frame.Add(timeText);

        var arrow = new MySprite()
        {
        Type = SpriteType.TEXTURE,
        Data = "Triangle",
        Position = new Vector2((_viewport.Size.X/6*5),(_viewport.Size.Y/12*3)),
        Size = new Vector2(_viewport.Size.X*0.25f,_viewport.Size.Y*-0.15f),
        Color = new Color(255,255,255),
        Alignment = TextAlignment.CENTER
        };
        
        frame.Add(arrow);

    } else {
        var timeText = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = clockDisplay,
        Position = new Vector2(((_viewport.Size.X/4*3)/2)-_viewport.Size.X/4 *-1,(_viewport.Size.Y/12)),
        RotationOrScale = _viewport.Size.X/100,
        Color = Color.White,
        Alignment = TextAlignment.CENTER,
        FontId = "White"
    };
    
    frame.Add(timeText);
        var arrow = new MySprite()
        {
        Type = SpriteType.TEXTURE,
        Data = "Triangle",
        Position = new Vector2((_viewport.Size.X/6),(_viewport.Size.Y/12*3)),
        Size = new Vector2(_viewport.Size.X*0.25f,_viewport.Size.Y*0.15f),
        Color = new Color(255,255,255),
        Alignment = TextAlignment.CENTER
        };
        frame.Add(arrow);
    }

    var progressbar = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = new Vector2(_viewport.Center.X,(_viewport.Position.Y+_viewport.Size.X/15f*10f)),
        Size = new Vector2(_viewport.Size.X*0.95f,_viewport.Size.Y/15),
        Color = new Color(150,150,150),
        Alignment = TextAlignment.CENTER
    };
    frame.Add(progressbar);

    if(lineEnd == 1f){
    var progressbarstate = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = new Vector2(_viewport.Center.X+(((1f-(lineEnd-lineStart)))*(_viewport.Size.X*0.95f)/2),(_viewport.Position.Y+_viewport.Size.X/15f*10F)), 
        Size = new Vector2((_viewport.Size.X*0.95f)*(lineEnd-lineStart),_viewport.Size.Y/15),
        Color = new Color(255,255,0),
        Alignment = TextAlignment.CENTER
    };
    frame.Add(progressbarstate);
    } else {
    var progressbarstate = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = new Vector2(_viewport.Center.X+(((1f-(lineEnd-lineStart))*-1)*(_viewport.Size.X*0.95f)/2),(_viewport.Position.Y+_viewport.Size.X/15f*10F)), 
        Size = new Vector2((_viewport.Size.X*0.95f)*(lineEnd-lineStart),_viewport.Size.Y/15),
        Color = new Color(255,255,0),
        Alignment = TextAlignment.CENTER
    };
    frame.Add(progressbarstate);
    }
    
}





// Auto-setup text surface
public void PrepareTextSurfaceForSprites(IMyTextSurface textSurface)
{
    textSurface.ContentType = ContentType.SCRIPT;
    // Make sure no built-in script has been selected
    textSurface.Script = "";
}