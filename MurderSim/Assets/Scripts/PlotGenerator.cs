using UnityEngine;
using System.Collections.Generic;

namespace MurderMystery {
    public enum Motives { none, revenge, loverRevenge, jealousLove, inheritance }

    [System.Serializable]
    public class Family {
        public string family_name;
        public List<Npc> family_members;
        public Npc husband;
        public Npc wife;
        public List<Npc> children;

        public Family(string name) {
            family_name = name;
            family_members = new List<Npc>();
            children = new List<Npc>();
        }
    }

    public class PlotGenerator : MonoBehaviour {

        public static Npc chosenNPC;

        public bool gameStarted = false;
        private float rumourSpreadChance = 0.4f;
        public bool debugMode;

        //References

        private ItemManager itemManager;
        private DebugRoomDisplay display;
        private Mansion mansion;
        private Transform npcHolder;

        //Characters and families
        public int number_of_characters = 8;
        private int max_families = 2;
        private int max_family_size = 3;
        private FamilyFeud feud = null;
        public List<Family> families;
        public List<Npc> npcs;
        private List<Npc> npcsWhoKnowTheTruth;
        public int[,] relationships;
        private readonly int nullRelationship = 100;

        public float kleptoChance = 0.05f;
        public float paranoidChance = 0.1f;
        public float minMeanderChance = 0.07f;
        public float maxMeanderChance = 0.3f;

        //Vars directly related to the murder
        public int timeSteps;
        private int timeUntilGameStart = 12;

        private History truth;
        public Motives motive;
        public Npc victim, murderer;
        public Weapon murderWeapon;
        public bool bodyFound;
        public bool weaponHidden;
        public int timeOfDeathLeeway; //The amount of time either side of the t.o.d estimate, so 30 means the estimate will be in the range of an hour

        //To be removed once a much better solution is found...
        private List<string> firstnames_m;
        private List<string> firstnames_f;
        private List<string> surnames;

        // Use this for initialization

        void Awake() {
            DontDestroyOnLoad(transform.gameObject);
        }

        [ContextMenu("Reset")]
        void Start() {
            Seed.setSeed();
            Ceilings.findCeilings();
            npcHolder = new GameObject("NPCS").transform;
            npcHolder.gameObject.AddComponent<DontDestroyOnLoad>();
            itemManager = GetComponent<ItemManager>();
            display = GetComponent<DebugRoomDisplay>();
            mansion = GetComponent<Mansion>();
            //mansion.setupRooms();

            families = new List<Family>();
            npcs = new List<Npc>();
            npcsWhoKnowTheTruth = new List<Npc>();

            relationships = new int[number_of_characters, number_of_characters];
            for (int i = 0; i < number_of_characters; i++) {
                for (int x = 0; x < number_of_characters; x++) {
                    relationships[i, x] = nullRelationship;
                }
            }

            victim = null; murderer = null;

            TestimonyManager.pg = this;
            loadNames();
            generateCharacters();
            gameObject.GetComponent<UIManager>().setupRelationPanel();
            createFamilies();
            if (motive == Motives.none) selectMotive();
            
            createRelationships();
            placeNPCs();
            prepareMotive();
            createRedHerrings();
            bodyFound = false;
            weaponHidden = false;
            timeSteps = 0;
            
        }

        void Update() {
            if (bodyFound && !gameStarted) timeUntilGameStart--;
            if (!gameStarted) timeSteps++;

            if (timeUntilGameStart <= 0 && !gameStarted) beginGame();
        }

        void beginGame() {
            gameStarted = true;
            GameObject.Find("Clock").GetComponent<Clock>().setClock();
            gameObject.GetComponent<BoardManager>().placeNPCs();
            gameObject.GetComponent<ItemManager>().placeItemsOnBoard();
            GameObject.Find("Player").GetComponent<playerControl>().LightRoom();

        }


        public void bodyWasFound() {
            bodyFound = true;
        }

        public void weaponWasHidden() {
            weaponHidden = true;
        }

        void placeWeaponAtMurderer() {
            itemManager.createItem(0, murderer.currentRoom);
        }

