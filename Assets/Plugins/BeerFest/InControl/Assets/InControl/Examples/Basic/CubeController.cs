using UnityEngine;

namespace BasicExample
{
  public class CubeController : MonoBehaviour
  {
  void Update()
  {
  var controls = BeerFest.ActiveDevice;

  // Rotate target object with left stick.
  transform.Rotate( Vector3.down,  500.0f * Time.deltaTime * controls.LeftStickX, Space.World );
  transform.Rotate( Vector3.right, 500.0f * Time.deltaTime * controls.LeftStickY, Space.World );
  }
  }
}

