using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class Toolbox : EditorWindow 
{
	// window scaleability vars \\
	static bool menuCondensed;													// if the width of the window is smaller than the condensed size
	[SerializeField] static float windowSizeCondensed = 125.0f;					// size (pixel) width when the window will reorgonize to a more vertical layout
	Vector2 scrollPos;															// window scroll position 
	Vector2 WindowSize = new Vector2(96,96);									// smallest limit the window can be scaled 
	
	// Colors
	Color btnBGColor = GUI.color;												// this is used to change the color of the fold out button 
	Color sectionBtnOpenColor = new Color (.4f,.4f,.4f,1);
	Color sectionBtnCloseColor = new Color (.6f,.6f,.6f,1);

	// Fold outs 
	AnimBool expandFields_AddEditors;
	AnimBool expandFields_ToolBoxOptions;
	AnimBool expandFields_AlignTools;
	AnimBool expandFields_AlignTools_Options;
	AnimBool expandFields_GroupTools;
	AnimBool expandFields_SelectionTools;
	AnimBool expandFields_CreateTools;
	AnimBool expandFields_CleanUpTools;
	AnimBool expandFields_CleanUpTools_Options;

	// Additonal editor items 
	string [] m_TypeNames;
	System.Type[] m_Types;
	[SerializeField] static string additionalEditorsPrefix = "wp_";

	// Selection and saved objects
	static GameObject[] selectedObjects; 										// array of all object selected 
	static GameObject[] saveObjs; 												// array of all objects selected to be saved 
	
	static bool storeSelection = true; 											// saves the current selection 

	// Align options
	static bool rotXAlignActive;
	static bool rotYAlignActive;
	static bool rotZAlignActive;
	static bool xAlignActive = true;
	static bool yAlignActive;
	static bool zAlignActive = true;

	// Clean Up Options
	static bool ignoreOrigin = true;													// ignore objects located at the world origin
	

	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	#region EDITOR WINDOW AND MENU 
	[MenuItem("Window/Toolbox Window")]
	public static void ShowWindow()
	{
		//show window instance, make one if there isnt one 
		EditorWindow.GetWindow(typeof(Toolbox));
	}
	#endregion
	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	#region WINDOW SET UP ON ENABLE 
	void OnEnable()
	{
		expandFields_AddEditors = new AnimBool(false);
		expandFields_AddEditors.valueChanged.AddListener(Repaint);
		expandFields_ToolBoxOptions = new AnimBool(false);
		expandFields_ToolBoxOptions.valueChanged.AddListener(Repaint);
		expandFields_AlignTools = new AnimBool(false);
		expandFields_AlignTools.valueChanged.AddListener(Repaint);
		expandFields_AlignTools_Options = new AnimBool(false);
		expandFields_AlignTools_Options.valueChanged.AddListener(Repaint);
		expandFields_GroupTools = new AnimBool(false);
		expandFields_GroupTools.valueChanged.AddListener(Repaint);
		expandFields_SelectionTools = new AnimBool(false);
		expandFields_SelectionTools.valueChanged.AddListener(Repaint);
		expandFields_CreateTools = new AnimBool(false);
		expandFields_CreateTools.valueChanged.AddListener(Repaint);
		expandFields_CleanUpTools = new AnimBool(false);
		expandFields_CleanUpTools.valueChanged.AddListener(Repaint);
		expandFields_CleanUpTools_Options = new AnimBool(false);
		expandFields_CleanUpTools_Options.valueChanged.AddListener(Repaint);
	}
	#endregion
	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	#region EDITOR WINDOW UI
	void OnGUI()
	{
		// This reloads scripts the in the project folder -  used to show new buttons for additonal editor windows
		if( m_TypeNames == null || m_Types == null) Reload();

		// Menu auto condensing 
		if (this.position.xMax - this.position.xMin < windowSizeCondensed)
			menuCondensed = true;
		else 
			menuCondensed = false;

		this.minSize = WindowSize;
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);


		#region  // STYLES \\
		GUIStyle titleStyle = new GUIStyle (GUI.skin.label);
		titleStyle.fontSize = 18;
		titleStyle.fontStyle = FontStyle.Bold;

		GUIStyle boldStyle = new GUIStyle (GUI.skin.label);
		titleStyle.fontSize = 13;
		boldStyle.fontStyle = FontStyle.Bold;
		#endregion


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		#region // WINDOW HEADER - TITLE \\
		EditorGUILayout.BeginVertical();
		{
			if (menuCondensed && WindowSize.x < 72f) titleStyle.fontSize = 12;

			GUILayout.Label ("Toolbox", titleStyle);
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}
		EditorGUILayout.EndVertical();
		#endregion
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		#region // ADDITIONAL EDITORS \\
		EditorGUILayout.BeginVertical("Box");
		{
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
			
			GUI.backgroundColor = (expandFields_AddEditors.target ? sectionBtnOpenColor : sectionBtnCloseColor );
						
			if (GUILayout.Button("Editors")) expandFields_AddEditors.target = !expandFields_AddEditors.target;

			GUI.backgroundColor = btnBGColor;
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}
		EditorGUILayout.EndVertical();

		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		// ADDITION EDITORS FOLD OUT 
		if (expandFields_AddEditors.target) EditorGUILayout.BeginVertical("Box");
		if (EditorGUILayout.BeginFadeGroup(expandFields_AddEditors.faded))
		{
			if (!menuCondensed) EditorGUILayout.BeginHorizontal();
			{
				for( int i = 0; i < m_Types.Length; i++ )
				{
					if (DrawButton(m_TypeNames[i].Substring(3), "Display editor window for tool", true, 15f))
					{
						EditorWindow.GetWindow(m_Types[i]);
					}
				}
			}
			if (!menuCondensed) EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndFadeGroup();
		if (expandFields_AddEditors.target) EditorGUILayout.EndVertical();
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

		EditorGUILayout.Space();
		#endregion
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		#region // ALIGN TOOLS \\
		EditorGUILayout.BeginVertical("Box");
		{
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
			
			GUI.backgroundColor = (expandFields_AlignTools.target ? sectionBtnOpenColor : sectionBtnCloseColor );
			
			if (GUILayout.Button("Align")) expandFields_AlignTools.target = !expandFields_AlignTools.target;
			
			GUI.backgroundColor = btnBGColor;
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}
		EditorGUILayout.EndVertical();

		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		// ALIGN TOOLS FOLDOUT
		if (expandFields_AlignTools.target) EditorGUILayout.BeginVertical("Box");
		if (EditorGUILayout.BeginFadeGroup(expandFields_AlignTools.faded))
		{
			if (!menuCondensed) EditorGUILayout.BeginHorizontal();
			{
				if (DrawButton("Origin", "Reset to World Origin", true, 25f)) 
				{
					selectedObjects = Selection.gameObjects;	
					if (selectedObjects != null && selectedObjects.Length > 0)
					{
						foreach (GameObject obj in Selection.gameObjects)
						{
							Undo.RecordObject(obj.transform, "Transform Change");
							obj.transform.position = Vector3.zero;
						}
					}
				}
				
				if (DrawButton("Local", "Reset to Parent Origin", true, 25f)) 
				{
					AlignLocal();
				}
				
				if (DrawButton("Grid", "Align Object to nearest GirdSpace", true, 25f)) 
				{
					Align();
				}
			}
			if (!menuCondensed) EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
			// OPTIONS FOLD OUT
			EditorGUILayout.BeginVertical();
			{
				GUI.backgroundColor = (expandFields_AlignTools_Options.target ? sectionBtnOpenColor : sectionBtnCloseColor );
				if (GUILayout.Button("Options")) expandFields_AlignTools_Options.target = !expandFields_AlignTools_Options.target;
				GUI.backgroundColor = btnBGColor;
			}
			EditorGUILayout.EndVertical();
			
			// Align Options foldout
			if (expandFields_AlignTools_Options.target) EditorGUILayout.BeginVertical("Box");
			{
				if (EditorGUILayout.BeginFadeGroup(expandFields_AlignTools_Options.faded))
				{
					if (!menuCondensed) EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label ("Position:");
						if (!menuCondensed) EditorGUILayout.BeginHorizontal();
						{
							xAlignActive = GUILayout.Toggle(xAlignActive, "X");
							yAlignActive = GUILayout.Toggle(yAlignActive, "Y");
							zAlignActive = GUILayout.Toggle(zAlignActive, "Z");
						}
						if (!menuCondensed) EditorGUILayout.EndHorizontal();
					}
					if (!menuCondensed) EditorGUILayout.EndHorizontal();

					if (!menuCondensed) EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label ("Rotation:");
						if (!menuCondensed) EditorGUILayout.BeginHorizontal();
						{
							rotXAlignActive = GUILayout.Toggle(rotXAlignActive, "X");
							rotYAlignActive = GUILayout.Toggle(rotYAlignActive, "Y");
							rotZAlignActive = GUILayout.Toggle(rotZAlignActive, "Z");
						}
						if (!menuCondensed) EditorGUILayout.EndHorizontal();
					}
					if (!menuCondensed) EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndFadeGroup();
			}
			if (expandFields_AlignTools_Options.target) EditorGUILayout.EndVertical();
			// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		}
		EditorGUILayout.EndFadeGroup();
		if (expandFields_AlignTools.target) EditorGUILayout.EndVertical();
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

		EditorGUILayout.Space();
		#endregion
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		#region // GROUP TOOLS \\
		EditorGUILayout.BeginVertical("Box");
		{
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
			
			GUI.backgroundColor = (expandFields_GroupTools.target ? sectionBtnOpenColor : sectionBtnCloseColor );
			
			if (GUILayout.Button("Group")) expandFields_GroupTools.target = !expandFields_GroupTools.target;
			
			GUI.backgroundColor = btnBGColor;
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}
		EditorGUILayout.EndVertical();

		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		// GROUP FOLD OUT
		if (expandFields_GroupTools.target) EditorGUILayout.BeginVertical("Box");
		if (EditorGUILayout.BeginFadeGroup(expandFields_GroupTools.faded))
		{
			if (!menuCondensed) EditorGUILayout.BeginHorizontal();
			{
				if (DrawButton("New", "Adds selected to a new group", true, 25f)) 
				{
					GroupObjects();
				}
				
				if (DrawButton("Move", "Adds selected to last selected objects parent", true, 25f)) 
				{
					groupPlus();
				}

				if (DrawButton("Clear", "Removes selected game objects parent object", true, 25f)) 
				{
					clearParent();
				}
			}
			if (!menuCondensed) EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndFadeGroup();
		if (expandFields_GroupTools.target) EditorGUILayout.EndVertical();
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

		EditorGUILayout.Space();
		#endregion
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		#region // SELECTION TOOLS \\
		EditorGUILayout.BeginVertical("Box");
		{
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
			
			GUI.backgroundColor = (expandFields_SelectionTools.target ? sectionBtnOpenColor : sectionBtnCloseColor );
			
			if (GUILayout.Button("Selection")) expandFields_SelectionTools.target = !expandFields_SelectionTools.target;
			
			GUI.backgroundColor = btnBGColor;
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}
		EditorGUILayout.EndVertical();

		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		// GROUP FOLD OUT
		if (expandFields_SelectionTools.target) EditorGUILayout.BeginVertical("Box");
		if (EditorGUILayout.BeginFadeGroup(expandFields_SelectionTools.faded))
		{
			if (!menuCondensed) EditorGUILayout.BeginHorizontal();
			{
				if (DrawButton("Hide", "Saved current selection and toggles active state", true, 25f)) 
				{
					toggleActive();
				}

				if (DrawButton("Parent", "Selects the parent object of the current selection", true, 25f)) 
				{
					SelectParent();
				}

				if (DrawButton("Children", "Selects all child objects of current selection", true, 25f)) 
				{
					SelectChildren();
				}
			}
			if (!menuCondensed) EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndFadeGroup();
		if (expandFields_SelectionTools.target) EditorGUILayout.EndVertical();
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

		EditorGUILayout.Space();
		#endregion
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		#region // CREATE TOOLS \\
		EditorGUILayout.BeginVertical("Box");
		{
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
			
			GUI.backgroundColor = (expandFields_CreateTools.target ? sectionBtnOpenColor : sectionBtnCloseColor );
			
			if (GUILayout.Button("Create")) expandFields_CreateTools.target = !expandFields_CreateTools.target;
			
			GUI.backgroundColor = btnBGColor;
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}
		EditorGUILayout.EndVertical();

		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		// GROUP FOLD OUT
		if (expandFields_CreateTools.target) EditorGUILayout.BeginVertical("Box");
		if (EditorGUILayout.BeginFadeGroup(expandFields_CreateTools.faded))
		{
			if (!menuCondensed) EditorGUILayout.BeginHorizontal();
			{
				if (DrawButton("Empty Child", "Creates an empty child object", true, 25f)) 
				{
					NewChildObject();
				}
			}
			if (!menuCondensed) EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndFadeGroup();
		if (expandFields_CreateTools.target) EditorGUILayout.EndVertical();
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

		EditorGUILayout.Space();
		#endregion
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		#region // CLEAN UP TOOLS \\
		EditorGUILayout.BeginVertical("Box");
		{
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
			
			GUI.backgroundColor = (expandFields_CleanUpTools.target ? sectionBtnOpenColor : sectionBtnCloseColor );
			
			if (GUILayout.Button("Clean Up")) expandFields_CleanUpTools.target = !expandFields_CleanUpTools.target;
			
			GUI.backgroundColor = btnBGColor;
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}
		EditorGUILayout.EndVertical();

		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		// GROUP FOLD OUT
		if (expandFields_CleanUpTools.target) EditorGUILayout.BeginVertical("Box");
		if (EditorGUILayout.BeginFadeGroup(expandFields_CleanUpTools.faded))
		{
			if (!menuCondensed) EditorGUILayout.BeginHorizontal();
			{
				if (DrawButton("Empty", "Find and select empty gameObjects with no children", true, 25f)) 
				{
					findEmpty();
				}

				if (DrawButton("Same POS", "Find and select gameObjects in the same location", true, 25f)) 
				{
					findSameLocation();
				}
			}
			if (!menuCondensed) EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();


			// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
			// OPTIONS FOLD OUT
			EditorGUILayout.BeginVertical();
			{
				GUI.backgroundColor = (expandFields_CleanUpTools_Options.target ? sectionBtnOpenColor : sectionBtnCloseColor );
				if (GUILayout.Button("Options")) expandFields_CleanUpTools_Options.target = !expandFields_CleanUpTools_Options.target;
				GUI.backgroundColor = btnBGColor;
			}
			EditorGUILayout.EndVertical();

			// Align Options foldout
			if (expandFields_CleanUpTools_Options.target) EditorGUILayout.BeginVertical("Box");
			{
				if (EditorGUILayout.BeginFadeGroup(expandFields_CleanUpTools_Options.faded))
				{
					if (!menuCondensed) EditorGUILayout.BeginHorizontal();
					{
						if (!menuCondensed) EditorGUILayout.BeginHorizontal();
						{
							ignoreOrigin = GUILayout.Toggle(ignoreOrigin, new GUIContent(" Origin", "Ignore the world origin when looking for objects at the same location") );
						}
						if (!menuCondensed) EditorGUILayout.EndHorizontal();
					}
					if (!menuCondensed) EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndFadeGroup();
			}
			if (expandFields_CleanUpTools_Options.target) EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFadeGroup();
		if (expandFields_CleanUpTools.target) EditorGUILayout.EndVertical();
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

		EditorGUILayout.Space();
		#endregion
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		#region // TOOLBOX OPTIONS \\
		EditorGUILayout.BeginVertical("Box");
		{
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
			
			GUI.backgroundColor = (expandFields_ToolBoxOptions.target ? sectionBtnOpenColor : sectionBtnCloseColor );
			
			if (GUILayout.Button("Options")) expandFields_ToolBoxOptions.target = !expandFields_ToolBoxOptions.target;
			
			GUI.backgroundColor = btnBGColor;
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}
		EditorGUILayout.EndVertical();

		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		// TOOLBOX OPTIONS FOLDOUT
		if (expandFields_ToolBoxOptions.target) EditorGUILayout.BeginVertical("Box");
		if (EditorGUILayout.BeginFadeGroup(expandFields_ToolBoxOptions.faded))
		{
			EditorGUILayout.Space();


			EditorGUILayout.BeginVertical();
			{
				GUILayout.Label("Window Options:", boldStyle, GUILayout.MinWidth(5f) );

				// Window condese width
				if (!menuCondensed) EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(new GUIContent("Condense Width:" , "The size in pixels when the tollbox window will condense" ), GUILayout.MinWidth(5f));
					windowSizeCondensed = EditorGUILayout.FloatField(windowSizeCondensed, GUILayout.MinWidth(5f), GUILayout.MaxWidth(80f) );
				}
				if (!menuCondensed) EditorGUILayout.EndHorizontal();

				// Window min size 
				if (!menuCondensed) EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(new GUIContent("Window Min Size:" , "The smallest size in pixels the tollbox window can scale down"), GUILayout.MinWidth(5f));
					//if (menuCondensed) EditorGUILayout.BeginHorizontal();
					//{
					WindowSize.x = EditorGUILayout.FloatField(WindowSize.x, GUILayout.MinWidth(5f), GUILayout.MaxWidth(40f) );
					WindowSize.y = EditorGUILayout.FloatField(WindowSize.y, GUILayout.MinWidth(5f), GUILayout.MaxWidth(40f) );
					//}
					//if (menuCondensed) EditorGUILayout.EndHorizontal();

					//WindowSize = EditorGUILayout.Vector2Field("", WindowSize, GUILayout.MinWidth(5f));
				}
				if (!menuCondensed) EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();
				GUILayout.Label("Button Options:", boldStyle, GUILayout.MinWidth(5f) );

				// Section Btn colors 
				if (!menuCondensed) EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(new GUIContent("Open Color:" , "The color section buttons will be when open" ), GUILayout.MinWidth(5f));
					sectionBtnOpenColor = EditorGUILayout.ColorField(sectionBtnOpenColor, GUILayout.MinWidth(5f), GUILayout.MaxWidth(80f));
				}
				if (!menuCondensed) EditorGUILayout.EndHorizontal();

				if (!menuCondensed) EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(new GUIContent("Closed Color:" , "The color section buttons will be when closed" ), GUILayout.MinWidth(5f));
					sectionBtnCloseColor = EditorGUILayout.ColorField(sectionBtnCloseColor, GUILayout.MinWidth(5f), GUILayout.MaxWidth(80f));
				}
				if (!menuCondensed) EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();
				GUILayout.Label("Additonal Editor Options:", boldStyle, GUILayout.MinWidth(5f) );

				// Additional editor prefix 
				if (!menuCondensed) EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(new GUIContent("Editor Prefix:" , "The prefix string of additonal editors that will load as editor buttons"), GUILayout.MinWidth(5f));
					additionalEditorsPrefix = EditorGUILayout.TextField(additionalEditorsPrefix, GUILayout.MinWidth(5f), GUILayout.MaxWidth(80f) ); 
				}
				if (!menuCondensed) EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
		}
		EditorGUILayout.EndFadeGroup();
		if (expandFields_ToolBoxOptions.target) EditorGUILayout.EndVertical();
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

		EditorGUILayout.Space();

		#endregion
		// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
		EditorGUILayout.EndScrollView();
	}
	#endregion
	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

	
	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	# region RELOAD SCRIPTS 
	void Reload()
	{
		Assembly[] asms = System.AppDomain.CurrentDomain.GetAssemblies().Where( x => x.FullName.StartsWith( "Assembly-CSharp" ) ).ToArray();
		m_Types = asms.SelectMany( x => x.GetTypes().Where( type => type.IsSubclassOf( typeof( EditorWindow ) ) && type.Name != "Toolbox" && type.Name.Contains(additionalEditorsPrefix)) ).OrderBy( x => x.Name ).ToArray();
		m_TypeNames = m_Types.Select( x => x.Name ).ToArray();
	}
	#endregion
	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	#region UI ELEMENTS - DRAW BUTTON
	static bool DrawButton (string title, string tooltip, bool enabled, float width) 
	{
		GUILayoutOption opt;
		opt = GUILayout.MinHeight(24f);
		
		if (enabled)
		{
			// Draw a regular button
			return GUILayout.Button(new GUIContent(title, tooltip), GUILayout.MinWidth(1f), GUILayout.MinHeight(8f));
		}
		else
		{
			// Button should be disabled -- draw it darkened and ignore its return value
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, 0.25f);
			GUILayout.Button(new GUIContent(title, tooltip), opt);
			GUI.color = color;
			return false;
		}
	}
	#endregion
	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	#region QUICK KEY FUNCTIONS 
	// HOT KEY FUNCTIONS
	//% == command 
	//# == shift
	//& == option
	//_ == no modifier 
	
	// options + shift + N
	//&#N

	#region ALIGN HOT KEY FUNCTIONS
	[MenuItem ("Toolbox/Alignment/Align #s")]
	static void Align()
	{
		selectedObjects = Selection.gameObjects;	
		if (selectedObjects != null && selectedObjects.Length > 0)
		{
			foreach (GameObject obj in Selection.gameObjects)
			{
				Undo.RecordObject(obj.transform, "Transform Change");
				
				// ALIGN POSITION
				float alignX = obj.transform.localPosition.x;
				float alignY = obj.transform.localPosition.y;
				float alignZ = obj.transform.localPosition.z;
				if (zAlignActive) alignX = Mathf.Round(obj.transform.localPosition.x / EditorPrefs.GetFloat("MoveSnapX")) * EditorPrefs.GetFloat("MoveSnapX");
				if (yAlignActive) alignY = Mathf.Round(obj.transform.localPosition.y / EditorPrefs.GetFloat("MoveSnapY")) * EditorPrefs.GetFloat("MoveSnapY");
				if (zAlignActive) alignZ = Mathf.Round(obj.transform.localPosition.z / EditorPrefs.GetFloat("MoveSnapZ")) * EditorPrefs.GetFloat("MoveSnapZ");
				
				obj.transform.localPosition = new Vector3(alignX, alignY, alignZ);
				
				// ALIGN ROTATION
				float alignRotX = obj.transform.localRotation.eulerAngles.x;
				float alignRotY = obj.transform.localRotation.eulerAngles.y;
				float alignRotZ = obj.transform.localRotation.eulerAngles.z;
				if (rotXAlignActive) alignRotX = Mathf.RoundToInt( obj.transform.localRotation.eulerAngles.x / EditorPrefs.GetFloat("RotationSnap")) * EditorPrefs.GetFloat("RotationSnap");
				if (rotYAlignActive) alignRotY = Mathf.RoundToInt( obj.transform.localRotation.eulerAngles.y / EditorPrefs.GetFloat("RotationSnap")) * EditorPrefs.GetFloat("RotationSnap");
				if (rotZAlignActive) alignRotZ = Mathf.RoundToInt( obj.transform.localRotation.eulerAngles.z / EditorPrefs.GetFloat("RotationSnap")) * EditorPrefs.GetFloat("RotationSnap");
				
				obj.transform.localRotation = Quaternion.Euler(alignRotX, alignRotY, alignRotZ);
			}
		}
	}
	
	[MenuItem ("Toolbox/Alignment/Align %#s")]
	static void AlignLocal()
	{
		selectedObjects = Selection.gameObjects;	
		if (selectedObjects != null && selectedObjects.Length > 0)
		{
			foreach (GameObject obj in Selection.gameObjects)
			{
				Undo.RecordObject(obj.transform, "Transform Change");
				obj.transform.localPosition = Vector3.zero;
				
				// ALIGN ROTATION
				if (obj.transform.parent)
					obj.transform.localRotation = Quaternion.Euler(obj.transform.parent.rotation.x, obj.transform.parent.rotation.y, obj.transform.parent.rotation.z);
			}
		}
	}
	#endregion


	#region GROUPING HOT KEY FUNCTIONS
	[MenuItem ("Toolbox/Grouping/Group Objects %g")]
	static void GroupObjects()
	{
		selectedObjects = Selection.gameObjects;
		Transform selParent = Selection.activeGameObject.transform.parent;
		GameObject myGameObject = new GameObject("___New Group");
		myGameObject.transform.position = selectedObjects[0].transform.position;
		myGameObject.transform.parent = selParent;
		Undo.RegisterCreatedObjectUndo(myGameObject, "Undo Create Group");
		//might need to add undo for parent 
		
		if (selectedObjects != null && selectedObjects.Length > 0)
		{
			foreach (GameObject obj in selectedObjects)
			{
				Undo.SetTransformParent(obj.transform, myGameObject.transform, "Parent Change");
				obj.transform.parent = myGameObject.transform;
			}
		}
		Selection.activeGameObject = myGameObject;
	}
	
	[MenuItem ("Toolbox/Grouping/Group Object to selected parent #g")]
	static void groupPlus()
	{
		selectedObjects = Selection.gameObjects;
		Transform newParent = Selection.activeGameObject.transform.parent;
		
		if (selectedObjects != null && selectedObjects.Length > 0)
		{
			foreach (GameObject obj in selectedObjects)
			{
				Undo.SetTransformParent(obj.transform, newParent, "Parent Change");
				obj.transform.parent = newParent;
			}
		}
		else 
			EditorApplication.Beep();
	}

	[MenuItem ("Toolbox/Grouping/Clear parents for selectied objects &g")]
	static void clearParent()
	{
		selectedObjects = Selection.gameObjects;
		if (selectedObjects != null && selectedObjects.Length > 0)
		{
			foreach (GameObject obj in selectedObjects)
			{
				Undo.SetTransformParent(obj.transform, obj.transform.parent, "Parent Change");
				obj.transform.parent = null;
			}
		}
		else 
			EditorApplication.Beep();
	}
	#endregion

	#region SELECTION HOT KEY FUNCTIONS
	//DEACTIVATE
	[MenuItem ("Toolbox/Selection/Toggle Active State &#A")]
	static void toggleActive()
	{
		selectedObjects = Selection.gameObjects;	
		if (selectedObjects != null && selectedObjects.Length > 0)
		{
			foreach (GameObject obj in Selection.gameObjects)
			{
				Undo.RecordObject(obj, "Set Active");
				obj.SetActive (!obj.activeSelf);
			}
		}
		else 
			EditorApplication.Beep();
	}

	// SAVE SELECTION ACTIVEATE 
	[MenuItem ("Toolbox/Selection/HideActive &s")]
	static void SaveActive()
	{
		selectedObjects = Selection.gameObjects;
		if (selectedObjects != null && selectedObjects.Length > 0)
		{
			// Store the selection if there is not already a stored selection
			if (storeSelection) saveObjs = Selection.gameObjects;
	
			storeSelection = !storeSelection;

			foreach (GameObject obj in saveObjs)
			{
				// this is dirty could lead to bac feedback because undo wont reset storeSelection
				Undo.RecordObject(obj, "Set Active");
				obj.SetActive(storeSelection);
			}
		}
		else 
			EditorApplication.Beep();
	}

	// SET STATIC
	[MenuItem ("Toolbox/Selection/SetStatic #&s")]
	static void SetStatic()
	{
		selectedObjects = Selection.gameObjects;
		if (selectedObjects != null && selectedObjects.Length > 0)
		{
			foreach (GameObject obj in Selection.gameObjects)
			{
				Undo.RecordObject(obj, "Set Static");
				obj.isStatic = !obj.isStatic;
			}
		}
		else 
			EditorApplication.Beep();
	}

	//SELECT PARENT
	[MenuItem ( "Toolbox/Selection/SelectParent &w")]
	static void SelectParent()
	{
		selectedObjects = Selection.gameObjects;
		if (selectedObjects != null && selectedObjects.Length == 1)
		{
			GameObject [] newSelection = new GameObject[1];

			if (Selection.activeTransform.parent == null)
			{
				EditorApplication.Beep();
			}
			else
			{
				newSelection[0] = Selection.activeTransform.parent.gameObject;
				Undo.RecordObject(newSelection[0], "Select Parent");
				Selection.objects = newSelection;
			}
		}
		else 
			EditorApplication.Beep();
	}

	//SELECT CHILDREN
	[MenuItem ( "Toolbox/Selection/SelectChildren #w")]
	static void SelectChildren()
	{
		selectedObjects = Selection.gameObjects;
		if (selectedObjects != null && selectedObjects.Length == 1)
		{
			Transform [] newTrans = new Transform[1000];
			newTrans = Selection.activeTransform.GetComponentsInChildren<Transform>();

			GameObject[] newObject = new GameObject[1000];

			for (int i = 1; i <= newTrans.Length -1; i ++)
			{
				if (newTrans[i] != null)
				{
					newObject[i] = newTrans[i].gameObject;
				}
			}

			Selection.objects = newObject;
		}
		else 
			EditorApplication.Beep();
			
	}
	#endregion

	#region CREATE HOT KEY FUNCTIONS
	[MenuItem ( "Toolbox/Create/New Child Object &#n" )]
	static void NewChildObject()
	{
		selectedObjects = Selection.gameObjects;	
		if (selectedObjects != null && selectedObjects.Length == 1)
		{
			GameObject myGameObject = new GameObject("___New Child");
			myGameObject.transform.parent = Selection.activeTransform;
			myGameObject.transform.localPosition = new Vector3( 0,0,0);
			myGameObject.transform.rotation = new Quaternion(0,0,0,0);
			myGameObject.transform.localScale = new Vector3(1,1,1);
			Undo.RegisterCreatedObjectUndo(myGameObject, "Undo Create Object");
			Selection.activeGameObject = myGameObject;
		}
		else 
			EditorApplication.Beep();
	}
	#endregion

	#region CLEAN UP HOT KEY FUNCTIONS
	[MenuItem ( "Toolbox/CleanUp/Find Empty Objects" )]
	static void findEmpty()
	{
		GameObject [] sceneObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

		GameObject [] emptyObjects = new GameObject[10000000];

		for (int i = 0; i <= sceneObjects.Length -1; i ++)
		{
			if (sceneObjects[i].activeInHierarchy)
			{
				// check how many companants are attached
				if (sceneObjects[i].transform.GetComponents<Component>().Length == 1)
				//if (sceneObjects[i].GetComponents<Component>().Length == 1)
				{
					// check if the object has and children objects attached
					if (sceneObjects[i].transform.childCount == 0)
					{
						emptyObjects[i] = sceneObjects[i];
					}
				}
			}
		}

		Selection.objects = emptyObjects;
	}

	[MenuItem ( "Toolbox/CleanUp/Find Objects in same location" )]
	static void findSameLocation()
	{
		GameObject [] sceneObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
		
		GameObject [] sameLocationObjects = new GameObject[10000000];
		
		for (int i = 0; i <= sceneObjects.Length -1; i ++)
		{
			// cycle one at a time to compare the pos to another one 
			for (int k = 0; k <= sceneObjects.Length-1; k ++)
			{
				// check pos and make sure its not the same obj
				if (sceneObjects[i].transform.position == sceneObjects[k].transform.position && sceneObjects[i] != sceneObjects[k])
				{
					// if we are ignoring objects at the origin
					if (ignoreOrigin)
					{
						if (sceneObjects[i].transform.position != new Vector3(0,0,0))
						{
							sameLocationObjects[i] = sceneObjects[i];
						}
					}
					else 
					{
						sameLocationObjects[i] = sceneObjects[i];
					}
				}
			}
		}
		
		Selection.objects = sameLocationObjects;
	}

	[MenuItem ( "Toolbox/CleanUp/Count Children" )]
	static void countChildren()
	{
		Debug.Log(Selection.activeTransform.childCount);
		// count children of children add them up 
	}

	#endregion


	#endregion
	// \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\







}
