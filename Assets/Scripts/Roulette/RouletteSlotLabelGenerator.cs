using System.Collections.Generic;
using UnityEngine;

namespace Roulette
{
    [ExecuteInEditMode]
    public class RouletteSlotLabelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject rewardAmountTextPrefab;
        [SerializeField] private float rewardAmountTextRadius = 225f;
        [SerializeField] private int numberOfSlots = 12;
        [SerializeField] private int offsetAngle = -15;
        [SerializeField] private Transform rewardLabelContainer;

        public void AlignText()
        {
            var objectsToDestroy = new List<GameObject>();
            for (int i = 0; i < rewardLabelContainer.childCount; i++)
            {
                var child = rewardLabelContainer.GetChild(i).gameObject;
                if (child.name.StartsWith("RewardAmount_"))
                {
                    objectsToDestroy.Add(child);
                }
            }

            foreach (var obj in objectsToDestroy)
            {
                DestroyImmediate(obj);
            }

            var anglePerSlot = 360f / numberOfSlots;
            var initialOffset = 90f + offsetAngle; 

            for (int i = 0; i < numberOfSlots; i++)
            {
                var textObject = Instantiate(rewardAmountTextPrefab, rewardLabelContainer);
                textObject.name = $"RewardAmount_{i}";

                var currentAngle = -(i * anglePerSlot) + initialOffset;
                var angleInRadians = currentAngle * Mathf.Deg2Rad;

                var xPos = rewardAmountTextRadius * Mathf.Cos(angleInRadians);
                var yPos = rewardAmountTextRadius * Mathf.Sin(angleInRadians);
                textObject.transform.localPosition = new Vector3(xPos, yPos, 0);

                textObject.transform.localRotation = Quaternion.Euler(0, 0, currentAngle - 90f);
            }
        }
    }
}