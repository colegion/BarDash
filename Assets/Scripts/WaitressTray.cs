using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitressTray : MonoBehaviour
{
    [SerializeField] private Transform outCircleOfTray;
    [SerializeField] private List<Transform> drinkFieldTransforms = new List<Transform>();
    private int currentDrinkCountOnTray = 0;
    public void Initialize(int drinkPositionsAmount, float drinkYDistanceFromTray = -0.73f)
    {
        CalculateDrinkPositions(drinkPositionsAmount, drinkYDistanceFromTray);
    }
    private void CalculateDrinkPositions(int positionsAmount, float drinkYPosition)
    {

        float radius = Mathf.Abs(Vector3.Distance(transform.localPosition, outCircleOfTray.localPosition));
        float angleStep = 2f * Mathf.PI / positionsAmount;
        GameObject parentObject = new GameObject("DrinkPointParent");
        parentObject.transform.position = Vector3.zero;
        parentObject.transform.eulerAngles = Vector3.zero;
        parentObject.transform.SetParent(transform);
        parentObject.transform.localPosition = Vector3.zero;
        parentObject.transform.localEulerAngles = new Vector3(90, 30, 30);

        for (int i = 0; i < positionsAmount; i++)
        {
            float xPosition = radius * Mathf.Cos(angleStep * i);
            float zPosition = radius * Mathf.Sin(angleStep * i);
            GameObject go = new GameObject("DrinkPoint");
            go.transform.SetParent(parentObject.transform);
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localPosition = Vector3.zero;
            Vector3 calculatedPoint = new Vector3(xPosition, drinkYPosition, zPosition);
            go.transform.localPosition = calculatedPoint;
            drinkFieldTransforms.Add(go.transform);

        }
    }
    public Transform GiveDrinkTransform()
    {
        if (currentDrinkCountOnTray < drinkFieldTransforms.Count)
        {
            Transform resultTransform = drinkFieldTransforms[currentDrinkCountOnTray];
            currentDrinkCountOnTray++;
            return resultTransform;
        }
        else
        {
            Debug.LogWarning("AQ sence bu tepsi bu kadar bardak alabilir mi?");
            return null;
        }

    }

}
