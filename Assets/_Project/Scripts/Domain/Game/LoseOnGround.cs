using UnityEngine;

public class LoseOnGround : MonoBehaviour
{
    [SerializeField] private LevelEndController endController;
    [SerializeField] private LayerMask groundLayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (endController == null) return;

        // если уже выиграли/проиграли Ч ничего не делаем
        if (endController.CurrentState != LevelEndController.State.Playing)
            return;

        // провер€ем, что столкнулись именно с землЄй
        if (((1 << collision.gameObject.layer) & groundLayer) == 0)
            return;

        endController.Lose();
    }
}
