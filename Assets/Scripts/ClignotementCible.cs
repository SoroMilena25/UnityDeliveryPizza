using UnityEngine;
public class ClignotementCible : MonoBehaviour
{
    // Tableaux pour gérer tous les renderers enfants du bâtiment
    private Renderer[] renderers;
    private Color[] couleursOriginales;
    public Color couleurCible = Color.red; // La couleur du clignotement
    public float vitesse = 3f; // La vitesse du clignotement
    void Start()
    {
        // GetComponentsInChildren récupère TOUS les renderers du bâtiment
        // y compris ceux des objets enfants (murs, toit, fenêtres...)
        renderers = GetComponentsInChildren<Renderer>();
        couleursOriginales = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                // On sauvegarde la couleur de base du bâtiment
                couleursOriginales[i] = renderers[i].material.color;
            }
        }
    }
    void Update()
    {
        // Mathf.PingPong crée un va-et-vient fluide entre 0 et 1
        float t = Mathf.PingPong(Time.time * vitesse, 1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                // On mélange la couleur de base avec le rouge selon le va-et-vient
                renderers[i].material.color = Color.Lerp(couleursOriginales[i], couleurCible, t);
            }
        }
    }
    // Appelé automatiquement quand le script est détruit (livraison validée)
    // Remet toutes les couleurs d'origine avant de passer au vert
    void OnDestroy()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].material.color = couleursOriginales[i];
        }
    }
}
