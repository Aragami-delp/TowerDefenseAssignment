using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helper
{
    public static class InputHelper
    {
        public static Vector3 GetMouseWorldPosition2D()
        {
            return GetMouseWorldPosition2D(Camera.main);
        }

        public static Vector3 GetMouseWorldPosition2D(Camera _cam)
        {
            Vector3 worldPosition = _cam.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            return worldPosition;
        }

        public static Vector3 GetMouseWorldPosition3D()
        {
            return GetMouseWorldPosition3D(Camera.main);
        }

        public static Vector3 GetMouseWorldPosition3D(Camera _cam)
        {
            return GetMouseWorldPosition3D(~0, _cam);
        }

        public static Vector3 GetMouseWorldPosition3D(LayerMask _layerMask)
        {
            return GetMouseWorldPosition3D(_layerMask, Camera.main);
        }

        public static Vector3 GetMouseWorldPosition3D(LayerMask _layerMask, Camera _cam)
        {
            Ray camRay = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(camRay, out RaycastHit hit, float.MaxValue, _layerMask))
            {
                return hit.point;
            }
            return Vector3.zero;
        }
    }
}
