using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeCanvas : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Canvas currentCanvas;

    [SerializeField] private float duration = 1;
    [SerializeField] private Ease easeType = Ease.InOutExpo;

    private Button SelectedButton;

    public void GetNewCanvas(Canvas newCanvas)
    {
        Button[] oldCanvasButtons = currentCanvas.GetComponentsInChildren<Button>();
        Button[] newCanvasButtons = newCanvas.GetComponentsInChildren<Button>();

        foreach (Button button in oldCanvasButtons)
        {
            button.interactable = false;
        }

        foreach (Button button in newCanvasButtons)
        {
            button.interactable = true;
        }

        currentCanvas = newCanvas;
    }

    public void SelectButton(Button button)
    {
        SelectedButton = button;
        Invoke("SelectButtonInvoke", duration);
    }

    public void GetNewPos(Transform pos)
    {
        target.DOMove(pos.position, duration).SetEase(easeType);
        target.DORotate(pos.rotation.eulerAngles, duration).SetEase(easeType);
    }

    private void SelectButtonInvoke()
    {
        SelectedButton.Select();
    }
}
