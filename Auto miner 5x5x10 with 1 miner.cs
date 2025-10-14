public Program()
{Runtime.UpdateFrequency = UpdateFrequency.Update10;}

int progress = 0;
int progress_y = 0;


//--setup
// Need 2 pistons called "Поршень X" and "Поршень Y" with horizontal nav
// Need 2 pistons after each other called "Поршень вниз 1" and "Поршень вниз 2"
// Programmable block must be called "programm"
// speed on x piston must be 2 and y = 0.5
// speed on vertical pistons controlled by a programm
// to start mining write "Start" in custom data tab


public void Main(string argument)
{
    IMyPistonBase piston_x = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень X");
    IMyPistonBase piston_y = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень Y");
    IMyPistonBase piston_d1 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень вниз 1");
    IMyPistonBase piston_d2 = (IMyPistonBase)GridTerminalSystem.GetBlockWithName("Поршень вниз 2");
    IMyProgrammableBlock programm = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("programm");

    float x_pos = piston_x.CurrentPosition;
    float y_pos = piston_y.CurrentPosition;
    float z_pos =  piston_d1.CurrentPosition + piston_d2.CurrentPosition;

    
    Echo("Progress: " + progress.ToString());
    Echo("Progress_y: " + progress_y.ToString());

    
    if(argument != "")
    {
        progress = Int32.Parse(argument);
    }


    if(programm.CustomData == "Start")
    {


        //speed controller
        if(piston_d1.Status == PistonStatus.Extending)
        {
            piston_d1.Velocity = 0.25f; //-0.25 to go down
            piston_d2.Velocity = 0.25f;
        }
        else
        {
            piston_d1.Velocity = 1.5f;
            piston_d2.Velocity = 1.5f;
        }


        //Y controller
        if(progress_y == 0)
        {
            piston_y.MaxLimit = 0f;
            piston_y.Retract();
        }
        if(progress_y == 1)
        {
            piston_y.MaxLimit = 3.33f;
            piston_y.Extend();
        }
        if(progress_y == 2)
        {
            piston_y.MaxLimit = 6.67f;
            piston_y.Extend();
        }
        if(progress_y == 3)
        {
            piston_y.MaxLimit = 10f;
            piston_y.Extend();
        }

        //x controller
        if(progress == 0) 
        {
            if(x_pos < 0.1f & z_pos < 0.1f)
            {
                Echo("progress_up");
                if(progress_y == 4)
                {
                    Echo("Done!");
                }
                else
                {
                progress = 1;
                }
            }
            else //go to pos
            {
                Echo("retracting");
                piston_x.Retract();
                piston_d1.Retract();
                piston_d2.Retract();
            }
        }

        if(progress == 1)
        {
            if(z_pos > 19.9f)
            {
                progress = 2;
            }
            else // mine down
            {
                piston_d1.Extend();
                piston_d2.Extend();
            }
        }

        if(progress == 2)
        {
            if(z_pos < 0.1f)
            {
                progress = 3;
            }
            else // get up
            {
                piston_d1.Retract();
                piston_d2.Retract();
            }
        }

        if(progress == 3)
        {
            if(x_pos > 3.2f)
            {
                progress = 4;
            }
            else // move to x = 3.33
            {
                piston_x.MaxLimit = 3.33f;
                piston_x.Extend();
                piston_d1.Retract();
                piston_d2.Retract();
            }
        }
        

        if(progress == 4)
        {
            if(z_pos > 19.9f)
            {
                progress = 5;
            }
            else // mine down
            {
                piston_d1.Extend();
                piston_d2.Extend();
            }
        }

        if(progress == 5)
        {
            if(z_pos < 0.1f)
            {
                progress = 6;
            }
            else // get up
            {
                piston_d1.Retract();
                piston_d2.Retract();
            }
        }

        if(progress == 6) 
        {
            if(x_pos > 6.6f & z_pos < 0.1f)
            {
                Echo("progress_up");
                progress = 7;
            }
            else //go to pos
            {
                Echo("retracting");
                piston_x.MaxLimit = 6.67f;
                piston_x.Extend();
                piston_d1.Retract();
                piston_d2.Retract();
            }
        }

        if(progress == 7)
        {
            if(z_pos > 19.9f)
            {
                progress = 8;
            }
            else // mine down
            {
                piston_d1.Extend();
                piston_d2.Extend();
            }
        }

        if(progress == 8)
        {
            if(z_pos < 0.1f)
            {
                progress = 9;
            }
            else // get up
            {
                piston_d1.Retract();
                piston_d2.Retract();
            }
        }

        if(progress == 9) 
        {
            if(x_pos > 9.9f & z_pos < 0.1f)
            {
                Echo("progress_up");
                progress = 10;
            }
            else //go to pos
            {
                Echo("retracting");
                piston_x.MaxLimit = 10f;
                piston_x.Extend();
                piston_d1.Retract();
                piston_d2.Retract();
            }
        }

        if(progress == 10)
        {
            if(z_pos > 19.9f)
            {
                progress = 11;
            }
            else // mine down
            {
                piston_d1.Extend();
                piston_d2.Extend();
            }
        }

        if(progress == 11)
        {
            if(z_pos < 0.1f)
            {
                progress = 0;
                progress_y++;
            }
            else // get up
            {
                piston_d1.Retract();
                piston_d2.Retract();
            }
        }
    }
}