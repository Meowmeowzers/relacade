using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Based from Renaissance Coders tutorials from YT
namespace HelloWorld.Editor
{
    public class TileSetUpWindow : EditorWindow
    {
        //Left
        private TileInputSet selectedInputTileSet;
        private SerializedObject serializedTileSetObject;
        private SerializedProperty selectedDirectionInputTileList;
        private string selectedInputTileSetProperty = "";

        //Right
        private SerializedObject selectedTileConstraints;
        private SerializedProperty id;
        private SerializedProperty gameObject;
        private SerializedProperty compatibleTopList;
        private SerializedProperty compatibleBottomList;
        private SerializedProperty compatibleLeftList;
        private SerializedProperty compatibleRightList;

        private string assetName = "NewScriptableObject";
        private string savePath = "Assets/";

        private enum DirectionToSet
        { FourDirection, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight, IFourDirection, ITopLeft, ITopRight, IBottomLeft, IBottomRight };

        #region Window Variables

        private readonly int headerSectionHeight = 40;
        private readonly int tileSetupSectionWidth = 320;

        private Texture2D headerBackgroundTexture;
        private Texture2D leftBackgroundTexture;
        private Texture2D rightBackgroundTexture;

        private Color headerBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
        private Color leftBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
        private Color rightBackgroundColor = new(0.6f, .2f, 0.7f, 0.7f);

        private Rect headerSection;
        private Rect tileSetupSection;
        private Rect tileConstraintSetupSection;

        private static readonly int windowMinWidth = 600;
        private static readonly int windowMinHeight = 350;

        private Vector2 scrollPositionLeft = Vector2.zero;
        private Vector2 scrollPositionRight = Vector2.zero;

        private int newArraySize;
        private int currentArraySize;

        #endregion Window Variables

        [MenuItem("Relacade/Tile Set Configuration Setup")]
        private static void StartWindow()
        {
            TileSetUpWindow window = (TileSetUpWindow)GetWindow(typeof(TileSetUpWindow));
            window.minSize = new(windowMinWidth, windowMinHeight);
            window.Show();
        }

        [MenuItem("Relacade/Create/Tile Set Configuration")]
        private static void CreateTileSetConfiguration()
        {
            Debug.Log("TODO");
        }

        [MenuItem("Relacade/Create/Input Tile")]
        private static void CreateInputTile()
        {
            Debug.Log("TODO");
        }

        private void OnEnable()
        {
            InitTextures();
            InitData();
        }

        private void OnGUI()
        {
            DrawLayouts();
            DrawHeader();
            DrawLeft();
            DrawRight();
        }

        private void InitTextures()
        {
            headerBackgroundTexture = new Texture2D(1, 1);
            headerBackgroundTexture.SetPixel(0, 0, headerBackgroundColor);
            headerBackgroundTexture.Apply();

            leftBackgroundTexture = new Texture2D(1, 1);
            leftBackgroundTexture.SetPixel(0, 0, leftBackgroundColor);
            leftBackgroundTexture.Apply();

            rightBackgroundTexture = new Texture2D(1, 1);
            rightBackgroundTexture.SetPixel(0, 0, rightBackgroundColor);
            rightBackgroundTexture.Apply();
        }

        private void InitData()
        {
            if(selectedInputTileSet != null)
            {
                serializedTileSetObject = new(selectedInputTileSet);
            }
        }

        private void DrawLayouts()
        {
            headerSection.x = 0;
            headerSection.y = 0;
            headerSection.width = Screen.width;
            headerSection.height = headerSectionHeight;

            tileSetupSection.x = 0;
            tileSetupSection.y = headerSectionHeight;
            tileSetupSection.width = tileSetupSectionWidth;
            tileSetupSection.height = Screen.height - headerSectionHeight;

            tileConstraintSetupSection.x = tileSetupSectionWidth;
            tileConstraintSetupSection.y = headerSectionHeight;
            tileConstraintSetupSection.width = Screen.width - tileSetupSectionWidth;
            tileConstraintSetupSection.height = Screen.height - headerSectionHeight;

            GUI.DrawTexture(headerSection, headerBackgroundTexture);
            GUI.DrawTexture(tileSetupSection, leftBackgroundTexture);
            //GUI.DrawTexture(tileConstraintSetupSection, rightBackgroundTexture);
        }

