using System.Collections;
using Components;
using Mirror;
using Models;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(
        typeof(Rigidbody), 
        typeof(Collider), 
        typeof(MeshRenderer)
    )]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float movementSpeed = 3;
        [SerializeField] private float rotationSpeed = 3;
        [SerializeField] private float strafeDistance = 10;
        [SerializeField] private float strafeSpeed = 12;
        [SerializeField] private float damageCooldown = 3;
        [SerializeField] private LayerMask damagableMask;
        [SerializeField] private Color damagedColor = Color.red;
        [SerializeField] private NicknameBehaviour nicknameField;

        [SyncVar] private string _nickname;
        private PlayerModel _model;
        private Rigidbody _rigidbody;
        private NetworkIdentity _networkIdentity;
        private CameraBehaviour _camera;
        private MeshRenderer _meshRenderer;
        private Color _defaultColor;

        public uint NetId => _networkIdentity.netId;
        public string Nickname => _nickname;

        public void Move(float vertical, float horizontal)
        {
            _model.Move(vertical, horizontal, movementSpeed);
        }

        public void Rotate(float vertical, float horizontal)
        {
            _model.Rotate(vertical, horizontal, rotationSpeed);
        }

        public void Strafe()
        {
            _model.StartStrafe();
        }

        [TargetRpc]
        public void Damage(NetworkConnection connection, NetworkIdentity murdererIdentity)
        {
            if (_model.TryDamage(damageCooldown))
            {
                ScoreManager.Instance.IncreaseScore(murdererIdentity.netId);
            }
        }

        public void UpdateScore(int score)
        {
            nicknameField.UpdateText($"{_nickname} | {score}");
        }
        
        public void Spawn()
        {
            if (isLocalPlayer)
            {
                ScoreManager.Instance.Connect(_networkIdentity.netId);
                CmdSpawn();
            }
            else
            {
                UpdateScore(ScoreManager.Instance.GetScore(_networkIdentity.netId));
            }
        }

        private void Awake()
        {
            _model = new PlayerModel();

            _model.OnMove += Moved;
            _model.OnRotate += Rotated;
            _model.OnStrafeStart += StrafeStarted;
            _model.OnDamage += (damageCooldown) =>
            {
                CmdColorChanged(gameObject, damagedColor);
                StartCoroutine(ChangeMeshColor(damageCooldown));
            };
        }
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _networkIdentity = GetComponent<NetworkIdentity>();
            
            _defaultColor = _meshRenderer.material.color;
            _camera = Camera.main.GetComponent<CameraBehaviour>();

            if (isLocalPlayer)
            {
                SetNickname(UIManager.Instance.Nickname);

                InputManager inputManager = InputManager.Instance;
                inputManager.SubscribeMovement(Move);
                inputManager.SubscribeRotation(Rotate);
                inputManager.SubscribeLMB(Strafe);
                
                nicknameField.gameObject.SetActive(false);
                _camera.SetAim(transform);
            }

            Spawn();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (
                !_model.IsStrafing ||
                (damagableMask & (1 << other.gameObject.layer)) == 0
            )
                return;

            DamageEnemy(other.gameObject, _networkIdentity);
        }

        [Command]
        private void SetNickname(string nickname)
        {
            _nickname = nickname;
        }
        
        [Command]
        private void DamageEnemy(GameObject enemy, NetworkIdentity murdererIdentity)
        {
            PlayerController enemyController = enemy.GetComponent<PlayerController>();
            enemyController.Damage(enemyController._networkIdentity.connectionToClient, murdererIdentity);
        }

        [Command]
        private void CmdSpawn()
        {
            Transform spawn = Spawner.Instance.GetPoint();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }

        private void Moved(Vector3 movement)
        {
            transform.position += transform.rotation * movement;
        }

        private void Rotated(Vector3 rotation)
        {
            _camera.Rotate(new Vector3(rotation.x, 0, 0));
            transform.eulerAngles += new Vector3(0, rotation.y, 0);
        }

        [Command]
        private void CmdColorChanged(GameObject gameObject, Color color)
        {
            SynchronizeColor(gameObject, color);
        }

        [ClientRpc]
        private void SynchronizeColor(GameObject gameObject, Color color)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = color;
        }

        private void StrafeStarted()
        {
            StartCoroutine(StrafeCoroutine());
        }

        private IEnumerator StrafeCoroutine()
        {
            while (_model.IsStrafing)
            {
                _model.Strafe(strafeDistance, strafeSpeed);
                
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator ChangeMeshColor(float cooldown)
        {
            yield return new WaitForSeconds(cooldown);

            CmdColorChanged(gameObject, _defaultColor);
        }

        private void OnDestroy()
        {
            InputManager inputManager = InputManager.Instance;
            inputManager.UnsubscribeMovement(Move);
            inputManager.UnsubscribeRotation(Rotate);
            inputManager.UnsubscribeLMB(Strafe);
            
            _model.OnMove -= Moved;
            _model.OnRotate -= Rotated;
            _model.OnStrafeStart -= StrafeStarted;
        }
    }
}