using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace AvaloniaUtils.Utils.Collections
{
    public class ObservableCollectionWithSelectedItem<T> : ObservableCollection<T> where T : ISelected
    {
        public delegate bool SelectedChangingHandler(object sender, T? currentValue, T? newValue);

        public event SelectedChangingHandler? SelectedChanging;

        public delegate void SelectedChangedHandler(object sender, T? currentValue);

        public event SelectedChangedHandler? SelectedChanged;
        private T? _selectedItem;

        public T? SelectedItem
        {
            get => _selectedItem;
            set
            {
                var res = SelectedChanging?.Invoke(this, _selectedItem, value) ?? true;
                if (res)
                {
                    if (_selectedItem != null) _selectedItem.IsSelected = false;
                    _selectedItem = value;
                    if (_selectedItem != null) _selectedItem.IsSelected = true;
                    SelectedChanged?.Invoke(this, _selectedItem);
                }

                OnPropertyChanged("CurrentPosition");
                OnPropertyChanged();
            }
        }

        public int CurrentPosition => SelectedItem != null ? IndexOf(SelectedItem) : -1;

        public ObservableCollectionWithSelectedItem() : base()
        {
            MoveCurrentToFirst();
        }

        public ObservableCollectionWithSelectedItem(IEnumerable<T> list) : base(list)
        {
            MoveCurrentToFirst();
        }

        public void MoveCurrentToFirst()
        {
            SelectedItem = this.FirstOrDefault();
        }

        public void MoveCurrentToLast()
        {
            SelectedItem = this.LastOrDefault();
        }

        public bool MoveCurrentTo(T item)
        {
            var obj = this.FirstOrDefault(x => x.Equals(item));
            if (obj == null) return false;
            SelectedItem = obj;
            return true;
        }

        public bool MoveCurrentToId(int id)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null) return false;
            foreach (var x in this)
            {
                if (!int.TryParse(prop.GetValue(x)?.ToString(), out var res) || res != id) continue;
                SelectedItem = x;
                return true;
            }

            return false;
        }

        public bool MoveCurrentToPosition(int pos)
        {
            if (pos < 0 || pos > Count - 1) return false;
            SelectedItem = this[pos];
            return true;
        }

        public T? GetPrev()
        {
            if (SelectedItem == null) return default;
            var ind = IndexOf(SelectedItem);
            return ind == 0 ? default : this[ind - 1];
        }

        public T? GetNext()
        {
            if (SelectedItem == null) return default;
            var ind = IndexOf(SelectedItem);
            return ind == Count - 1 ? default : this[ind - 1];
        }

        public new void Clear()
        {
            SelectedItem = default;
            base.Clear();
        }

        public void SetRange(IEnumerable<T> list)
        {
            Clear();
            this.AddRange(list);
            MoveCurrentToFirst();
        }

        public bool IsSelectedLast => Count > 0 && CurrentPosition == Count - 1;
        public bool IsSelectedFirst => Count > 0 && CurrentPosition == 0;
        protected override event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
