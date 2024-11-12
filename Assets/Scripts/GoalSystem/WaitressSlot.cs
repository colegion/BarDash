using UnityEngine;

namespace GoalSystem
{
    public class WaitressSlot : MonoBehaviour
    {
        [SerializeField] private Transform waitressTarget;
        
        private Waitress _waitress;

        public void SetWaitressRef(Waitress waitress)
        {
            _waitress = waitress;
        }

        public Waitress GetWaitressRef()
        {
            return _waitress;
        }

        public bool IsAvailable()
        {
            return _waitress != null;
        }

        public Transform GetTarget()
        {
            return waitressTarget;
        }

        public void ResetSelf()
        {
            _waitress = null;
        }
    }
}
