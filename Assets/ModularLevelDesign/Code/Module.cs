using UnityEngine;

namespace ProceduralLevelDesign
{
    public enum Visibility {
        ON,
        OFF
    }

    [System.Serializable]
    public struct Walls {
        public GameObject NORTHWALL;
        public GameObject SOUTHWALL;
        public GameObject EASTWALL;
        public GameObject WESTWALL;
        public GameObject FLOOR;
        //public GameObject NORTHDOOR;
        //public GameObject SOUTHDOOR;
        //public GameObject EASTDOOR;
        //public GameObject WESTDOOR;
    }

    [System.Serializable]
    public struct Pillars {
        public GameObject SOUTHEASTPILAR;
        public GameObject SOUTHWESTPILAR;
        public GameObject NORTHEASTPILAR;
        public GameObject NORTHWESTPILAR;
    }

    public class Module : MonoBehaviour
    {
        public LevelBuilder _levelBuilder;
        [SerializeField] protected int xCoordinateAtMatrix;
        [SerializeField] protected int zCoordinateAtMatrix;
        [SerializeField] public Walls walls; 
        [SerializeField] public Pillars pillars;
        [SerializeField] public Visibility _currentVisibility;
        protected Module moduleCandidate;

        public void VisibilityOfTheModule(Visibility value) {
            _currentVisibility = value;

            switch (value) {
                case Visibility.ON:
                    ActivateModule();
                    break; 
                case Visibility.OFF:
                    DeactivateModule();
                    break;
            }
        }

        public void ConnectModule() {
            NeighborDetected(xCoordinateAtMatrix - 1, zCoordinateAtMatrix, walls.EASTWALL);
            NeighborDetected(xCoordinateAtMatrix + 1, zCoordinateAtMatrix, walls.WESTWALL);
            NeighborDetected(xCoordinateAtMatrix, zCoordinateAtMatrix - 1, walls.SOUTHWALL);
            NeighborDetected(xCoordinateAtMatrix, zCoordinateAtMatrix + 1, walls.NORTHWALL);
            PillarToSetActive(xCoordinateAtMatrix - 1, zCoordinateAtMatrix + 1, walls.EASTWALL, walls.NORTHWALL, pillars.NORTHEASTPILAR);
            PillarToSetActive(xCoordinateAtMatrix + 1, zCoordinateAtMatrix + 1, walls.WESTWALL, walls.NORTHWALL, pillars.NORTHWESTPILAR);
            PillarToSetActive(xCoordinateAtMatrix - 1, zCoordinateAtMatrix - 1, walls.EASTWALL, walls.SOUTHWALL, pillars.SOUTHEASTPILAR);
            PillarToSetActive(xCoordinateAtMatrix + 1, zCoordinateAtMatrix - 1, walls.WESTWALL, walls.SOUTHWALL, pillars.SOUTHWESTPILAR);
        }

        public void NeighborDetected(int x, int z, GameObject wallToTurn) {
            moduleCandidate = _levelBuilder.ModuleAtMatrixPosition(x, z); 
            if (moduleCandidate?._currentVisibility == Visibility.ON) {
                wallToTurn.SetActive(false);
            }
            else if (moduleCandidate?._currentVisibility == Visibility.OFF) {
                wallToTurn.SetActive(true);
            }
        }

        public void PillarToSetActive (int x, int z, GameObject wallA, GameObject wallB, GameObject pillarToTurn) {
            if (!wallA.activeSelf && !wallB.activeSelf) {
                moduleCandidate = _levelBuilder.ModuleAtMatrixPosition(x, z);
                Debug.Log(x + ", " + z);
                if (moduleCandidate?._currentVisibility == Visibility.ON) {
                    Debug.Log("hay esquina");
                    pillarToTurn.SetActive(false);
                } else if (moduleCandidate?._currentVisibility == Visibility.OFF) {
                    Debug.Log("no hay esquina");
                    pillarToTurn.SetActive(true);
                }
            } else {
                pillarToTurn.SetActive(true);
            }
        }

        public void SetPositionLabel(int x, int z) {
            xCoordinateAtMatrix = x;
            zCoordinateAtMatrix = z;
        }

        protected void ActivateModule() {
            gameObject.SetActive(true);
        }

        protected void DeactivateModule() {
            gameObject.SetActive(false);
        }
    }
}