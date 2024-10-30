using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class GeneticAlgorithm : MonoBehaviour
{
    public int populationSize = 10;     // Taille de la population
    public int generations = 10;        // Nombre de générations à simuler
    public float mutationRate = 0.01f;  // Taux de mutation
    public float selectionThreshold = 0.7f;  // Taux de sélection
    public float fitnessImportanceBias = 0.5f;  // Biais de selection d'importance que l'on donne aux hautes valeurs de fitness 
   
    private int genomeLength = 10;       // Longueur du génome
    private Vector3 startingPosition = Vector3.zero;  // Position de départ pour l'affichage des créatures
    private float creatureSpacing = 7f;               // Espacement entre les générations dans l'espace 3D
    private List<Creature> population;                // Population actuelle
    private List<List<Creature>> allGenerations;      // Historique de toutes les générations
    public CreatureGenerator creatureGenerator;       // Référence au générateur de créatures
     

    /// <summary>
    /// Initialisation la population de créatures, lancement de l'évolution (pour le nombre de generations définies) et affichage des créatures
    /// </summary>
    void Start()
    {
        InitializePopulation();
        EvolvePopulation();
        DisplayAllGenerations(); 
    }

    /// <summary>
    /// Initialise la population en créant un ensemble de créatures avec des génomes aléatoires
    /// </summary>
    void InitializePopulation()
    {
        population = new List<Creature>();
        allGenerations = new List<List<Creature>>();
        for (int i = 0; i < populationSize; i++)
        {
            Creature newCreature = new Creature(genomeLength, creatureGenerator);
            population.Add(newCreature);
        }
        allGenerations.Add(new List<Creature>(population)); 
    }


    /// <summary>
    /// Gère l'évolution de la population sur le nombre de générations défini =>  sélection, recombinaison et mutation
    /// </summary>
    void EvolvePopulation()
    {
        int iteration = 0; 
        while (iteration < generations)
        {
            // Sélectionner les meilleures créatures
            List<Creature> selectedPopulation = Selection();

            // Création la nouvelle génération
            List<Creature> newPopulation = new List<Creature>();
            while (newPopulation.Count < populationSize)
            {
                Creature parent1 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];
                Creature parent2 = selectedPopulation[Random.Range(0, selectedPopulation.Count)];

                Creature child = Recombination(parent1, parent2); 
                Mutate(child); 
                newPopulation.Add(child);
            }

            // Remplacer l'ancienne population par la nouvelle
            population = newPopulation;
            allGenerations.Add(new List<Creature>(newPopulation));

            iteration++; 
        }
    }


    /// <summary>
    /// Sélectionne les créatures les plus aptes (par leur fitness) pour être les parents de la prochaine génération
    /// Utilise une roulette biaisée pour choisir en fonction du fitness
    /// </summary>
    /// <returns>Liste des créatures sélectionnées</returns>
List<Creature> Selection()
{
    List<Creature> selectedPopulation = new List<Creature>();
    
    // Trier la population par fitness décroissante
    population.Sort((a, b) => b.fitness.CompareTo(a.fitness));
    foreach (var creature in population)
    {
        // Calculer une probabilité de sélection basée sur la fitness
        float selectionChance = creature.fitness + Random.Range(0f, 1f) * creature.fitness * 0.1f;

        // Ajouter les meilleures créatures selon le facteur de probabilité
        if (selectionChance >= (population[0].fitness * fitnessImportanceBias )) 
        {
            selectedPopulation.Add(creature);
        }

        // Limiter la sélection au pourcentage défini par selectionThreshold
        if (selectedPopulation.Count >= (int)(populationSize * selectionThreshold))
        {
            break;
        }
    }

    return selectedPopulation;
}


    /// <summary>
    /// Réalise la recombinaison entre deux parents pour créer une nouvelle créature
    /// </summary>
    /// <param name="parent1">Premier parent</param>
    /// <param name="parent2">Deuxième parent</param>
    /// <returns>Nouvelle créature, résultat de la recombinaison des génomes des parents</returns>
    Creature Recombination(Creature parent1, Creature parent2)
    {
        List<int> genome = new List<int>(new int[genomeLength]);
        int crossoverPoint = Random.Range(0, genomeLength / 2);

        // Copier la première partie du parent1 et la deuxième du parent2 en fonction du point de coupure 
        for (int i = 0; i < genomeLength; i++)
        {
            if (i < crossoverPoint)
            {
                genome[i] = parent1.genome[i];
            }
            else
            {
                genome[i] = parent2.genome[i];
            }
        }

        return new Creature(genome, creatureGenerator);
    }

    /// <summary>
    /// Applique ou non une mutation aléatoire sur le génome d'une créature selon le taux de mutation
    /// </summary>
    /// <param name="creature">La créature dont le génome sera muté</param>
    void Mutate(Creature creature)
    {
        for (int i = 0; i < creature.genome.Count; i++)
        {
            if (Random.Range(0f, 1f) < mutationRate)
            {
                creature.genome[i] = 1 - creature.genome[i];  // Inverser le bit (0 -> 1, 1 -> 0)
            }
        }
    }

    /// <summary>
    /// Affiche toutes les générations de créatures dans l'espace en les positionnant de manière espacée
    /// Lignes par génération, donc autant de ligne que de générations définies
    /// </summary>
    void DisplayAllGenerations()
    {
        for (int generationIndex = 0; generationIndex < allGenerations.Count; generationIndex++)
        {
            List<Creature> generation = allGenerations[generationIndex];

            for (int creatureIndex = 0; creatureIndex < generation.Count; creatureIndex++)
            {
                Creature creature = generation[creatureIndex];
                Vector3 creaturePosition = startingPosition + new Vector3(creatureIndex * creatureSpacing, 0f, -generationIndex * creatureSpacing);
                creature.model.transform.position = creaturePosition;
            }
        }

    }

}
