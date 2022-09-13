using UnityEngine;

namespace Components
{
    public class NicknameBehaviour : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private TextMesh textMesh;
        
        private Transform _cameraTransform;

        public void UpdateText(string content)
        {
            textMesh.text = content;
        }

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
        }
        
        private void LateUpdate()
        {
            transform.LookAt(_cameraTransform);
            transform.rotation *= Quaternion.Euler(rotationOffset);
        }
    }
}