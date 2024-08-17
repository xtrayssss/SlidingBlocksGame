using LightScrollSnap;
using UnityEngine;

[CreateAssetMenu]
public class ExampleScrollSnapEffect : BaseScrollSnapEffect
{
    public override void OnItemUpdated(RectTransform item, float displacement)
    {
        Debug.Log(displacement);
        // Логика применения эффекта, например, изменение цвета или размера элемента
        item.localScale = Vector3.one * (1 - Mathf.Abs(displacement));
    }
}
