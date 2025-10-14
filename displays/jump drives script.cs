public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}

string toDisplay = "";

public void Main(string args)
{
    IMyJumpDrive jump1 = GridTerminalSystem.GetBlockWithName("! Прыжковый двигатель 1") as IMyJumpDrive;
    IMyJumpDrive jump2 = GridTerminalSystem.GetBlockWithName("! Прыжковый двигатель 2") as IMyJumpDrive;
    bool isjumping = false;
  
    display_add("Jump drives:");
    if(jump1 != null)
    {
        if(jump1.IsWorking)
        {
            if(jump1.Status == MyJumpDriveStatus.Ready)
            {
                display_add("JD 1 - Ready!");
            }
                if(jump1.Status == MyJumpDriveStatus.Jumping)
            {
                display_add("JD 1 - JUMPING!");
                isjumping = true;
            }
            if(jump1.Status == MyJumpDriveStatus.Charging)
            {
                if(jump1.Recharge)
                {
                    display_add("JD 1 - + " + (jump1.CurrentStoredPower/0.03).ToString("#") + "%");
                }
                else
                {
                    display_add("JD 1 - X " + (jump1.CurrentStoredPower/0.03).ToString("#") + "%");
                }
            }
        }
        else
        {
            if(jump1.Enabled)
            {
            display_add("JD 1 - Fail!");
            }
            else
            {
            display_add("JD 1 - Off");
            }
        }
    }
    else
    {
        display_add("JD 1 - Not found!");
    }




    if(jump2 != null)
    {
        if(jump2.IsWorking)
        {
            if(jump2.Status == MyJumpDriveStatus.Ready)
            {
                display_add("JD 2 - Ready!");
            }
                if(jump2.Status == MyJumpDriveStatus.Jumping)
            {
                display_add("JD 2 - JUMPING!");
                isjumping = true;
            }
            if(jump2.Status == MyJumpDriveStatus.Charging)
            {
                if(jump2.Recharge)
                {
                    display_add("JD 2 - + " + (jump2.CurrentStoredPower/0.03).ToString("#") + "%");
                }
                else
                {
                    display_add("JD 2 - X " + (jump2.CurrentStoredPower/0.03).ToString("#") + "%");
                }
            }
        }
        else
        {
            if(jump2.Enabled)
            {
            display_add("JD 2 - Fail!");
            }
            else
            {
            display_add("JD 2 - Off");
            }
        }
    }
    else
    {
        display_add("JD 2 - Not found!");
    }
 
    alarm(isjumping);
    
    IMyTextPanel panel = GridTerminalSystem.GetBlockWithName("Jump drive status display") as IMyTextPanel;
    if(panel != null){
    panel.WriteText(toDisplay);}

    IMyTextSurfaceProvider display_seat = (IMyTextSurfaceProvider)GridTerminalSystem.GetBlockWithName("! Кресло пилота J-3");
    if(display_seat != null)
    {
        display_seat.GetSurface(2).WriteText(toDisplay, false);
    }
    
    toDisplay = "";


}


public void display_add(string add)
{
    toDisplay = toDisplay + add + System.Environment.NewLine;
}
public void alarm(bool activate)
{
    
    if(activate)
    {
        Echo("alarm on!");
    }
    else
    {
        Echo("alarm off!");
    }
}