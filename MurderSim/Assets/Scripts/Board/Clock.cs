using UnityEngine;
using System.Collections;
using System;

namespace MurderMystery {
    public class Clock : MonoBehaviour {

        public Transform minuteHand;
        public Transform hourHand;
        private PlotGenerator pg;

        // Use this for initialization
        public void setClock() {
            pg = GameObject.Find("GameManager").GetComponent<PlotGenerator>();
            DateTime curTime = Timeline.convertTimeDateTime(pg.timeSteps);

            Debug.Log(curTime.Hour + ":" + curTime.Minute);
            float hourAngle = -360 * ((float)curTime.Hour/12);
            float minuteAngle = -360 * ((float)curTime.Minute/60);
            Debug.Log(hourAngle + ":" + minuteAngle);

            hourHand.localRotation = Quaternion.Euler(0, 0, hourAngle);
            minuteHand.localRotation = Quaternion.Euler(0, 0, minuteAngle);
        }

    }

}
