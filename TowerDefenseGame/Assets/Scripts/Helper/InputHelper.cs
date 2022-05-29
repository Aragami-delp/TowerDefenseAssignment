using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helper
{
    public static class InputHelper
    {
        /// <summary>
        /// Return the mouse position in 2D
        /// </summary>
        /// <returns>2D mouse position</returns>
        public static Vector2 GetMouseWorldPosition2D()
        {
            return GetMouseWorldPosition2D(Camera.main);
        }

        /// <summary>
        /// Return the mouse position in 2D
        /// </summary>
        /// /// <param name="_cam">Camera for mouse</param>
        /// <returns>2D mouse position</returns>
        public static Vector2 GetMouseWorldPosition2D(Camera _cam)
        {
            Vector3 worldPosition = _cam.ScreenToWorldPoint(Input.mousePosition);
            //worldPosition.z = 0f;
            return worldPosition;
        }

        /// <summary>
        /// Gets the mouse position in world space in 3D
        /// </summary>
        /// <returns>Mouse position in world space</returns>
        public static Vector3? GetMouseWorldPosition3D()
        {
            return GetMouseWorldPosition3D(Camera.main);
        }

        /// <summary>
        /// Gets the mouse position in world space in 3D
        /// </summary>
        /// <param name="_cam">Camera for mouse</param>
        /// <returns>Mouse position in world space</returns>
        public static Vector3? GetMouseWorldPosition3D(Camera _cam)
        {
            return GetMouseWorldPosition3D(~0, _cam);
        }

        /// <summary>
        /// Gets the mouse position in world space in 3D
        /// </summary>
        /// <param name="_layerMask">Layers to check for</param>
        /// <returns>Mouse position in world space</returns>
        public static Vector3? GetMouseWorldPosition3D(LayerMask _layerMask)
        {
            return GetMouseWorldPosition3D(_layerMask, Camera.main);
        }

        /// <summary>
        /// Gets the mouse position in world space in 3D
        /// </summary>
        /// <param name="_layerMask">Layers to check for</param>
        /// <param name="_cam">Camera for mouse</param>
        /// <returns>Mouse position in world space</returns>
        public static Vector3? GetMouseWorldPosition3D(LayerMask _layerMask, Camera _cam)
        {
            Ray camRay = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(camRay, out RaycastHit hit, float.MaxValue, _layerMask))
            {
                return hit.point;
            }
            return null;
        }
    }
}
