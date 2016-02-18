using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace MurderMystery {

    public enum dialog { none, introduction, greeting, alibi, eventsWitnessed, suspect, corpse, accusation, murderAccusation, lockedOut};
    public enum conversationState { none, moreText, playerInput, npcSpeaking };

    public class ConversationScript : MonoBehaviour {

        public conversationState state;
        public dialog dialogType;
        public float letterDelay = 0.04f;
        public float detectiveAudioPitch = 0.8f;

        //References
        public AudioClip letterSound;
        public AudioSource audioSource;

        public UIManager uiManager;
        public Npc speakingNPC;
        public GameObject textPanel;
        public Text TextArea;
        public Text responseArea;
        public Text nameText;
        private PlotGenerator pg;

        //Responses
        private List<String> responses;
        public int selected;

        //Text and testimony holders
        private Queue<string> dialogueQueue;
        private Queue<Testimony> testimonyQueue;
        private string fullString;
        private string shownString;

        void Start() {
            pg = gameObject.GetComponent<PlotGenerator>();
            uiManager = gameObject.GetComponent<UIManager>();
            audioSource = gameObject.GetComponent<AudioSource>();
            letterSound = Resources.Load<AudioClip>("Audio/text-letter");

            //Load gameobjects
            textPanel = GameObject.Find("Text Panel");
            TextArea = GameObject.Find("Text Area").GetComponent<Text>();
            responseArea = GameObject.Find("Response Area").GetComponent<Text>();
            nameText = GameObject.Find("Name Text").GetComponent<Text>();

            state = conversationState.none;
            responses = new List<string>();
            dialogueQueue = new Queue<string>();
            testimonyQueue = new Queue<Testimony>();
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                setStateNone();
            }

            if (state == conversationState.npcSpeaking) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    skipText();
                }
            } else

            //More text is set when the current line is finished printing, but there is more dialogue from the NPC waiting to be displayed upon pressing shift
            if (state == conversationState.moreText) {
                //Alibi and events witnessed go to more text after each time the NPC speaks, and wait for either continue or accuse
                if (dialogType == dialog.alibi || dialogType == dialog.eventsWitnessed) {
                    if (Input.GetKeyDown(KeyCode.F)) {
                        AccuseOfLying();
                    }
                    if (Input.GetKeyDown(KeyCode.LeftShift)){
                        if (dialogueQueue.Count > 0) {
                            displayText(dialogueQueue.Dequeue());
                        } else {
                            dialogType = dialog.greeting;
                            displayText("Can I help you with anything else?");
                            testimonyQueue.Clear();
                            state = conversationState.playerInput;
                        }
                    }
                } else if (dialogType == dialog.suspect || dialogType == dialog.accusation || dialogType == dialog.introduction) {
                    if (dialogueQueue.Count > 0) {
                        if (Input.GetKeyDown(KeyCode.LeftShift)) {
                            displayText(dialogueQueue.Dequeue());
                        }
                    } else {
                        if (Input.GetKeyDown(KeyCode.LeftShift)) {
                            dialogType = dialog.greeting;
                            displayText("Can I help you with anything else?");
                            testimonyQueue.Clear();
                            state = conversationState.playerInput;
                        }
                       /* else if (Input.GetKeyDown(KeyCode.F)) {
                            AccuseOfLying();
                        }
                        */
                    }
                }
                else if (dialogType == dialog.lockedOut) {
                    if (dialogueQueue.Count > 0) {
                        if (Input.GetKeyDown(KeyCode.LeftShift)) {
                            displayText(dialogueQueue.Dequeue());
                        }
                    }
                    else {
                        if (Input.GetKeyDown(KeyCode.LeftShift)) {
                            setStateNone();
                        }
                    }
                }
                
            }

            //Player input is when the NPC has finished talking and the player uses the menu to respond to them
            else if (state == conversationState.playerInput) {
                if (Input.GetKeyDown("w")) {
                    selected = Math.Max(0, selected - 1);
                }
                else if (Input.GetKeyDown("s")) {
                    selected = Math.Min(responses.Count - 1, selected + 1);
                }
                if (Input.GetKeyDown(KeyCode.LeftShift)) {
                    selectResponse(selected);
                }
            }
        }

        void OnGUI() {

            if (state != conversationState.none) {
                if (pg.debugMode) uiManager.displayRelationManager(true);
                if (textPanel != null) textPanel.SetActive(true);
                if (speakingNPC.nameKnown == true || !speakingNPC.isAlive)  nameText.text = speakingNPC.getFullName();
                else nameText.text = "???";
            }

            if (state == conversationState.npcSpeaking) {
                responseArea.text = "";
                TextArea.text = shownString;
            }
            else if (state == conversationState.playerInput) {
                TextArea.text = shownString;
                string responseText = "";
                for (int i = 0; i < responses.Count; i++) {
                    if (selected == i)
                        responseText += String.Format("<b> {0}. {1} </b>\n", i + 1, responses[i]);
                    else
                        responseText += String.Format("{0}. {1} \n", i + 1, responses[i]);
                }
                responseArea.text = responseText;
            }
            else if (state == conversationState.moreText) {
                //Debug.Log(testimonyQueue.Peek());
                string responseText;
                if (dialogType != dialog.murderAccusation) {
                    responseText = "Press Shift to Continue";
                    if (dialogType == dialog.alibi || dialogType == dialog.eventsWitnessed) responseText += Environment.NewLine + "Press F to accuse of lying";
                }
                else {
                    responseText = "Press F to <b>Accuse</b>" + Environment.NewLine + "Press Shift to cancel";
                }

                responseArea.text = responseText;
            }
            else if (state == conversationState.none) {
                if (pg.debugMode) uiManager.displayRelationManager(false);
                nameText.text = "";
                responseArea.text = "";
                TextArea.text = "";
                textPanel.SetActive(false);
                audioSource.Stop();
                audioSource.pitch = 1.0f;
            }

        }

        void setStateNone() {
            state = conversationState.none;
            dialogType = dialog.none;
            testimonyQueue.Clear();
            dialogueQueue.Clear();
            selected = 0;
            shownString = "";
            fullString = "";
        }

        void selectResponse(int i) {
            selected = 0;
            string selectedText = responses[i];
            if (selectedText.Equals("Who are you?")) {
                NPCIntroduction();
            }
            if (selectedText.Equals("Where were you at the time of the murder?")) {
                NPCAlibi();
            }
            else if (selectedText.Equals("What did you see around the time of the murder?")) {
                NPCWitnessed();
            }
            else if (selectedText.Equals("Examine the injuries")) {
                ExamineInjuries();
            }
            else if (selectedText.Equals("Estimate the time of death")) {
                TimeOfDeath();
            }
            else if (selectedText.Equals("Is there anyone you suspect?")) {
                NPCSuspects();
            }
            else if (selectedText.Equals("I think you're hiding something.")) {
                AccuseOfOmission();
            }
            else if (selectedText.Equals("(Accuse of committing the murder)")) {
                accuseOfMurder();
            }
            else if (selectedText.Equals("Accuse")) {
                selectMurdererAndGoToEnding();
            }
            else if (selectedText.Equals("Cancel")) {
                setStateNone();
            }
        }

        public void displayText(string text) {
            StopAllCoroutines();
            shownString = "";
            fullString = capitaliseFirstLetter(text);

            state = conversationState.npcSpeaking;
            StartCoroutine(RevealString());
        }

        string capitaliseFirstLetter(string text) {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            char[] letters = text.ToCharArray();
            letters[0] = char.ToUpper(letters[0]);

            return new string(letters);

        }

        IEnumerator RevealString() {

            foreach (char letter in fullString.ToCharArray()) {
                shownString += letter;
                if (state == conversationState.npcSpeaking || state == conversationState.playerInput)audioSource.PlayOneShot(letterSound);

                if(dialogType != dialog.corpse && dialogType != dialog.murderAccusation) {
                    float combinedLetterDelay = letterDelay;
                    if (letter == ',') combinedLetterDelay += 0.09f;    //add a pause to commas
                    else if (letter == '.') combinedLetterDelay += 0.12f;   //slightly longer pause to full stops

                    //chance to stammer between each word to indicate stress
                    else if (letter == ' ') {
                        float chanceToStutter = UnityEngine.Random.Range(0.1f, 1.0f);
                        if (chanceToStutter < speakingNPC.stress) {
                            combinedLetterDelay += UnityEngine.Random.Range(0.09f, 0.2f);
                        }
                    }

                    yield return new WaitForSeconds(combinedLetterDelay);
                }
                else yield return new WaitForSeconds(letterDelay);
            }

            if (shownString == fullString) {
                if (dialogType == dialog.alibi || dialogType == dialog.eventsWitnessed || dialogType == dialog.suspect || dialogueQueue.Count > 0) {
                    state = conversationState.moreText;
                }
                else {
                    setUpDialogOptions();
                    if (speakingNPC.isAlive) dialogType = dialog.greeting;
                    else dialogType = dialog.corpse;
                    state = conversationState.playerInput;
                }
                StopAllCoroutines();
            }

        }

        void skipText() {
            StopAllCoroutines();
            shownString = fullString;
            TextArea.text = shownString;

            if (dialogType == dialog.alibi || dialogType == dialog.eventsWitnessed || dialogType == dialog.suspect || dialogueQueue.Count > 0) {
                state = conversationState.moreText;
            }
            else {
                setUpDialogOptions();
                if (speakingNPC.isAlive) dialogType = dialog.greeting;
                else dialogType = dialog.corpse;
                state = conversationState.playerInput;
            }


        }

        void setUpDialogOptions() {
            responses.Clear();
            if (speakingNPC.isAlive) {
                if (dialogType != dialog.murderAccusation && dialogType != dialog.lockedOut ) {
                    responses.Add("Who are you?");
                    responses.Add("Where were you at the time of the murder?");
                    responses.Add("What did you see around the time of the murder?");
                    responses.Add("Is there anyone you suspect?");
                    responses.Add("I think you're hiding something.");
                    responses.Add("(Accuse of committing the murder)");
                }
                else if (dialogType == dialog.murderAccusation){
                    responses.Add("Accuse");
                }
            }
            else {
                responses.Add("Examine the injuries");
                responses.Add("Estimate the time of death");
            }

            responses.Add("Cancel");
        }

        void ExamineInjuries() {
            Murder murderInfo = Timeline.murderEvent;

            switch (murderInfo.weapon.damageType) {
                case (Weapon.DamageType.blunt):
                    displayText("From the bruising and broken bones you deduce that the murder was committed using a blunt weapon");
                    break;
                case (Weapon.DamageType.poison):
                    displayText("From the skin discoloration and lack of any other physical marks, you deduce that the murder was committed using some kind of poison");
                    break;
                case (Weapon.DamageType.shot):
                    displayText("The body has several gunshot wounds, clearly indicating that this murder was committed using a firearm");
                    break;
                case (Weapon.DamageType.stab):
                    displayText("The body is riddled with stab marks, indicating that the murder was committed using some kind of sharp, pointed weapon");
                    break;
            }
        }

        void selectMurdererAndGoToEnding() {
            PlotGenerator.chosenNPC = speakingNPC;
            speakingNPC.gameObject.AddComponent<DontDestroyOnLoad>();
            Application.LoadLevel("Recap");
        }

        void TimeOfDeath() {
            string time1 = (Timeline.convertTime(Timeline.murderEvent.time - (pg.timeOfDeathLeeway / Timeline.timeIncrements)));
            string time2 = (Timeline.convertTime(Timeline.murderEvent.time + (pg.timeOfDeathLeeway / Timeline.timeIncrements)));
            displayText(String.Format("You ascertain that the murder occurred sometime between {0} and {1}", time1, time2));
        }

        void NPCAlibi() {
            dialogType = dialog.alibi;
            List<Event> events = Timeline.locationDuringTimeframe(speakingNPC, Timeline.murderEvent.time - (pg.timeOfDeathLeeway / Timeline.timeIncrements), Timeline.murderEvent.time + (pg.timeOfDeathLeeway / Timeline.timeIncrements));
            if (events.Count == 0)
                displayText("I've been here the whole evening.");

            else {
                foreach (SwitchRooms e in events) {
                    EventTestimony t;
                    if (speakingNPC.testimonies.TryGetValue(e, out t)) {
                        testimonyQueue.Enqueue(t);
                    } else {
                        t = TestimonyManager.createTestimony(speakingNPC, e);
                        speakingNPC.addTestimony(t, e);
                        testimonyQueue.Enqueue(t);
                    }
                    
                    SwitchRooms switchrooms = t.e as SwitchRooms;
                    string s = String.Format("At {0} I moved to the {1}", Timeline.convertTime(switchrooms.time), switchrooms.newRoom.roomName);

                    List<Npc> peopleInNewRoom = switchrooms.peopleInNewRoom;
                    if (peopleInNewRoom.Count > 0) {
                        if (peopleInNewRoom.Count == 1) {
                            string npcEncounters = String.Format(", {0} was there too.", peopleInNewRoom[0].firstname);
                            s += npcEncounters;
                        } else if (peopleInNewRoom.Count == 2) {
                            s += String.Format(", {0} and {1} were there too.", peopleInNewRoom[0].firstname, peopleInNewRoom[1].firstname);
                        } else {
                            s += ". ";
                            for (int i = 0; i < peopleInNewRoom.Count; i++) {
                                if (i < peopleInNewRoom.Count - 1) {
                                    s += String.Format("{0}, ", peopleInNewRoom[i].firstname);
                                } else {
                                    s += String.Format("and {0} were there too.", peopleInNewRoom[i].firstname);
                                }
                            }
                        }
                    }
                   
                    dialogueQueue.Enqueue(s);
                }
                speakingNPC.timeBuffer = 0; //reset the timebuffer so the lies don't get further and further from the truth
                displayText(dialogueQueue.Dequeue());
            }
        }

        //Todo - Make ALL these events into testimonies so they can be lied about
        void NPCWitnessed(List<Event> events = null, bool tellTruth = false, bool dontDisplay = false) {
            if (!dontDisplay) dialogType = dialog.eventsWitnessed;
            if (events == null) events = Timeline.EventsWitnessedDuringTimeframe(speakingNPC, Timeline.murderEvent.time - (pg.timeOfDeathLeeway / Timeline.timeIncrements), Timeline.murderEvent.time + (pg.timeOfDeathLeeway / Timeline.timeIncrements));
            if (events.Count == 0)
                displayText("I haven't seen anything unusual");
            else {
                for (int i = 0; i < events.Count; i++) {
                    if (events[i] is SwitchRooms) {
                        EventTestimony t;
                        if (!speakingNPC.testimonies.TryGetValue(events[i], out t) || tellTruth) {
                            if (tellTruth) t = TestimonyManager.getTrueEventTestimony(speakingNPC, events[i]);
                            else t = TestimonyManager.createTestimony(speakingNPC, events[i]);
                            speakingNPC.addTestimony(t, events[i]);
                        }

                        testimonyQueue.Enqueue(t);

                        if (!t.omitted) {
                            SwitchRooms e = events[i] as SwitchRooms;
                            dialogueQueue.Enqueue(String.Format("At {0} I saw {1} move into the {2}", Timeline.convertTime(events[i].time), e.npc.getFullName(), e.newRoom.roomName));
                        }
                    }

                    else if (events[i] is FoundBody) {
                        EventTestimony t;
                        if (!speakingNPC.testimonies.TryGetValue(events[i], out t) || tellTruth) {
                            if (tellTruth) t = TestimonyManager.getTrueEventTestimony(speakingNPC, events[i]);
                            else t = TestimonyManager.createTestimony(speakingNPC, events[i]);
                            speakingNPC.addTestimony(t, events[i]);
                        }

                        testimonyQueue.Enqueue(t);

                        if (!t.omitted) {
                            FoundBody e = events[i] as FoundBody;
                            dialogueQueue.Enqueue(String.Format("At {0} I found {1}'s body here", Timeline.convertTime(events[i].time), pg.victim.firstname));
                        }
                    }

                    else if (events[i] is Murder) {
                        EventTestimony t;
                        if (!speakingNPC.testimonies.TryGetValue(events[i], out t) || tellTruth) {
                            if (tellTruth) t = TestimonyManager.getTrueEventTestimony(speakingNPC, events[i]);
                            else t = TestimonyManager.createTestimony(speakingNPC, events[i]);
                            speakingNPC.addTestimony(t, events[i]);
                        }

                        testimonyQueue.Enqueue(t);

                        if (!t.omitted) {
                            Murder e = events[i] as Murder;
                            dialogueQueue.Enqueue(String.Format("At {0} I saw {1} murder {2} in the {3}! I swear it's true!", Timeline.convertTime(events[i].time), e.npc.getFullName(), e.npc2.getFullName(), e.room.roomName));
                        }
                    }

                    else if (events[i] is PickupItem) {
                        EventTestimony t;
                        if (!speakingNPC.testimonies.TryGetValue(events[i], out t) || tellTruth) {
                            if (tellTruth) t = TestimonyManager.getTrueEventTestimony(speakingNPC, events[i]);
                            else t = TestimonyManager.createTestimony(speakingNPC, events[i]);
                            speakingNPC.addTestimony(t, events[i]);
                        }

                        testimonyQueue.Enqueue(t);

                        if (!t.omitted) {
                            PickupItem e = t.e as PickupItem;
                            dialogueQueue.Enqueue(String.Format("At {0} I saw {1} pick up something in the {2}", Timeline.convertTime(e.time), e.npc.getFullName(), e.room.roomName));
                        }
                       
                    }
                    else if (events[i] is DropItem) {
                        EventTestimony t;
                        if (!speakingNPC.testimonies.TryGetValue(events[i], out t) || tellTruth) {
                            if (tellTruth) t = TestimonyManager.getTrueEventTestimony(speakingNPC, events[i]);
                            else t = TestimonyManager.createTestimony(speakingNPC, events[i]);
                            speakingNPC.addTestimony(t, events[i]);
                        }

                        testimonyQueue.Enqueue(t);

                        //Todo - Particularly shrewd NPCs have a chance of knowing what the item was
                        if (!t.omitted) {
                            DropItem e = t.e as DropItem;
                            dialogueQueue.Enqueue(String.Format("At {0} I saw {1} put something away in the {3}", Timeline.convertTime(e.time), e.npc.getFullName(), e.item.name, e.room.roomName));
                        }

                    }
                }
                if (!dontDisplay) displayText(dialogueQueue.Dequeue());
            }

        }

        void revealTruth(Testimony lie) {
            if (lie is EventTestimony) {
                EventTestimony eventLie = lie as EventTestimony;
                Event ev = eventLie.e;
                Event truth;
                speakingNPC.testimoniesReversed.TryGetValue(eventLie, out truth);

                if (ev is SwitchRooms) {
                    SwitchRooms e = ev as SwitchRooms;
                    SwitchRooms convertedTruth = truth as SwitchRooms;
                    if (e.time != truth.time) {
                        dialogueQueue.Enqueue(string.Format("{0} actually moved to the {1} at {2}, not {3}", Grammar.getSubjectPronoun(e.npc, speakingNPC), e.newRoom.roomName,Timeline.convertTime(truth.time), Timeline.convertTime(e.time)));
                    }
                    else if (e.newRoom != convertedTruth.newRoom) {
                        dialogueQueue.Enqueue(string.Format("{0} actually moved to the {1}, not the {3}", Grammar.getSubjectPronoun(e.npc, speakingNPC), e.newRoom.roomName, convertedTruth.newRoom, e.newRoom));
                    }
                }

                EventTestimony trueTestimony = new EventTestimony(truth, speakingNPC, true, false);
                speakingNPC.removeTestimony(eventLie, truth);
                speakingNPC.addTestimony(trueTestimony, truth);
            }

        }

        void revealOmission(Testimony omission) {
            if (omission is EventTestimony) {
                EventTestimony et = omission as EventTestimony;
                Event e = et.e;
                List<Event> events = new List<Event>();
                events.Add(et.e);
                NPCWitnessed(events, true, true);
            }
        }

        void NPCIntroduction() {
            dialogType = dialog.introduction;
            speakingNPC.nameKnown = true;
            displayText(string.Format("I am {0}", speakingNPC.getFullName()));

            Family family = speakingNPC.family;

            if (family != null) {
                bool isHusband = false; bool isWife = false;
                if (family.husband == speakingNPC)
                    isHusband = true;
                else if (family.wife == speakingNPC)
                    isWife = true;

                if (isHusband || isWife) {
                    if (isHusband) dialogueQueue.Enqueue(string.Format("{0} is my wife", family.wife.firstname));
                    else if (isWife) dialogueQueue.Enqueue(string.Format("{0} is my husband", family.husband.firstname));

                    if (family.children.Count == 1) {
                        if (family.children[0].gender == Npc.Gender.Male)
                            dialogueQueue.Enqueue(string.Format("Our son, {0}, is also here tonight.", family.children[0].firstname));
                        else
                            dialogueQueue.Enqueue(string.Format("Our daughter, {0}, is also here tonight.", family.children[0].firstname));
                    }
                    else if (family.children.Count > 1) {
                        string children = "Our children, ";
                        for (int i = 0; i < family.children.Count - 1; i++) {
                            children += String.Format("{0}, ", family.children[i].firstname);
                        }
                        children += String.Format("and {0} are also here tonight.", family.children[family.children.Count - 1].firstname);
                        dialogueQueue.Enqueue(children);
                    }
                }

                //If not husband or wife, must be a child
                else {
                    dialogueQueue.Enqueue(string.Format("{0} and {1} are my parents", family.husband.firstname, family.wife.firstname));

                    if (family.children.Count == 2) {
                        int childNumber = family.children.IndexOf(speakingNPC);
                        if (childNumber == 0) {
                            if (family.children[1].gender == Npc.Gender.Male) dialogueQueue.Enqueue(string.Format("My brother, {0}, is also here tonight.", family.children[1].firstname));
                            else dialogueQueue.Enqueue(string.Format("My sister, {0}, is also here tonight.", family.children[1].firstname));
                        }
                        else {
                            if (family.children[0].gender == Npc.Gender.Male) dialogueQueue.Enqueue(string.Format("My brother, {0}, is also here tonight.", family.children[0].firstname));
                            else dialogueQueue.Enqueue(string.Format("My sister, {0}, is also here tonight.", family.children[0].firstname));
                        }
                    } else {
                        string siblings = "My siblings, ";
                        List<Npc> siblingsList = new List<Npc>(family.children);
                        siblingsList.Remove(speakingNPC);

                        for (int i = 0; i < siblingsList.Count - 1; i++) {
                            siblings += String.Format("{0}, ", siblingsList[i].firstname);
                        }
                        siblings += String.Format("and {0} are also here tonight.", siblingsList[siblingsList.Count - 1].firstname);
                        dialogueQueue.Enqueue(siblings);
                    }
                        
                }
            }
           
        }

        void NPCSuspects() {
            dialogType = dialog.suspect;
            Debug.Log("Running npc suspects");
            SuspectTestimony st = TestimonyManager.pickASuspect(speakingNPC);
            if (st != null) {
                testimonyQueue.Enqueue(st);
                Npc.Gender suspectGender = st.npc.gender;
                Npc.Gender victimGender = pg.victim.gender;
                displayText(string.Format("I think {0} did it.", st.npc.getFullName()));

                if (st.motive is StoleLover) {
                    StoleLover motive = st.motive as StoleLover;
                    dialogueQueue.Enqueue(string.Format("Not long ago {0} left {1} for {2} and {3} clearly never got over it.", Grammar.selfOrName(motive.lover, speakingNPC), Grammar.getObjectPronoun(st.npc, speakingNPC), pg.victim.firstname, Grammar.getSubjectPronoun(st.npc, speakingNPC)));
                }

                else if (st.motive is CompetingForLove) {
                    CompetingForLove motive = st.motive as CompetingForLove;
                    dialogueQueue.Enqueue(string.Format("{0} been fighting {1} for {2}'s affections for a long time now, it was bound to come to a head at some point.", Grammar.getSubjectPronounHave(st.npc, speakingNPC), pg.victim.firstname, Grammar.selfOrNamePossessive(motive.lover, speakingNPC)));
                }

                else if (st.motive is BadBreakup) {
                    BadBreakup motive = st.motive as BadBreakup;
                    dialogueQueue.Enqueue(string.Format("It's no secret that {0} and {1} split up, {2} took it very badly.", Grammar.getSubjectPronoun(st.npc, speakingNPC), pg.victim.firstname, Grammar.getSubjectPronoun(st.npc, speakingNPC)));
                }

                else if (st.motive is FiredBy) {
                    FiredBy motive = st.motive as FiredBy;
                    dialogueQueue.Enqueue(string.Format("{0} used to work under {1} until {2} was fired, and {3} been looking for work since.", st.npc.firstname, pg.victim.firstname, Grammar.getSubjectPronoun(st.npc, speakingNPC), Grammar.getSubjectPronounHave(st.npc, speakingNPC)));
                }

                else if (st.motive is PutOutOfBusiness) {
                    PutOutOfBusiness motive = st.motive as PutOutOfBusiness;
                    dialogueQueue.Enqueue(string.Format("Everyone knows that {0} put {1} out of business, it completely ruined {2} and {3} family.", pg.victim.firstname, st.npc.firstname, Grammar.getObjectPronoun(st.npc, speakingNPC), Grammar.myHisHer(st.npc, speakingNPC)));
                }

                else if (st.motive is Nemeses) {
                    Nemeses motive = st.motive as Nemeses;
                    dialogueQueue.Enqueue(string.Format("{0} and {1} have been at eachother's throats for as long as I can remember.", Grammar.getObjectPronoun(st.npc, speakingNPC), pg.victim.firstname, Grammar.getObjectPronoun(st.npc, speakingNPC)));
                }

                else if (st.motive is RejectedLove) {
                    Nemeses motive = st.motive as Nemeses;
                    dialogueQueue.Enqueue(string.Format("{0} confessed {1} love to {2} recently, but {3} rejected {4} very harshly.", Grammar.getSubjectPronoun(st.npc, speakingNPC), Grammar.myHisHer(st.npc, speakingNPC), pg.victim.firstname, Grammar.getSubjectPronoun(pg.victim, speakingNPC), Grammar.getObjectPronoun(st.npc, speakingNPC)));
                }

                else displayText(string.Format("I think {0} did it, they've been out for revenge ever since {1} did that {2} to them", st.npc.getFullName(), pg.victim.getFullName(), st.motive.GetType()));
            }
            else
                displayText("Sorry, I have no idea");
        }

        void AccuseOfLying() {
            dialogType = dialog.accusation;
            Testimony testimony = testimonyQueue.Dequeue();

            if (testimony.truth == false) {
                speakingNPC.stress = Mathf.Min(speakingNPC.stress + speakingNPC.stressIncrements, 1.0f);
                displayText("Very shrewd detective... Fine, I'll tell you the truth");
                revealTruth(testimony);
            }
            else {
                speakingNPC.lieAccusations--;
                displayText("How dare you accuse me of such a thing!");
                if (speakingNPC.lieAccusations <= 0)
                    lockedOut();
            }
                
        }

        //Collects all the omitted truths from the NPCs memory, and randomly selects one of them
        void AccuseOfOmission() {
            dialogType = dialog.accusation;
            List<EventTestimony> omissions = new List<EventTestimony>();

            foreach (KeyValuePair<Event, EventTestimony> entry in speakingNPC.testimonies) {
                if (entry.Value.omitted) {
                    omissions.Add(entry.Value);
                }
            }

            if (omissions.Count == 0) {
                if (speakingNPC.testimonies.Count == 0)
                    displayText("But detective, I haven't even given you my testimony yet.");
                else {
                    speakingNPC.lieAccusations--;
                    displayText("How dare you make such wild accusations. Have you any proof?");
                    if (speakingNPC.lieAccusations <= 0)
                        lockedOut();
                }
                    
            }
            else {
                int r = UnityEngine.Random.Range(0, omissions.Count);
                speakingNPC.stress = Mathf.Min(speakingNPC.stress + speakingNPC.stressIncrements, 1.0f);
                displayText("...Yes, there is something I still need to tell you.");
                revealOmission(omissions[r]);
            }
        }

        void accuseOfMurder() {
            dialogType = dialog.murderAccusation;
            displayText(String.Format("(Are you sure you want to accuse this person of the murder? This will end the game)"));
        }

        void NPCGreeting(Npc npc) {
            if (npc.lieAccusations > 0) {
                dialogType = dialog.greeting;
                displayText("Good evening detective");
            }
            else {
                dialogType = dialog.lockedOut;
                displayText("I have nothing more to say to you.");
            }
        }

        void lockedOut() {
            dialogType = dialog.lockedOut;
            dialogueQueue.Enqueue("If you're going to keep making these baseless accusations I see no reason to continue this conversation.");
            displayText(dialogueQueue.Dequeue());
        }

        void examineBody(Npc npc) {
            dialogType = dialog.corpse;
            displayText(npc.getFullName() + "'s lifeless body lies before you");
        }

        public void handleInteractionWith(Npc npc) {
            gameObject.GetComponent<UIManager>().setRelationships(npc);
            speakingNPC = npc;
            setAudioPitch();
            if (npc.isAlive) {
                NPCGreeting(npc);
            }
            else
                examineBody(npc);
        }

        private void setAudioPitch() {
            if (speakingNPC.isAlive) audioSource.pitch = speakingNPC.audioPitch;
            else audioSource.pitch = detectiveAudioPitch;
        }

    }
}   