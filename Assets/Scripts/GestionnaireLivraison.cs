using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class GestionnaireLivraison : MonoBehaviour
{
    public GameObject quartier1;
    public GameObject quartier2;
    public GameObject quartier3;

    public List<GameObject> ciblesActuelles = new List<GameObject>();

    void Start()
    {
        
        SceneManager.LoadScene("City2", LoadSceneMode.Additive);
        StartCoroutine(InitRoutine());
    }

    IEnumerator InitRoutine()
    {
        yield return new WaitForSeconds(5f);

        quartier3 = GameObject.Find("Quartier3");
        ChoisirMaisonsAuHasard();
    }

    void ChoisirMaisonsAuHasard()
    {
        quartier3 = GameObject.Find("Quartier3");
        ciblesActuelles.Clear();

        ciblesActuelles.Add(GetMaisonAleatoire(quartier1));
        ciblesActuelles.Add(GetMaisonAleatoire(quartier2));
        ciblesActuelles.Add(GetMaisonAleatoire(quartier3));
    }

    GameObject GetMaisonAleatoire(GameObject quartier)
    {
        // 1. On récupère TOUS les composants Transform en profondeur (enfants, petits-enfants...)
        // 2. On filtre (.Where) pour ne garder que ceux qui ont le tag "Building"
        // 3. On transforme (.Select) le résultat en liste de GameObject
        List<GameObject> buildings = quartier.GetComponentsInChildren<Transform>()
            .Where(t => t.CompareTag("Building"))
            .Select(t => t.gameObject)
            .ToList();

        if (buildings.Count > 0)
        {
            // On choisit un index aléatoire dans la liste obtenue
            return buildings[Random.Range(0, buildings.Count)];
        }

        Debug.LogWarning("Aucun bâtiment avec le tag 'Building' trouvé dans : " + quartier.name);
        return null;
    }
}
