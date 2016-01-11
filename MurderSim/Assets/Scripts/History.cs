using UnityEngine;
using System.Collections;

namespace MurderMystery {

    public interface History {
        int whichNpcIsVictim { get; set; }  //Determines who was hurt by the action, if 0 then both of them
        Npc npc1 { get; set; }
        Npc npc2 { get; set; }
    }

    public class StoleLover : History {
        public int whichNpcIsVictim { get; set; }
        public Npc npc1 { get; set; }
        public Npc npc2 { get; set; }
        public Npc lover;

        public StoleLover(int whichNpcIsVictim, Npc npc1, Npc npc2, Npc lover) {
            this.whichNpcIsVictim = whichNpcIsVictim;
            this.npc1 = npc1;
            this.npc2 = npc2;
            this.lover = lover;
        }
    }

    public class CompetingForLove : History {
        public int whichNpcIsVictim { get; set; }
        public Npc npc1 { get; set; }
        public Npc npc2 { get; set; }
        public Npc lover;

        public CompetingForLove(int whichNpcIsVictim, Npc npc1, Npc npc2, Npc lover) {
            this.whichNpcIsVictim = whichNpcIsVictim;
            this.npc1 = npc1;
            this.npc2 = npc2;
            this.lover = lover;
        }
    }

    public class BadBreakup : History {
        public int whichNpcIsVictim { get; set; }
        public Npc npc1 { get; set; }
        public Npc npc2 { get; set; }

        public BadBreakup(int whichNpcIsVictim, Npc npc1, Npc npc2) {
            this.whichNpcIsVictim = whichNpcIsVictim;
            this.npc1 = npc1;
            this.npc2 = npc2;
        }
    }

    public class RejectedLove : History {
        public int whichNpcIsVictim { get; set; }
        public Npc npc1 { get; set; }
        public Npc npc2 { get; set; }

        public RejectedLove(int whichNpcIsVictim, Npc npc1, Npc npc2) {
            this.whichNpcIsVictim = whichNpcIsVictim;
            this.npc1 = npc1;
            this.npc2 = npc2;
        }
    }

    public class FiredBy : History {
        public int whichNpcIsVictim { get; set; }
        public Npc npc1 { get; set; }
        public Npc npc2 { get; set; }

        public FiredBy(int whichNpcIsVictim, Npc npc1, Npc npc2) {
            this.whichNpcIsVictim = whichNpcIsVictim;
            this.npc1 = npc1;
            this.npc2 = npc2;
        }
    }

    public class PutOutOfBusiness : History {
        public int whichNpcIsVictim { get; set; }
        public Npc npc1 { get; set; }
        public Npc npc2 { get; set; }

        public PutOutOfBusiness(int whichNpcIsVictim, Npc npc1, Npc npc2) {
            this.whichNpcIsVictim = whichNpcIsVictim;
            this.npc1 = npc1;
            this.npc2 = npc2;
        }
    }

    public class Nemeses : History {
        public int whichNpcIsVictim { get; set; }
        public Npc npc1 { get; set; }
        public Npc npc2 { get; set; }

        public Nemeses(int whichNpcIsVictim, Npc npc1, Npc npc2) {
            this.whichNpcIsVictim = whichNpcIsVictim;
            this.npc1 = npc1;
            this.npc2 = npc2;
        }

    }

    public class FamilyFeud : History {
        public int whichNpcIsVictim { get; set; }
        public Npc npc1 { get; set; }
        public Npc npc2 { get; set; }

        public FamilyFeud(int whichNpcIsVictim, Npc npc1, Npc npc2) {
            this.whichNpcIsVictim = whichNpcIsVictim;
            this.npc1 = npc1;
            this.npc2 = npc2;
        }
    }

}