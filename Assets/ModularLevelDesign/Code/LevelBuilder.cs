using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ProceduralLevelDesign
{
    #region Interfaces
    public interface ILevelEditor
    {
        public void ClearLevel();
        public void DeleteModule(Vector2 value);
        public void CreateModule(/*Vector2 value*/);
    }

    #endregion

    #region Structs

    [System.Serializable]
    public struct Dungeon {
        public int minX;
        public int minY;
        public int maxX;
        public int maxY;

        public bool isSliceableOnX;
        public bool isSliceableOnY;

        public int Width() {
            return maxX - minX;
        }
        public int Height() {
            return maxY - minY;
        }
    }

    #endregion

    #region Enums

    public enum PreviousCut {
        VERTICAL,
        HORIZONTAL,
        NONE
    }

    #endregion

    public class LevelBuilder : MonoBehaviour, ILevelEditor
    {
        #region Parameters

        [SerializeField] GameObject _modulePrefab;
        [SerializeField] public int sizeX = 50;
        [SerializeField] public int sizeZ = 50;

        #endregion

        #region InternalData

        [SerializeField] protected List<Module> _allModulesInScene;

        #endregion

        #region RuntimeVariables

        [SerializeField] protected Module[,] _bidimentionalMatrix;
        protected Ray rayFromSceneCamera;
        protected RaycastHit raycastHit;
        protected GameObject moduleInstance;
        protected Vector3 modulePosition;

        [SerializeField] protected int minDungeonSizeX = 1;
        [SerializeField] protected int minDungeonSizeY = 1;

        #endregion

        #region UnityMethods

        void Awake() {
            LevelBuilder levelBuilder = this;

            levelBuilder?.CreateModule();
            levelBuilder.CheckNeighbours();

            Dungeon initialDungeon = new Dungeon() { minX = 0, maxX = levelBuilder.sizeX - 1, minY = 0, maxY = levelBuilder.sizeZ - 1 };
            levelBuilder.BinarySpacePartition(initialDungeon, -1, PreviousCut.NONE);
        }

        void OnDrawGizmos() {
            Debug.Log("Hola");
            if (_bidimentionalMatrix == null || _bidimentionalMatrix.Length == 0 || this.transform.GetChild(0).childCount == 0) {
                CreateMatrix();
            }
        }

        #endregion

        #region PublicMethods

        public void CreateMatrix() {
            _bidimentionalMatrix = new Module[sizeX, sizeZ];
            for (int x = 0; x < sizeX; x++) {
                for (int z = 0; z < sizeZ; z++) {
                    moduleInstance = Instantiate(_modulePrefab);
                    moduleInstance.transform.position = new Vector3(x, 0, z);
                    moduleInstance.transform.parent = this.transform.GetChild(0);
                    moduleInstance.GetComponent<Module>().SetPositionLabel((int)x, (int)z);
                    moduleInstance.GetComponent<Module>()._levelBuilder = this;
                    _bidimentionalMatrix[x, z] = moduleInstance.GetComponent<Module>();
                    _allModulesInScene.Add(moduleInstance.GetComponent<Module>());
                    //TODO: Desactivar todos los módulos por default ;)
                    //DisableAllModules();
                }
            }
        }

        public void DisableAllModules() {
            foreach (Module module in transform.GetChild(0).GetComponentsInChildren<Module>()) {
                module.VisibilityOfTheModule(Visibility.OFF);
            }
        }

        public Module ModuleAtMatrixPosition(int x, int z) {
            if (x < 0 || z < 0) { return null; }
            if (x >= sizeX || z >= sizeZ) { return null; }
            return _bidimentionalMatrix[x, z];
        }

        public void CheckNeighbours() {
            foreach (Module module in transform.GetChild(0).GetComponentsInChildren<Module>()) {
                if (module._currentVisibility == Visibility.ON) {
                    module.ConnectModule();
                }
            }
        }

        #endregion

        #region InterfaceMethods

        public void ClearLevel()
        {
            foreach (Module module in _allModulesInScene) {
                module.VisibilityOfTheModule(Visibility.ON);
            }
            //Debug.Log(this.name + " - " + gameObject.name + " ClearLevel()", gameObject);
            foreach (Module module in transform.GetChild(0).GetComponentsInChildren<Module>())
            {
                DestroyImmediate(module.gameObject);
            }
            _bidimentionalMatrix = null;
            _allModulesInScene.Clear();
        }

        public void DeleteModule(Vector2 value)
        {
            //Debug.Log(this.name + " - " + gameObject.name + " DeleteModule(" + value.ToString() + ")", gameObject);
            rayFromSceneCamera = HandleUtility.GUIPointToWorldRay(value);  //Camera.main.ScreenPointToRay(value);
            Debug.DrawRay(rayFromSceneCamera.origin, rayFromSceneCamera.direction * 10000f, Color.red, 5f);
            if (Physics.Raycast(rayFromSceneCamera, out raycastHit, 10000f))
            {
                if (raycastHit.collider.gameObject.layer != 6)
                {
                    //moduleInstance = raycastHit.collider.transform.parent.parent.gameObject;

                    //Module moduleToDelete = moduleInstance.GetComponent<Module>();
                    //Vector3 position = moduleInstance.transform.position;
                    //int x = (int)position.x;
                    //int z = (int)position.z;

                    //if (_bidimentionalMatrix != null &&
                    //    x >= 0 && x < _bidimentionalMatrix.GetLength(0) &&
                    //    z >= 0 && z < _bidimentionalMatrix.GetLength(1)) {
                    //    _bidimentionalMatrix[x, z] = null;
                    //}
                    //_allModulesInScene.Remove(moduleInstance.GetComponent<Module>());
                    //DestroyImmediate(moduleInstance);
                    modulePosition = raycastHit.point;
                    if ((int)modulePosition.x < sizeX && modulePosition.x >= 0) //inside the bounds of the array
                    {
                        if ((int)modulePosition.z < sizeZ && modulePosition.z >= 0) {
                            _bidimentionalMatrix[(int)modulePosition.x, ((int)modulePosition.z)].VisibilityOfTheModule(Visibility.OFF);
                        }
                    }
                }
            }
        }
        public void CreateModule(/*Vector2 value*/)
        {
            Debug.Log("Hola");
            if (_bidimentionalMatrix == null || _bidimentionalMatrix.Length == 0 || this.transform.GetChild(0).childCount == 0) {
                CreateMatrix();
            }

            //Debug.Log(this.name + " - " + gameObject.name + " CreateModule(" + value.ToString() + ")", gameObject);
            //rayFromSceneCamera = HandleUtility.GUIPointToWorldRay(value);  //Camera.main.ScreenPointToRay(value);
            Debug.DrawRay(rayFromSceneCamera.origin, rayFromSceneCamera.direction * 10000f, Color.green, 5f);
            if (Physics.Raycast(rayFromSceneCamera, out raycastHit, 10000f))
            {
                if (raycastHit.collider.gameObject.layer == 6) //Layer -> Layout
                {
                    //moduleInstance = Instantiate(_modulePrefab);
                    //moduleInstance.transform.parent = transform;
                    //modulePosition = raycastHit.point;
                    //modulePosition.x = (int)modulePosition.x - 1f;
                    //modulePosition.y = (int)modulePosition.y;
                    //modulePosition.z = (int)modulePosition.z - 1f;
                    //moduleInstance.transform.position = modulePosition;
                    modulePosition = raycastHit.point;
                    if ((int)modulePosition.x < sizeX && modulePosition.x >= 0) //inside the bounds of the array
                    {
                        if ((int)modulePosition.z < sizeZ && modulePosition.z >= 0) {
                            //TODO: Activate module
                            _bidimentionalMatrix[(int)modulePosition.x, ((int)modulePosition.z)].VisibilityOfTheModule(Visibility.ON);
                        }
                    }

                    //if (_bidimentionalMatrix == null) {
                    //    _bidimentionalMatrix = new Module[(int)modulePosition.x, (int)modulePosition.z];
                    //}

                    //_bidimentionalMatrix[(int)modulePosition.x, ((int)modulePosition.z)] = moduleInstance.GetComponent<Module>();

                    //_allModulesInScene.Add(moduleInstance.GetComponent<Module>());
                }
            }
        }

        #endregion

        #region BinarySpacePartionMethods

        public void BinarySpacePartition(Dungeon dungeon, int previousBridge, PreviousCut previousCut) {
            if (dungeon.Width() > minDungeonSizeX * 2) {
                dungeon.isSliceableOnX = true;
            }
            if (dungeon.Height() > minDungeonSizeY * 2) {
                dungeon.isSliceableOnY = true;
            }

            if (!dungeon.isSliceableOnX && !dungeon.isSliceableOnY) {
                return;
            }

            if (dungeon.isSliceableOnX && dungeon.isSliceableOnY) {
                int randomAxisToCut = (int)Random.Range(0f, 1.99f);

                if (randomAxisToCut == 0) {
                    dungeon.isSliceableOnX = false;
                }
                else {
                    dungeon.isSliceableOnY = false;
                }
            }

            if (dungeon.isSliceableOnX && !dungeon.isSliceableOnY) {
                int randomCut = (int)Random.Range(dungeon.minX + minDungeonSizeX + 1, dungeon.maxX - minDungeonSizeX - 1);
                
                if (previousCut == PreviousCut.VERTICAL) {
                    if ((dungeon.maxX - minDungeonSizeX - 1) - (dungeon.minX + minDungeonSizeX + 1) > 1) {
                        while (randomCut == previousBridge) {
                            randomCut = (int)Random.Range(dungeon.minX + minDungeonSizeX + 1, dungeon.maxX - minDungeonSizeX - 1);
                        }
                    }
                    else {
                        return;
                    }
                }


                for (int i = dungeon.minY; i <= dungeon.maxY; i++) {
                    if(_bidimentionalMatrix[randomCut, i].isABridge == false)
                    _bidimentionalMatrix[randomCut, i].VisibilityOfTheModule(Visibility.OFF);
                }

                int randomBridge = Random.Range(dungeon.minY, dungeon.maxY);

                _bidimentionalMatrix[randomCut, randomBridge].VisibilityOfTheModule(Visibility.ON);

                Dungeon dungeon1 = new Dungeon() { minX = dungeon.minX, maxX = randomCut - 1, minY = dungeon.minY, maxY = dungeon.maxY };
                Dungeon dungeon2 = new Dungeon() { minX = randomCut + 1, maxX = dungeon.maxX, minY = dungeon.minY, maxY = dungeon.maxY };

                BinarySpacePartition(dungeon1, randomBridge, PreviousCut.HORIZONTAL);
                BinarySpacePartition(dungeon2, randomBridge, PreviousCut.HORIZONTAL);

                CheckNeighbours();
            }

            if (!dungeon.isSliceableOnX && dungeon.isSliceableOnY) {
                int randomCut = (int)Random.Range(dungeon.minY + minDungeonSizeY + 1, dungeon.maxY - minDungeonSizeY - 1);

                if (previousCut == PreviousCut.HORIZONTAL) {
                    if ((dungeon.maxY - minDungeonSizeY - 1) - (dungeon.minY + minDungeonSizeY + 1) > 1) {
                        while (randomCut == previousBridge) {
                            randomCut = (int)Random.Range(dungeon.minY + minDungeonSizeY + 1, dungeon.maxY - minDungeonSizeY - 1);
                        }
                    } else {
                        return;
                    }
                }

                for (int i = dungeon.minX; i <= dungeon.maxX; i++) {
                    if(_bidimentionalMatrix[i, randomCut].isABridge == false)
                    _bidimentionalMatrix[i, randomCut].VisibilityOfTheModule(Visibility.OFF);
                }

                int randomBridge = Random.Range(dungeon.minX, dungeon.maxX);

                _bidimentionalMatrix[randomBridge, randomCut].VisibilityOfTheModule(Visibility.ON);
                _bidimentionalMatrix[randomBridge, randomCut].bridgeWall.SetActive(true);
                _bidimentionalMatrix[randomBridge, randomCut].isABridge = true;

                Dungeon dungeon1 = new Dungeon() { minX = dungeon.minX, maxX = dungeon.maxX, minY = dungeon.minY, maxY = randomCut - 1 };
                Dungeon dungeon2 = new Dungeon() { minX = dungeon.minX, maxX = dungeon.maxX, minY = randomCut + 1, maxY = dungeon.maxY };

                BinarySpacePartition(dungeon1, randomBridge, PreviousCut.VERTICAL);
                BinarySpacePartition(dungeon2, randomBridge, PreviousCut.VERTICAL);

                CheckNeighbours();
            }
        }

        #endregion
    }
}
