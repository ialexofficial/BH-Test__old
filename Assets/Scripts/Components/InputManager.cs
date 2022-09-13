using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent<float, float> movementAxis = new UnityEvent<float, float>();
        [SerializeField] private UnityEvent<float, float> rotationAxis = new UnityEvent<float, float>();
        [SerializeField] private UnityEvent OnLMBClick = new UnityEvent();
        [SerializeField] private UnityEvent OnCancel = new UnityEvent();
        
        private static InputManager _instance;

        public static InputManager Instance => _instance;

        public bool IsActive { get; set; } = false;

        public void SubscribeMovement(UnityAction<float, float> listener) =>
            movementAxis.AddListener(listener);
        
        public void UnsubscribeMovement(UnityAction<float, float> listener) =>
            movementAxis.RemoveListener(listener);
        
        public void SubscribeRotation(UnityAction<float, float> listener) =>
            rotationAxis.AddListener(listener);

        public void UnsubscribeRotation(UnityAction<float, float> listener) =>
            rotationAxis.RemoveListener(listener);

        public void SubscribeLMB(UnityAction listener) =>
            OnLMBClick.AddListener(listener);

        public void UnsubscribeLMB(UnityAction listener) =>
            OnLMBClick.RemoveListener(listener);

        public void SubscribeCancel(UnityAction listener) =>
            OnCancel.AddListener(listener);

        public void UnsubscribeCancel(UnityAction listener) =>
            OnCancel.RemoveListener(listener);

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                OnCancel.Invoke();
            
            if (!IsActive)
                return;
            
            if(Input.GetKeyDown(KeyCode.Mouse0))
                OnLMBClick.Invoke();
        }

        private void FixedUpdate()
        {
            if (!IsActive)
                return;
            
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseX = Input.GetAxis("Mouse X");
            rotationAxis.Invoke(mouseY, mouseX);

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            movementAxis.Invoke(vertical, horizontal);
        }
    }
}