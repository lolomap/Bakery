using System;
using BakerySO;
using Craft;
using UnityEngine;
namespace Kitchen
{
    public class KitchenManager : MonoBehaviour
    {
        public static KitchenManager Instance { get; private set; }
        
        public ProducerConfig Configuration;

        private void Awake()
        {
            Instance = this;
            Producer.Init();
        }
    }
}
