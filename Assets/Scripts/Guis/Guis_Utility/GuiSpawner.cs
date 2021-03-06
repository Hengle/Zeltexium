﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeltex;
using Zeltex.Guis.Maker;

namespace Zeltex.Guis
{
    /// <summary>
    /// Spawns all the guis for the player
    /// </summary>
    [ExecuteInEditMode]
    public class GuiSpawner : ManagerBase<GuiSpawner>
    {
        public static bool IsUseSortOrders = true;
        [Header("Debug")]
        public bool IsDebugGui;
        [SerializeField]
        private EditorAction ActionScanMap;
        private KeyCode DebugOpenKey = KeyCode.F2;
        [Header("Start")]
        public bool IsLoadOnStart;
        //public string LoadGui = "";
        [Header("Data")]
        public int ButtonHeight = 60;
        public int ButtonWidth = 200;
        [Header("Prefabs")]
        public List<GameObject> GuiPrefabs;
        public List<GameObject> PainterPrefabs;
        public List<GameObject> MetaMakers;
        public List<GameObject> MainPrefabs;
        private bool IsContentMakers;
        private bool IsPainters;
        private int GuiPrefabType = 0;
        [SerializeField]
        private List<GameObject> GuiSpawns = new List<GameObject>();

        void Start()
        {
            if (Application.isPlaying)
            {
                ScanForGuis();
                SpawnIfNotSpawned("MainMenu");
                SpawnIfNotSpawned("LoadingGui");
                SpawnIfNotSpawned("TooltipGui");
            }
        }
        
        private void SpawnIfNotSpawned(string NewGui)
        {
            if (!HasSpawned(NewGui))
            {
                SpawnGui(NewGui);
            }
        }

        private void ScanForGuis()
        {
            ZelGui[] MyObjects = GameObject.FindObjectsOfType<ZelGui>();
            GuiSpawns.Clear();
            for (int i = 0;i < MyObjects.Length; i++)
            {
                GuiSpawns.Add(MyObjects[i].gameObject);
            }
        }

        private void Update()
        {
            if (ActionScanMap.IsTriggered())
            {
                ScanForGuis();
            }
            if (Application.isPlaying)
            {
                if (Input.GetKeyDown(DebugOpenKey))
                {
                    IsDebugGui = !IsDebugGui;
                }
            }
        }

        public GameObject SpawnGui(string GuiName)
        {
            if (GuiName != "")
            {
                if (SpawnFromList(GuiName, MainPrefabs))
                {
                    return GuiSpawns[GuiSpawns.Count - 1];
                }
                if (SpawnFromList(GuiName, GuiPrefabs))
                {
                    return GuiSpawns[GuiSpawns.Count - 1];
                }
                if (SpawnFromList(GuiName, PainterPrefabs))
                {
                    return GuiSpawns[GuiSpawns.Count - 1];
                }
                if (SpawnFromList(GuiName, MetaMakers))
                {
                    return GuiSpawns[GuiSpawns.Count - 1];
                }
            }
            Debug.LogError("[SpawnGui] Could not Spawn: " + GuiName + " - inside: " + name + " at time " + Time.time);
            return null;
        }

        public void EnableGui(string GuiName)
        {
            GameObject MyGuiObject = GuiSpawner.Get().GetGui(GuiName);
            if (MyGuiObject)
            {
                ZelGui MyGui = MyGuiObject.GetComponent<ZelGui>();
                if (MyGui)
                {
                    MyGui.Enable();
                }
            }
        }

        public void DisableGui(string GuiName)
        {
            GameObject MyGuiObject = GuiSpawner.Get().GetGui(GuiName);
            if (MyGuiObject)
            {
                ZelGui MyGui = MyGuiObject.GetComponent<ZelGui>();
                if (MyGui)
                {
                    MyGui.Disable();
                }
            }
        }

        private bool SpawnFromList(string GuiName, List<GameObject> Prefabs)
        {
            for (int i = 0; i < Prefabs.Count; i++)
            {
                if (Prefabs[i].name == GuiName)
                {
                    SpawnGui(Prefabs[i]);
                    return true;
                }
            }
            //Debug.LogError("Failed To Spawn " + GuiName);
            return false;
        }

