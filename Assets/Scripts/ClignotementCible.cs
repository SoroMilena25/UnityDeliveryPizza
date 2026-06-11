using UnityEngine;

public class ClignotementCible : MonoBehaviour
{
    private Renderer objRenderer;
    private Color couleurOriginale;
    public Color couleurCible = Color.red; // La couleur du clignotement
    public float vitesse = 3f; // La vitesse du clignotement

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            // On sauvegarde la couleur de base du bâtiment
            couleurOriginale = objRenderer.material.color;
        }
    }

    void Update()
    {
        if (objRenderer != null)
        {
            // Mathf.PingPong crée un va-et-vient fluide entre 0 et 1
            float t = Mathf.PingPong(Time.time * vitesse, 1f);

            // On mélange la couleur de base avec le rouge selon le va-et-vient
            objRenderer.material.color = Color.Lerp(couleurOriginale, couleurCible, t);
        }
    }
}
