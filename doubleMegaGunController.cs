//Add your group names there:
string list1 = "RailGuns1Right";
string list2 = "RailGuns1Left";


public static WcPbApi api;
List<IMyTerminalBlock> guns0 = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> guns1 = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> guns2 = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
IMyTerminalBlock mainGun1;
IMyTerminalBlock mainGun2;
List<IMyTerminalBlock> guns1list;
List<IMyTerminalBlock> guns2list;
bool isFiring = false;
bool constFire = false;

List<IMyTerminalBlock> fireList = new List<IMyTerminalBlock>();






public Program(){
    api = new WcPbApi();
	api.Activate(Me);
    getGunsFromGroup(list1,list2);
    Echo(guns1.Count +  "+" + guns2.Count + " guns detected :3");
}

public void Main(string arg){
    if(arg == "FireAllOnce"){
        prepList();
        isFiring = true;
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }
    if(arg == "ConstFire"){
        constFire = !constFire;
        isFiring = constFire;
        if(constFire){prepList();Runtime.UpdateFrequency = UpdateFrequency.Update10;}
        else{Runtime.UpdateFrequency = UpdateFrequency.None;}
    }
    if(isFiring){
        if(!fireNext()){
            if(constFire){
                prepList();
            }
            else {
                isFiring = false;
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
        }
    }
}

public void prepList(){
    fireList = new List<IMyTerminalBlock>();
    for(int i = 0; i < (guns1.Count + guns2.Count);i++){
        if(i%2 == 0){
            if(guns1.Count > i/2) fireList.Add(guns1[i/2]); else fireList.Add(guns2[i/2]); // NOT TESTED!
        } else {
            if(guns2.Count > (i-1)/2) fireList.Add(guns2[(i-1)/2]); else fireList.Add(guns1[(i-1)/2]);
        }
    }
}
public bool fireNext(){
    if(fireList.Count > 0){
        try{
            if(fireList[0] != null) if(api.IsWeaponReadyToFire(fireList[0],0,false)){
                api.FireWeaponOnce(fireList[0]); 
                fireList.RemoveAt(0);
            }
        }
        return true;
    } else {
        return false;
    }
}



public void getGunsFromGroup(string group1Name, string group2Name){
    guns1 = new List<IMyTerminalBlock>();
    guns2 = new List<IMyTerminalBlock>();
    IMyBlockGroup guns1group = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName(group1Name);
    IMyBlockGroup guns2group = (IMyBlockGroup)GridTerminalSystem.GetBlockGroupWithName(group2Name);
    if(guns1group!=null){
        guns1list = new List<IMyTerminalBlock>();
        guns1group.GetBlocks(guns1list);
        foreach(IMyTerminalBlock gun in guns1list){
            guns1.Add(gun as IMyTerminalBlock);
        }
    }
    if(guns2group!=null){
        guns2list = new List<IMyTerminalBlock>();
        guns2group.GetBlocks(guns2list);
        foreach(IMyTerminalBlock gun in guns2list){
            guns2.Add(gun as IMyTerminalBlock);
        }
    }
}

public void getGuns(string mGun1, string mGun2){ //Useless ig
    guns0 = new List<IMyTerminalBlock>();
    guns1 = new List<IMyTerminalBlock>();
    guns2 = new List<IMyTerminalBlock>();
    blocks = new List<IMyTerminalBlock>();
    mainGun1 = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(mGun1);
    mainGun2 = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(mGun2);
    
    if(mainGun1 != null || mainGun2 != null){
        GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks);
        guns0 = blocks.ConvertAll(x => (IMyTerminalBlock)x);
        Vector3D gun1pos = mainGun1.GetPosition();
        Vector3D gun2pos = mainGun2.GetPosition();
        foreach(IMyTerminalBlock gun in guns0){
            Vector3D lPos = gun.GetPosition();
            if(Vector3D.Distance(lPos,gun1pos) < Vector3D.Distance(lPos,gun2pos) && mainGun1.CubeGrid == gun.CubeGrid){
                guns1.Add(gun);
            }
            if(Vector3D.Distance(lPos,gun2pos) < Vector3D.Distance(lPos,gun1pos) && mainGun2.CubeGrid == gun.CubeGrid){
                guns2.Add(gun);
            }
        }
    }
}

