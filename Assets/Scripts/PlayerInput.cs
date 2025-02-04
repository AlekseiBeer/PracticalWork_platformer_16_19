using UnityEngine;

namespace Platformer.Player.Inputs
{
    public class PlayerInput : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public bool Attack { get; private set; }
        public bool Jump { get; private set; }
        public bool Run { get; private set; }

        void Update()
        {
            Horizontal = Input.GetAxis(GlobalStringVars.HORIZONTAL_AXIS);
            Vertical = Input.GetAxis(GlobalStringVars.VERTICAL_AXIS);

            Attack = Input.GetButton(GlobalStringVars.Attack_BUTTON);
            Jump = Input.GetButtonDown(GlobalStringVars.Jump_BUTTON);
            Run = Input.GetButtonDown(GlobalStringVars.SHIFT_BUTTON);
        }
    }
}
