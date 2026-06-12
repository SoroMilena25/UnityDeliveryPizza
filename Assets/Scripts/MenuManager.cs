using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Ce script gère le panneau de menu principal affiché au démarrage.
// Il se place sur le GameManager ou un objet dédié dans la scène City1.
public class MenuManager : MonoBehaviour
{
    [Header("Panel Menu")]
    [Tooltip("Glissez ici le panel UI du menu principal")]
    public GameObject menuPanel;

    [Header("Référence au gestionnaire")]
    private GestionnaireLivraison gestionnaire;

    // 
    void Start()
    {
        gestionnaire = FindAnyObjectByType<GestionnaireLivraison>();

        // On affiche le menu au démarrage
        AfficherMenu(true);
    }

    // 
    // Appelé par le bouton "Commencer" dans l'Inspector Unity
    public void CommencerPartie()
    {
        // On cache le menu
        AfficherMenu(false);

        // On démarre le jeu via le gestionnaire
        if (gestionnaire != null)
            gestionnaire.jeuDemarre = true;
    }

    // 
    // Affiche ou cache le panel menu
    private void AfficherMenu(bool afficher)
    {
        if (menuPanel != null)
            menuPanel.SetActive(afficher);
    }
}
