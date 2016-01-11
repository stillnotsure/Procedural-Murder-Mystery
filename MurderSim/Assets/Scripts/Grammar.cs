using UnityEngine;
using System.Collections;

namespace MurderMystery {

    public static class Grammar {

        public static string getSubjectPronoun(Npc npcReferredTo, Npc npcSpeaking) {
            if (npcSpeaking == npcReferredTo) return "I";
            else {
                if (npcReferredTo.gender == Npc.Gender.Male) return "he";
                else return "she";
            }
        }

        public static string getObjectPronoun(Npc npcReferredTo, Npc npcSpeaking) {
            if (npcSpeaking == npcReferredTo) return "me";
            else {
                if (npcReferredTo.gender == Npc.Gender.Male) return "him";
                else return "her";
            }
        }

        public static string getHave(Npc npcReferredTo, Npc npcSpeaking) {
            if (npcSpeaking == npcReferredTo) return "'ve";
            else return "'s";
        }

        public static string getSubjectPronounHave(Npc npcReferredTo, Npc npcSpeaking) {
            return getSubjectPronoun(npcReferredTo, npcSpeaking) + getHave(npcReferredTo, npcSpeaking);
        }

        public static string checkSelf(Npc npcReferredTo, Npc npcSpeaking) {
            if (npcReferredTo == npcSpeaking) return "I";
            else return npcReferredTo.firstname;
        }

        public static string selfOrNamePossessive(Npc npcReferredTo, Npc npcSpeaking) {
            if (npcReferredTo == npcSpeaking) return "my";
            else return npcReferredTo.firstname + "'s";
        }

        public static string selfOrName(Npc npcReferredTo, Npc npcSpeaking) {
            if (npcReferredTo == npcSpeaking) return "I";
            else return npcReferredTo.firstname;
        }

        public static string myHisHer(Npc npcReferredTo, Npc npcSpeaking) {
            if (npcReferredTo == npcSpeaking) return "my";
            else {
                if (npcReferredTo.gender == Npc.Gender.Male) return "his";
                else return "her";
            }
        }

    }
}


