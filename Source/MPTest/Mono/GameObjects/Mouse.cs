using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Managers;
using Multiplayer.Misc;
using Shared.PacketData;
using UnityEngine;

namespace Multiplayer.Mono.GameObjects
{
    public class Mouse : MonoBehaviour
    {
        private Vector3 targetPos;
        private Vector3 startPos;
        private float currentTime;
        private bool finished;
        void Start()
        {
            startPos = transform.position;
            finished = true;
            GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        }
        public void SetLerpData(MouseData data) 
        {
            if (!finished)
            {
                currentTime = 0f;
            }
            targetPos = new Vector3(data.x, data.y, 300);
            startPos = transform.position;
            finished = false;
        }
        void Update() 
        {
            if (!finished)
                Lerp();
        }
        private void Lerp() 
        {
            if (currentTime < InputManager.sleepTime)
            {
                currentTime += Time.unscaledDeltaTime;
                float lerpFraction = currentTime / (InputManager.sleepTime / 1000f);
                transform.position = Vector3.Lerp(startPos, targetPos, lerpFraction);
            }
            else
            {
                transform.position = targetPos;
                currentTime = 0;
                startPos = transform.position;
                finished = true;
            }
        }
    }
}
