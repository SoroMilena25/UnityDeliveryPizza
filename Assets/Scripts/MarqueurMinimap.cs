using UnityEngine;

// Ce script se place sur chaque maison cible.
// Il crée automatiquement un marqueur 3D flottant visible uniquement sur la minimap.
public class MarqueurMinimap : MonoBehaviour
{
    [Header("Apparence")]
    public Color couleurMarqueur = Color.red;
    public float hauteur = 20f;      // hauteur au-dessus du bâtiment
    public float taille = 8f;        // taille du marqueur dans le monde

    private GameObject marqueurObj;

    // -------------------------------------------------------
    void Start()
    {
        CreerMarqueur();
    }

    void CreerMarqueur()
    {
        // 1. Créer un quad (plan) au-dessus de la maison
        marqueurObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        marqueurObj.name = "MarqueurMinimap_" + gameObject.name;

        // 2. Positionner au-dessus de la maison, orienté vers le haut (face à la minimap)
        marqueurObj.transform.position = transform.position + Vector3.up * hauteur;
        marqueurObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // couché à plat
        marqueurObj.transform.localScale = new Vector3(taille, taille, taille);

        // 3. Assigner le layer MinimapOnly
        // IMPORTANT : vérifie que ce nom correspond exactement au layer créé dans Unity
        int layer = LayerMask.NameToLayer("MinimapOnly");
        if (layer == -1)
        {
            Debug.LogError("Layer 'MinimapOnly' introuvable ! Crée-le dans Edit > Project Settings > Tags and Layers");
            return;
        }
        marqueurObj.layer = layer;

        // 4. Supprimer le collider (on ne veut pas d'interaction physique)
        Destroy(marqueurObj.GetComponent<MeshCollider>());

        // 5. Appliquer la couleur
        Renderer r = marqueurObj.GetComponent<Renderer>();
        if (r != null)
        {
            // Utilise le shader Unlit pour que la couleur soit visible même sans lumière
            r.material = new Material(Shader.Find("Unlit/Color"));
            r.material.color = couleurMarqueur;
        }
    }

    // -------------------------------------------------------
    // Appelé quand la livraison est validée (depuis LivreurPizza)
    public void SupprimerMarqueur()
    {
        if (marqueurObj != null)
            Destroy(marqueurObj);
    }

    // Nettoyage au cas où la maison serait détruite sans validation
    void OnDestroy()
    {
        SupprimerMarqueur();
    }
}
