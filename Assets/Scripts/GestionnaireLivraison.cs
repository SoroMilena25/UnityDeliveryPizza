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
    [Tooltip("Glissez ici le texte UI pour le score")]
    public TextMeshProUGUI scoreText;
    [Tooltip("Glissez ici le texte UI pour le popup de tip (+X pts)")]
    public TextMeshProUGUI tipPopupText;

    [Header("Jeu & Timer")]
    public float tempsMax = 60f; // 1 minute
    private float tempsRestant;
    private float tempsEcoule;
    public bool jeuDemarre = false;
    public bool jeuTermine = false;
    private int livraisonsFaites = 0;

    // --- Score ---
    [Header("Score")]
    public int scoreTipBase = 100;          // points de base par livraison réussie
    public float bonusTempsMultiplier = 2f; // points bonus par seconde restante
    private int scoreTotal = 0;

    public List<GameObject> ciblesActuelles = new List<GameObject>();

    void Start()
    {
        tempsRestant = tempsMax;
        if (messageText != null)  messageText.text = "Chargement des villes...";
        if (timerText != null)    timerText.text = "Temps : 60.0 s";
        if (scoreText != null)    scoreText.text = "Score : 0";
        if (tipPopupText != null) tipPopupText.gameObject.SetActive(false);

        // Chargement additif des deux scènes
        SceneManager.LoadScene("City2", LoadSceneMode.Additive);
        SceneManager.LoadScene("City3", LoadSceneMode.Additive);

        StartCoroutine(InitRoutine());
    }

    IEnumerator InitRoutine()
    {
        // Attente sécurisée : on vérifie que les scènes sont bien chargées
        // (plus fiable que WaitForSeconds qui pouvait rater sur les machines lentes)
        yield return new WaitUntil(() =>
            SceneManager.GetSceneByName("City2").isLoaded &&
            SceneManager.GetSceneByName("City3").isLoaded
        );

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

            // Couleur du timer selon urgence
            if (timerText != null)
            {
                if (tempsRestant <= 10f)      timerText.color = Color.red;
                else if (tempsRestant <= 20f) timerText.color = Color.yellow;
                else                          timerText.color = Color.white;
            }

            // Défaite : Temps écoulé
            if (tempsRestant <= 0)
            {
                FinDePartie(false);
            }

            // Victoire : Toutes les livraisons sont faites
            if (livraisonsFaites >= 3) // toujours 3 maisons par partie
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

        // Calcul du tip : base + bonus selon temps restant
        int tip = scoreTipBase + Mathf.RoundToInt(tempsRestant * bonusTempsMultiplier);
        AjouterScore(tip);

        Debug.Log("Pizza livrée ! Total : " + livraisonsFaites + "/3 — Tip : " + tip + " pts");
    }

    // Gestion du score et affichage du popup
    private void AjouterScore(int montant)
    {
        scoreTotal += montant;

        if (scoreText != null)
            scoreText.text = "Score : " + scoreTotal;

        // Affiche le popup de tip
        if (tipPopupText != null)
        {
            tipPopupText.text = "+" + montant + " pts !";
            tipPopupText.gameObject.SetActive(true);
            CancelInvoke(nameof(CacherPopup));
            Invoke(nameof(CacherPopup), 1.5f);
        }
    }

    private void CacherPopup()
    {
        if (tipPopupText != null)
            tipPopupText.gameObject.SetActive(false);
    }

    void FinDePartie(bool victoire)
    {
        jeuTermine = true;

        // Sauvegarde du high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (scoreTotal > highScore)
        {
            PlayerPrefs.SetInt("HighScore", scoreTotal);
            PlayerPrefs.Save();
        }

        if (victoire)
        {
            messageText.text = "GAGNÉ ! Temps : " + tempsEcoule.ToString("F1") + " s\n"
                             + "Score : " + scoreTotal + " pts\n"
                             + "Record : " + PlayerPrefs.GetInt("HighScore", 0) + " pts";
            messageText.color = Color.green;
        }
        else
        {
            messageText.text = "PERDU ! Trop lent pour les pizzas...\n"
                             + livraisonsFaites + "/3 pizzas livrées\n"
                             + "Score : " + scoreTotal + " pts";
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

            // On ajoute aussi le marqueur minimap automatiquement !
            if (maison.GetComponent<MarqueurMinimap>() == null)
            {
                maison.AddComponent<MarqueurMinimap>();
            }
        }
    }
}
