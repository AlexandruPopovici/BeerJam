﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using ProceduralToolkit.Examples;
using ProceduralToolkit;

public class MazeGeneratorHelper : MonoBehaviour
{
    private const float backgroundSaturation = 0.25f;
    private const float backgroundValue = 0.7f;
    private const float fadeDuration = 0.5f;

    private const float gradientSaturation = 0.7f;
    private const float gradientSaturationOffset = 0.1f;
    private const float gradientValue = 0.7f;
    private const float gradientValueOffset = 0.1f;

    public Texture2D texture;
    private int textureWidth = 64;
    private int textureHeight = 64;
    private bool useGradient = false;
    private MazeGenerator mazeGenerator;
    private MazeGenerator.Algorithm generatorAlgorithm = MazeGenerator.Algorithm.RandomTraversal;
    private int cellSize = 1;
    private int wallSize = 3;
    private float hue;
    private float gradientLength = 30;

    private MazeGenerator.Algorithm[] algorithms = new[]
    {
        MazeGenerator.Algorithm.None,
        MazeGenerator.Algorithm.RandomTraversal,
        MazeGenerator.Algorithm.RandomDepthFirstTraversal,
        MazeGenerator.Algorithm.RandomBreadthFirstTraversal,
    };

    private Dictionary<MazeGenerator.Algorithm, string> algorithmToString =
        new Dictionary<MazeGenerator.Algorithm, string>
        {
            {MazeGenerator.Algorithm.None, "None"},
            {MazeGenerator.Algorithm.RandomTraversal, "Random traversal"},
            {MazeGenerator.Algorithm.RandomDepthFirstTraversal, "Random depth-first traversal"},
            {MazeGenerator.Algorithm.RandomBreadthFirstTraversal, "Random breadth-first traversal"}
        };

    public System.Action onMazeGenerated;

    private void Awake()
    {
        // var header = InstantiateControl<TextControl>(algorithmsGroup.transform.parent);
        // header.Initialize("Generator algorithm");
        // header.transform.SetAsFirstSibling();
        // for (int i = 0; i < algorithms.Length; i++)
        // {
        //     MazeGenerator.Algorithm algorithm = algorithms[i];
        //     var toggle = InstantiateControl<ToggleControl>(algorithmsGroup.transform);
        //     toggle.Initialize(
        //         header: algorithmToString[algorithm],
        //         value: algorithm == generatorAlgorithm,
        //         onValueChanged: isOn =>
        //         {
        //             if (isOn)
        //             {
        //                 generatorAlgorithm = algorithm;
        //                 Generate();
        //             }
        //         },
        //         toggleGroup: algorithmsGroup);
        // }

        // InstantiateControl<SliderControl>(leftPanel).Initialize("Cell size", 1, 10, cellSize, value =>
        // {
        //     cellSize = value;
        //     Generate();
        // });

        // InstantiateControl<SliderControl>(leftPanel).Initialize("Wall size", 1, 10, wallSize, value =>
        // {
        //     wallSize = value;
        //     Generate();
        // });

        // InstantiateControl<ToggleControl>(leftPanel).Initialize("Use gradient", useGradient, value =>
        // {
        //     useGradient = value;
        //     Generate();
        // });

        // InstantiateControl<ButtonControl>(leftPanel).Initialize("Generate new maze", Generate);

        Generate();
    }

    public void Generate()
    {
        StopAllCoroutines();

        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false, true)
        {
            filterMode = FilterMode.Point
        };
        texture.Clear(Color.black);
        texture.Apply();

        mazeGenerator = new MazeGenerator(textureWidth, textureHeight, cellSize, wallSize);

        StartCoroutine(GenerateCoroutine());
    }

    private IEnumerator GenerateCoroutine()
    {
        var algorithm = generatorAlgorithm;
        if (algorithm == MazeGenerator.Algorithm.None)
        {
            algorithm = RandomE.GetRandom(MazeGenerator.Algorithm.RandomTraversal,
                MazeGenerator.Algorithm.RandomDepthFirstTraversal,
                MazeGenerator.Algorithm.RandomBreadthFirstTraversal);
        }

        hue = Random.value;
        // var backgroundColor = new ColorHSV(hue, backgroundSaturation, backgroundValue).complementary.ToColor();
        // background.CrossFadeColor(backgroundColor, fadeDuration, true, false);

        switch (algorithm)
        {
            case MazeGenerator.Algorithm.RandomTraversal:
                yield return StartCoroutine(mazeGenerator.RandomTraversal(DrawEdge, texture.Apply));
                break;
            case MazeGenerator.Algorithm.RandomDepthFirstTraversal:
                yield return StartCoroutine(mazeGenerator.RandomDepthFirstTraversal(DrawEdge, texture.Apply));
                break;
            case MazeGenerator.Algorithm.RandomBreadthFirstTraversal:
                yield return StartCoroutine(mazeGenerator.RandomBreadthFirstTraversal(DrawEdge, texture.Apply));
                break;
        }
        texture.Apply();
        if(onMazeGenerated != null)
            onMazeGenerated();
        //byte[] bytes = texture.EncodeToPNG();
        // For testing purposes, also write to a file in the project folder
        //File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
    }

    private void DrawEdge(Edge edge)
    {
        int x, y, width, height;
        if (edge.origin.direction == Directions.Left || edge.origin.direction == Directions.Down)
        {
            x = Translate(edge.exit.x);
            y = Translate(edge.exit.y);
        }
        else
        {
            x = Translate(edge.origin.x);
            y = Translate(edge.origin.y);
        }

        if (edge.origin.direction == Directions.Left || edge.origin.direction == Directions.Right)
        {
            width = cellSize*2 + wallSize;
            height = cellSize;
        }
        else
        {
            width = cellSize;
            height = cellSize*2 + wallSize;
        }

        Color color;
        if (useGradient)
        {
            float gradient01 = Mathf.Repeat(edge.origin.depth/gradientLength, 1);
            float gradient010 = Mathf.Abs((gradient01 - 0.5f)*2);

            float saturation = gradient010*gradientSaturation + gradientSaturationOffset;
            float value = gradient010*gradientValue + gradientValueOffset;
            color = new ColorHSV(hue, saturation, value).ToColor();
        }
        else
        {
            color = Color.white;
        }
        texture.DrawRect(x, y, width, height, color);
    }

    private int Translate(int x)
    {
        return wallSize + x*(cellSize + wallSize);
    }
}