        //Called by conversation script when the player is locked out of speaking to an NPC. Checks that at least one NPC who knows the truth can still be spoken to, and if not distributes the truth again.
        public void checkNPCKnowledge() {
            Debug.Log("checking NPC knowledge");
            bool truthAccessible = false;
            foreach (Npc npc in npcsWhoKnowTheTruth) {
                if (npc.lieAccusations > 0 && npc.isAlive && !npc.isMurderer) {
                    truthAccessible = true;
                    Debug.Log(npc.firstname + " still knows the truth");
                }
                
            }

            if (!truthAccessible) {
                SpreadTruth();
                checkNPCKnowledge();
            }
        }

        void selectMotive() {
            int i;
            if (families.Count > 0) {
                i = Random.Range(0, 4);
            }
            else i = Random.Range(0, 3);

            switch (i) {
                case 0:
                    motive = Motives.revenge;       //Killing over an event in the murderer's history
                    break;
                case 1:
                    motive = Motives.loverRevenge;  //Killing because the victim refused to love them back
                    break;
                case 2:
                    motive = Motives.jealousLove;   //Killing because the NPC they love was in love with the victim
                    break;
                case 3:
                    motive = Motives.inheritance;   //Killing a family member for the sake of gaining inheritance, either from the victim or from someone who would've otherwise given money to that victim
                    break;
            }

            //Check that there is a suitable sibling pair for the inheritance plot
            if (motive == Motives.inheritance) {
                Debug.Log("checking if there are siblings for an inheritance plot");
                if (findSiblings()[0] != null) {
                    Debug.Log("Found suitable siblings for inheritance plot");
                    motive = Motives.inheritance;
                }
                else {
                    Debug.Log("No suitable siblings found for the inheritance plot, choosing new motive");
                    selectMotive();
                }
            }
        }

        void prepareMotive() {
            //revenge just requires one NPC to hate another
            if (motive == Motives.revenge) {
                int m = Random.Range(0, number_of_characters);
                setMurderer(npcs[m]);
                int v = 0;

                while (victim == null) {
                    v = Random.Range(0, number_of_characters);
                    if (npcs[v] != murderer) {
                        setVictim(npcs[v]);
                    }
                }
                

                //Make murderer hate victim
                relationships[m, v] = -3;
                bool historyChosen = false;

                while (!historyChosen) {
                    int r = Random.Range(0, 4);
                    switch (r) {
                        case 0: {
                                History history = new FiredBy(1, npcs[m], npcs[v]);
                                npcs[m].addHistory(history);
                                npcs[v].addHistory(history);
                                historyChosen = true;
                                SpreadTruth(history);
                                break;
                            }
                            
                        case 1:
                            {
                                //Histories added in the family feud method
                                if (npcs[m].family != null && npcs[v].family != null) {
                                    createFamilyFeud(npcs[m].family, npcs[v].family);
                                    historyChosen = true;
                                }
                                break;
                            }
                            
                        case 2:
                            {
                                History history = new PutOutOfBusiness(1, npcs[m], npcs[v]);
                                npcs[m].addHistory(history);
                                npcs[v].addHistory(history);
                                historyChosen = true;
                                SpreadTruth(history);
                                break;
                            }
                        case 3:
                            {
                                History history = new Nemeses(0, npcs[m], npcs[v]);
                                relationships[v, m] = -3;
                                npcs[m].addHistory(history);
                                npcs[v].addHistory(history);
                                historyChosen = true;
                                SpreadTruth(history);
                                break;
                            }

                    }
                    
                }
                
                
            }

            //inheritance for now just means an NPC killing their sibling to inherit from their (wealthy) parents later. Could later include murdering a spouse
            else if (motive == Motives.inheritance) {
                Npc[] siblings = findSiblings();
                setMurderer(siblings[0]);
                setVictim(siblings[1]);
            }

            else if (motive == Motives.loverRevenge) {
                setMurderer(npcs[Random.Range(0, npcs.Count)]);
                setVictim(findPotentialLover(murderer));

                //Now manipulate the m & v's affections for eachother
                int m = npcs.IndexOf(murderer);
                int v = npcs.IndexOf(victim);

                relationships[m, v] = 3;
                relationships[v, m] = randomRelationshipValue(-3, 2, 0); //Set to anything OTHER than love

                int r = Random.Range(0, 2);
                History history;
                if (r == 0) {
                    history = new RejectedLove(1, npcs[m], npcs[v]);
                } else if (r == 1) {
                    history = new BadBreakup(0, npcs[m], npcs[v]);
                } else {
                    history = null;
                }
               
                npcs[m].addHistory(history);
                npcs[v].addHistory(history);
                SpreadTruth(history);
            }

            else if (motive == Motives.jealousLove) {
                //Randomly choose murderer
                //Find opposite gendered counterpart, same as with lover revenge
                //Give THEM a randomly chosen gendered counterpart and make them the victim
                setMurderer(npcs[Random.Range(0, npcs.Count)]);
                Npc stalkee = findPotentialLover(murderer);
                setVictim(findPotentialLover(stalkee));

                //Now manipulate the m & v's affections for eachother
                int m = npcs.IndexOf(murderer);
                int s = npcs.IndexOf(stalkee);
                int v = npcs.IndexOf(victim);

                relationships[m, s] = 3;
                relationships[s, m] = randomRelationshipValue(-3, 2, 0); //Set to anything OTHER than love
                relationships[v, s] = 3;
                relationships[v, m] = randomRelationshipValue(-3, 2, 0); //Set to anything other than love again
                relationships[m, v] = randomRelationshipValue(-3, -2, -2); //Make the murderer strongly dislike the victim

                int r = Random.Range(0, 2);

                History history;
                if (r == 0) {
                    relationships[s, v] = 3;
                    history = new StoleLover(1, npcs[m], npcs[v], npcs[s]);
                    npcs[m].addHistory(history); npcs[v].addHistory(history); npcs[s].addHistory(history);
                }
                else if (r == 1) {
                    history = new CompetingForLove(0, npcs[m], npcs[v], npcs[s]);
                    npcs[m].addHistory(history); npcs[v].addHistory(history);  npcs[s].addHistory(history);
                }
                else {
                    history = null;
                }
                SpreadTruth(history);

            }
        }

