using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class TestInput : MonoBehaviour
{
  void Start()
  {
  }

  void Update()
  {
    transform.Rotate( Vector3.down,  500.0f * Time.deltaTime * Controls.LeftStickX, Space.World );
    transform.Rotate( Vector3.right, 500.0f * Time.deltaTime * Controls.LeftStickY, Space.World );
  }
}
