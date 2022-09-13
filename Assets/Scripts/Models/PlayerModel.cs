using System;
using UnityEngine;

namespace Models
{
    public class PlayerModel
    {
        private bool _isStrafing = false;
        private float _passedStrafeDistance = 0;
        private float _lastDamageTime = 0;

        public bool IsStrafing => _isStrafing;

        public event Action<Vector3> OnMove;
        public event Action<Vector3> OnRotate;
        public event Action<float> OnDamage;
        public event Action OnStrafeStart;

        public void Move(float vertical, float horizontal, float speed)
        {
            if (_isStrafing)
                return;
            
            Vector3 movement = new Vector3(horizontal, 0, vertical).normalized * speed * Time.fixedDeltaTime;
            
            OnMove?.Invoke(movement);
        }

        public void Rotate(float vertical, float horizontal, float speed)
        {
            Vector3 rotation = new Vector3(-vertical, horizontal, 0) * speed * Time.fixedDeltaTime;
            
            OnRotate?.Invoke(rotation);
        }

        public void StartStrafe()
        {
            if (_isStrafing)
                return;

            _passedStrafeDistance = 0;
            _isStrafing = true;
            
            OnStrafeStart?.Invoke();
        }

        public void Strafe(float distance, float speed)
        {
            Vector3 movement = Vector3.forward * speed * Time.fixedDeltaTime;
            _passedStrafeDistance += speed;

            if (_passedStrafeDistance >= distance)
                _isStrafing = false;
            
            OnMove?.Invoke(movement);
        }

        public bool TryDamage(float damageCooldown)
        {
            float currentTime = Time.time;
            
            if (currentTime - _lastDamageTime < damageCooldown)
                return false;

            _lastDamageTime = currentTime;
            OnDamage?.Invoke(damageCooldown);

            return true;
        }
    }
}