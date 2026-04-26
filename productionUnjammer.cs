// Production unjammer script by AlexeyPG
// Version 1
// Prevents assemblers from stopping when their inventory is overfilled by clearing them
// barely tested, hopefully optimized

public int scanInterval = 0;
// After how many 100 ticks programs scans whole grid to find production blocks and inventories
// WARNING! Grids with large amount of functional blocks may heavily affect game simulation speed, scans may cause significant lag spikes
// 10 - every 10th tick (16.6 sec)
// 0 - scan only once when script recompiled or world started

public int clearInterval = 3;
// After how many 100 ticks program checks every scanned assembler for being overfilled
// 1 - every tick (1.6 sec)
// 2 - every 2nd tick
// 5 - every 5th tick

public float clearRatio = 0.75f;
// Ratio over which production block will be emptied
// 1f - only 100% full inventory (not recommended)
// 0.75f - if 3000L (75%) of 4000L inventory is filled it will be cleared

public bool onlyProgramGrid = true;
// Scan inventories only on a grid where program block is placed
// true - only a grid with program block will be scanned and inventories from other grids won't be used
// false - inventory from any connected grid can send items to any other connected grid

public bool ignoreRefineries = true;
// Refineries are also production blocks
// They usually clear on their own and will refine anything they can accept, so there is no need to think about them



public List<IMyTerminalBlock> blocks;
public List<IMyInventory> invs;
public List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>{};
public int tick = 0;
public bool enoughSpace = true;

public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    scan();
}

public void Main(string argument, UpdateType updateSource)
{
    Echo("Operational for " + tick + " ticks");
    if(!enoughSpace) Echo("Not enough space");

    if(scanInterval != 0) if(tick % scanInterval == 0){
        scan();
    }
    if(clearInterval != 0) if(tick % clearInterval == 0){
        Echo("Clearing");
        bool foundAny = false;
        foreach(IMyTerminalBlock assembler in assemblers){
            for(int i = 0; i < assembler.InventoryCount; i++){
                IMyInventory inv = assembler.GetInventory(i);
                if(inv.VolumeFillFactor >= clearRatio){
                    Echo("Found overfilled production block " + assembler.CustomName);
                    foundAny = true;
                    clear(inv);
                }
            }
        }
        if(!foundAny){
            enoughSpace = true;
            Echo("Everything good!");
        } 
    }
    tick++;
}

public void clear(IMyInventory inv){
        List<MyInventoryItem> itemsInInv = new List<MyInventoryItem>{};
        inv.GetItems(itemsInInv);
            foreach(MyInventoryItem invItem in itemsInInv){
                //Echo("Searching for inventories with similar item");
                List<IMyInventory> sameItemInvs = new List<IMyInventory>{};
                foreach(IMyInventory inv_ in invs){
                    if(inv_.FindItem(invItem.Type)!=null){
                        sameItemInvs.Add(inv_);
                    }
                }
                while(sameItemInvs.Count != 0){
                    IMyInventory selInv = null;
                    long best = 0;
                    foreach(IMyInventory inv_ in sameItemInvs){
                        if(inv_.GetItemAmount(invItem.Type).RawValue >= best){
                            selInv = inv_;
                            best = inv_.GetItemAmount(invItem.Type).RawValue;
                        }
                    }
                    if(selInv !=null){
                        if(inv.CanTransferItemTo(selInv, invItem.Type)){
                            inv.TransferItemTo(selInv,invItem,invItem.Amount);
                        }
                        sameItemInvs.Remove(selInv);
                    }
                }
                if(inv.GetItemAmount(invItem.Type).RawValue > 0){
                    //Echo("Searching for filled inventories");
                    List<IMyInventory> fittingInvs = new List<IMyInventory>{};
                    foreach(IMyInventory inv_ in invs){
                        if(inv_.CanItemsBeAdded(invItem.Amount, invItem.Type)){
                            fittingInvs.Add(inv_);
                        }
                    }
                    while(fittingInvs.Count != 0){
                        IMyInventory selInv = fittingInvs[0];
                        long best = fittingInvs[0].CurrentVolume.RawValue;
                        foreach(IMyInventory inv_ in fittingInvs){
                            if((inv_.MaxVolume.RawValue - inv_.CurrentVolume.RawValue) <= best){
                                selInv = inv_;
                                best = (inv_.MaxVolume.RawValue - inv_.CurrentVolume.RawValue);
                            }
                        }
                        if(selInv !=null){
                            if(inv.CanTransferItemTo(selInv, invItem.Type)){
                            inv.TransferItemTo(selInv,invItem,invItem.Amount);
                        }
                        fittingInvs.Remove(selInv);
                        }
                    }
                }
                if(inv.GetItemAmount(invItem.Type).RawValue > 0){
                    //Echo("Searching for largest inventories");
                    List<IMyInventory> largestInvs = new List<IMyInventory>{};
                    foreach(IMyInventory inv_ in invs){
                        if(inv_.CanTransferItemTo(inv_, invItem.Type)){
                            largestInvs.Add(inv_);
                        }
                    }
                    while(largestInvs.Count != 0){
                        IMyInventory selInv = null;
                        long best = 0;
                        foreach(IMyInventory inv_ in largestInvs){
                            if(inv_.MaxVolume.RawValue >= best){
                                selInv = inv_;
                                best = inv_.MaxVolume.RawValue;
                            }
                        }
                        if(selInv !=null){
                            if(inv.CanTransferItemTo(selInv, invItem.Type)){
                            inv.TransferItemTo(selInv,invItem,invItem.Amount);
                        }
                        largestInvs.Remove(selInv);
                        }
                    }
                }
                if(inv.GetItemAmount(invItem.Type).RawValue > 0){
                    Echo("Not enough storage on grid!");
                    enoughSpace = false;
                } else {
                    Echo("Item cleared");
                    enoughSpace = true;
                }
            }
}

public void scan(){
        blocks = new List<IMyTerminalBlock>{};
        invs = new List<IMyInventory>{};
        assemblers = new List<IMyTerminalBlock>{};

        GridTerminalSystem.GetBlocks(blocks);
        foreach(IMyTerminalBlock block in blocks){
            if(block != null)
            if(block.CubeGrid == Me.CubeGrid || !onlyProgramGrid)
            if(block is IMyProductionBlock){
                if(block is IMyRefinery){
                    if(!ignoreRefineries) assemblers.Add(block);
                } else assemblers.Add(block);
            }
        }
        foreach(IMyTerminalBlock block in blocks){
            if(block.HasInventory && !(block is IMyProductionBlock)){
                for(int i = 0; i < block.InventoryCount; i++){
                    foreach(IMyTerminalBlock assembler in assemblers){
                        if(block.CubeGrid == Me.CubeGrid || !onlyProgramGrid) if(assembler.GetInventory().IsConnectedTo(block.GetInventory(i))){
                            invs.Add(block.GetInventory(i)); break;
                        }
                    }
                }
            }
        }
        Echo(invs.Count + " total inventories");
        Echo(assemblers.Count + " total production blocks");
}
