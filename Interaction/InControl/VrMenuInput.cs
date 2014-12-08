using UnityEngine;
using System.Collections;

namespace InControl
{
    public class VrMenu : MonoBehaviour

    {
        private VRMenuManager menuManager;

        // Use this for initialization
        private void Start()
        {
            menuManager = GameObject.Find("VRMenu").GetComponent<VRMenuManager>();
        }

        // Update is called once per frame
        private void Update()
        {
            if(!menuManager)
                return;

            var device = InControl.InputManager.ActiveDevice;
            if (device.Action2.WasReleased)
            {
                menuManager.ToggleVisiblity();
            }
        }
    }
}