using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace AnimationIntegration
{
    public class Player : MonoBehaviour
    {
        public Transform targetBone;

        [SerializeField] private GameObject gun;
        [SerializeField] private GameObject sword;

        [SerializeField] private float speed = 2f;
        
        [SerializeField] private Camera gameCamera;
        private Vector3 _camDisposition;

        [SerializeField] private GameManager gameManager;

        private const float FinishingRange = 5f;
        private const float FinishingDistanceFromEnemy = 2f;
        private GameObject _enemyToFinish;
        private bool _finishing;

        private CharacterController _controller;
        private Animator _animator;

        private static readonly int AIsRunning = Animator.StringToHash("isRunning");
        private static readonly int AFinishing = Animator.StringToHash("Finishing");

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _camDisposition = gameCamera.transform.position - transform.position;
        }

        private void Update()
        {
            if (!_finishing)
            {
                Move();
                CheckFinishing();
            }
        }

        private void CheckFinishing()
        {
            var enemies = Physics.OverlapSphere(transform.position, FinishingRange, LayerMask.GetMask("Enemies"));
            gameManager.SetMessageActive(enemies.Length != 0);
            if (enemies.Length == 0) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _enemyToFinish = enemies[0].gameObject;
                _finishing = true;
                _animator.SetTrigger(AFinishing);
                sword.SetActive(true);
                gun.SetActive(false);
                gameManager.SetMessageActive(false);

                // Rotating player towards enemy
                transform.LookAt(_enemyToFinish.transform);
                targetBone.localEulerAngles = Vector3.zero;

                // Moving player to enemy
                var dir = transform.position - _enemyToFinish.transform.position;
                dir.Normalize();
                dir *= FinishingDistanceFromEnemy;
                transform.position = _enemyToFinish.transform.position + dir;

            }
        }

        // Used by animator in moment of killing enemy in finishing animation
        public void KillEnemy()
        {
            _enemyToFinish.GetComponent<Enemy>().Die();
        }

        // Used by animator in the end of finishing animation
        public void Finish()
        {
            _enemyToFinish = null;
            _finishing = false;
            sword.SetActive(false);
            gun.SetActive(true);
        }

        private void Move()
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(hor, 0, ver);
            direction.Normalize();

            _controller.SimpleMove(direction * speed);
            transform.LookAt(transform.position + direction);

            _animator.SetBool(AIsRunning, !direction.Equals(Vector3.zero));
        }

        public void LateUpdate()
        {
            if (!_finishing)
            {
                RotateTowardsMouse();
            }

            gameCamera.transform.position = transform.position + _camDisposition;
        }

        private void RotateTowardsMouse()
        {
            Vector3 dir = GetMousePosition() - targetBone.position;
            Vector3 fw = transform.forward;
            float angle = Vector2.SignedAngle(new Vector2(fw.x, fw.z), new Vector2(dir.x, dir.z));
            Vector3 rot = new Vector3(angle, 0, 0);
            rot.x = angle;
            targetBone.localEulerAngles += rot;
        }

        private Vector3 GetMousePosition()
        {
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                return raycastHit.point;
            }

            return new Vector3();
        }
    }
}