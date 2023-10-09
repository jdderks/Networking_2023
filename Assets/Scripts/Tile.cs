using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Tile : MonoBehaviour
{
    [SerializeField, ReadOnly]private int xPosition;
    [SerializeField, ReadOnly]private int zPosition;

    [SerializeField]private GameObject cubeObject;

    public int XPosition { get => xPosition; set => xPosition = value; }
    public int ZPosition { get => zPosition; set => zPosition = value; }
    public GameObject CubeObject { get => cubeObject; set => cubeObject = value; }
}