        private void DrawHeader()
        {
            GUILayout.BeginArea(headerSection);
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Load Tile Set Configuration", GUILayout.Height(30), GUILayout.Width(170));
            selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.Height(30));

            if (GUILayout.Button("Auto Generate", GUILayout.Height(30)))
            {
                if (selectedInputTileSet != null && serializedTileSetObject != null)
                {
                    AutoGenerateDirect();
                }
            }

            if (GUILayout.Button("Clear", GUILayout.Height(30)))
            {
                if (selectedInputTileSet != null && serializedTileSetObject != null)
                {
                    ClearAllInputTileConstraints();
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void AutoGenerateDirect()
        {
            for (int i = 0; i < selectedInputTileSet.FourDirectionTiles.Count; i++)
            {
                selectedInputTileSet.FourDirectionTiles[i] = MainCombine(selectedInputTileSet.FourDirectionTiles[i], selectedInputTileSet, DirectionToSet.FourDirection);
            }
            for (int i = 0; i < selectedInputTileSet.TopTiles.Count; i++)
            {
                selectedInputTileSet.TopTiles[i] = MainCombine(selectedInputTileSet.TopTiles[i], selectedInputTileSet, DirectionToSet.Top);
            }
            for (int i = 0; i < selectedInputTileSet.BottomTiles.Count; i++)
            {
                selectedInputTileSet.BottomTiles[i] = MainCombine(selectedInputTileSet.BottomTiles[i], selectedInputTileSet, DirectionToSet.Bottom);
            }
            for (int i = 0; i < selectedInputTileSet.LeftTiles.Count; i++)
            {
                selectedInputTileSet.LeftTiles[i] = MainCombine(selectedInputTileSet.LeftTiles[i], selectedInputTileSet, DirectionToSet.Left);
            }
            for (int i = 0; i < selectedInputTileSet.RightTiles.Count; i++)
            {
                selectedInputTileSet.RightTiles[i] = MainCombine(selectedInputTileSet.RightTiles[i], selectedInputTileSet, DirectionToSet.Right);
            }
            for (int i = 0; i < selectedInputTileSet.TopLeftTiles.Count; i++)
            {
                selectedInputTileSet.TopLeftTiles[i] = MainCombine(selectedInputTileSet.TopLeftTiles[i], selectedInputTileSet, DirectionToSet.TopLeft);
            }
            for (int i = 0; i < selectedInputTileSet.TopRightTiles.Count; i++)
            {
                selectedInputTileSet.TopRightTiles[i] = MainCombine(selectedInputTileSet.TopRightTiles[i], selectedInputTileSet, DirectionToSet.TopRight);
            }
            for (int i = 0; i < selectedInputTileSet.BottomLeftTiles.Count; i++)
            {
                selectedInputTileSet.BottomLeftTiles[i] = MainCombine(selectedInputTileSet.BottomLeftTiles[i], selectedInputTileSet, DirectionToSet.BottomLeft);
            }
            for (int i = 0; i < selectedInputTileSet.BottomRightTiles.Count; i++)
            {
                selectedInputTileSet.BottomRightTiles[i] = MainCombine(selectedInputTileSet.BottomRightTiles[i], selectedInputTileSet, DirectionToSet.BottomRight);
            }

            for (int i = 0; i < selectedInputTileSet.IFourDirectionTiles.Count; i++)
            {
                selectedInputTileSet.IFourDirectionTiles[i] = MainCombine(selectedInputTileSet.IFourDirectionTiles[i], selectedInputTileSet, DirectionToSet.IFourDirection);
            }
            for (int i = 0; i < selectedInputTileSet.ITopRightTiles.Count; i++)
            {
                selectedInputTileSet.ITopRightTiles[i] = MainCombine(selectedInputTileSet.ITopRightTiles[i], selectedInputTileSet, DirectionToSet.ITopRight);
            }
            for (int i = 0; i < selectedInputTileSet.ITopLeftTiles.Count; i++)
            {
                selectedInputTileSet.ITopLeftTiles[i] = MainCombine(selectedInputTileSet.ITopLeftTiles[i], selectedInputTileSet, DirectionToSet.ITopLeft);
            }
            for (int i = 0; i < selectedInputTileSet.IBottomLeftTiles.Count; i++)
            {
                selectedInputTileSet.IBottomLeftTiles[i] = MainCombine(selectedInputTileSet.IBottomLeftTiles[i], selectedInputTileSet, DirectionToSet.IBottomLeft);
            }
            for (int i = 0; i < selectedInputTileSet.IBottomRightTiles.Count; i++)
            {
                selectedInputTileSet.IBottomRightTiles[i] = MainCombine(selectedInputTileSet.IBottomRightTiles[i], selectedInputTileSet, DirectionToSet.IBottomRight);
            }
            GetAllUniqueInputTiles();
            GiveUniqueIDToTiles();
        }

        private TileInput MainCombine(TileInput tileInput, TileInputSet set, DirectionToSet direction)
        {
            switch (direction)
            {
                case DirectionToSet.FourDirection:

                    #region FourDirection

                    if (!tileInput.compatibleTop.Contains(tileInput))
                    {
                        tileInput.compatibleTop.Add(tileInput);
                    }
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopRightTiles);
                    if (!tileInput.compatibleBottom.Contains(tileInput))
                    {
                        tileInput.compatibleBottom.Add(tileInput);
                    }
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomRightTiles);
                    if (!tileInput.compatibleLeft.Contains(tileInput))
                    {
                        tileInput.compatibleLeft.Add(tileInput);
                    }
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.LeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomLeftTiles);
                    if (!tileInput.compatibleRight.Contains(tileInput))
                    {
                        tileInput.compatibleRight.Add(tileInput);
                    }
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.RightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomRightTiles);
                    break;

