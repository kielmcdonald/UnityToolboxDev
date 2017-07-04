using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class SwapObjects : EditorWindow {

	GameObject Parent;
	GameObject ItemToBeReplaced;
	GameObject NewItem;

	[MenuItem ("Window/Utils/SwapObjects")]
	static void Init ()
    {
        SwapObjects window = EditorWindow.GetWindow<SwapObjects>();
        window.Show();
    }


	void OnGUI()
    {
		Parent = (GameObject)EditorGUILayout.ObjectField("Folder to search", Parent, typeof(GameObject), true);
		ItemToBeReplaced = (GameObject)EditorGUILayout.ObjectField("Replace Me", ItemToBeReplaced, typeof(GameObject), true);
		NewItem = (GameObject)EditorGUILayout.ObjectField("New Item", NewItem, typeof(GameObject), true);

		if (GUILayout.Button("Replace Objects") )
        {
			
       		 // search the children of the parent for matches ( item to be replaced ) 
       		 // place new object in place of old 
       		 // delete old object 

			for( int i = 0; i < Parent.transform.childCount -1; i++ ) 
			{

				//Debug.Log(Parent.FindChild(i).name);

				if(ItemToBeReplaced.name == Parent.transform.GetChild(i).name) 
				{
				/*
					GameObject myGameObject = new GameObject("___New Group");
					myGameObject.transform.position = Parent.transform.GetChild(i).transform.position;
					myGameObject.transform.parent = Parent.transform;
					myGameObject.name = NewItem.name;
					*/
					GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(NewItem as GameObject);
					go.transform.position = Parent.transform.GetChild(i).transform.position;
					go.transform.rotation = Parent.transform.GetChild(i).transform.rotation;
					go.transform.localScale = Parent.transform.GetChild(i).transform.localScale;
					go.transform.parent = Parent.transform;

					Parent.transform.GetChild(i).gameObject.SetActive(false);
					Debug.Log("hey");
				}

			}
    	
        }
    }

}
