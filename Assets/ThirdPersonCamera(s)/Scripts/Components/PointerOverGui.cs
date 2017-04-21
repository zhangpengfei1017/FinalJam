using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// Used for determining if Unity's mouse is over a provided Rect, or over a GUI element.
    /// </summary>
    [Serializable]
    public class PointerOverGui
    {
        /// <summary>
        /// Input will not work while the mouse is over a gui.
        /// Uses "EventSystem.current.IsPointerOverGameObject()"
        /// </summary>
        [Tooltip("Input will not work while the mouse is over a gui. Uses \"EventSystem.current.IsPointerOverGameObject()\"")]
        public bool EventSystemWhenPointerOverGuiElement = true;

        /// <summary>
        /// When the pointer is within any of these rects, it will not provide input.
        /// </summary>
        [Tooltip("When the pointer is within any of these rects, it will not provide input.")]
        public List<Rect> WhenPointerOverRects = new List<Rect>();

        /// <summary>
        /// Returns whether or not the pointer is over a Gui element or within a rect provided.
        /// </summary>
        /// <returns>Whether the pointer is on a GUI element.</returns>
        public bool IsPointerOverGui()
        {
            if (EventSystemWhenPointerOverGuiElement && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
            
            foreach (var rect in WhenPointerOverRects)
            {
                if (rect.Contains(Input.mousePosition))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
