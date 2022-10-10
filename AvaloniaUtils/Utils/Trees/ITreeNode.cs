using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaUtils.Utils.Collections;
using DynamicData.Kernel;

namespace AvaloniaUtils.Utils.Trees
{
    public interface ITreeNode<T> where T : ISelected
    {
        public T?                                      Parent { get; set; }
        public ObservableCollectionWithSelectedItem<T> Childs { get; set; }

        public void AddChild(T    child);
        public void RemoveChild(T child);
    }
}
