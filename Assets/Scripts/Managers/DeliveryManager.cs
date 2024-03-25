using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;


    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private List<RecipeSO> waitingrecipeSOList;
    [SerializeField] private float spawnRecipeTimer;
    [SerializeField] private float spawnRecipeTimerMax = 4;
    [SerializeField] private int waitingRecipesMax = 4;

    private async void Awake()
    {
        Instance = this;

        waitingrecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if(waitingrecipeSOList.Count < waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeListSO[UnityEngine.Random.Range(0, recipeListSO.recipeListSO.Count)];
                Debug.Log(waitingRecipeSO.name);
                waitingrecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }            
        }
        
    }


    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for(int i = 0; i< waitingrecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingrecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                //Has the same number of ingredients
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    //cycling thorugh all ingredients in the recipe
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //cycling thorugh all ingredients in the Plate
                        if(plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            //ingredient matches!
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        //This recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe)
                {
                    //player delivered the correct recipe
                    Debug.Log("player delivered the correct recipe");
                    waitingrecipeSOList.RemoveAt(i);

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);

                    return;
                }
            }
                

        }

        //No matches found
        //player did not deliver a correct recipe
        Debug.Log("player did not deliver a correct recipe");
    }


    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingrecipeSOList;
    }

}