        void createRedHerrings() {
            int redHerrings = 0;
            int attempts = 0;
            int requiredRedHerrings = 2;

            float r = Random.Range(0.0f, 1.0f);
            if (r > 0.8 && r <= 1.0) requiredRedHerrings = 3;

            while (redHerrings < requiredRedHerrings && attempts < 100) {
                int random = Random.Range(0, 5);
                attempts++;

                switch (random) {
                    //Creates a family feud if there is an applicable family and no other feud
                    case 0:
                        {
                            if (victim.family != null && families.Count >= 2 && feud == null) {
                                Family targetFamily = null;
                                for (int i = 0; i < families.Count; i++) {
                                    if (families[i] != victim.family) targetFamily = families[i];
                                }

                                int disputes = createFamilyFeud(victim.family, targetFamily);
                                redHerrings += disputes;
                            }
                            break;
                        }
                    case 1:
                        {
                            if (createAndSpreadHistory(typeof(FiredBy))) redHerrings++;
                            break;
                        }
                    case 2:
                        {
                            if (createAndSpreadHistory(typeof(Nemeses))) redHerrings++;
                            break;
                        }
                    case 3:
                        {
                            if (createAndSpreadHistory(typeof(PutOutOfBusiness))) redHerrings++;
                            break;
                        }
                    case 4:
                        {
                            if (createAndSpreadHistory(typeof(BadBreakup))) redHerrings++;
                            break;
                        }
                }   //end switch
            }

        }
        
        bool createAndSpreadHistory(System.Type historyType) {

                bool suitableNPC = false;
                List<Npc> tempNpcs = new List<Npc>(npcs);

                while (!suitableNPC && tempNpcs.Count > 0) {
                    Npc npc = randomNPCfromList(tempNpcs);
                    tempNpcs.Remove(npc);

                    if (npc != victim && npc != murderer && npc.family != victim.family) {

                        suitableNPC = true;

                        History redHerring = null;
                        if (historyType == typeof(FiredBy))
                            redHerring = new FiredBy(1, npc, victim);
                        else if (historyType == typeof(Nemeses))
                            redHerring = new Nemeses(1, npc, victim);
                        else if (historyType == typeof(PutOutOfBusiness))
                            redHerring = new PutOutOfBusiness(1, npc, victim);
                        else if (historyType == typeof(BadBreakup))
                            redHerring = new BadBreakup(1, npc, victim);

                        //TODO - Try and make Love-based red herrings. Difficuly because need to find plausible lovers

                        npc.addHistory(redHerring);
                        victim.addHistory(redHerring);

                        SpreadRumour(redHerring, npc);
                        return true;
                    }
                }
            return false;
        }

