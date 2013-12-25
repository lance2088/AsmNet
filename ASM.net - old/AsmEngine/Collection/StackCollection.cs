using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using AsmEngine.DataTypes;
using AsmEngine.Events;
using AsmEngine.Instructions;

namespace AsmEngine.Collection
{
    public class StackCollection : CollectionBase
    {
        public StackCollection()
            : base()
        {
        }

        public PUSH this[int id]
        {
            get { return (PUSH)this.List[id]; }
            set { this.List[id] = value; }
        }

        public int IndexOf(int item)
        {
            return base.List.IndexOf(item);
        }

        public int Add(PUSH item)
        {
            if (!Contains(item))
                return this.List.Add(item);
            return 0;
        }

        public void Remove(PUSH push)
        {
            this.List.Remove(push);
        }

        public void Remove(int VariableId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (((PUSH)this.List[i]).Id == VariableId)
                    this.List.RemoveAt(i);
            }
        }

        public void CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        }

        public void AddRange(StackCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
                Add(collection[i]);
        }

        public void AddRange(PUSH[] collection)
        {
            this.AddRange(collection);
        }

        public PUSH[] InnerList()
        {
            PUSH[] list = new PUSH[this.Count];
            for (int i = 0; i < this.Count; i++)
                list[i] = this[i];
            return list;
        }

        public bool Contains(PUSH item)
        {
            return this.List.Contains(item);
        }

        public void Insert(int index, int item)
        {
            this.List.Insert(index, item);
        }

        protected override void OnInsertComplete(int index, object value)
        {
            StackUpdateEvent.onStackUpdate(new StackUpdateEventArgs(this));
            base.OnInsertComplete(index, value);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            StackUpdateEvent.onStackUpdate(new StackUpdateEventArgs(this));
            base.OnRemoveComplete(index, value);
        }

        protected override void OnClearComplete()
        {
            StackUpdateEvent.onStackUpdate(new StackUpdateEventArgs(this));
            base.OnClearComplete();
        }

    }
}