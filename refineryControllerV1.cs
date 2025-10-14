// Refinery Controller V1 By AlexeyPG
//
// How to use:
//
// Use argument to execute commands:
// 
// auto <Ore> Automaticly finds refineries, clears them and places <Ore>
// 
// scan - Checks all inventories and if they are connected to refineries. Finds refineries.
// set <Ore> - Selects ore resource to work with. Example: "set Iron"
// replace - Clears scanned refineries and places selected resource
// clear - Removes every resource from scanned refineries input
// fill - Fills scanned refineries with selected resource
//
// Currently script does not care about conveyor grids and does not put items equally between all refinries.


public List<IMyTerminalBlock> blocks;
public List<IMyInventory> invs;
public List<IMyTerminalBlock> refineries = new List<IMyTerminalBlock>{};
public MyItemType resource = new MyItemType{};
public List<MyInventoryItem> resources = new List<MyInventoryItem>{};

//public Program(){Runtime.UpdateFrequency = UpdateFrequency.Update100;} nah use fucking timers for this shit

public void Main(string argument, UpdateType updateSource)
{
    if(argument == "scan"){
        scan();
    }
    if(argument == "fill"){
        findResources();
    }
    if(argument == "clear"){
        emptyRefs();
    }
    if(argument == "replace"){
        emptyRefs();
        findResources();
    }
    if(argument.StartsWith("set ")){
            resource = new MyItemType("MyObjectBuilder_Ore",argument.Substring("set ".Length));
            Echo("Resource Type Set: " + resource.SubtypeId);
    }
    if(argument.StartsWith("auto ")){
        scan();
        resource = new MyItemType("MyObjectBuilder_Ore",argument.Substring("auto ".Length));
        emptyRefs();
        findResources();
    }

    
}
public void emptyRefs(){
    foreach(IMyTerminalBlock refinery_ in refineries){
        IMyRefinery refinery = refinery_ as IMyRefinery;
        IMyInventory RefInp = refinery.InputInventory;
        foreach(IMyInventory inv in invs){
            if(RefInp.CurrentVolume.RawValue == 0) break;
            long freeSpace = inv.MaxVolume.RawValue - inv.CurrentVolume.RawValue;
            if(freeSpace != 0){
                List<MyInventoryItem> itemsInRef = new List<MyInventoryItem>{};
                RefInp.GetItems(itemsInRef);
                foreach(MyInventoryItem item in itemsInRef){
                    RefInp.TransferItemTo(inv,item);
                }
            }
        }
    }
}
public void findResources(){

    try{
    foreach(IMyInventory inv in invs){
        List<MyInventoryItem> items = new List<MyInventoryItem>{};
        inv.GetItems(items);
        foreach(MyInventoryItem item in items){
            if(item.Type == resource){
                foreach(IMyTerminalBlock refinery in refineries){
                    IMyInventory RefInp = (refinery as IMyRefinery).InputInventory;
                    try{RefInp.TransferItemFrom(inv,item);}catch{}
                }
            }
        }
    }
    Echo("Done!");
    } catch {Echo("Error!");}
}
public void scan(){
    blocks = new List<IMyTerminalBlock>{};
        invs = new List<IMyInventory>{};
        refineries = new List<IMyTerminalBlock>{};

        GridTerminalSystem.GetBlocks(blocks);
        foreach(IMyTerminalBlock block in blocks){
            if(block != null)
            if(block is IMyRefinery){
                if(!block.CustomInfo.Contains("Shield")){
                    refineries.Add(block);
                }
            } else if(block.HasInventory){
                for(int i = 0; i < block.InventoryCount; i++){
                    bool connected = false;
                    foreach(IMyTerminalBlock refinery in refineries){
                        if(refinery.GetInventory().IsConnectedTo(block.GetInventory(i))){
                            connected = true; break;
                        }
                    }
                    if(connected) invs.Add(block.GetInventory(i));
                }
            }
        }
        Echo(invs.Count + " total inventories");
        Echo(refineries.Count + " total refineries");
}