                #endregion FourDirection

                case DirectionToSet.Top:

                    #region Top

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.ITopLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.ITopRightTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IFourDirectionTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.FourDirectionTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomRightTiles);
                    if (!tileInput.compatibleLeft.Contains(tileInput))
                    {
                        tileInput.compatibleLeft.Add(tileInput);
                    }
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IBottomRightTiles);
                    if (!tileInput.compatibleRight.Contains(tileInput))
                    {
                        tileInput.compatibleRight.Add(tileInput);
                    }
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IBottomLeftTiles);
                    break;

                #endregion Top

                case DirectionToSet.Bottom:

                    #region Bottom

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.FourDirectionTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IBottomLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IBottomRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IFourDirectionTiles);
                    if (!tileInput.compatibleLeft.Contains(tileInput))
                    {
                        tileInput.compatibleLeft.Add(tileInput);
                    }
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.ITopRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomRightTiles);
                    if (!tileInput.compatibleRight.Contains(tileInput))
                    {
                        tileInput.compatibleRight.Add(tileInput);
                    }
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.ITopLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomLeftTiles);

                    #endregion Bottom

                    break;

                case DirectionToSet.Left:

                    #region Left

                    if (!tileInput.compatibleTop.Contains(tileInput))
                    {
                        tileInput.compatibleTop.Add(tileInput);
                    }
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IBottomRightTiles);
                    if (!tileInput.compatibleBottom.Contains(tileInput))
                    {
                        tileInput.compatibleBottom.Add(tileInput);
                    }
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.ITopRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.RightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.ITopLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IBottomLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IFourDirectionTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.FourDirectionTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.RightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopRightTiles);

                    #endregion Left

                    break;

                case DirectionToSet.Right:

                    #region Right

                    if (!tileInput.compatibleTop.Contains(tileInput))
                    {
                        tileInput.compatibleTop.Add(tileInput);
                    }
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomRightTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IBottomLeftTiles);
                    if (!tileInput.compatibleBottom.Contains(tileInput))
                    {
                        tileInput.compatibleBottom.Add(tileInput);
                    }
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.ITopLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.FourDirectionTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.LeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.LeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IBottomRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.ITopRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IFourDirectionTiles);

                    #endregion Right

                    break;

                case DirectionToSet.TopLeft:

                    #region TopLeft

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.LeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IBottomRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.FourDirectionTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IBottomRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.FourDirectionTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.RightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopRightTiles);

                    #endregion TopLeft

                    break;

                case DirectionToSet.TopRight:

                    #region TopRight

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.RightTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomRightTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IBottomLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.FourDirectionTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.BottomLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.FourDirectionTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.LeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IBottomLeftTiles);

                    #endregion TopRight

                    break;

                case DirectionToSet.BottomLeft:

                    #region BottomLeft

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.FourDirectionTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.LeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.ITopRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.ITopRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.FourDirectionTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.RightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopRightTiles);

                    #endregion BottomLeft

                    break;

                case DirectionToSet.BottomRight:

                    #region BottomRight

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.FourDirectionTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.TopRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.RightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.ITopLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.FourDirectionTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.LeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.ITopLeftTiles);

                    #endregion BottomRight

                    break;

                case DirectionToSet.IFourDirection:

                    #region IFourDirection

                    if (!tileInput.compatibleTop.Contains(tileInput))
                    {
                        tileInput.compatibleTop.Add(tileInput);
                    }
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.ITopLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.ITopRightTiles);
                    if (!tileInput.compatibleBottom.Contains(tileInput))
                    {
                        tileInput.compatibleBottom.Add(tileInput);
                    }
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IBottomLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IBottomRightTiles);
                    if (!tileInput.compatibleLeft.Contains(tileInput))
                    {
                        tileInput.compatibleLeft.Add(tileInput);
                    }
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.RightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.ITopLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IBottomLeftTiles);
                    if (!tileInput.compatibleRight.Contains(tileInput))
                    {
                        tileInput.compatibleRight.Add(tileInput);
                    }
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.LeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.ITopRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IBottomRightTiles);

                    #endregion IFourDirection

                    break;

                case DirectionToSet.ITopLeft:

                    #region ITopLeft

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.RightTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomRightTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IBottomLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IBottomLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IBottomRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IFourDirectionTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.BottomRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.ITopRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.LeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IBottomRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.ITopRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IFourDirectionTiles);

                    #endregion ITopLeft

                    break;

                case DirectionToSet.ITopRight:

                    #region ITopRight

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.LeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IBottomRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IBottomLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IBottomRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.IFourDirectionTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.RightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IBottomLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.ITopLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IFourDirectionTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.BottomLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.ITopLeftTiles);

                    #endregion ITopRight

                    break;

                case DirectionToSet.IBottomLeft:

                    #region IBottomLeft

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.ITopLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.ITopRightTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IFourDirectionTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.RightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopRightTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.ITopLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.TopRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IBottomRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.LeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IBottomRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.ITopRightTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IFourDirectionTiles);

                    #endregion IBottomLeft

                    break;

                case DirectionToSet.IBottomRight:

                    #region IBottomRight

                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.BottomTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.ITopLeftTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.ITopRightTiles);
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.IFourDirectionTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.LeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.TopLeftTiles);
                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.ITopRightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.RightTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IBottomLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.ITopLeftTiles);
                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.IFourDirectionTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.TopLeftTiles);
                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.IBottomLeftTiles);

                    #endregion IBottomRight

                    break;

                default:
                    break;
            }
            return tileInput;
        }

        private List<TileInput> SubCombine(List<TileInput> tileConstraintsList, List<TileInput> tilesToAdd)
        {
            foreach (var item in tilesToAdd)
            {
                if (!tileConstraintsList.Contains(item))
                {
                    tileConstraintsList.Add(item);
                }
            }
            return tileConstraintsList;
        }

        private void AutoGenerateSerialized()
        {
            //TODO: Use serialized objects and properties to enable undo functionality in editor
            /*
            string[] directions = { "TopLeftTiles", "TopTiles", "TopRightTiles", "LeftTiles", "FourDirectionTiles",
            "RightTiles", "BottomLeftTiles", "BottomTiles", "BottomRightTiles"};
            */

            //TileInputSet temp = selectedInputTileSet;

            /*
            SerializedProperty fourDirection = serializedTileSetObject.FindProperty("FourDirectionTiles");
            SerializedProperty topDirection = serializedTileSetObject.FindProperty("TopTiles");
            SerializedProperty bottomDirection = serializedTileSetObject.FindProperty("BottomTiles");
            SerializedProperty leftDirection = serializedTileSetObject.FindProperty("LeftTiles");
            SerializedProperty rightDirection = serializedTileSetObject.FindProperty("RightTiles");

            for (int i = 0; i < fourDirection.arraySize; i++)
            {
                SerializedProperty top = fourDirection.GetArrayElementAtIndex(i).FindPropertyRelative("compatibleTop");
                SerializedProperty bottom = fourDirection.GetArrayElementAtIndex(i).FindPropertyRelative("compatibleBottom");
                SerializedProperty left = fourDirection.GetArrayElementAtIndex(i).FindPropertyRelative("compatibleLeft");
                SerializedProperty right = fourDirection.GetArrayElementAtIndex(i).FindPropertyRelative("compatibleRight");

                for (int j = 0; j < top.arraySize; j++)
                {
                    top.GetArrayElementAtIndex(j).objectReferenceValue = topDirection.GetArrayElementAtIndex(j).objectReferenceValue;
                }
            }
            */
            /*
            List<TileInput> temp = selectedInputTileSet.TopLeftTiles;
            for(int i = 0; i < temp.Count; i++)
            {
                fourDirection.GetArrayElementAtIndex(i).objectReferenceValue = temp[i];
            }

            /*
            for (int i = 0; i < topDirection.arraySize; i++)
            {
                SerializedProperty listCombine;
                listCombine.InsertArrayElementAtIndex(i);
                listCombine.GetArrayElementAtIndex(i).objectReferenceValue = listCombine.GetArrayElementAtIndex(i).objectReferenceValue;
            }
            */
        }

        private void DrawLeft()
        {
            GUILayout.BeginArea(tileSetupSection);
            scrollPositionLeft = EditorGUILayout.BeginScrollView(scrollPositionLeft, false, false);

            #region Buttons

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DirectionButton("TopLeft", "TopLeftTiles");
            DirectionButton("Top", "TopTiles");
            DirectionButton("TopRight", "TopRightTiles");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DirectionButton("Left", "LeftTiles");
            DirectionButton("4 Direction", "FourDirectionTiles");
            DirectionButton("Right", "RightTiles");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DirectionButton("BottomLeft", "BottomLeftTiles");
            DirectionButton("Bottom", "BottomTiles");
            DirectionButton("BottomRight", "BottomRightTiles");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DirectionButton("ITopLeft", "ITopLeftTiles");
            DirectionButton("I4 Direction", "IFourDirectionTiles");
            DirectionButton("ITopRight", "ITopRightTiles");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DirectionButton("IBottomLeft", "IBottomLeftTiles");
            DirectionButton("IBottomRight", "IBottomRightTiles");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            #endregion Buttons

            EditorGUILayout.Space(10);

            if (selectedInputTileSet != null && selectedInputTileSetProperty != "")
            {
                ShowTileList();
                serializedTileSetObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(10);
            GUILayout.EndArea();
        }

        private void DirectionButton(string buttonName, string property)
        {
            if (GUILayout.Button(buttonName, GUILayout.Height(30), GUILayout.Width(90)))
            {
                SetCurrentTileToConfig(property);
            }
        }

        private void ShowTileList()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(selectedInputTileSetProperty);
            GUILayout.FlexibleSpace();

            // Array size field
            currentArraySize = selectedDirectionInputTileList.arraySize;
            newArraySize = EditorGUILayout.IntField(currentArraySize, GUILayout.Width(50));
            if (newArraySize != currentArraySize)
            {
                selectedDirectionInputTileList.arraySize = newArraySize;
            }

            // Plus button
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                selectedDirectionInputTileList.arraySize++;
            }

            // Minus button
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                if (selectedDirectionInputTileList.arraySize > 0)
                {
                    selectedDirectionInputTileList.arraySize--;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            for (int i = 0; i < selectedDirectionInputTileList.arraySize; i++)
            {
                SerializedProperty elementProperty = selectedDirectionInputTileList.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Item {i + 1}", GUILayout.Width(60));
                EditorGUILayout.PropertyField(elementProperty, GUIContent.none);

                if (GUILayout.Button(">", GUILayout.Width(30)))
                {
                    if (elementProperty.objectReferenceValue != null)
                    {
                        Object elementReference = elementProperty.objectReferenceValue;
                        selectedTileConstraints = new(elementReference);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(10);
        }

        private void SetCurrentTileToConfig(string property)
        {
            if (selectedInputTileSet != null)
            {
                serializedTileSetObject = new(selectedInputTileSet);
                selectedInputTileSetProperty = property;
                serializedTileSetObject.Update();
                selectedDirectionInputTileList = serializedTileSetObject.FindProperty(property);
                selectedDirectionInputTileList.isExpanded = true;
            }
        }

        private void DrawRight()
        {
            GUILayout.BeginArea(tileConstraintSetupSection);
            scrollPositionRight = EditorGUILayout.BeginScrollView(scrollPositionRight);

            if (selectedInputTileSet == null)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(100));
                EditorGUILayout.LabelField("Load a tile set configuration");
                selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.Height(30));
                GUILayout.Space(10);
                EditorGUILayout.LabelField("or");
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Create a tile set configuration");
                //EditorGUILayout.LabelField("Create Tile Set Data", GUILayout.Width(60));
                assetName = EditorGUILayout.TextField(assetName, GUILayout.Width(250));

                if (GUILayout.Button("Create Tile Set Configuration", GUILayout.Height(30)))
                {
                    ScriptableObject scriptableObject = ScriptableObject.CreateInstance<TileInputSet>();
                    savePath = EditorUtility.SaveFilePanelInProject("Save Scriptable Object", assetName, "asset", "Choose a location to save the ScriptableObject.");
                    if (string.IsNullOrEmpty(savePath)) return;
                    AssetDatabase.CreateAsset(scriptableObject, savePath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    selectedInputTileSet = scriptableObject as TileInputSet;
                    GUILayout.EndArea();
                }

                EditorGUILayout.EndVertical();
            }
            else if (selectedDirectionInputTileList == null)
            {
                EditorGUILayout.LabelField("Select an input tile direction on the right panel");
            }
            else if (selectedDirectionInputTileList.arraySize == 0)
            {
                EditorGUILayout.LabelField("Press the plus button on the right panel to add an input tile on selected tile direction");
            }
            else if (selectedTileConstraints == null)
            {
                EditorGUILayout.LabelField("- Select an item from the tile list by pressing the > button");
                EditorGUILayout.LabelField("- Make sure to fill the item with an input tile then press the > button");
                EditorGUILayout.LabelField("- Dont have input tiles? you should plan, create and setup input tiles first");
            }
            else
            {
                if (selectedTileConstraints != null)
                {
                    selectedTileConstraints.Update();

                    EditorGUILayout.Space(5);

                    id = selectedTileConstraints.FindProperty("id");
                    gameObject = selectedTileConstraints.FindProperty("gameObject");
                    compatibleTopList = selectedTileConstraints.FindProperty("compatibleTop");
                    compatibleBottomList = selectedTileConstraints.FindProperty("compatibleBottom");
                    compatibleLeftList = selectedTileConstraints.FindProperty("compatibleLeft");
                    compatibleRightList = selectedTileConstraints.FindProperty("compatibleRight");

                    EditorGUILayout.PropertyField(id);
                    EditorGUILayout.PropertyField(gameObject);
                    EditorGUILayout.PropertyField(compatibleTopList, true);
                    EditorGUILayout.PropertyField(compatibleBottomList, true);
                    EditorGUILayout.PropertyField(compatibleLeftList, true);
                    EditorGUILayout.PropertyField(compatibleRightList, true);

                    selectedTileConstraints.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(20);
            GUILayout.EndArea();
        }

        private void GetAllUniqueInputTiles()
        {
            GetUniqueTiles(selectedInputTileSet.FourDirectionTiles);
            GetUniqueTiles(selectedInputTileSet.TopTiles);
            GetUniqueTiles(selectedInputTileSet.BottomTiles);
            GetUniqueTiles(selectedInputTileSet.LeftTiles);
            GetUniqueTiles(selectedInputTileSet.RightTiles);
            GetUniqueTiles(selectedInputTileSet.TopLeftTiles);
            GetUniqueTiles(selectedInputTileSet.TopRightTiles);
            GetUniqueTiles(selectedInputTileSet.BottomLeftTiles);
            GetUniqueTiles(selectedInputTileSet.BottomRightTiles);
            GetUniqueTiles(selectedInputTileSet.IFourDirectionTiles);
            GetUniqueTiles(selectedInputTileSet.ITopLeftTiles);
            GetUniqueTiles(selectedInputTileSet.ITopRightTiles);
            GetUniqueTiles(selectedInputTileSet.IBottomLeftTiles);
            GetUniqueTiles(selectedInputTileSet.IBottomRightTiles);
        }

        private void GetUniqueTiles(List<TileInput> list)
        {
            foreach (TileInput item in list)
            {
                if (!selectedInputTileSet.AllInputTiles.Contains(item))
                {
                    selectedInputTileSet.AllInputTiles.Add(item);
                }
            }
        }

        private void GiveUniqueIDToTiles()
        {
            for(int i = 0; i < selectedInputTileSet.AllInputTiles.Count; i++)
            {
                selectedInputTileSet.AllInputTiles[i].id = i + 1;
            }
        }

        private void ClearAllInputTiles()
        {
            ClearAllInputTilesInDirection(selectedInputTileSet.AllInputTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.FourDirectionTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.TopTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.BottomTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.LeftTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.RightTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.TopLeftTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.TopRightTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.BottomLeftTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.BottomRightTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.IFourDirectionTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.ITopLeftTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.ITopRightTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.IBottomLeftTiles);
            ClearAllInputTilesInDirection(selectedInputTileSet.IBottomRightTiles);
        }

        private void ClearAllInputTilesInDirection(List<TileInput> tilesInDirectionList)
        {
            tilesInDirectionList.Clear();
        }

        private void ClearAllInputTileConstraints()
        {
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.AllInputTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.FourDirectionTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.TopTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.BottomTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.LeftTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.RightTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.TopLeftTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.TopRightTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.BottomLeftTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.BottomRightTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.IFourDirectionTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ITopLeftTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ITopRightTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.IBottomLeftTiles);
            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.IBottomRightTiles);
        }

        private void ClearAllInputTileConstraintsInDirection(List<TileInput> tileInDirectionList)
        {
            foreach(var item in tileInDirectionList)
            {
                item.compatibleTop.Clear();
                item.compatibleBottom.Clear();
                item.compatibleLeft.Clear();
                item.compatibleRight.Clear();
            }
        }
    }
}