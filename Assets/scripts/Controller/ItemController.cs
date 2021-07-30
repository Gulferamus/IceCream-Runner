using System.Collections.Generic;
using UnityEngine;
using Properties;

namespace Controller
{
    public class ItemController : Object
    {
        // Controller for item Spawning
        private List<GameObject> items = new List<GameObject>();
        private string itemType;

        // Constructor
        public ItemController(string itmType)
        {
            // Loading all items from Resources in itmType folder
            items.AddRange(Resources.LoadAll<GameObject>(itmType));
        }

        // Methods
        public void SpawnItems(int budget, CellProperties[] cells)
        {
            // buy items using budget
            int buyIndex;
            List<GameObject> boughtItems = new List<GameObject>();

            while (budget > 0) // REVIEW: could be useful to put a counter of sorts to break the while if it takes too long to find a suitable item?
            {
                buyIndex = Random.Range(0, items.Count);
                if (budget > items[buyIndex].GetComponent<ItemProperties>().Cost)
                {
                    boughtItems.Add(items[buyIndex]);
                    budget -= items[buyIndex].GetComponent<ItemProperties>().Cost;
                }
            }

            // spawn items
            while (boughtItems.Count > 0)
            {
                // choose a random cell
                int cellIndex = Random.Range(0, cells.Length); // TOFIX: Implement distributed probability for random cell picker
                CellProperties chosenCell = cells[cellIndex];

                // spawn item
                bool canSpawn = chosenCell.CellValue > 0;
                if (canSpawn)
                {
                    Instantiate(boughtItems[0],chosenCell.CellCoordinates,Quaternion.identity);
                    boughtItems.RemoveAt(0);
                    chosenCell.CellValue -= boughtItems[0].GetComponent<ItemProperties>().Cost;

                    // TODO: update cell points of neighbouring cells, remember to differentiate between 2 sidewalks!
                }
            }
        }
    } 
}