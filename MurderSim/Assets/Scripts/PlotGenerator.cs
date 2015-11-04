using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Family
{
    public string family_name;
    public List<Npc> family_members;

    public Family(string name)
    {
        family_name = name;
        family_members = new List<Npc>();
    }
}

public class PlotGenerator : MonoBehaviour {

    public int number_of_characters = 8;

    public int max_families = 2;
    public int max_family_size = 3;

    public List<Family> families; //Surnames that appear more than once go in here and become families
    public List<Npc> npcs;

    //To be removed once a much better solution is found...
    //Change these to lists
    private string[] firstnames_m;
    private string[] firstnames_f;
    private List<string> surnames;
    
    // Use this for initialization
    [ContextMenu("Reset")]
    void Start () {
        families = new List<Family>();
        loadNames();
        npcs = new List<Npc>();
        generateCharacters();
        createFamilies();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void generateCharacters(){
        for (int i = 0; i < number_of_characters; i++){
            npcs.Add(newCharacter());
        }
        
    }

    Npc newCharacter()
    {
        Npc newNPC = new Npc();

        //Assign gender
        int r = Random.Range(0, 2);
        if (r == 0) { newNPC.gender = Npc.Gender.Male; } else { newNPC.gender = Npc.Gender.Female; }

        //Assign firstname
        if (newNPC.gender == Npc.Gender.Male) {
            r = Random.Range(0, firstnames_m.Length);
            newNPC.firstname = firstnames_m[r];
        }
        else {
            r = Random.Range(0, firstnames_f.Length);
            newNPC.firstname = firstnames_f[r];
        }

        return newNPC;
    }

    void createFamilies()
    {
        foreach (Npc npc in npcs)
        {
            /* Family logic
            If less families than the max, randomly attempt to create a family
            If random attempt failed or families at max, then randomly choose existing family and attempt to join
            If unsuccesful, just take a surname and remove it from the list
            If at the end of the NPC loop there are families with only one member, randomly choose a non family member to join. If none available, remove from that family and make the family name just a surname
            */

             Debug.Log("Finding a surname for " + npc.firstname);

             //If there is room for a new family, attempt to make one
             if (families.Count < max_families)
             {
                Debug.Log("Room for a new family, attempting to create with " + npc.firstname);
                //If attempt is succesful add the NPC to it and change their surname
                int r = Random.Range(0, 100);
                if (r > 60)
                {
                    r = Random.Range(0, surnames.Count);
                    string surname = surnames[r];
                    surnames.RemoveAt(r);

                    Family family = new Family(surname);
                    Debug.Log("Sucess, created the " + surname + " family");

                    npc.family = family;
                    families.Add(family);
                    family.family_members.Add(npc);
                    npc.surname = surname;
                }
            }

            //If there is room in an existing family, try to join it
            if (npc.surname == null)
            {
                foreach (Family family in families)
                {
                    Debug.Log(family.family_name + "family has " + family.family_members.Count + " members");

                    if (family.family_members.Count < max_family_size)
                    {
                        Debug.Log(npc.firstname + "is attempting to join the " + family.family_name + " family");
                        int r = Random.Range(0, 100);
                        if (r > 50)
                        {
                            Debug.Log("Sucess, "+ npc.firstname +" joined " + family.family_name + " family");
                            npc.family = family;
                            family.family_members.Add(npc);
                            npc.surname = family.family_name;
                            break;
                        }
                    }
                }
            }

            //If not joined a family by this point, take a random surname
            if (npc.surname == null)
            {
                int i = Random.Range(0, surnames.Count);
                string randomSurname = surnames[i];
                surnames.RemoveAt(i);
                Debug.Log("Taking random surname " + randomSurname);
                npc.surname = randomSurname;
            }
        }

        //IF there are any families with 1 member, add the first non-family npc to it. If none available, remove the family.

        for (int f = families.Count-1; f >= 0; f--)
        {
            if (families[f].family_members.Count == 1)
            {
                Debug.Log("found a family with one member");
                bool stillSearching = true;
                int i = 0;

                while (stillSearching == true && i > npcs.Count-1){
                    if (npcs[i].family == null)
                    {
                        npcs[i].family = families[f];
                        families[f].family_members.Add(npcs[i]);
                        npcs[i].surname = families[f].family_name;
                        Debug.Log("added " + npcs[i].firstname + "to the " + families[f].family_name +"family");

                        stillSearching = false;
                    }
                    i++;
                }

                if (stillSearching)
                {
                    Debug.Log("No suitable npcs found, removing " + families[f].family_name +" family");
                    families[f].family_members[0].family = null;
                    families.Remove(families[f]);
                }
            }
        }
    }

    void loadNames()
    {
        firstnames_m = new string[10] {
            "Miguel",
            "Osvaldo",
            "Reinaldo",
            "Roy",
            "Moses",
            "Hugh",
            "Rocky",
            "Austin",
            "Walter",
            "Gustavo"
        };

        firstnames_f = new string[10] {
            "Sally",
            "Lucille",
            "Betsey",
            "Hattie",
            "Loni",
            "Dorathy",
            "Krystle",
            "Iona",
            "Tyra",
            "Jeanna"
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
            "Cole"
        };
    }
}
