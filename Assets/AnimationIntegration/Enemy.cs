using System.Collections;
using UnityEngine;

namespace AnimationIntegration
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Vector3 spawnRangeP1;
        [SerializeField] private Vector3 spawnRangeP2;

        private Animator _animator;
        private CapsuleCollider _collider;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _collider = GetComponent<CapsuleCollider>();
        }

        public void Die()
        {
            _animator.enabled = false;
            _collider.enabled = false;
            StartCoroutine(DieCoroutine());
        }

        private IEnumerator DieCoroutine()
        {
            yield return new WaitForSeconds(5f);
            InitNewEnemy();
        }

        private void InitNewEnemy()
        {
            Vector3 pos = new Vector3
            {
                x = Random.Range(spawnRangeP1.x, spawnRangeP2.x),
                y = Random.Range(spawnRangeP1.y, spawnRangeP2.y),
                z = Random.Range(spawnRangeP1.z, spawnRangeP2.z)
            };
            Quaternion rot = Quaternion.Euler(0, Random.Range(-180, 180), 0);
            transform.SetPositionAndRotation(pos, rot);
            _animator.enabled = true;
            _collider.enabled = true;
        }
    }
}
