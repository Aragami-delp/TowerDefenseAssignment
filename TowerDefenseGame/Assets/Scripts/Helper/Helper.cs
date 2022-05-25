using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helper
{
    public static class InputHelper
    {
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return worldPosition;
        }
    }
}
