using UnityEngine;

namespace Game
{
    public class RendererToggle : MonoBehaviour
    {
        private new Renderer renderer;
        [SerializeField] private KeyCode hideKey = KeyCode.H;

        private void Awake() => renderer = GetComponent<Renderer>();

        private void Update()
        {
            if (Input.GetKeyDown(hideKey))
                renderer.enabled = !renderer.enabled;
        }
    }
}
