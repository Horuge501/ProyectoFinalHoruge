using UnityEngine;
using UnityEditor;

namespace ProceduralLevelDesign
{
    public class EditorInput
    {
        #region LocalMethods

        #region UnityMethods

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ScriptsHasBeenReloaded()
        {
            SceneView.duringSceneGui += DuringSceneGui;
        }

        #endregion

        #region DelegateMethods

        private static void DuringSceneGui(SceneView sceneView) //OnDrawGizmos()
        {
            Event e = Event.current; //equivalent to InputAction.CallbackContext / InputValue
            //Event stores data input from the level designer / programmer
            //Debug.Log("EditorInput - DuringSceneGui(): " + e);
            LevelBuilder levelBuilder = GameObject.FindFirstObjectByType<LevelBuilder>();

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Delete) //"Suprimir"
            {
                //Method to clean all the level
                levelBuilder?.ClearLevel();
            }
            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Backspace) // Delete / "Borrar"
            {
                //Method to delete a tile / module from the scene
                levelBuilder?.DeleteModule(e.mousePosition);
                levelBuilder.CheckNeighbours();
            }
            if (e.type == EventType.MouseUp && e.button == 0)
            {
                //Method to instantiate a tile / module in the scene
                levelBuilder?.CreateModule(/*e.mousePosition*/);
                levelBuilder.CheckNeighbours();
            }
            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.C) {
                levelBuilder.CheckNeighbours();
            }
            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.B) {
                Dungeon initialDungeon = new Dungeon() { minX = 0, maxX = levelBuilder.sizeX - 1, minY = 0, maxY = levelBuilder.sizeZ - 1 };
                levelBuilder.BinarySpacePartition(initialDungeon, -1, PreviousCut.NONE);
            }
        }

        #endregion DelegateMethods

        #endregion
    }
}