public class WcPbApi
        {
            private Action<IMyTerminalBlock, IDictionary<MyDetectedEntityInfo, float>> _getSortedThreats;
            private Func<long, int, MyDetectedEntityInfo> _getAiFocus;
            private Func<IMyTerminalBlock, int, MyDetectedEntityInfo> _getWeaponTarget;
            private Action<IMyTerminalBlock, bool, int> _fireWeaponOnce;
            private Action<IMyTerminalBlock, bool, bool, int> _toggleWeaponFire;
            private Func<IMyTerminalBlock, int, bool, bool, bool> _isWeaponReadyToFire;
            private Func<long, bool> _hasGridAi;
            private Func<IMyTerminalBlock, bool> _hasCoreWeapon;

            public bool Activate(IMyTerminalBlock pbBlock)
            {
                var dict = pbBlock.GetProperty("WcPbAPI")?.As<IReadOnlyDictionary<string, Delegate>>().GetValue(pbBlock);
                if (dict == null) throw new Exception($"WcPbAPI failed to activate");
                return ApiAssign(dict);
            }

            public bool ApiAssign(IReadOnlyDictionary<string, Delegate> delegates)
            {
                if (delegates == null)
                    return false;
                AssignMethod(delegates, "GetSortedThreats", ref _getSortedThreats);
                AssignMethod(delegates, "GetAiFocus", ref _getAiFocus);
                AssignMethod(delegates, "GetWeaponTarget", ref _getWeaponTarget);
                AssignMethod(delegates, "FireWeaponOnce", ref _fireWeaponOnce);
                AssignMethod(delegates, "ToggleWeaponFire", ref _toggleWeaponFire);
                AssignMethod(delegates, "IsWeaponReadyToFire", ref _isWeaponReadyToFire);
                AssignMethod(delegates, "HasGridAi", ref _hasGridAi);
                AssignMethod(delegates, "HasCoreWeapon", ref _hasCoreWeapon);
                return true;
            }

            private void AssignMethod<T>(IReadOnlyDictionary<string, Delegate> delegates, string name, ref T field) where T : class
            {
                if (delegates == null)
                {
                    field = null;
                    return;
                }
                Delegate del;
                if (!delegates.TryGetValue(name, out del))
                    throw new Exception($"{GetType().Name} :: Couldn't find {name} delegate of type {typeof(T)}");
                field = del as T;
                if (field == null)
                    throw new Exception(
                        $"{GetType().Name} :: Delegate {name} is not type {typeof(T)}, instead it's: {del.GetType()}");
            }
            public void GetSortedThreats(IMyTerminalBlock pbBlock, IDictionary<MyDetectedEntityInfo, float> collection) =>
                _getSortedThreats?.Invoke(pbBlock, collection);
            public MyDetectedEntityInfo? GetAiFocus(long shooter, int priority = 0) => _getAiFocus?.Invoke(shooter, priority);
            public MyDetectedEntityInfo? GetWeaponTarget(IMyTerminalBlock weapon, int weaponId = 0) =>
                _getWeaponTarget?.Invoke(weapon, weaponId) ?? null;
            public void FireWeaponOnce(IMyTerminalBlock weapon, bool allWeapons = true, int weaponId = 0) =>
                _fireWeaponOnce?.Invoke(weapon, allWeapons, weaponId);
            public void ToggleWeaponFire(IMyTerminalBlock weapon, bool on, bool allWeapons, int weaponId = 0) =>
                _toggleWeaponFire?.Invoke(weapon, on, allWeapons, weaponId);
            public bool IsWeaponReadyToFire(IMyTerminalBlock weapon, int weaponId = 0, bool anyWeaponReady = true,
                bool shootReady = false) =>
                _isWeaponReadyToFire?.Invoke(weapon, weaponId, anyWeaponReady, shootReady) ?? false;
            public bool HasGridAi(long entity) => _hasGridAi?.Invoke(entity) ?? false;
            public bool HasCoreWeapon(IMyTerminalBlock weapon) => _hasCoreWeapon?.Invoke(weapon) ?? false;
        }