using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GameModel;

namespace StageObject
{
    public class StoneObstacle : Obstacle
    {
        protected override void Destroy()
        {
            gameObject.SetActive(false);
        }
    }
}