        private void OnGUI()
        {
            if (IsDebugGui)
            {
                GUILayout.BeginArea(new Rect(Screen.width - ButtonWidth * 2, 0, ButtonWidth * 2, 64));
                GuiPrefabType = GUILayout.Toolbar(GuiPrefabType, new string[] { "Makers", "Tools", "Makers++", "Main" }, GUILayout.Height(64));
                GUILayout.EndArea();
                GUILayout.BeginArea(new Rect(Screen.width - ButtonWidth, 64, ButtonWidth, 1000));
                GUILayout.BeginVertical();
                if (GUILayout.Button("Close"))
                {
                    IsDebugGui = false;
                }
                // toggles for things
                if (GuiPrefabType == 0)
                {
                    ShowSpawnList(GuiPrefabs);
                }
                else if (GuiPrefabType == 1)
                {
                    ShowSpawnList(PainterPrefabs);
                }
                else if (GuiPrefabType == 2)
                {
                    ShowSpawnList(MetaMakers);
                }
                else if (GuiPrefabType == 3)
                {
                    ShowSpawnList(MainPrefabs);
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        private void ShowSpawnList(List<GameObject> MyPrefabs)
        {
            for (int i = 0; i < MyPrefabs.Count; i++)
            {
                if (HasSpawned(MyPrefabs[i].name))
                {
                    //GUILayout.Label(GuiPrefabs[i].name);
                    if (GUILayout.Button(MyPrefabs[i].name + " [On]", GUILayout.Height(ButtonHeight)))
                    {
                        for (int j = 0; j < GuiSpawns.Count; j++)
                        {
                            if (GuiSpawns[j].name == MyPrefabs[i].name)
                            {
                                DestroySpawn(GuiSpawns[j]);
                            }
                        }
                    }
                }
                else if (GUILayout.Button(MyPrefabs[i].name + " [Off]", GUILayout.Height(ButtonHeight)))
                {
                    SpawnGui(MyPrefabs[i]);
                }
            }
        }

        private bool HasSpawned(string GuiName)
        {
            for (int i = 0; i < GuiSpawns.Count; i++)
            {
                if (GuiSpawns[i] && GuiSpawns[i].name == GuiName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Spawns a gui
        /// </summary>
        public void SpawnGui(GameObject MyPrefab)
        {
            if (HasSpawned(MyPrefab.name))
            {
                Debug.LogError(MyPrefab.name + " already exists.");
                return;
            }
            GameObject NewGui = Instantiate(MyPrefab);
            NewGui.name = MyPrefab.name;
            NewGui.transform.SetParent(transform);
            NewGui.gameObject.SetActive(true);
            if (IsUseSortOrders)
            {
                NewGui.GetComponent<Canvas>().sortingOrder = GuiSpawns.Count;
            }
            GuiSpawns.Add(NewGui);

            Transform CloseTransform = NewGui.transform.Find("CloseButton");
            if (CloseTransform == null)
            {
                //Debug.LogError("Bam");
                CloseTransform = NewGui.transform.Find("HeaderButtons");
                if (CloseTransform)
                {
                    //Debug.LogError("Bam2");
                    CloseTransform = CloseTransform.Find("CloseButton");
                }
            }
            if (CloseTransform)
            {
                UnityEngine.UI.Button CloseButton = CloseTransform.gameObject.GetComponent<UnityEngine.UI.Button>();
                //Debug.LogError("Bam3 " + CloseButton.name);
                CloseButton.onClick.RemoveAllListeners();
                CloseButton.onClick.AddListener(
                    delegate 
                    {
                        DestroySpawn(NewGui);
                    });
            }
            ZelGui MyGui = NewGui.GetComponent<ZelGui>();
            if (MyGui)
            {
                MyGui.OnBegin();
            }
        }

        public void DestroySpawn(string GuiName)
        {
            GameObject MyGui = GetGui(GuiName);
            if (MyGui)
            {
                GuiSpawns.Remove(MyGui);
                MyGui.Die();
            }
        }

        public void DestroySpawn(GameObject MyGui)
        {
            if (GuiSpawns.Contains(MyGui))
            {
                GuiSpawns.Remove(MyGui);
            }
			ZelGui MyZelGui = MyGui.GetComponent<ZelGui>();
			if (MyZelGui) 
			{
				MyZelGui.OnToggledOffEvent();
			}
            MyGui.Die();
        }

        public GameObject GetGui(string GuiName)
        {
            for (int i = GuiSpawns.Count - 1; i >= 0; i--)
            {
                if (GuiSpawns[i])
                {
                    if (GuiSpawns[i].name == GuiName)
                    {
                        return GuiSpawns[i];
                    }
                }
                else
                {
                    GuiSpawns.RemoveAt(i);
                }
            }
            return null;
        }

        public void Disable(GameObject MyGui)
        {
            if (MyGui)
            {
                ZelGui MyGuiZelGui = MyGui.GetComponent<ZelGui>();
                if (MyGuiZelGui)
                {
                    MyGuiZelGui.Disable();
                }
            }
        }
        public void Enable(GameObject MyGui)
        {
            if (MyGui)
            {
                ZelGui MyGuiZelGui = MyGui.GetComponent<ZelGui>();
                if (MyGuiZelGui)
                {
                    MyGuiZelGui.Enable();
                }
            }
        }

        public static bool IsSingleMakerMode = true;
        private GameObject PreviousSpawnedMakerGui;
        public GameObject SpawnMakerGui(string MakerName)
        {
            GameObject PreviousPreviousMakerGui = PreviousSpawnedMakerGui;
            if (MakerName.Contains("Maker"))
            {
                PreviousSpawnedMakerGui = SpawnGui(MakerName);
            }
            if (PreviousPreviousMakerGui && IsSingleMakerMode)
            {
                PreviousPreviousMakerGui.Die();
            }
            return PreviousSpawnedMakerGui;
        }
    }
}