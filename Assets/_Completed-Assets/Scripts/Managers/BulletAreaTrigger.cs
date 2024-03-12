using System;
using UnityEngine;

namespace Complete
{
    public class BulletAreaTrigger : MonoBehaviour
    {
        private void OnEnable()
        {
            GameManager.Current.onIsFocusedChanged+=OnTargetFocus;
        }

        private void OnTargetFocus(GameManager arg1, bool arg2, bool arg3)
        {
            if (!arg3)
            {
                GameManager.Current.IsEnemyFocused = false;
            }
        }

        private void OnDisable()
        {
            GameManager.Current.onIsFocusedChanged-=OnTargetFocus;
        }
 

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy") && GameManager.Current.IsFocused)
            {
                GameManager.Current.IsEnemyFocused = true;
            }
        }
    }
}