        bool SpreadRumour(History history, bool truth = false, Npc npc = null) {
            bool rumourSpread = false;
            //Give unrelated NPCs memory of this event
            List<Npc> NpcCopy = new List<Npc>(npcs);
            NpcCopy.Remove(victim); NpcCopy.Remove(murderer); if (npc!=null) NpcCopy.Remove(npc);

            for (int i = 0; i < NpcCopy.Count; i++) {
                float random = Random.Range(0.0f, 1.0f);
                if (random < rumourSpreadChance) {
                    if (truth) npcsWhoKnowTheTruth.Add(NpcCopy[i]);
                    NpcCopy[i].addHistory(history);
                    rumourSpread = true;
                }
            }
            return rumourSpread;
        }

        void SpreadTruth(History history = null) {
            if (debugMode) Debug.Log("Spreading truth");
            if (history == null) history = truth;
            truth = history;
            bool truthSpread = false;

            while (truthSpread == false) {
                truthSpread = SpreadRumour(history, true);
            }
        }

        Npc findPotentialLover(Npc seeker) {
            List<Npc> potentialLovers = new List<Npc>();
            Npc lover = null;

            foreach (Npc npc in npcs) {
                if (npc != murderer && npc != seeker && npc.gender != seeker.gender && npc.family != seeker.family) {
                    potentialLovers.Add(npc);
                }
            }

            //If none in the right gender exist outside the seeker's family, then create one
            if (potentialLovers.Count == 0) {
                //Find a random NPC not in the seeker's family and not the possible murderer and make them the opposite gender
                while (lover == null || lover == seeker || lover.family == seeker.family || lover == murderer) {
                    lover = npcs[Random.Range(0, npcs.Count)];
                }
                if (lover.gender == Npc.Gender.Female)
                    lover.gender = Npc.Gender.Male;
                else
                    lover.gender = Npc.Gender.Female;
            }
            //Otherwise, select one randomly from the potentials
            else {
                lover = potentialLovers[Random.Range(0, potentialLovers.Count)];
            }

            return lover;
        }

        Npc randomNPCfromList(List<Npc> list) {
            int i = Random.Range(0, list.Count);
            return npcs[i];
        }

        Npc[] findSiblings() {
            //Find a family with two children
            int a = -1;
            int b = -1;
            Npc[] siblings = new Npc[2];

            foreach (Family family in families) {

                if (family.children.Count > 1) {
                    a = Random.Range(0, family.children.Count);
                    while (b == -1 || b == a) {
                        b = Random.Range(0, family.children.Count);
                    }
                    siblings[0] = family.children[a];
                    siblings[1] = family.children[b];
                    break;
                }
            }
            return siblings;
        }

        void setMurderer(Npc npc) {
            npc.isMurderer = true;
            murderer = npc;
        }

        void setVictim(Npc npc) {
            npc.isVictim = true;
            victim = npc;
        }

        void generateCharacters() {
            for (int i = 0; i < number_of_characters; i++) {
                npcs.Add(newCharacter());
            }
        }

        void createRelationships() {
            for (int i = 0; i < number_of_characters; i++) {
                for (int x = 0; x < number_of_characters; x++) {
                    if (i == x) {
                        relationships[i, i] = nullRelationship;
                    }
                    else if (relationships[i, x] == nullRelationship) {
                        if (npcs[i].family != null && npcs[x].family != null) {
                            if (npcs[i].family == npcs[x].family) {
                                float r = Random.Range(0f, 1f);

                                //0.2f chance for families to have random relationships
                                if (r <= 0.2f) {
                                    relationships[i, x] = randomRelationshipValue(-3, 3, 3);
                                    relationships[x, i] = randomRelationshipValue(-3, 3, 3);
                                }
                                //Otherwise they love eachother
                                else {
                                    relationships[i, x] = 3;
                                    relationships[x, i] = 3;
                                }
                            }
                        }
                        relationships[i, x] = randomRelationshipValue(-3, 3, 0);
                        relationships[i, x] = randomRelationshipValue(-3, 3, 0);
                    }
                    //Debug.Log(npcs[i].firstname + " " + npcs[i].surname + " an attitude of " + relationships[i, x] + " towards " + npcs[x].firstname + npcs[x].surname);
                }
            }
        }

