using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppMineSweeper.Tools;

namespace WpfAppMineSweeper.Model
{
    public class Tile : Bindable
    {
        private bool _isCovered = true;
        private bool _isMine;
        private int _mineNeighbours;
        private int _index;
        private int _row;
        private int _col;
        private string? _title;

        public bool IsCovered 
        {
            get
            {
                return _isCovered;
            }
            set
            {
                _isCovered = value; 
                propertyIsChanged();
            }
        }
        public bool IsMine
        {
            get
            {
                return _isMine;
            }
            set
            {
                _isMine = value;
                propertyIsChanged();
            }
        }
        public int MineNeighbours
        {
            get
            {
                return _mineNeighbours;
            }
            set
            {
                _mineNeighbours = value;
                propertyIsChanged();
            }
        }
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
                propertyIsChanged();
            }
        }
        public int Row
        {
            get
            {
                return _row;
            }
            set
            {
                _row = value;
                propertyIsChanged();
            }
        }
        public int Col
        {
            get
            {
                return _col;
            }
            set
            {
                _col = value;
                propertyIsChanged();
            }
        }
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                propertyIsChanged();
            }
        }
    }
}
