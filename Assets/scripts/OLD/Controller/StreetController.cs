using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Controller
{
    public class StreetController : Object
    {
        // Manager for steet Spawning, Destroying and Moving
        public List<Street> Streets { get; set; } = new List<Street>();
        private GameObject streetPrefab;
        private int maxStreets;
        private float streetLength;
        
        // Constructor
        public StreetController(int maxStreets, GameObject streetPrefab)
        {
            this.maxStreets = maxStreets;
            this.streetPrefab = streetPrefab;
            streetLength = Tools.GetSize(streetPrefab, 'z');
        }

        // Helper Methods
        #region Street Methods
        public bool SpawnStreetIfNeeded(int streetBudget, int xCellNum, int cellNumber, int startCellValue)
        {
            // Instantiate a new street segment
            Vector3 nextStreetPosition = new Vector3();
            GameObject nextStreet;

            if (Streets.Count < maxStreets)
            {
                nextStreetPosition.z = Streets.Count == 0 ? 0 : Streets[Streets.Count - 1].StreetObject.transform.position.z + streetLength;
                nextStreet = Instantiate(streetPrefab, nextStreetPosition, Quaternion.identity);

                List<Street.CellProperties> newCells = InitializeCells(nextStreet, xCellNum, cellNumber, startCellValue);

                Street newStreet = new Street(nextStreet, streetBudget, newCells);
                MoveStreet(1000, newStreet);
                Streets.Add(newStreet);

                return true;
            }

            return false;
        }

        public void DestroyStreetIfOld(int bufferStreetNumber = 2)
        {
            // Destroy old street segments behind the player
            float bufferLength = -bufferStreetNumber * streetLength;
            for (int i = 0; i < Streets.Count; i++)
            {
                if (Streets[i].StreetObject.transform.position.z < bufferLength)
                {
                    Destroy(Streets[i].StreetObject);
                    Streets.RemoveAt(i);
                }
            }
        }

        public void MoveStreets(float streetMovement)
        {
            // Move street segments
            //foreach (Street street in Streets)
            //{
            //    street?.StreetObject.transform.Translate(0, 0, -streetMovement);
            //    MoveCellCoordinates(-streetMovement, street.Cells);
            //}
        }

        public void MoveStreet(float streetMovement, Street street)
        {
            Vector3 velocityVector = new Vector3(0, 0, -streetMovement);
            street.StreetObject.GetComponent<Rigidbody>().AddForce(velocityVector, ForceMode.VelocityChange);
            MoveCellCoordinates(-streetMovement, street.Cells);
        }
        #endregion

        #region Cell Methods
        public List<Street.CellProperties> InitializeCells(GameObject street, int xCellNum, int cellNumber, int startValue)
        {
            // creates a new cell list
            List<Street.CellProperties> newCells = new List<Street.CellProperties>();
            Vector3[] cellCoords;
            int[] cellValues;

            cellCoords = GetCellCoordinates(street, xCellNum, cellNumber);
            cellValues = GetCellValues(startValue, cellNumber);

            for (int i = 0; i < cellNumber; i++)
            {
                newCells.Add(new Street.CellProperties(cellCoords[i], cellValues[i]));
            }

            return newCells;
        }

        public Vector3[] GetCellCoordinates(GameObject street, int xCellNum, int cellNumber)
        {
            // Returns an array with the coordinates of each cell in the street segment
            // IMPORTANT: a sidewalk MUST have leght as a multiple of width, or the cells won't be square
            Vector3[] cellCoords = new Vector3[cellNumber];
            Vector3[] leftSidewalkCoords = new Vector3[cellNumber / 2];
            Vector3[] rightSidewalkCoords = new Vector3[cellNumber / 2];

            float streetWidth = Tools.GetSize(street.transform.GetChild(0).gameObject, 'x');
            float SidewalkHeight = Tools.GetSize(street.transform.GetChild(0).GetChild(0).gameObject, 'y');
            float cellWidth = Tools.GetSize(street.transform.GetChild(0).GetChild(0).gameObject, 'x') / xCellNum;

            int zIndex = 0;
            int xIndex = 0;

            //DEBUG
            for (int i = 0; i < leftSidewalkCoords.Length; i++)
            {
                leftSidewalkCoords[i] = street.transform.position;
                rightSidewalkCoords[i] = street.transform.position;
            }

            for (int i = 0; i < leftSidewalkCoords.Length; i++)
            {
                zIndex = i / xCellNum;
                xIndex = i - (i / xCellNum) * xCellNum;
                //DEBUG
                Vector3 test = new Vector3(-(streetWidth / 2f + (xIndex + 0.5f) * cellWidth), SidewalkHeight / 2f, (zIndex + 0.5f) * cellWidth);

                leftSidewalkCoords[i].x += -(streetWidth / 2f + (xIndex + 0.5f) * cellWidth);
                leftSidewalkCoords[i].y += SidewalkHeight / 2f;
                leftSidewalkCoords[i].z += (zIndex + 0.5f) * cellWidth;

                rightSidewalkCoords[i].x += streetWidth / 2f + (xIndex + 0.5f) * cellWidth;
                rightSidewalkCoords[i].y += SidewalkHeight / 2f;
                rightSidewalkCoords[i].z += (zIndex + 0.5f) * cellWidth;
            }

            leftSidewalkCoords.CopyTo(cellCoords, 0);
            rightSidewalkCoords.CopyTo(cellCoords, leftSidewalkCoords.Length);

            return cellCoords;
        }

        public int[] GetCellValues(int value, int cellNumber)
        {
            // returns starting values for cells
            int[] values = new int[cellNumber];

            for (int i = 0; i < cellNumber; i++)
            {
                values[i] = value;
            }

            return values;
        }

        public void MoveCellCoordinates(float movement, List<Street.CellProperties> cells)
        {
            // returns new cell coordinates to move with the street
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].Coordinates += new Vector3(0, 0, movement);
            }
        }
        #endregion
    }
}