        Npc newCharacter() {
            GameObject npcGameobject = (GameObject)Instantiate(Resources.Load("Prefabs/NPC"));
            Npc newNPC = npcGameobject.GetComponent<Npc>();

            //Assign gender
            int r = Random.Range(0, 2);
            if (r == 0) { newNPC.gender = Npc.Gender.Male; } else { newNPC.gender = Npc.Gender.Female; }

            //Assign firstname
            if (newNPC.gender == Npc.Gender.Male) {
                r = Random.Range(0, firstnames_m.Count);
                newNPC.firstname = firstnames_m[r];
                firstnames_m.RemoveAt(r);
            }
            else {
                r = Random.Range(0, firstnames_f.Count);
                newNPC.firstname = firstnames_f[r];
                firstnames_f.RemoveAt(r);
            }

            NPCSpriteCreator spriteCreator = GetComponent<NPCSpriteCreator>();
            spriteCreator.getBody(newNPC, npcGameobject);
            npcGameobject.transform.SetParent(npcHolder);
            return newNPC;
        }

        void createFamilies() {
            foreach (Npc npc in npcs) {
                /* Family logic
                If less families than the max, randomly attempt to create a family
                If random attempt failed or families at max, then randomly choose existing family and attempt to join
                If unsuccesful, just take a surname and remove it from the list
                If at the end of the NPC loop there are families with only one member, randomly choose a non family member to join. If none available, remove from that family and make the family name just a surname
                */

                //If there is room for a new family, attempt to make one
                if (families.Count < max_families) {
                    //If attempt is succesful add the NPC to it and change their surname
                    int r = Random.Range(0, 100);
                    if (r > 60) {
                        r = Random.Range(0, surnames.Count);
                        string surname = surnames[r];
                        surnames.RemoveAt(r);

                        Family family = new Family(surname);

                        npc.family = family;
                        families.Add(family);
                        family.family_members.Add(npc);
                        npc.surname = surname;

                        if (npc.gender == Npc.Gender.Male) family.husband = npc;
                        else family.wife = npc;
                    }
                }

                //If there is room in an existing family, try to join it
                if (npc.surname == "") {
                    foreach (Family family in families) {
                        //If the family has room for children, or is missing a husband or wife and this NPC is the right gender, add to family
                        if (family.family_members.Count < max_family_size && ((family.husband != null && family.wife != null) || (npc.gender == Npc.Gender.Male && family.husband == null) || (npc.gender == Npc.Gender.Female && family.wife == null))) {
                            int r = Random.Range(0, 100);
                            if (r > 30) {
                                npc.family = family;
                                family.family_members.Add(npc);
                                npc.surname = family.family_name;

                                if (family.husband != null && family.wife != null) {
                                    family.children.Add(npc);
                                }
                                else if (family.husband == null) {
                                    family.husband = npc;
                                }
                                else if (family.wife == null) {
                                    family.wife = npc;
                                }
                                break;
                            }
                        }
                    }
                }

                //If not joined a family by this point, take a random surname
                if (npc.surname == "") {
                    int i = Random.Range(0, surnames.Count);
                    string randomSurname = surnames[i];
                    surnames.RemoveAt(i);
                    npc.surname = randomSurname;
                    npc.family = null;
                }
            }

            //IF there are any families with 1 member, add the first non-family npc to it. If none available, remove the family.

            for (int f = families.Count - 1; f >= 0; f--) {
                if (families[f].family_members.Count == 1) {
                    Npc.Gender searchingFor;
                    if (families[f].wife == null) searchingFor = Npc.Gender.Female;
                    else searchingFor = Npc.Gender.Male;

                    bool stillSearching = true;
                    int i = 0;

                    while (stillSearching == true && i < npcs.Count - 1) {
                        if (npcs[i].family == null && npcs[i].gender == searchingFor) {
                            npcs[i].family = families[f];
                            families[f].family_members.Add(npcs[i]);
                            npcs[i].surname = families[f].family_name;

                            if (searchingFor == Npc.Gender.Male) families[f].husband = npcs[i];
                            else families[f].wife = npcs[i];

                            Debug.Log("added " + npcs[i].firstname + "to the " + families[f].family_name + "family");

                            stillSearching = false;
                        }
                        i++;
                    }

                    if (stillSearching) {
                        Debug.Log("No suitable npcs found, removing " + families[f].family_name + " family");
                        families[f].family_members[0].family = null;
                        families.Remove(families[f]);
                    }
                }
            }
        }

