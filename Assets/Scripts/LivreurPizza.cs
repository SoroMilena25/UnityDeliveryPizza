using UnityEngine;
public class LivreurPizza : MonoBehaviour
{
    private GestionnaireLivraison gestionnaire;
    void Start()
    {
        gestionnaire = FindAnyObjectByType<GestionnaireLivraison>();
    }
    // Fonction si ton bâtiment a un collider normal (choc physique)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            ValiderMaison(collision.gameObject);
        }
    }
    // Fonction si ton bâtiment a un "Is Trigger" (passage à travers)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Building"))
        {
            ValiderMaison(other.gameObject);
        }
    }
    // La logique de validation (commune aux deux types de chocs)
    void ValiderMaison(GameObject batimentTouche)
    {
        if (gestionnaire != null && gestionnaire.ciblesActuelles.Contains(batimentTouche))
        {
            // C'est une bonne adresse ! On valide.
            gestionnaire.ciblesActuelles.Remove(batimentTouche);
            gestionnaire.ValiderUneLivraison();

            // 1. On arrête le clignotement
            ClignotementCible clignotement = batimentTouche.GetComponent<ClignotementCible>();
            if (clignotement != null)
            {
                Destroy(clignotement);
            }

            // 2. On supprime le marqueur minimap
            MarqueurMinimap marqueur = batimentTouche.GetComponent<MarqueurMinimap>();
            if (marqueur != null)
            {
                marqueur.SupprimerMarqueur();
            }

            // 3. On met la maison en vert
            // GetComponentsInChildren gère les bâtiments avec plusieurs renderers enfants
            Renderer[] renderers = batimentTouche.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.material.color = Color.green;
            }
        }
    }
}
