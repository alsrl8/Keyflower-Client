using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class MaterialManager : MonoBehaviour
    {
        public static MaterialManager Instance { get; private set; }
        public Material[] materials;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public Material GetNumberMaterial(int num)
        {
            if (num >= materials.Length)
            {
                num = materials.Length - 1;
            }

            return materials[num];
        }
    }
}