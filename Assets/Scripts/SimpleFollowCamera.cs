using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    [Tooltip("L'objet que la caméra doit suivre (votre joueur)")]
    public Transform target;

    [Tooltip("La distance fixe entre la caméra et le joueur")]
    public Vector3 offset = new Vector3(0, 10, -5);

    [Tooltip("Vitesse de suivi (plus c'est haut, plus c'est réactif)")]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;

        // On calcule la position cible
        Vector3 desiredPosition = target.position + offset;

        // On déplace la caméra vers la position cible
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // SUPPRESSION DE transform.LookAt(target);
        // Au lieu de ça, on définit une rotation fixe manuellement
        transform.rotation = Quaternion.Euler(60f, 0f, 0f); ;
    }
}
