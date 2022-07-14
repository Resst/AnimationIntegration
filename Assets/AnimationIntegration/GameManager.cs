using UnityEngine;
using UnityEngine.UI;

namespace AnimationIntegration
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Text message;

        public void SetMessageActive(bool active)
        {
            message.enabled = active;
        }
    }
}
