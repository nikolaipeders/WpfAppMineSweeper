using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using WpfAppMineSweeper.Model;
using WpfAppMineSweeper.Tools;

namespace WpfAppMineSweeper.ViewModel
{
    public class MainWindowViewModel : Bindable
    {
        #region Private members
        private List<List<Tile>>? _tiles;
        private List<Tile> _friendlyTiles;
        private bool _isGameOver = false;
        private int _rows = 10;
        private int _columns = 10;
        private int _mines;
        private int _uncoveredTiles = 0;
        private string _timespan = "00:00";
        private string _windowTitle = "Minesweeper";

        private DispatcherTimer timer = new DispatcherTimer();
        private Stopwatch stopWatch = new Stopwatch();
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            // Load tiles on launch
            InitialiseGameTiles(null);

            // Load commands on launch
            ButtonClicker = new Command(ClickingOnTile);
            ResetCommand = new Command(InitialiseGameTiles);

            // Logic for the live timer in upper left corner
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += stopwatchRunner;
            timer.Start();
        }

        #endregion

        #region Public properties
        public List<List<Tile>>? TilesList
        {
            get
            {
                return _tiles;
            }
            set
            {
                _tiles = value;
                propertyIsChanged();
            }
        }

        public List<Tile> FriendlyTiles
        {
            get
            {
                return _friendlyTiles;
            }
            set
            {
                _friendlyTiles = value;
                propertyIsChanged();
            }
        }

        public bool IsGameOver
        {
            get
            {
                return _isGameOver;
            }
            set
            {
                _isGameOver = value;
                propertyIsChanged();
            }
        }

