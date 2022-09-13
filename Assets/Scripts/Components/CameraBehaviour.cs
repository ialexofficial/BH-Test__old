using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(Camera))]
    public class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] private Vector3 positionOffset = new Vector3(0, 2, -3);
        [SerializeField] private Vector3 rotationOffset = new Vector3(0, 0 , 0);
        [SerializeField] private float[] verticalRange = {30f, 60f};
        
        private Camera _camera;
        private Transform _cameraTransform;
        private Transform _aim;

        public void SetAim(Transform aim)
        {
            _aim = aim;
        }

        public void Rotate(Vector3 rotation)
        {
            rotationOffset += rotation;
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (!_aim)
                return;
            
            transform.position = _aim.position + _aim.transform.TransformVector(positionOffset);
            
            transform.LookAt(_aim);
            Vector3 rotation = transform.eulerAngles + rotationOffset;

            if (rotation.x < verticalRange[0])
            {
                rotationOffset.x -= rotation.x - verticalRange[0];
                rotation.x = verticalRange[0];
            }

            if (rotation.x > verticalRange[1])
            {
                rotationOffset.x -= rotation.x - verticalRange[1];
                rotation.x = verticalRange[1];
            }

            transform.eulerAngles = rotation;
        }
    }
}