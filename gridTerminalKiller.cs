public void Main(string argument, UpdateType updateSource)
{
    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(allBlocks);
    allBlocks.Remove(Me);
    Random rnd = new Random();
    int r = rnd.Next(allBlocks.Count);
    int accessed = 0;int fails = 0;int changed = 0;int total = 0;
    IMyTerminalBlock selected = allBlocks[r];
    foreach(IMyTerminalBlock block in allBlocks){
        total++;
        try{
            List<Sandbox.ModAPI.Interfaces.ITerminalProperty> PropList=new List<Sandbox.ModAPI.Interfaces.ITerminalProperty>();
            block.GetProperties(PropList);
            foreach(ITerminalProperty Prop in PropList){
                if(Prop.TypeName == "Single") {
                    //block.SetValue<float>(Prop.Id,rnd.Next((int)(block.GetMinimum<float>(Prop.Id)*100),(int)(block.GetMaximum<float>(Prop.Id)*100))/100);
                    Echo(Prop.Id + " : " + block.GetValue<float>(Prop.Id));
                } 
            }
            accessed++;
        }catch{
            fails++;
        }
    }
    IMyTextSurfaceProvider disp = (IMyTextSurfaceProvider)Me;
    disp.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    disp.GetSurface(0).FontSize = 1.5f;
    disp.GetSurface(0).TextPadding = 30f;
    disp.GetSurface(0).Alignment = TextAlignment.CENTER;
    disp.GetSurface(0).WriteText("Changed " + changed + " settings" + System.Environment.NewLine + "Access " + accessed + "/" + total + " items" + System.Environment.NewLine + "Failed " + fails + " times", false);
    
}
//to do

/*
cool display
Check access
what is Int64?
*/