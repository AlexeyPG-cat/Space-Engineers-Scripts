//disco ball script
//by AlexeyPG

//Setup: add "script:discoball" in custom info
//for every lamp and LCD that you want to be affected
//by this script

//to automatically add CustomData for all LCDs and lamps
//run with argument: "autoadd" once
//WARNING: THIS WILL AFFECT ALL LCDS AND LAMPS EVEN THOUGH CONNECTORS! (not landing gears)

//other arguments:
//red, green, blue, white, black, bright, dark, white

//skip script counter
int skips = 0;
//0 - run every update
//1 - run once after 1 skips
//2 - run once after 2 skips
//3 - run once after 3 skips
//etc

//is probably terribly optimised but it is one of my first scripts

//20.10.2025 Ah yes multiple checks to terminal in UpdateFrequency 10, sounds bad... i'll fix that soon, i'll need it soon


Random rnd = new Random();
string type = "random";
int counter_s = 0;
int warn_t = 0;
bool arg_fail = false;


public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}

public void Main(string arg)
{
    bool autoadd = false;
    if(arg != "")
    {type = arg;}
    if(arg == "autoadd")
    {
        type = "random";
        autoadd = true;
    }
    
if(counter_s >= skips)
{

    counter_s = 0;


    
    if(warn_t > 10)
    {warn_t = 0;}
    warn_t++;

    int counter = 0;
    List<IMyTerminalBlock> lcds = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds);
    foreach(var lcd_ in lcds)
    {
        var lcd = lcd_ as IMyTextPanel;
        if(lcd != null)
        {
            if(lcd.CustomData.Contains("script:discoball"))
            {
                lcd.BackgroundColor = ColorSet();
                counter++;
            }
            else if(autoadd)
            {
                if(lcd.CustomData == "")
                {
                    lcd.CustomData = lcd.CustomData + "script:discoball-autoadd";
                }
                else
                {
                    lcd.CustomData = lcd.CustomData + System.Environment.NewLine + "script:discoball-autoadd";
                }
                lcd.BackgroundColor = new Color(0,200,0,255);
                counter++;
            }
        }
    }
    Echo("[i] LCDs found: " + counter);
    counter = 0;
    
    List<IMyTerminalBlock> lamps = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lamps);
    foreach(var lamp_ in lamps)
    {
        var lamp = lamp_ as IMyLightingBlock;
        if(lamp != null)
        {
            if(lamp.CustomData.Contains("script:discoball"))
            {
                lamp.SetValue("Color", ColorSet());
                counter++;
            }
            else if(autoadd)
            {
                if(lamp.CustomData == "")
                {
                    lamp.CustomData = lamp.CustomData + "script:discoball-autoadd";
                }
                else
                {
                    lamp.CustomData = lamp.CustomData + System.Environment.NewLine + "script:discoball-autoadd";
                }
                lamp.SetValue("Color", new Color(0,200,0,255));
                counter++;
            }
        }
    }
    Echo("[i] Lamps found: " + counter);
    counter = 0;
    
    if(arg_fail)
    {
        Echo("[W] Can't find argument: " + type);
    }
}  
else
{
    counter_s++;
}
}

public Color ColorSet()
{
    //color set types
    arg_fail = false;
    if(type == "random")
    {
        return new Color(rnd.Next(1,255),rnd.Next(1,255),rnd.Next(1,255),255);
    }
    else if(type == "red")
    {
        return new Color(rnd.Next(100,255),rnd.Next(1,25),rnd.Next(1,25),255);
    }
    else if(type == "green")
    {
        return new Color(rnd.Next(1,25),rnd.Next(100,255),rnd.Next(1,25),255);
    }
    else if(type == "blue")
    {
        return new Color(rnd.Next(1,25),rnd.Next(1,25),rnd.Next(100,255),255);
    }
    else if(type == "white")
    {
        return new Color(255,255,255,255);
    }
    else if(type == "black")
    {
        return new Color(0,0,0,0);
    }
    else if(type == "bright")
    {
        return new Color(rnd.Next(100,255),rnd.Next(100,255),rnd.Next(100,255),255);
    }
    else if(type == "dark")
    {
        return new Color(rnd.Next(1,100),rnd.Next(1,100),rnd.Next(1,100),255);
    }
    else if(type == "white")
    {
        return new Color(255,255,255,255);
    }
    else
    {
        arg_fail = true;
        if(warn_t <= 4) // 5/20 or 1/4 red alarm
        {return new Color(200,0,0,255);}
        else
        {return new Color(1,1,1,255);}
    }
}