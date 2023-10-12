using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class MaterialManager : MonoBehaviour
    {
        public static MaterialManager Instance { get; private set; }
        [FormerlySerializedAs("materials")] public Material[] _materials;

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
            if (num >= _materials.Length)
            {
                num = _materials.Length - 1;
            }

            return _materials[num];
        }
    }
}