        public int Rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
                propertyIsChanged();
            }
        }

        public int Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
                propertyIsChanged();
            }
        }

        public int Mines
        {
            get
            {
                return _mines;
            }
            set
            {
                _mines = value;
                propertyIsChanged();
            }
        }
        public int UncoveredTiles
        {
            get
            {
                return _uncoveredTiles;
            }
            set
            {
                _uncoveredTiles = value;
                propertyIsChanged();
            }
        }
        public string TimeSpanText
        {
            get
            {
                return _timespan;
            }
            set
            {
                _timespan = value;
                propertyIsChanged();
            }
        }
        public string WindowTitle
        {
            get
            {
                return _windowTitle;
            }
            set
            {
                _windowTitle = value;
                propertyIsChanged();
            }
        }
        #endregion

        #region Commands

        public Command ButtonClicker // This enables the option to write a new Command in line 25 above
        {
            get;
            set;
        }

        public Command ResetCommand // This enables the option to write a new Command in line 25 above
        {
            get;
            set;
        }

        #endregion

        #region Public methods
        /**
         * "Fills" the grid with Tiles and places a random number of mines.
        **/
        public void InitialiseGameTiles(Object? parameter)
        {
            // Reset counters
            UncoveredTiles = 0;
            Mines = 0;

            Random rnd = new Random(); // Used in nesten for loop below 

            // Creating a list of list of tiles allows the grid to use this as source for itemscontrol
            List<List<Tile>> listOfTiles = new List<List<Tile>>();

            for (int i = 0; i < Rows; i++)
            {
                listOfTiles.Add(new List<Tile>());

                for (int j = 0; j < Columns; j++)
                {
                    Tile tile = new Tile() { Row = i, Col = j };

                    if (rnd.Next(1, Rows * Columns) % rnd.Next(1, Rows * Columns) == 0) // Place mines in a super random way
                    {
                        tile.IsMine = true;
                        tile.Title = "M"; // For developer to see where mines are
                        Mines++;
                    }
                    else // Ordinary tiles 
                    {
                        tile.IsMine = false;
                    }
                    listOfTiles[i].Add(tile);
                }
            }
            TilesList = listOfTiles;
        }

        /**
        * This method is called in a command everytime the user clicks a button. 
        * Checks íf tile is a mine and what the stat of the surrounding mines are. 
        **/
        public void ClickingOnTile(Object parameter)
        {
            // Reset Window Title to name of game 
            WindowTitle = "Minesweeper";

            // New List of Friendly Tiles in the neighbourhood 
            FriendlyTiles = new List<Tile>();

            // Which tile was clicked?
            Tile? selectedTile = parameter as Tile;

            // Can't do tile logic without a tile
            if (selectedTile != null)
            {
                // If this is the first click after a new game or on launch, restart stopwatch and set game to be running.
                if (IsGameOver)
                {
                    InitialiseGameTiles(null);
                    timer.Start();
                    IsGameOver = false;
                }

                // Log how many tiles is uncovered and only count a click if it's uncovered
                if (selectedTile.IsCovered)
                {
                    selectedTile.IsCovered = false;
                }

                if (selectedTile.IsMine) // Game Over
                {
                    // Declare game is over
                    IsGameOver = true;

                    // Show that this was as mine 
                    selectedTile.Title = "M";

                    // Stop the timer
                    stopWatch.Stop();
                    timer.Stop();

                    // Depressing text on tiles to show lack of skills
                    WindowTitle = "Game Over";
                }

                else // Game Logic
                {
                    checkNeighbours(selectedTile);
                    foreach (Tile friendlyTile in FriendlyTiles.ToList())
                    {
                        checkNeighbours(friendlyTile);
                    }
                }


                // Check for victory
                if (UncoveredTiles == (Rows * Columns - Mines))
                {
                    // Stop the timer
                    stopWatch.Stop();
                    timer.Stop();

                    // Nice text on tiles to show victory
                    WindowTitle = "Victory";
                }
            }
            #endregion
        }

        public void checkNeighbours(Tile selectedTile)
        {
            // Method variables
            int minesAsNeighbourCounter = 0;
            List<Tile> listOfNeighborTiles = new List<Tile>();

            // Content is needed
            if (selectedTile != null && selectedTile.Title == null && TilesList != null)
            {

            // Determine Col/Row of Level Neighbours
            if (selectedTile.Col != 0)
                {
                    Tile? leftTile = TilesList[selectedTile.Row].Find(tile => tile.Col == selectedTile.Col - 1);
                    if (leftTile != null)
                    {
                        listOfNeighborTiles.Add(leftTile);
                    }
                }

                if (selectedTile.Col != Columns - 1)
                {
                    Tile? rightTile = TilesList[selectedTile.Row].Find(tile => tile.Col == selectedTile.Col + 1);
                    if (rightTile != null)
                    {
                        listOfNeighborTiles.Add(rightTile);
                    }
                }

                // Determine Col/Row of Top Neighbours
                if (selectedTile.Row != 0)
                {
                    Tile? aboveTile = TilesList[selectedTile.Row - 1].Find(tile => tile.Col == selectedTile.Col);
                    if (aboveTile != null)
                    {
                        listOfNeighborTiles.Add(aboveTile);
                    }

                    if (selectedTile.Col != 0)
                    {
                        Tile? leftTopTile = TilesList[selectedTile.Row - 1].Find(tile => tile.Col == selectedTile.Col - 1);
                        if (leftTopTile != null)
                        {
                            listOfNeighborTiles.Add(leftTopTile);
                        }
                    }
                    if (selectedTile.Col != Columns - 1)
                    {
                        Tile? rightTopTile = TilesList[selectedTile.Row - 1].Find(tile => tile.Col == selectedTile.Col + 1);
                        if (rightTopTile != null)
                        {
                            listOfNeighborTiles.Add(rightTopTile);
                        }
                    }
                }

                // Determine Col/Row of Bottom Neighbours
                if (selectedTile.Row != Rows - 1)
                {
                    Tile? belowTile = TilesList[selectedTile.Row + 1].Find(tile => tile.Col == selectedTile.Col);
                    if (belowTile != null)
                    {
                        listOfNeighborTiles.Add(belowTile);
                    }

                    if (selectedTile.Col != 0)
                    {
                        Tile? belowLeftTile = TilesList[selectedTile.Row + 1].Find(tile => tile.Col == selectedTile.Col - 1);
                        if (belowLeftTile != null)
                        {
                            listOfNeighborTiles.Add(belowLeftTile);
                        }
                    }

                    if (selectedTile.Col != Columns - 1)
                    {
                        Tile? belowRightTile = TilesList[selectedTile.Row + 1].Find(tile => tile.Col == selectedTile.Col + 1);
                        if (belowRightTile != null)
                        {
                            listOfNeighborTiles.Add(belowRightTile);
                        }
                    }

                    // CHECK FOR MINES
                    foreach (Tile tile in listOfNeighborTiles)
                    {
                        if (tile.IsMine)
                        {
                            minesAsNeighbourCounter++;
                        }
                        else
                        {
                            FriendlyTiles.Add(tile);
                        }
                    }
                }
                selectedTile.Title = "" + minesAsNeighbourCounter;
                UncoveredTiles++;
            }
        }
        /**
         * Converting the current timespan to a string
        **/
        public void stopwatchRunner(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                TimeSpanText = ts.ToString("mm':'ss");
            }
            else
            {
                stopWatch.Restart();
            }
        }
    }
}
