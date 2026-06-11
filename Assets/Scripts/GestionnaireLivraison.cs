using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem; // Ajout pour le nouveau système d'inputs

public class GestionnaireLivraison : MonoBehaviour
{
    [Header("Quartiers")]
    public GameObject quartier1;
    public GameObject quartier2;
    public GameObject quartier3;

    [Header("Interface (UI)")]
    [Tooltip("Glissez ici le texte UI qui affichera le timer")]
    public TextMeshProUGUI timerText;
    [Tooltip("Glissez ici le texte UI pour les messages (Commencer, Gagné, Perdu)")]
    public TextMeshProUGUI messageText;

    [Header("Jeu & Timer")]
    public float tempsMax = 60f; // 1 minute
    private float tempsRestant;
    private float tempsEcoule;
    private bool jeuDemarre = false;
    private bool jeuTermine = false;
    private int livraisonsFaites = 0;

    public List<GameObject> ciblesActuelles = new List<GameObject>();

    void Start()
    {
        tempsRestant = tempsMax;
        if (messageText != null) messageText.text = "Chargement des villes...";
        if (timerText != null) timerText.text = "Temps : 60.0 s";

        // Chargement additif des deux scènes
        SceneManager.LoadScene("City2", LoadSceneMode.Additive);
        SceneManager.LoadScene("City3", LoadSceneMode.Additive);

        StartCoroutine(InitRoutine());
    }

    IEnumerator InitRoutine()
    {
        // On attend 5 secondes pour s'assurer que les deux scènes sont bien chargées
        yield return new WaitForSeconds(5f);

        ChoisirMaisonsAuHasard();

        // Les maps sont chargées, on invite le joueur à commencer
        if (messageText != null) messageText.text = "Cliquez n'importe où pour commencer !";
    }

    void Update()
    {
        // 1. Attente du clic pour commencer avec le New Input System
        if (!jeuDemarre && !jeuTermine && ciblesActuelles.Count > 0)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                jeuDemarre = true;
                if (messageText != null) messageText.text = ""; // On cache le message
            }
        }

        // 2. Le jeu tourne
        if (jeuDemarre && !jeuTermine)
        {
            tempsRestant -= Time.deltaTime; // Le temps diminue
            tempsEcoule += Time.deltaTime;  // On compte combien de temps s'est écoulé

            // Mise à jour de l'affichage du timer
            if (timerText != null)
                timerText.text = "Temps : " + Mathf.Max(0, tempsRestant).ToString("F1") + " s";

            // Défaite : Temps écoulé
            if (tempsRestant <= 0)
            {
                FinDePartie(false);
            }

            // Victoire : Toutes les livraisons sont faites
            if (livraisonsFaites >= ciblesActuelles.Count)
            {
                FinDePartie(true);
            }
        }
    }

    GameObject GetMaisonAleatoire(GameObject quartier)
    {
        if (quartier == null) return null;

        List<GameObject> buildings = quartier.GetComponentsInChildren<Transform>()
            .Where(t => t.CompareTag("Building"))
            .Select(t => t.gameObject)
            .ToList();

        if (buildings.Count > 0)
            return buildings[Random.Range(0, buildings.Count)];

        Debug.LogWarning("Aucun bâtiment avec le tag 'Building' trouvé dans : " + quartier.name);
        return null;
    }

    // Fonction à appeler depuis le script du LIVREUR quand il touche une maison
    public void ValiderUneLivraison()
    {
        if (jeuTermine || !jeuDemarre) return;

        livraisonsFaites++;
        Debug.Log("Pizza livrée ! Total : " + livraisonsFaites + "/" + ciblesActuelles.Count);
    }

    void FinDePartie(bool victoire)
    {
        jeuTermine = true;

        if (victoire)
        {
            messageText.text = "GAGNÉ ! Temps : " + tempsEcoule.ToString("F1") + " s";
            messageText.color = Color.green;
        }
        else
        {
            messageText.text = "PERDU ! Trop lent pour les pizzas...";
            messageText.color = Color.red;
        }
    }

    void ChoisirMaisonsAuHasard()
    {
        // On cherche dynamiquement les deux quartiers fraîchement chargés
        quartier3 = GameObject.Find("Quartier3"); // Vient de City2
        quartier2 = GameObject.Find("Quartier2"); // Vient de City3

        ciblesActuelles.Clear();

        AjouterCible(quartier1);
        AjouterCible(quartier2);
        AjouterCible(quartier3);
    }

    // Petite fonction intermédiaire pour simplifier l'ajout
    void AjouterCible(GameObject quartier)
    {
        GameObject maison = GetMaisonAleatoire(quartier);
        if (maison != null)
        {
            ciblesActuelles.Add(maison);

            // MAGIE : On ajoute le script de clignotement automatiquement au bâtiment !
            if (maison.GetComponent<ClignotementCible>() == null)
            {
                maison.AddComponent<ClignotementCible>();
            }
        }
    }
}