        void placeNPCs() {
            foreach (Npc npc in npcs) {
                int r = Random.Range(0, mansion.rooms.Count);
                npc.currentRoom = mansion.rooms[r];
                mansion.rooms[r].npcs.Add(npc);
                npc.gameObject.name = npc.getFullName();
            }
        }

        int randomRelationshipValue(int min, int max, int mostLikely, int minWeight = 20, int maxWeight = 20) {

            int num, weight, randomWeight;
            do {
                num = Random.Range(min, max + 1);

                if (num <= mostLikely) {
                    weight = ((mostLikely - num) * minWeight + (num - min) * 100) / Mathf.Max((mostLikely - min), 1);
                }
                else {
                    weight = ((num - mostLikely) * maxWeight + (max - num) * 100) / (max - mostLikely);
                }

                randomWeight = Random.Range(0, 101);
            } while (randomWeight > weight);

            return num;    
        }

        int createFamilyFeud(Family family1, Family family2) {

            int victimDisputesCreated = 0;
            for (int x = 0; x < family1.family_members.Count; x++) {
                for (int y = 0; y < family2.family_members.Count; y++) {

                    int a = npcs.IndexOf(family1.family_members[x]);
                    int b = npcs.IndexOf(family2.family_members[y]);

                    FamilyFeud feudHistory = new FamilyFeud(0, npcs[a], npcs[b]);
                    //Make them hate eachother if they don't already love eachother (romeo juliet situation)
                    if (relationships[a,b] != 3) {
                        relationships[a, b] = randomRelationshipValue(-3, -2, -3);
                        npcs[a].addHistory(feudHistory);
                    }   
                        
                    if (relationships[b, a] != 3) {
                        if (npcs[a] == victim) victimDisputesCreated++;
                        npcs[b].addHistory(feudHistory);
                        relationships[b, a] = randomRelationshipValue(-3, -2, -3);
                    }

                    if (feud == null) { feud = feudHistory; }
                }
            }
            return victimDisputesCreated;
        }

        void loadNames() {
            firstnames_m = new List<string> {
            "Quentin",
            "Bill",
            "Benedict",
            "Joseph",
            "Phillip",
            "Ralph",
            "Peter",
            "Jack",
            "Sean",
            "Miguel",
            "Osvaldo",
            "Reinaldo",
            "Roy",
            "Moses",
            "Hugh",
            "Rocky",
            "Austin",
            "Adam",
            "Samuel",
            "Walter",
            "Gustavo",
            "Columbus",
            "Antony",
            "Angel",
            "Sterling",
            "Carlton",
            "Andrew",
            "Jack",
            "Franklin",
            "George",
            "David",
            "Harvey",
            "Charlie",
            "Jasper"
        };

            firstnames_f = new List<string> {
            "Jazmine",
            "Phyllis",
            "Muriel",
            "Maggie",
            "Elizabeth",
            "Sally",
            "Lucille",
            "Betsey",
            "Hattie",
            "Loni",
            "Dorathy",
            "Krystle",
            "Iona",
            "Tyra",
            "Jeanna",
            "Lucy",
            "Sarah",
            "Julie",
            "Erica",
            "Chrissie",
            "Laura",
            "Samantha",
            "Beatrice",
            "Edith",
            "Gloria",
            "Fran",
            "Hannah",
            "Suzie",
            "Holly",
            "Amy",
            "Amelia",
            "Kirsty"
        };

            surnames = new List<string>
            {
            "Smith",
            "Jones",
            "Williams",
            "Lawrence",
            "Morse",
            "Lewis",
            "Davis",
            "Murphy",
            "Price",
            "Cole",
            "Brown",
            "White",
            "Schwartz",
            "Simpson",
            "Potts",
            "Johnson",
            "Abernathy",
            "Avidan",
            "Hanson",
            "Donovan",
            "Wade",
            "Ware",
            "Stones",
            "Marshall",
            "Pickett",
            "Eliott",
            "Stockdale"
        };
        }
    }
}